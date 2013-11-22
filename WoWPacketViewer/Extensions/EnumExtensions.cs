using System;

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
    }
}