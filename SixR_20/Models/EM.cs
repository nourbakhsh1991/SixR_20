using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public static class EM
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            var attr = enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttributes(typeof(TAttribute), false);
            if (!attr.Any())
                return null;

            return (TAttribute)attr[0];
        }

        internal static int GetParameterSize(this X p)
        {
            var attr = p.GetAttribute<TypeAttribute>();
            if (attr == null)
                return 0;
            var tAtt = attr.Type.GetAttribute<NumericAttribute>();
            return attr.Length * tAtt.Number;
        }

        public static T? CastToEnum<T>(this string ParamName)
            where T : struct, IConvertible
        {
            try
            {
                var param = (T)Enum.Parse(typeof(T), ParamName);
                return param;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static T To<T>(this object inp)
        {
            if (inp == null)
                throw new NullReferenceException("inp can not be null");

            var m = typeof(T).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(c => c.Name == "Parse");

            if (!m.Any())
                throw new Exception("Type must have Parse method");

            return (T)m.ToList()[0].Invoke(null, new object[] { inp.ToString() });
        }
    }
}
