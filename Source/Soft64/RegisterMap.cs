using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections;

namespace Soft64
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RegisterMappingAttribute : Attribute
    {
        private String m_FieldName;
        private Int32 m_FieldSize;
        private Int32 m_FieldOffset;
        private RuntimeTypeHandle m_FieldType;

        public RegisterMappingAttribute(String name, Int32 size, Int32 offset, Type type)
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

    public abstract class RegisterMap<T>
        where T : struct
    {
        private T m_Register;
        private IEnumerable<RegisterMappingAttribute> m_MapDefinitions;
        private ExpandoObject m_DynamicObject;

        protected RegisterMap()
        {
            m_MapDefinitions = GetType().GetCustomAttributes(typeof(RegisterMappingAttribute), true).OfType<RegisterMappingAttribute>();
            m_DynamicObject = new ExpandoObject();
        }

        private void BuildProps()
        {
            Func<T> getter;
            Action<T> setter;

           /* 2 ^ x - 1 = bit mask
            * So we need to figure out how many bits will be on the left side */

           foreach (var def in m_MapDefinitions)
           {
               getter = () =>
               {

               };


               ((IDictionary<String, Object>)m_DynamicObject).Add("get_" + def.FieldName, getter);
           }
        }

        public T RegisterValue
        {
            get { return m_Register; }
            set { m_Register = value; }
        }
    }
}
