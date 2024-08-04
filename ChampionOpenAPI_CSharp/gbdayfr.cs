using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public struct gbdayfr
    {
        public List<gbdays> gbdayss;
        public int nRtn;
        public string sNextKey;
        public bool success;
        public override string ToString()
        {
            return str.Format("gbdayss.Count={0} nRtn={1} sNextKey={2} success={3}", gbdayss.Count, nRtn, sNextKey, success);
        }
    }
}
