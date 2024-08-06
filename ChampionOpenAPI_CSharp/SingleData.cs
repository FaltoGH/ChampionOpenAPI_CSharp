using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{

    public struct SingleData<T>
    {
        public T Data;
        public int nRtn;
        public string sNextKey;
        public bool bSuccess;
        public int nRtnType;

        public override string ToString()
        {
            return "Data={0} nRtn={1} sNextKey={2} bSuccess={3} nRtnType={4}".NullSafeFormat(Data, nRtn, sNextKey, bSuccess, nRtnType);
        }

    }
}
