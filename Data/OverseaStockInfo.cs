/*
함수명 BSTR GetOverseaStockInfo(LPCTSTR sCode, LONG nItemIndex) 
설  명 해외주식 종목정보를 구하는 함수 
  
인자값 LPCTSTR sCode 해외 종목 표준 코드 ( 거래소코드+종목코드 ) 
LONG nItemIndex 옵션 구분 인덱스 
비고 - 해외종목코드는 「Tran서비스IO」 문서 파일 참조 - nItemIndex 옵션 설명 
0 : 자체단축코드(shortcode) 
1 : 자체표준코드(expcode) 
5 : 한글명(hname) 
6 : 영문명(ename) 
7 : 국제표준코드(isincode) 
8 : 업종코드(upcodes) 
9 : 종목분류(div_code, 1:주식, 7:ETF) 
10 : 주문수량단위(tradeunit) 
11 : 가격 소숫점 자릿수(floatpoint) 
12 : 틱사이즈(ticktype, 0:미정의, 나머지는 표 참조) 
13 : 외환코드(currency) by Code 
14 : 영업일자(bymd) 
15 : 계정계용 거래소 코드 
16 : 계정계용 거래소+심볼코드를 풀코드로 변환 
*/

using System;

namespace ChampionOpenAPI_CSharp.Data
{
    public struct OverseaStockInfo
    {
        /// <summary>
        /// AMX_AAMC
        /// </summary>
        public string shortcode;

        /// <summary>
        /// 0066AAMC
        /// </summary>
        public string expcode;

        /// <summary>
        /// 알티소스 에셋 매니지먼트
        /// </summary>
        public string hname;

        /// <summary>
        /// ALTISOURCE ASSET MANAGEMENT CORP
        /// </summary>
        public string ename;

        /// <summary>
        /// VI02153X1080
        /// </summary>
        public string isincode;

        /// <summary>
        /// 4540
        /// </summary>
        public string upcodes;

        /// <summary>
        /// 1:주식, 7:ETF
        /// </summary>
        public string div_code;

        /// <summary>
        /// 00000001
        /// </summary>
        public string tradeunit;

        /// <summary>
        /// 4
        /// </summary>
        public string floatpoint; 

        /// <summary>
        /// 1
        /// </summary>
        public string ticktype;

        /// <summary>
        /// USD
        /// </summary>
        public string currency;

        /// <summary>
        /// 20240709
        /// </summary>
        public string bymd;

        /// <summary>
        /// 020
        /// </summary>
        public string corebankingexchangecode;

        public string fullcode;

        public string exchangecode => expcode.Substring(0, 4);

/*
※ 해외주식 종목코드 입력방법
 - 거래소코드 4자리 + 종목코드 16자리 
 - 거래소코드 정의 
  "0321" : 미국뉴욕종목 
 "0066" : 미국아멕스종목 
 "0537" : 미국나스닥종목 
 "0215" : 중국상해종목 
 "0214" : 중국심천종목 
 "0104" : 홍콩종목 
ex) 뉴욕거래소 CEE종목 : “0321” + “CEE” → “0321CEE”
*/

        public string exchangename
        {
            get
            {
                switch (exchangecode)
                {
                    case "0321": return "미국뉴욕종목";
                    case "0066": return "미국아멕스종목";
                    case "0537": return "미국나스닥종목";
                    case "0215": return "중국상해종목";
                    case "0214": return "중국심천종목";
                    case "0104": return "홍콩종목";
                    default: throw new ArgumentException();
                }
            }
        }

        public string jmcode => expcode.Substring(4, expcode.Length - 4);

        public override string ToString()
        {
            return jmcode.PadRight(20) + "\t" + hname.PadRight(54) + "\t" + exchangename;
        }
    }
}
