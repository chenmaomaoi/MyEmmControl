using System;
using System.Reflection;

namespace MyEmmControl.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 获取字段上的自定义特性
        /// </summary>
        /// <typeparam name="TAtteibute"></typeparam>
        /// <param name="obj"></param>
        /// <param name="attributeType">typeof(TAtteibute)</param>
        /// <returns></returns>
        public static TAtteibute GetFieldAttribute<TAtteibute>(this object obj, Type attributeType) where TAtteibute : Attribute
        {
            //获取字段信息
            FieldInfo field = obj.GetType().GetField(obj.ToString());
            //检查字段是否含有指定特性
            if (field.IsDefined(attributeType, true))
            {
                //获取字段上的自定义特性
                TAtteibute remarkAttribute = (TAtteibute)field.GetCustomAttribute(attributeType);
                return remarkAttribute;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取字段上的自定义特性
        /// </summary>
        /// <typeparam name="TAtteibute"></typeparam>
        /// <param name="obj"></param>
        /// <remarks>在迭代器中该方法可能带来性能损失</remarks>
        /// <returns></returns>
        public static TAtteibute GetFieldAttribute<TAtteibute>(this object obj) where TAtteibute : Attribute
        {
            return GetFieldAttribute<TAtteibute>(obj, typeof(TAtteibute));
        }
    }
}
