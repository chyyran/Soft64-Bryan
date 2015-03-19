/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CompiledAction =
    System.Action<
        Soft64.IO._StreamIOCompiler,
        System.Byte[],
        Soft64.IO._StreamOp>;

namespace Soft64.IO
{
    internal sealed class _StreamIOCompiler
    {
        private Dictionary<Int64, CompiledAction> m_CompiledOperations;
        private List<Stream> m_StreamReferences;
        private HashSet<Stream> m_StreamHashSet;
        private static readonly Type[] s_RequestParamTypes = { typeof(Byte[]), typeof(Int32), typeof(Int32) };

        public _StreamIOCompiler()
        {
            ClearCache();
        }

        public void ClearCache()
        {
            m_CompiledOperations = new Dictionary<Int64, CompiledAction>();
            m_StreamReferences = new List<Stream>();
            m_StreamHashSet = new HashSet<Stream>();
        }

        public void ExecuteOperation(
            _IOTransfer request,
            _StreamQuery query,
            Boolean writeMode,
            _StreamOp operation
            )
        {
            var key = GenerateHash(query.RequestedAddress, request.Offset, request.Count);

            if (m_CompiledOperations.ContainsKey(key))
            {
                m_CompiledOperations[key](this, request.Buffer, operation);
            }
            else
            {
                CompiledAction compiledAction = null;

                /* Begin building a new memory IO method */
                Type typeReturn = typeof(void);
                Type[] typeParams = { this.GetType(), typeof(Byte[]), typeof(_StreamOp) };
                String methodName = "CompiledMemIO_" + key.ToString("X8");

                /* Declare a new dynamic method to host our generated code */
                DynamicMethod method = new DynamicMethod(methodName, typeReturn, typeParams, true);

                /* Get the IL emitter for the dynamic method instance */
                ILGenerator emitter = method.GetILGenerator();

                /* Setup some things of the method */
                emitter.DeclareLocal(typeof(Stream));
                var methodGeStream = GetType().GetMethod("GeStream");
                var methodSetSteamPosition = typeof(Stream).GetMethod("set_Position");
                var methodInvoke = typeof(_StreamOp).GetMethod("Invoke");

                /* Compile the whole operation into a method using the interpreter engine */
                _StreamIOInterpreter.RunOperation(request, query, writeMode,
                    (r, s) =>
                    {
                        r = RunEmitter(emitter, methodGeStream, methodSetSteamPosition, methodInvoke, r, s);
                    });

                emitter.Emit(OpCodes.Nop);
                emitter.Emit(OpCodes.Ret);

                compiledAction = (CompiledAction)method.CreateDelegate(typeof(CompiledAction));
                m_CompiledOperations.Add(key, compiledAction);

                compiledAction(this, request.Buffer, operation);
            }
        }

        private _IOTransfer RunEmitter(ILGenerator emitter, MethodInfo methodGeStream, MethodInfo methodSetSteamPosition, MethodInfo methodInvoke, _IOTransfer r, Stream s)
        {
            /* Push the delegate object */
            emitter.Emit(OpCodes.Ldarg_2);

            /* Push some values */
            emitter.Emit(OpCodes.Ldarg_1);   /* Buffer reference */
            emitter.Emit(OpCodes.Ldc_I4, r.Offset); /* Offset in buffer */
            emitter.Emit(OpCodes.Ldc_I4, r.Count); /* Length of operation */
            emitter.Emit(OpCodes.Newobj, typeof(_IOTransfer).GetConstructor(s_RequestParamTypes));

            /* Stream reference */
            emitter.Emit(OpCodes.Ldarg_0);
            emitter.Emit(OpCodes.Ldc_I4, ResolveStreamReference(s));
            emitter.Emit(OpCodes.Callvirt, methodGeStream);
            emitter.Emit(OpCodes.Nop);

            /* Quick store reference in local 0 */
            emitter.Emit(OpCodes.Stloc_0);
            emitter.Emit(OpCodes.Ldloc_0);

            /* Set the stream position */
            emitter.Emit(OpCodes.Ldc_I8, s.Position);   /* Position in stream */
            emitter.Emit(OpCodes.Callvirt, methodSetSteamPosition);
            emitter.Emit(OpCodes.Nop);

            /* Get the cached stream referenced */
            emitter.Emit(OpCodes.Ldloc_0);

            /* Lastly, execute the operation */
            emitter.Emit(OpCodes.Callvirt, methodInvoke);
            emitter.Emit(OpCodes.Nop);
            emitter.Emit(OpCodes.Nop);
            return r;
        }

        private Int32 ResolveStreamReference(Stream stream)
        {
            if (!m_StreamHashSet.Contains(stream))
            {
                m_StreamHashSet.Add(stream);
                m_StreamReferences.Add(stream);
                return m_StreamReferences.Count - 1;
            }
            else
            {
                return (from index in Enumerable.Range(0, m_StreamReferences.Count - 1)
                        where Object.ReferenceEquals(m_StreamReferences[index], stream)
                        select index).SingleOrDefault();
            }
        }

        public Stream GeStream(Int32 index)
        {
            return m_StreamReferences[index];
        }

        private Int64 GenerateHash(Int64 address, Int32 offset, Int32 count)
        {
            var abase = address >> 16;
            var aoffset = address << 16;

            return (abase ^ aoffset) ^ (count + offset);
        }

        public Boolean ContainsOperation(Int64 address, Int32 bufferOffset, Int32 count)
        {
            return m_CompiledOperations.ContainsKey(GenerateHash(address, bufferOffset, count));
        }
    }
}