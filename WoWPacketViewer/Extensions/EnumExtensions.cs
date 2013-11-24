using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomExtensions
{
    public static class EnumExtensions
    {
        public static string GetFullName(this Object obj)
        {
            // NOTE: Applies to all objects, but really only intended for enumerations.
            // Generics prevent this otherwise.
            return string.Format("{0}.{1}", obj.GetType().Name, obj.ToString());
        }

        public static string GetIndividualFlagString(this Enum value)
        {
            return string.Join(" | ", value.GetIndividualFlagsArray());
        }

        public static string GetFlagString(this Enum value)
        {
            return string.Join(" | ", value.GetFlagsArray());
        }

        public static string[] GetIndividualFlagsArray(this Enum value)
        {
            return value.GetIndividualFlagsEnumerable().ToArray();
        }

        public static string[] GetFlagsArray(this Enum value)
        {
            return value.GetFlagsEnumerable().ToArray();
        }

        public static IEnumerable<string> GetIndividualFlagsEnumerable(this Enum value)
        {
            foreach (var flag in value.GetIndividualFlags())
                yield return flag.ToString();
        }

        public static IEnumerable<string> GetFlagsEnumerable(this Enum value)
        {
            foreach (var flag in value.GetFlags())
                yield return flag.ToString();
        }

        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            var bits = Convert.ToUInt64(value);
            var results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                var mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;

                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }

            if (bits != 0L)
                return Enumerable.Empty<Enum>();

            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();

            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);

            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                var bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    yield return value;
                    //continue; // skip the zero value

                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }
    }
}