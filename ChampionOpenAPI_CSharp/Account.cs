using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public class Account
    {
        public string Number;
        public string Password;

        public Account(string number)
        {
            Number = number;
        }
    }
}
