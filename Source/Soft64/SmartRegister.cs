using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Soft64
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RegisterFieldAttribute : Attribute
    {
        private String m_FieldName;
        private Int32 m_FieldSize;
        private Int32 m_FieldOffset;
        private RuntimeTypeHandle m_FieldType;

        public RegisterFieldAttribute(String name, Int32 size, Int32 offset, Type type)
        {
            m_FieldName = name;
            m_FieldSize = size;
            m_FieldOffset = offset;
            m_FieldType = type.TypeHandle;
        }

        public RuntimeTypeHandle FieldType
        {
            get { return m_FieldType; }
        }

        public Int32 FieldOffset
        {
            get { return m_FieldOffset; }
        }

        public Int32 FieldSize
        {
            get { return m_FieldSize; }
        }

        public String FieldName
        {
            get { return m_FieldName; }
        }

    }

    public abstract class SmartRegister<T>
        where T : struct
    {
        private T m_Register;
        private IEnumerable<RegisterFieldAttribute> m_MapDefinitions;
        private ExpandoObject m_DynamicObject;

        protected SmartRegister()
        {
            m_MapDefinitions = GetType().GetCustomAttributes(typeof(RegisterFieldAttribute), true).OfType<RegisterFieldAttribute>();
            m_DynamicObject = new ExpandoObject();
            BuildProps();
        }

        private void BuildProps()
        {
           foreach (var def in m_MapDefinitions)
           {
               Func<dynamic> getter;
               Action<dynamic> setter;
               Type fieldType = Type.GetTypeFromHandle(def.FieldType);
               dynamic mask = ((UInt64)Math.Pow(2, def.FieldSize) - 1) << def.FieldOffset;

               getter = () =>
               {
                   dynamic value = Activator.CreateInstance(fieldType);
                   dynamic maskedRegValue = m_Register;
                   maskedRegValue &= (T)mask;
                   maskedRegValue >>= def.FieldOffset;
                   return Convert.ChangeType(maskedRegValue, fieldType);
               };

               setter = (v) =>
               {
                   dynamic newValue = (UInt64)v;
                   newValue <<= def.FieldOffset;

                   dynamic maskedRegValue = m_Register;
                   maskedRegValue &= ~mask;
                   maskedRegValue |= newValue;
                   m_Register = (T)maskedRegValue;
               };


               ((IDictionary<String, Object>)m_DynamicObject).Add("Get" + def.FieldName, getter);
               ((IDictionary<String, Object>)m_DynamicObject).Add("Set" + def.FieldName, setter);
           }
        }

        public T RegisterValue
        {
            get { return m_Register; }
            set { m_Register = value; }
        }

        public dynamic AutoRegisterProps
        {
            get { return m_DynamicObject; }
        }
    }
}
