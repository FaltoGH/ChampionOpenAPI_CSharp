using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{

    public struct MultiData<T>
    {
        public List<T> m_list;
        public int nRtn;
        public string sNextKey;
        public bool bSuccess;

        public int Count
        {
            get
            {
                return m_list?.Count ?? 0;
            }
        }

        public override string ToString()
        {
            return "Count={0} nRtn={1} sNextKey={2} bSuccess={3}".NullSafeFormat(Count, nRtn, sNextKey, bSuccess);
        }

    }
}
