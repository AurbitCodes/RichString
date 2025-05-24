using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RichString
{
    public class MemberReference
    {
        public MemberInfo MemberInfo { get; set; }
        public object DeclaringObject { get; set; }
        public string OriginalExpression { get; set; }
        public bool IsEnumerable { get; set; } = false;
        public int Index { get; set; } = -1;

        public object GetValue()
        {
            return GetValue(MemberInfo, DeclaringObject, IsEnumerable, Index);
        }
        public string GetStringValue(bool alternate = false)
        {
#nullable enable
            object? value = GetValue();
            if (value == null)
            {
                return string.Empty;
            }
#nullable disable
            if (value is IRichStringCustomFormat)
            {
                var richFormat = value as IRichStringCustomFormat;
                return alternate ? richFormat.GetAlternateForm() : richFormat.GetNormalForm();
            }
            return value.ToString();
        }
        public Type GetMemeberType()
        {
            return GetMemberType(MemberInfo);
        }
        public static object GetValue(MemberInfo member, object declaringObject, bool isEnumerable, int index)
        {
            object value;
            object enumerateCheckedValue;
            if (member is FieldInfo)
            {
                value = ((FieldInfo)member).GetValue(declaringObject);

                if (isEnumerable && !(value is IEnumerable))
                {
                    RichString.HandleError($"Member {member.Name} is not an IEnumerable, but you're trying to use it as an IEnumerable");
                    return null;
                }
            }
            else if (member is PropertyInfo)
            {
                value = ((PropertyInfo)member).GetValue(declaringObject);

                if (isEnumerable && !(value is IEnumerable))
                {
                    RichString.HandleError($"Member {member.Name} is not an IEnumerable, but you're trying to use it as an IEnumerable");
                    return null;
                }
            }
            else
            {
                RichString.HandleError($"Member {member.Name} is neither a Field or a Property. Member Type: {member.MemberType}");
                return null;
            }

            enumerateCheckedValue = isEnumerable ? GetEnumerableValue(value as IEnumerable, index) : value;

            return enumerateCheckedValue;    
        }
        public static object GetEnumerableValue(IEnumerable value, int index)
        {
            int count = 0;
            foreach (var item in value)
            {
                if (count == index) return item;
                count++;
            }

            throw new Exception($"Index was out of range of IEnumerable. IEnumerable: {value}, Index: {index}");
        }

        // Get member type as System.Type and not the MemberTypes enum.
        public static Type GetMemberType(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                var type = ((FieldInfo)member).FieldType;
                if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    return type.GetGenericArguments()[0];
                }
                return type;
            }
            if (member is PropertyInfo)
            {
                var type = ((PropertyInfo)member).PropertyType;
                if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    return type.GetGenericArguments()[0];
                }
                return type;
            }

            throw new Exception($"Member {member.Name} is neither a Field or a Property. Member Type: {member.MemberType}");
        }
    }
}
