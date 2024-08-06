using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public static class StringExtensions
    {
        public static string NullSafeFormat(this string format, params object[] args)
        {
            if (format == null) return string.Empty;
            if (string.IsNullOrWhiteSpace(format)) return format;
            if (args.Length <= 0) return format;

            object[] args2 = (object[])args.Clone();

            for(sbyte i = 0; i < args2.Length; i++)
            {
                if (args2[i] == null) { args2[i] = string.Empty; }
            }

            return string.Format(format, args2);
        }
    }
}
