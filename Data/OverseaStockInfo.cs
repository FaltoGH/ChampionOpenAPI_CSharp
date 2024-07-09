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

namespace ChampionOpenAPI_CSharp.Data
{
    public struct OverseaStockInfo
    {
        public string shortcode,
            expcode, hname, ename,
            isincode, upcodes, div_code,
            tradeunit, floatpoint, ticktype,
            currency, bymd, corebankingexchangecode,
            fullcode;
    }
}
