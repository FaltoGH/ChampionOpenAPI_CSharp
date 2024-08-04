using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public struct gbdays
    {
        public string ldate;       //일자
        public string cpcheck;     //전일대비부호
        public string ldiff;       //전일대비
        public string ldiffratio;      //대비율
        public string lcprice;     //현재가
        public string lvolume;     //누적거래량
        public string lvalue;      //누적거래대금
        public string loprice;     //시가
        public string lhprice;     //고가
        public string llprice;     //저가
        public string lbprice;     //기준가
        public override string ToString()
        {
            string[] a = new string[] { ldate, cpcheck, ldiff, ldiffratio, lcprice, lvolume, lvalue, loprice, lhprice, llprice, lbprice };
            return string.Join(";", a);
        }
    }
}
