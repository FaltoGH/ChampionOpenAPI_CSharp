using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public static class str
    {
        /// <summary>
        /// Provides null-safe formatting method.
        /// </summary>
        public static string Format(string format, params object[] args)
        {
            object[] arg = (object[])args.Clone();
            for(sbyte i = 0; i < arg.Length; i++)
            {
                if (arg[i] == null) { arg[i] = string.Empty; }
            }
            return string.Format(format, arg);
        }
    }
}
