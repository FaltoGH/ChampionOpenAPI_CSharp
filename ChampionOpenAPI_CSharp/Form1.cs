/*
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Expando;
using System.Diagnostics;
using Microsoft.Win32;

namespace ChampionOpenAPI_CSharp
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        String g_sOpenAPI_PATH = "";    //오픈API 설치경로
        int g_nVersionCheck = 0;        //버전처리 체크키
        bool g_bLoginYN = false;        //로그인 여부
        bool g_bSetReal = false;        //실시간 여부
        bool g_bMultiReal = false;      // 복수실시간 시세 여부
        string g_sLoginId = "";         //로그인아이디
        string g_sMsg = "";             //처리결과 메세지

        //국내주식 TR Code
        string g_sTrcode_Sise = "stscur";       //현재가 TrCode
        string g_sTrcode_Hoga = "stpbid";       //호가 TrCode
        string g_sTrcode_Che = "msvstbid";      //체결시세 TrCode
        string g_sTrcode_Kwansim = "concern";   //관심종목FID TrCode
        string g_sTrcode_BSOrder = "OTD1101U";  //매수/매도 주문전송 TrCode
        string g_sTrcode_MCOrder = "OTD1102U";  //정정/취소 주문전송 TrCode
        string g_sTrcode_CheList = "OTD4105Q";  //체결/미체결 내역조회 TrCode
        string g_sTrcode_JanList = "OTD3108Q";  //잔고 내역조회 TrCode
        String g_sNextKey_Jan = "";             //잔고조회 연속조회키
        String g_sNextKey_Che = "";             //체결내역 연속조회키

        //해외주식 TR Code
        string g_sTrcode_gbSise = "gbmst";          //해외주식 현재가 TrCode
        string g_sTrcode_gbHoga = "gbpbid";         //해외주식 호가 TrCode
        string g_sTrcode_gbChe = "gbtick";          //해외주식 체결시세 TrCode
        string g_sTrcode_gbKwansim = "concern2";    //해외주식 관심종목FID TrCode
        string g_sTrcode_gbBSOrder = "OTD6101U";    //해외주식 매수/매도 주문전송 TrCode
        string g_sTrcode_gbMCOrder = "OTD6103U";    //해외주식 정정/취소 주문전송 TrCode
        string g_sTrcode_gbCheList = "OTD6214Q";    //해외주식 일자별 주문체결 내역조회 TrCode
        string g_sTrcode_gbMiCheList = "OTD6216Q";  //해외주식 당일 미체결 내역조회 TrCode
        string g_sTrcode_gbJanList = "OTD6209Q";    //해외주식 종목별 주식평가손익 조회 TrCode
        string g_sTrcode_gbAccInfo = "OCA1725Q";    //해외주식 예수금 조회 TrCode
        string g_sNextKey_gbJan = "";               //해외주식 잔고조회 연속조회키
        string g_sNextKey_gbChe = "";               //해외주식 체결내역 연속조회키

        //선물옵션 TR Code
        string g_sTrcode_FSise = "ffcur";           //지수선물 현재가+호가 시세 TrCode
        string g_sTrcode_OSise = "ogibon";          //지수옵션 현재가+호가 시세 TrCode
        string g_sTrcode_FChe = "msvifbid";         //지수선물 체결시세 TrCode
        string g_sTrcode_OChe = "msviobid";         //지수옵션 체결시세 TrCode
        string g_sTrcode_FOBSOrder = "OFU1101U";    //선물옵션 매수/매도 주문전송 TrCode
        string g_sTrcode_FOMCOrder = "OFU1102U";    //선물옵션 정정/취소 주문전송 TrCode
        string g_sTrcode_FOCheList = "OFU4772Q";    //선물옵션 체결/미체결 내역조회 TrCode
        string g_sTrcode_FOJanList = "OFU4793Q";    //선물옵션 잔고내역 조회 TrCode
        const string g_sTrcode_FOAccInfo = "OFU5628Q";    //선물옵션 예수금 조회 TrCode
        string g_sNextKey_FOJan = "";               //선물옵션 잔고조회 연속조회키
        string g_sNextKey_FOChe = "";               //선물옵션 체결내역 연속조회키

        //조건검색
        string g_sTrcode_007 = "msvf007";           //조건검색 리스트 조회(msvf007)
        string g_sTrcode_027 = "msvf027";           //조건검색 내용 조회(msvf027)
        string g_sTrcode_028 = "msvf028";           //조건검색 실시간 등록(msvf028)
        string g_sOldRealKey = "";                  //실시간 등록시 직전에 등록한 키값
        int g_nLastIdx = -1;                        //조건검색 리스트에서 마지막으로 선택한 인덱스

        public Form1(AxChampionCommAgentLib.AxChampionCommAgent axChampionCommAgent1, string strID)
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            this.axChampionCommAgent1 = axChampionCommAgent1;
            DynamicInitializeComponent();
            OnVersionCheckSuccess();
            OnLoginSuccess(strID);
        }

        //================================
        // 국내주식
        //================================
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;
            g_bMultiReal = false;
            String sJmCode = TB_JmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 1 || nMarketGb > 8)
            {
                MessageBox.Show("주식 종목코드를 확인 해주세요.");
                TB_JmCode.Select(0, sJmCode.Length);
                return;
            }

            Requset_Sise(sJmCode);  //현재가 시세 조회
            Requset_Hoga(sJmCode);  //호가 시세 조회
            Requset_Che(sJmCode);   //체결 시세 조회
        }

        //================================
        // 현재가 시세 조회
        //================================
        private void Requset_Sise(String sJmCode)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Sise, "InRec1", "SCODE", sJmCode);
            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_Sise, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("현재가 조회 요청 실패");
            }
        }

        //================================
        // 호가 시세 조회
        //================================
        private void Requset_Hoga(String sJmCode)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Hoga, "InRec1", "SCODE", sJmCode);

            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_Hoga, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("호가 조회 요청 실패");
            }
        }

        //================================
        // 체결 시세 조회
        //================================
        private void Requset_Che(String sJmCode)
        {
            LV_CheSise.Items.Clear();

            int nRqId = axChampionCommAgent1.CreateRequestID();

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Che, "InRec1", "CMODE", "A");
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Che, "InRec1", "SCODE", sJmCode);
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Che, "InRec1", "LTIME", "0");
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_Che, "InRec1", "LCNT", "20");

            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_Che, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("체결시세 조회 요청 실패");
            }
        }

        //================================
        // 현재가 시세조회 응답결과 처리
        //================================
        private void OnGetData_Sise()
        {
            String sOutPut = "";
            long nOutval = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "SNAME", 0); //종목명
            LB_JmName.Text = sOutPut.Trim();

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LCPRICE", 0);      //현재가
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LDIFF", 0);        //전일대비
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[1].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LVOLUME", 0);      //거래량
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[2].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LOPRICE", 0);      //시가    
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[3].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LHPRICE", 0);      //고가
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[4].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LLPRICE", 0);      //저가
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[5].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LULIMITPRICE", 0); //상한가
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[6].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LLLIMITPRICE", 0); //하한가
            nOutval = ToLong(sOutPut);
            LV_Sise.Items[7].SubItems[1].Text = String.Format("{0:#,###}", nOutval);

            //sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Sise, "OutRec1", "LLSTCPRICE", 0);   //전일종가
            //nOutval = ToLong(sOutPut);
            //LV_Sise.Items[8].SubItems[1].Text = String.Format("{0:#,###}", nOutval);
        }

        //================================
        // 호가 시세조회 응답결과 처리
        //================================
        private void OnGetData_Hoga()
        {
            String sOutPut = "";
            long nOutVal = 0;
            String sTime = "";      //호가 시간
            String sPrevPrc = "";   //전일종가
            String sCurrPrc = "";   //현재가

            sPrevPrc = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LLSTCPRICE", 0);      //전일종가
            sCurrPrc = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LCPRICE", 0);         //현재가

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LTOTOFFERPRE", 0);     //총매도잔량변동수량
            nOutVal = ToLong(sOutPut);
            LV_HogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LTOTOFFER", 0);        //총매도수량
            nOutVal = ToLong(sOutPut);
            LV_HogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LTIME", 0);            //시간
            sOutPut.Trim();
            if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
            sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
            LV_HogaTot.Items[0].SubItems[3].Text = sTime;

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LTOTBID", 0);          //총매수수량
            nOutVal = ToLong(sOutPut);
            LV_HogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", "LTOTBIDPRE", 0);       //총매수잔량변동수량
            nOutVal = ToLong(sOutPut);
            LV_HogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,###}", nOutVal);

            String sOutRecItem; //아웃풋 아이템명
            String sIdx;        //아웃풋 아이템명 순번
            int nAskRow;
            int nBidRow;
            nAskRow = 9;
            nBidRow = 0;

            //1호가 ~ 10호가 처리
            for (int i = 1; 1 <= 10; i++)
            {
                //2호가부터 Row위치 변경
                if (i > 1)
                {
                    nAskRow = nAskRow - 1;
                    nBidRow = nBidRow + 1;
                }

                sIdx = i.ToString();
                sOutRecItem = "LOFFER" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매도 호가
                nOutVal = ToLong(sOutPut);
                LV_MedoHoga.Items[nAskRow].SubItems[3].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LOFFERREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매도 잔량
                nOutVal = ToLong(sOutPut);
                LV_MedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LOFFERRESTPRE" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매도직전잔량
                nOutVal = ToLong(sOutPut);
                LV_MedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBID" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매수 호가
                nOutVal = ToLong(sOutPut);
                LV_MesuHoga.Items[nBidRow].SubItems[3].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBIDREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매수 잔량
                nOutVal = ToLong(sOutPut);
                LV_MesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBIDRESTPRE" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Hoga, "OutRec1", sOutRecItem, 0); //매수직전잔량
                nOutVal = ToLong(sOutPut);
                LV_MesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nOutVal);
            }
        }

        //================================
        // 체결 시세조회 응답결과 처리
        //================================
        private void OnGetData_Che()
        {
            String sOutPut = "";
            long nOutVal = 0;
            String sTime = "";

            long nDataCnt = 0;
            nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt(g_sTrcode_Che, "OutRec2"); //조회건수
            if (nDataCnt == 0) return;

            LV_CheSise.Items.Clear();

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Che, "OutRec2", "LTIME", i); //시간
                sOutPut.Trim();
                nOutVal = ToLong(sOutPut);  // Trim 이 안되는 경우가 발생하여 숫자 형변환으로 처리 추가
                sOutPut = nOutVal.ToString();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                item.SubItems.Add(sTime);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Che, "OutRec2", "LCPRICE", i); //현재가(체결가)
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,###}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Che, "OutRec2", "LNETVOLUME", i); //체결량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,###}", nOutVal));

                LV_CheSise.Items.Add(item);
            }
        }

        //================================
        // TR 조회 응답 처리
        //================================
        private void axChampionCommAgent1_OnGetTranData(object sender, AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetTranDataEvent e)
        {
            String sTrCode = axChampionCommAgent1.GetCommRecvOptionValue(0);    // TR 코드
            String sNextGb = axChampionCommAgent1.GetCommRecvOptionValue(1);    // 이전/다음 조회구분(0:없음, 4:다음없음, 5:다음없음, 6:다음있음, 7:다음있음)
            String sNextKey = axChampionCommAgent1.GetCommRecvOptionValue(2);    // 연속조회키
            String sMsg = axChampionCommAgent1.GetCommRecvOptionValue(4);    // 응답 메세지
            String sSubMsg = axChampionCommAgent1.GetCommRecvOptionValue(5);    // 부가 메세지
            String sErrCode = axChampionCommAgent1.GetCommRecvOptionValue(7);    // 에러여부

            //국내주식 TR
            if (sTrCode == g_sTrcode_Sise)
                OnGetData_Sise();
            else if (sTrCode == g_sTrcode_Hoga)
                OnGetData_Hoga();
            else if (sTrCode == g_sTrcode_Che)
                OnGetData_Che();
            else if (sTrCode == g_sTrcode_CheList)
                OnGetData_CheList(sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_JanList)
                OnGetData_JanList(sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_BSOrder || sTrCode == g_sTrcode_MCOrder)
            {
                String sOrdNo;
                sOrdNo = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "ORD_NO", 0);   //주문번호
                if (sOrdNo.Trim().Length > 0)
                {
                    TB_OrdNo.Text = sOrdNo;
                    Requset_CheList("");  //체결내역 조회
                    Requset_JanList("");  //잔고내역 조회
                }
            }
            //해외주식 TR
            else if (sTrCode == g_sTrcode_gbSise)
                OnGetData_GBSise();
            else if (sTrCode == g_sTrcode_gbHoga)
                OnGetData_GBHoga();
            else if (sTrCode == g_sTrcode_gbChe)
                OnGetData_GBChe();
            else if (sTrCode == g_sTrcode_gbAccInfo)    //예수금 조회
                OnGetData_GBAccInfo();
            else if (sTrCode == g_sTrcode_gbCheList || sTrCode == g_sTrcode_gbMiCheList)
                OnGetData_GBCheList(sTrCode, sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_gbJanList)
                OnGetData_GBJanList(sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_gbBSOrder || sTrCode == g_sTrcode_gbMCOrder)
            {
                String sOrdNo;
                sOrdNo = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "ORD_NO", 0);   //주문번호
                if (sOrdNo.Trim().Length > 0)
                {
                    TB_GBOrdNo.Text = sOrdNo;
                    Requset_GBAccInfo();                //계좌 예수금 조회
                    Btn_GBCheList_Click(null, null);    //해외주식 체결/미체결 내역 조회
                    Requset_GBJanList("");              //해외주식 잔고내역 조회
                }
            }
            //선물옵션 TR
            else if (sTrCode == g_sTrcode_FSise || sTrCode == g_sTrcode_FSise)
                OnGetData_FOSise(sTrCode);
            else if (sTrCode == g_sTrcode_FChe || sTrCode == g_sTrcode_OChe)
                OnGetData_FOChe(sTrCode);
            else if (sTrCode == g_sTrcode_FOCheList)
                OnGetData_FOCheList(sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_FOJanList)
                OnGetData_FOJanList(sNextGb, sNextKey);
            else if (sTrCode == g_sTrcode_FOBSOrder || sTrCode == g_sTrcode_FOMCOrder)
            {
                String sOrdNo;
                sOrdNo = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "ORD_NO", 0);   //주문번호
                if (sOrdNo.Trim().Length > 0)
                {
                    TB_FOOrdNo.Text = sOrdNo;
                    Requset_FOCheList("");  //선물옵션 체결내역 조회
                    Requset_FOJanList("");  //선물옵션 잔고내역 조회
                }
            }
            else if (sTrCode == g_sTrcode_007)
                OnGetData_007();
            else if (sTrCode == g_sTrcode_027)
                OnGetData_027();
            else if (sTrCode == g_sTrcode_028)
                OnGetData_028();
        }

        //================================
        // FID 조회 응답 처리
        //================================
        private void axChampionCommAgent1_OnGetFidData(object sender, AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetFidDataEvent e)
        {
            String sTrCode = axChampionCommAgent1.GetCommRecvOptionValue(0);    // TR 코드
            String sNextGb = axChampionCommAgent1.GetCommRecvOptionValue(1);    // 이전/다음 조회구분(0:없음, 4:다음없음, 5:다음없음, 6:다음있음, 7:다음있음)
            String sNextKey = axChampionCommAgent1.GetCommRecvOptionValue(2);    // 연속조회키
            String sMsg = axChampionCommAgent1.GetCommRecvOptionValue(4);    // 응답 메세지
            String sSubMsg = axChampionCommAgent1.GetCommRecvOptionValue(5);    // 부가 메세지
            String sErrCode = axChampionCommAgent1.GetCommRecvOptionValue(7);    // 에러여부

            String sOutPut = "";
            long nOutVal = 0;

            String sCode = "";
            String sRate = "";
            double fRate = 0.0f;

            if (sTrCode == g_sTrcode_Kwansim)   // 국내주식 관심종목(FID)조회
            {
                LV_Kwansim.Items.Clear();

                int nDataCnt = axChampionCommAgent1.GetFidOutputRowCnt(e.nRequestId);   //조회건수
                if (nDataCnt == 0) return;

                ListViewItem item = null;
                for (int i = 0; i < nDataCnt; i++)
                {
                    item = new ListViewItem();

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "153", i);    // 표준코드로 조회
                    sCode = axChampionCommAgent1.GetShCode(sOutPut.Trim());    // 표준코드를 단축코드로 변환
                    item.SubItems.Add(sCode);

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "154", i);    //종목명
                    item.SubItems.Add(sOutPut.Trim());

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "000", i);    //현재가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "002", i);    //전일대비
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "003", i);    //등락률(소수점 미포함)
                    fRate = Math.Round(ToLong(sOutPut) / 100.0f, 2);
                    sRate = fRate.ToString() + "%";
                    item.SubItems.Add(sRate);

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "004", i);    //거래량
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "011", i);    //상한가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "012", i);    //하한가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "013", i);    //시가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "014", i);    //고가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "015", i);    //저가
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    LV_Kwansim.Items.Add(item);
                }
            }
            else if (sTrCode == g_sTrcode_gbKwansim)    // 해외주식 관심종목(FID)조회
            {
                double fOutVal = 0.0f;
                String sPlaces = "", sFormat = "";

                LV_GBKwansim.Items.Clear();

                int nDataCnt = axChampionCommAgent1.GetFidOutputRowCnt(e.nRequestId);   //조회건수
                if (nDataCnt == 0) return;

                ListViewItem item = null;
                for (int i = 0; i < nDataCnt; i++)
                {
                    item = new ListViewItem();

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "153", i);    // 종목코드
                    sCode = axChampionCommAgent1.GetShCode(sOutPut.Trim());    // 표준코드를 단축코드로 변환
                    item.SubItems.Add(sCode);

                    sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sOutPut.Trim(), 11);    // 소숫점 자리수
                    sFormat = "{0:F" + sPlaces + "}";   //소숫점 자리수에 맞게 포멧 적용

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "154", i);    //종목명
                    item.SubItems.Add(sOutPut.Trim());

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "000", i);    //현재가
                    fOutVal = ToLong(sOutPut) / 10000.0f;
                    item.SubItems.Add(String.Format(sFormat, fOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "002", i);    //전일대비
                    fOutVal = ToLong(sOutPut) / 10000.0f;
                    item.SubItems.Add(String.Format(sFormat, fOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "003", i);    //등락률(소수점 미포함)
                    fRate = Math.Round(ToLong(sOutPut) / 100.0f, 2);
                    sRate = fRate.ToString() + "%";
                    item.SubItems.Add(sRate);

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "004", i);    //거래량
                    nOutVal = ToLong(sOutPut);
                    item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                    //                     sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "011", i);    //상한가
                    //                     fOutVal = ToLong(sOutPut) / 10000.0f;
                    //                     item.SubItems.Add(String.Format(sFormat, fOutVal));
                    // 
                    //                     sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "012", i);    //하한가
                    //                     fOutVal = ToLong(sOutPut) / 10000.0f;
                    //                     item.SubItems.Add(String.Format(sFormat, fOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "013", i);    //시가
                    fOutVal = ToLong(sOutPut) / 10000.0f;
                    item.SubItems.Add(String.Format(sFormat, fOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "014", i);    //고가
                    fOutVal = ToLong(sOutPut) / 10000.0f;
                    item.SubItems.Add(String.Format(sFormat, fOutVal));

                    sOutPut = axChampionCommAgent1.GetFidOutputData(e.nRequestId, "015", i);    //저가
                    fOutVal = ToLong(sOutPut) / 10000.0f;
                    item.SubItems.Add(String.Format(sFormat, fOutVal));

                    LV_GBKwansim.Items.Add(item);
                }
            }
        }

        //================================
        // 호가 시세 실시간 수신 처리([1]S01)
        //================================
        private void OnGetRealData_Hoga(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            String sRealItem = ""; //실시간 아이템명
            String sIdx = "";      //실시간 아이템명 순번
            String sTime = "";     //호가 시간

            if (g_bMultiReal == true)
            {
                String sData;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "SCODE");        // 표준코드
                sRealData.Trim();
                sData = "표준코드 : " + sRealData + " Data : ";

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LBID1");        // 매수호가1
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LOFFER1");        // 매도호가1
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFERREST");        // 총매도호가잔량
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTBIDREST");        // 총매수호가잔량
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                MultiHoga.Text = sData;
            }
            else
            {
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "SCODE");    //표준종목코드

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOT_PREOFFERCHA"); //총매도잔량변동수량
                nRealVal = ToLong(sRealData);
                LV_HogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFERREST");    //총매도수량
                nRealVal = ToLong(sRealData);
                LV_HogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTIME");    //시간
                sRealData.Trim();
                if (sRealData.Length == 5) sRealData = "0" + sRealData;
                sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);
                LV_HogaTot.Items[0].SubItems[3].Text = sTime;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTBIDREST");      //총매수수량
                nRealVal = ToLong(sRealData);
                LV_HogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOT_PREBIDCHA");   //총매수잔량변동수량
                nRealVal = ToLong(sRealData);
                LV_HogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,###}", nRealVal);

                int nAskRow = 9;
                int nBidRow = 0;

                for (int i = 1; 1 <= 10; i++)
                {
                    //2호가부터 Row위치 변경
                    if (i > 1)
                    {
                        nAskRow = nAskRow - 1;
                        nBidRow = nBidRow + 1;
                    }

                    sIdx = i.ToString();

                    sRealItem = "LOFFER" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 호가
                    nRealVal = ToLong(sRealData);
                    LV_MedoHoga.Items[nAskRow].SubItems[3].Text = String.Format("{0:#,###}", nRealVal);

                    sRealItem = "LOFFERREST" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 잔량
                    nRealVal = ToLong(sRealData);
                    LV_MedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nRealVal);

                    sRealItem = "LPREOFFERCHA" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도직전잔량
                    nRealVal = ToLong(sRealData);
                    LV_MedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                    sRealItem = "LBID" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 호가
                    nRealVal = ToLong(sRealData);
                    LV_MesuHoga.Items[nBidRow].SubItems[3].Text = String.Format("{0:#,###}", nRealVal);

                    sRealItem = "LBIDREST" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 잔량
                    nRealVal = ToLong(sRealData);
                    LV_MesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nRealVal);

                    sRealItem = "LPREBIDCHA" + sIdx;
                    sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 잔량
                    nRealVal = ToLong(sRealData);
                    LV_MesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nRealVal);
                }
            }
        }

        //================================
        // 체결 시세 실시간 수신 처리([21]S00)
        //================================
        private void OnGetRealData_Che(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            String sTime = "";     //호가 시간

            int nRowCnt = LV_CheSise.Items.Count;
            if (nRowCnt > 19)
                LV_CheSise.Items.RemoveAt(nRowCnt - 1);

            if (g_bMultiReal == true)
            {
                String sData;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "SCODE");        // 표준코드
                sRealData.Trim();
                sData = "표준코드 : " + sRealData + " Data : ";

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCPRICE");        // 현재가
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCURVOLUME");        // 체결량
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LDIFFRATE");        // 등락율
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCURSTRENGTH");        // 체결강도
                sRealData.Trim();
                sData = sData + " , " + sRealData;

                MultiChegyul.Text = sData;
            }
            else
            {
                ListViewItem item = null;
                item = new ListViewItem();

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTIME");        //시간
                sRealData.Trim();
                if (sRealData.Length == 5) sRealData = "0" + sRealData;
                sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);
                item.SubItems.Add(sTime);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCPRICE");      //현재가
                nRealVal = ToLong(sRealData);
                item.SubItems.Add(String.Format("{0:#,###}", nRealVal));
                LV_Sise.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCURVOLUME");   //체결량
                nRealVal = ToLong(sRealData);
                item.SubItems.Add(String.Format("{0:#,###}", nRealVal));

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LDIFF");        //전일대비
                nRealVal = ToLong(sRealData);
                LV_Sise.Items[1].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LVOLUME");      //누적거래량
                nRealVal = ToLong(sRealData);
                LV_Sise.Items[2].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LOPRICE");      //시가
                nRealVal = ToLong(sRealData);
                LV_Sise.Items[3].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LHPRICE");      //고가
                nRealVal = ToLong(sRealData);
                LV_Sise.Items[4].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LLPRICE");      //저가
                nRealVal = ToLong(sRealData);
                LV_Sise.Items[5].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                LV_CheSise.Items.Insert(0, item);
            }
        }

        //================================
        // 주문 체결/미체결 실시간 통보 처리([191]RSC)
        //================================
        private void OnGetRealData_CheList(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;

            String sOrdNo = "";
            String sJmCode = "";
            String sJmName = "";
            String sOrdTypeNm = "";
            String sTradeGb = "";
            String sTime = "";
            long nOrdPrc = 0;
            long nOrdQty = 0;
            long nCheQty = 0;
            long nMiCheQty = 0;

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORDERNO");        //주문번호
            nRealVal = ToLong(sRealData);   // 앞에 "0"제거
            sOrdNo = nRealVal.ToString();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "EXPCODE");      //종목코드(표준코드)
            sRealData.Trim();
            sJmCode = axChampionCommAgent1.GetShCode(sRealData);    //단축코드로 변환

            sJmName = axChampionCommAgent1.GetNameByCode(sJmCode); //종목코드로 종목명 찾기

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MEDOSU");   //매매구분(매수,매도)
            sRealData.Trim();
            sTradeGb = (sRealData == "1") ? "매도" : "매수";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JPRC");        //주문가격
            nOrdPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JQTY");      //주문수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHEQTY");      //체결수량
            nCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MICHEQTY");      //미체결수량
            nMiCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MEMEGB");        //매매구분
            sRealData.Trim();
            if (sRealData == "010")
                sOrdTypeNm = "지정가";
            else if (sRealData == "020")
                sOrdTypeNm = "시장가";
            else if (sRealData == "030")
                sOrdTypeNm = "조건부지정가";
            else if (sRealData == "040")
                sOrdTypeNm = "최유리지정가";
            else if (sRealData == "050")
                sOrdTypeNm = "최우선지정가";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHETIME");        //주문/체결시간
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문번호 : " + sOrdNo + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매매구분 : " + sTradeGb + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문유형 : " + sOrdTypeNm + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문가격 : " + String.Format("{0:#,###}", nOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결수량 : " + String.Format("{0:#,###}", nCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "미체결수량 : " + String.Format("{0:#,###}", nMiCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결시간 : " + sTime;

            MessageBox.Show(sRealMsg, "[주문 체결/미체결 실시간 통보 확인]");

        }

        //================================
        // 잔고 실시간 통보 처리([192]RSJ)
        //================================
        private void OnGetRealData_JanList(short nPBID)
        {
            String sRealData = "";

            String sJmCode = "";
            String sJmName = "";
            long nJanQty = 0;   //잔고수량(보유수량)
            long nOrdQty = 0;   //주문가능수량
            long nOrdPrc = 0;   //매입단가
            long nCurPrc = 0;   //현재가
            long nSonick = 0;   //평가손익

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ITEM_COD");  //종목코드(표준코드)
            sRealData.Trim();
            sJmCode = axChampionCommAgent1.GetShCode(sRealData);    //단축코드로 변환

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "HNAME");  //종목명
            sJmName = sRealData.Trim();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BQTY");      //보유수량
            nJanQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORDGAQTY");  //주문가능수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BUYAMT");    //매입단가
            nOrdPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "PRICE");     //현재가
            nCurPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ESTSONIK");  //평가손익
            nSonick = ToLong(sRealData);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "보유수량 : " + String.Format("{0:#,###}", nJanQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문가능수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매입단가 : " + String.Format("{0:#,###}", nOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "현 재 가 : " + String.Format("{0:#,###}", nCurPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "평가손익 : " + String.Format("{0:#,###}", nSonick) + System.Environment.NewLine;
            MessageBox.Show(sRealMsg, "[잔고 실시간 통보 확인]");
        }

        private void Btn_SetReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제

            String sJmCode = TB_JmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 1 || nMarketGb > 8)
            {
                MessageBox.Show("주식 종목코드를 확인 해주세요.");
                return;
            }

            axChampionCommAgent1.RegisterReal(1, sJmCode);  //주식 종목 우선호가
            axChampionCommAgent1.RegisterReal(21, sJmCode); //주식,ELW 종목 체결시세
            g_bSetReal = true;
        }

        private void Btn_UnReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;
            g_bMultiReal = false;
            // 개별 실시간 해제
            //String sJmCode = TB_JmCode.Text;
            //if (sJmCode.Length == 0)
            //{
            //    MessageBox.Show("종목코드를 입력 해주세요.");
            //    return;
            //}
            //
            //axChampionCommAgent1.UnRegisterReal(1, sJmCode);
            //axChampionCommAgent1.UnRegisterReal(21, sJmCode);
        }

        //================================
        // 실시간 응답 처리
        //================================
        private void axChampionCommAgent1_OnGetRealData(object sender, AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetRealDataEvent e)
        {
            // 시세 실시간
            if (e.nPBID == 1)    //주식종목 우선호가(S01)
            {
                OnGetRealData_Hoga(e.nPBID);
            }
            else if (e.nPBID == 21)  //주식,ELW종목 체결시세(S00)
            {
                OnGetRealData_Che(e.nPBID);
            }
            else if (e.nPBID == 230) //해외주식 체결시세(G00)
            {
                OnGetRealData_GBChe(e.nPBID);
            }
            else if (e.nPBID == 231) //해외주식 호가(G01)
            {
                OnGetRealData_GBHoga(e.nPBID);
            }
            else if (e.nPBID == 51 || e.nPBID == 52) //지수선물 호가(F01) / 지수선물 체결(O01)
            {
                OnGetRealData_FOHoga(e.nPBID);
            }
            else if (e.nPBID == 65 || e.nPBID == 66) //지수선물 호가(F01) / 지수선물 체결(O01)
            {
                OnGetRealData_FOChe(e.nPBID);
            }

            // 주문체결 실시간
            if (e.nPBID == 191)    //주문 체결/미체결 실시간 통보
            {
                OnGetRealData_CheList(e.nPBID);
            }
            else if (e.nPBID == 192)    //주문 잔고 실시간 통보
            {
                OnGetRealData_JanList(e.nPBID);
            }
            else if (e.nPBID == 193)    //선옵 체결/미체결 실시간 통보
            {
                OnGetRealData_FOCheList(e.nPBID);
            }
            else if (e.nPBID == 194)    //선옵 잔고 실시간 통보
            {
                OnGetRealData_FOJanList(e.nPBID);
            }
            else if (e.nPBID == 204)    //해외주식 주문 체결/미체결 실시간 통보
            {
                OnGetRealData_GBCheList(e.nPBID);
            }
            else if (e.nPBID == 205)    //해외주깃 주문 잔고 실시간 통보
            {
                OnGetRealData_GBJanList(e.nPBID);
            }

            //조건검색
            if (e.nPBID == 154)
            {
                String sRealData, sJmCode, sJmName, sRealMsg;
                long nCurPrc, nOpenPrc, nHighPrc, nLowPrc, nVolume;

                sJmCode = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sItemCode");
                sJmName = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sItemName");

                sRealData = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sPrice");
                nCurPrc = ToLong(sRealData);

                sRealData = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sOpen");
                nOpenPrc = ToLong(sRealData);

                sRealData = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sHigh");
                nHighPrc = ToLong(sRealData);

                sRealData = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sLow");
                nLowPrc = ToLong(sRealData);

                sRealData = axChampionCommAgent1.GetRealOutputData(e.nPBID, "sVolume");
                nVolume = ToLong(sRealData);

                sRealMsg = "종   목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
                sRealMsg = sRealMsg + "현재가 : " + String.Format("{0:#,###}", nCurPrc) + System.Environment.NewLine;
                sRealMsg = sRealMsg + "시   가 : " + String.Format("{0:#,###}", nOpenPrc) + System.Environment.NewLine;
                sRealMsg = sRealMsg + "고   가 : " + String.Format("{0:#,###}", nHighPrc) + System.Environment.NewLine;
                sRealMsg = sRealMsg + "저   가 : " + String.Format("{0:#,###}", nLowPrc) + System.Environment.NewLine;
                sRealMsg = sRealMsg + "거래량 : " + String.Format("{0:#,###}", nVolume) + System.Environment.NewLine;
                TB_RealMsg.Text = sRealMsg;
            }
        }

        private void Btn_Kwansim_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sFidNums = "153,154,000,002,003,004,011,012,013,014,015";        //Fid조회 항목 리스트
            String sFidCodes = "006400,373220,003670,000660,005930,000270,035420,035720,024110,001040";    //Fid조회 종목코드 리스트

            byte chGubun = Convert.ToByte(',');
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = axChampionCommAgent1.RequestPortfolioFid(nRqId, g_sTrcode_Kwansim, sFidNums, sFidCodes, chGubun, 10);
            if (nRtn < 1)
            {
                MessageBox.Show("관심종목 FID조회 요청 실패");
            }

        }

        private void TB_JmCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Btn_Search_Click(null, null);
            }
        }

        private void TB_AccPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (g_bLoginYN == false)
                {
                    MessageBox.Show("로그인 상태를 확인 바랍니다.");
                    return;
                }

                Requset_CheList("");  //체결/미체결 내역 조회
                Requset_JanList("");  //잔고내역 조회
            }
        }

        private void Btn_BuyOrd_Click(object sender, EventArgs e)
        {
            SendBSOrder(true);  // T:Buy, F:Sell
        }

        private void Btn_SellOrd_Click(object sender, EventArgs e)
        {
            SendBSOrder(false); // T:Buy, F:Sell
        }

        private void Btn_ModifyOrd_Click(object sender, EventArgs e)
        {
            SendMCOrder(true);  // T:Modify, F:Cancel
        }

        private void Btn_CancelOrd_Click(object sender, EventArgs e)
        {
            SendMCOrder(false);  // T:Modify, F:Cancel
        }


        //================================
        // 매수/매도 주문 전송
        //================================
        private void SendBSOrder(bool bBuy)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sAccNo = "";     //계좌번호
            String sAccPwd = "";    //계좌비번
            String sOrdType = "";   //주문유형
            String sOrdQty = "";    //주문수량
            String sOrdPrc = "";    //주문가격
            String sTradeGb = "";   //매매구분(20:매수, 10:매도)
            String sJmCode = "";    //종목코드

            //매매구분(20:매수, 10:매도)
            if (bBuy == true)
                sTradeGb = "20";
            else
                sTradeGb = "10";

            sJmCode = TB_OrdJmCode.Text;
            sAccNo = TB_AccNo.Text;
            sAccPwd = TB_AccPwd.Text;
            sOrdType = CB_OrdType.Text;
            sOrdQty = UD_OrdQty.Text;
            sOrdPrc = TB_OrdPrc.Text;

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 1 || nMarketGb > 8)
            {
                MessageBox.Show("주식 종목코드를 확인 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            String sOrdCond = "";

            if (sOrdType == "지정가")
            {
                sOrdTypeCode = "010";
                sOrdCond = "0";
            }
            else if (sOrdType == "시장가")
            {
                sOrdTypeCode = "020";
                sOrdCond = "0";
            }
            else if (sOrdType == "조건부지정가")
            {
                sOrdTypeCode = "030";
                sOrdCond = "0";
            }
            else if (sOrdType == "최유리지정가")
            {
                sOrdTypeCode = "040";
                sOrdCond = "0";
            }
            else if (sOrdType == "최우선지정가")
            {
                sOrdTypeCode = "050";
                sOrdCond = "0";
            }
            else if (sOrdType == "지정가(IOC)")
            {
                sOrdTypeCode = "010";
                sOrdCond = "1";
            }
            else if (sOrdType == "시장가(IOC)")
            {
                sOrdTypeCode = "020";
                sOrdCond = "1";
            }
            else if (sOrdType == "최유리(IOC)")
            {
                sOrdTypeCode = "040";
                sOrdCond = "1";
            }
            else if (sOrdType == "지정가(FOK)")
            {
                sOrdTypeCode = "010";
                sOrdCond = "2";
            }
            else if (sOrdType == "시장가(FOK)")
            {
                sOrdTypeCode = "020";
                sOrdCond = "2";
            }
            else if (sOrdType == "최유리(FOK)")
            {
                sOrdTypeCode = "040";
                sOrdCond = "2";
            }
            else if (sOrdType == "장전시간외")
            {
                sOrdTypeCode = "210";
                sOrdCond = "0";
            }
            else if (sOrdType == "장후시간외")
            {
                sOrdTypeCode = "280";
                sOrdCond = "0";
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "AC_PWD", sAccPwd);    //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "ITEM_COD", sJmCode);         //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "ORD_Q", sOrdQty);            //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "STK_BD_ORD_UPR", sOrdPrc);   //주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "BUY_SEL_TR_TCD", sTradeGb);  //매매구분(10:매수, 20:매도)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "ORD_BNS_TCD", sOrdTypeCode); //주문유형구분
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_BSOrder, "InRec1", "ORD_COND_TCD", sOrdCond);    //주문조건구분

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_BSOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "매수/매도 주문전송 실패");
                return;
            }
        }

        //================================
        // 정정/취소 주문 전송
        //================================
        private void SendMCOrder(bool bModify)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sAccNo = "";     //계좌번호
            String sAccPwd = "";    //계좌비번
            String sOrdType = "";   //주문유형
            String sOrdQty = "";    //주문수량
            String sOrdPrc = "";    //주문가격
            String sTradeGb = "";   //매매구분(20:정정, 30:취소)
            String sJmCode = "";    //종목코드
            String sOrgOrdNo = "";  //원주문번호

            //매매구분(20:정정, 30:취소)
            if (bModify == true)
                sTradeGb = "20";
            else
                sTradeGb = "30";

            sJmCode = TB_OrdJmCode.Text;
            sAccNo = TB_AccNo.Text;
            sAccPwd = TB_AccPwd.Text;
            sOrdType = CB_OrdType.Text;
            sOrdQty = UD_OrdQty.Text;
            sOrdPrc = TB_OrdPrc.Text;
            sOrgOrdNo = TB_OrgOrdNo.Text;

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 1 || nMarketGb > 8)
            {
                MessageBox.Show("주식 종목코드를 확인 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            if (sOrgOrdNo.Trim().Length == 0)
            {
                MessageBox.Show("원주문번호를 입력 해주세요.");
                return;
            }

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            String sOrdCond = "";

            if (sOrdType == "지정가")
            {
                sOrdTypeCode = "010";
                sOrdCond = "0";
            }
            else if (sOrdType == "시장가")
            {
                sOrdTypeCode = "020";
                sOrdCond = "0";
            }
            else if (sOrdType == "조건부지정가")
            {
                sOrdTypeCode = "030";
                sOrdCond = "0";
            }
            else if (sOrdType == "최유리지정가")
            {
                sOrdTypeCode = "040";
                sOrdCond = "0";
            }
            else if (sOrdType == "최우선지정가")
            {
                sOrdTypeCode = "050";
                sOrdCond = "0";
            }
            else if (sOrdType == "지정가(IOC)")
            {
                sOrdTypeCode = "010";
                sOrdCond = "1";
            }
            else if (sOrdType == "시장가(IOC)")
            {
                sOrdTypeCode = "020";
                sOrdCond = "1";
            }
            else if (sOrdType == "최유리(IOC)")
            {
                sOrdTypeCode = "040";
                sOrdCond = "1";
            }
            else if (sOrdType == "지정가(FOK)")
            {
                sOrdTypeCode = "010";
                sOrdCond = "2";
            }
            else if (sOrdType == "시장가(FOK)")
            {
                sOrdTypeCode = "020";
                sOrdCond = "2";
            }
            else if (sOrdType == "최유리(FOK)")
            {
                sOrdTypeCode = "040";
                sOrdCond = "2";
            }
            else if (sOrdType == "장전시간외")
            {
                sOrdTypeCode = "210";
                sOrdCond = "0";
            }
            else if (sOrdType == "장후시간외")
            {
                sOrdTypeCode = "280";
                sOrdCond = "0";
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ACNO", sAccNo);           //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "AC_PWD", sAccPwd);        //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "OORD_NO", sOrgOrdNo);            //원주문번호
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ORD_MDFY_CNCL_TCD", sTradeGb);   //매매구분(20:정정, 30:취소)    
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "PAT_ALL_TCD", "20");             //일부/전부 구분(10:일부, 20:전부)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ITEM_COD", sJmCode);             //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ORD_Q", sOrdQty);                //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "STK_BD_ORD_UPR", sOrdPrc);       //주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ORD_BNS_TCD", sOrdTypeCode);     //주문유형구분  
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_MCOrder, "InRec1", "ORD_COND_TCD", sOrdCond);        //주문조건구분

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_MCOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "정정/취소 주문전송 실패");
                return;
            }
        }

        private void Btn_CheList_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            g_sNextKey_Che = "";
            Requset_CheList(g_sNextKey_Che);
        }

        private void Btn_CheNext_Click(object sender, EventArgs e)
        {
            Requset_CheList(g_sNextKey_Che);
        }

        private void Btn_JanList_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            g_sNextKey_Jan = "";
            Requset_JanList(g_sNextKey_Jan);
        }

        private void Btn_JanNext_Click(object sender, EventArgs e)
        {
            Requset_JanList(g_sNextKey_Jan);
        }

        //================================
        // 체결/미체결 내역 조회 요청
        //================================
        private void Requset_CheList(String sNextKey)
        {
            g_sNextKey_Che = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_Che.Trim().Length == 0)
            {
                LV_CheList.Items.Clear();   //체결/미체결내역 초기화
            }

            String sAccNo; //계좌번호
            String sAccPwd; //계좌비번
            sAccNo = TB_AccNo.Text;
            sAccPwd = TB_AccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();

            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "ACNO", sAccNo);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "AC_PWD", sAccPwd);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "ORD_NO", "");              //주문번호(연속조최 키값)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "BUY_SEL_TR_TCD", "%");     //조회구분(%:전체, 10:매도, 20:매수)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "ITEM_COD", "%");           //종목코드(%:전체)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "SORT_TURN_IO1CD", "2");    //주문번호를 기준으로 정순/역순 조회(1:정순, 2:역순)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_CheList, "InRec1", "SCR_QRY_TCD", "01");       //조회구분(01:전체, 02:미체결)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_CheList, g_sNextKey_Che, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "체결내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 잔고내역 조회 요청
        //================================
        private void Requset_JanList(String sNextKey)
        {
            g_sNextKey_Jan = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_Jan.Trim().Length == 0)
            {
                LV_JanList.Items.Clear();   //체결/미체결내역 초기화
            }

            String sAccNo;    //계좌번호
            String sAccPwd;   //계좌비번
            sAccNo = TB_AccNo.Text;
            sAccPwd = TB_AccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_JanList, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_JanList, "InRec1", "AC_PWD", sAccPwd);    //계좌 비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_JanList, "InRec1", "CMSN_ICLN_YN", "Y");         //수수료포함여부(Y:포함, N:미포함)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_JanList, g_sNextKey_Jan, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "잔고내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 체결/미체결 내역 조회 응답결과 처리
        //================================
        private void OnGetData_CheList(String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_CheNext.Enabled = true;
                g_sNextKey_Che = sNextKey;
            }
            else
            {
                Btn_CheNext.Enabled = false;
                g_sNextKey_Che = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;
            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            String sOrdTypeNm = "";     //주문유형
            String sTradeGb = "";       //매매구분
            String sOrdTime = "";       //주문시간

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ITEM_ABBR_NM", i); //종목명
                item.SubItems.Add(sOutPut.Trim());

                //매매구분(매수,매도)(첨부문서참조)
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_KCD ", i);
                if (sOutPut == "0010")
                    sTradeGb = "매도";
                else if (sOutPut == "0020")
                    sTradeGb = "매수";

                item.SubItems.Add(sTradeGb);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_UPR", i);  //주문단가
                fOutVal = ToDouble(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_Q", i);    //주문수량
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_RQ", i);   //미체결수량
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_NO", i);   //주문번호
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "OORD_NO", i);  //원주문번호
                item.SubItems.Add(sOutPut.Trim());

                //주문유형(10:지정가, 20:시장가, 30:조건부지정가, 40:최유리지정가, 50:최우선지정가)(첨부문서참조)
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_BNS_TCD ", i);
                if (sOutPut == "010")
                    sOrdTypeNm = "지정가";
                else if (sOutPut == "020")
                    sOrdTypeNm = "시장가";
                else if (sOutPut == "030")
                    sOrdTypeNm = "조건부지정가";
                else if (sOutPut == "040")
                    sOrdTypeNm = "최유리지정가";
                else if (sOutPut == "050")
                    sOrdTypeNm = "최우선지정가";

                item.SubItems.Add(sOrdTypeNm);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_CheList, "OutRec2", "ORD_DTTM", i); //주문시간
                sOutPut.Trim();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sOrdTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                item.SubItems.Add(sOrdTime);

                LV_CheList.Items.Add(item);
            }
        }


        //================================
        // 잔고 내역 조회 응답결과 처리
        //================================
        private void OnGetData_JanList(String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_JanNext.Enabled = true;
                g_sNextKey_Jan = sNextKey;
            }
            else
            {
                Btn_JanNext.Enabled = false;
                g_sNextKey_Jan = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;

            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "ITEM_NM", i);      //종목명
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "BNS_BAL_Q", i);    //보유수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "BNS_ABLE_Q", i);   //매도가능수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "BUY_UPR", i);      //매입단가
                fOutVal = ToDouble(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "STK_CRPR", i);     //현재가
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "EV_PL_A", i);      //평가손익
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "STK_EA", i);       //평가금액
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                //sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "ERN_R", i);        //수익률
                //sRate = string.Format("{0:0.00}%", ToDouble(sOutPut));
                //item.SubItems.Add(sRate);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_JanList, "OutRec2", "ACBK_A", i);       //매입금액
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                LV_JanList.Items.Add(item);
            }
        }

        //================================
        // 해외주식
        //================================
        private void TB_GBJmCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Btn_GBSearch_Click(null, null);
            }
        }

        private void Btn_GBSearch_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;

            // ※ 해외주식 종목코드 입력방법
            // - 거래소코드 4자리 + 종목코드 16자리
            // - 거래소코드 정의
            //     "0321" : 미국뉴욕종목
            //     "0066" : 미국아멕스종목
            //     "0537" : 미국나스닥종목
            //     "0215" : 중국상해종목
            //     "0214" : 중국심천종목
            //     "0104" : 홍콩종목


            //TB_GBJmCode.Text = "0537AAPL";
            //TB_GBJmCode.Text = "0214000002";
            //TB_GBJmCode.Text = "0215510050";
            //TB_GBJmCode.Text = "010400700";
            //TB_GBJmCode.Text = "010409988";
            String sJmCode = TB_GBJmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(91:뉴욕, 92:아멕스, 93:나스닥, 94:상해, 95:심천, 96:홍콩)
            if (nMarketGb < 91 || nMarketGb > 96)
            {
                MessageBox.Show("해외주식 종목코드를 확인 해주세요.");
                return;
            }

            Requset_GBSise(sJmCode);  //해외주식 현재가 시세 조회
            Requset_GBHoga(sJmCode);  //해외주식 호가 시세 조회
            Requset_GBChe(sJmCode);   //해외주식 체결 시세 조회
        }

        //================================
        // 해외주식 현재가 시세 조회
        //================================
        private void Requset_GBSise(String sJmCode)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbSise, "InRec1", "SCODE", sJmCode);
            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbSise, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("해외주식 현재가 조회 요청 실패");
            }
        }

        //================================
        // 해외주식 호가 시세 조회
        //================================
        private void Requset_GBHoga(String sJmCode)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbHoga, "InRec1", "SCODE", sJmCode);

            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbHoga, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("해외주식 호가 조회 요청 실패");
            }
        }

        //================================
        //  해외주식 체결 시세 조회
        //================================
        private void Requset_GBChe(String sJmCode)
        {
            LV_GBCheSise.Items.Clear();

            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbChe, "InRec1", "SCODE", sJmCode);
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbChe, "InRec1", "STP", "A");

            int nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbChe, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("해외주식 체결시세 조회 요청 실패");
            }
        }

        //================================
        // 해외주식 현재가 시세조회 응답결과 처리
        //================================
        private void OnGetData_GBSise()
        {
            String sOutPut = "";
            long nOutval = 0;
            double fOutval = 0.0f;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "SKORNAME", 0);     //종목명
            LB_GBJmName.Text = sOutPut.Trim();

            String sJmCode = TB_GBJmCode.Text;
            if (sJmCode.Length == 0) return;

            String sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sJmCode, 11); // 가격 소수점자리수
            String sFormat = "{0:F" + sPlaces + "}";

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LCPRICE", 0);      //현재가
            fOutval = ToLong(sOutPut) / 10000.0f;
            LV_GBSise.Items[0].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LDIFF", 0);        //전일대비
            fOutval = ToLong(sOutPut) / 10000.0f;
            LV_GBSise.Items[1].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LVOLUME", 0);      //거래량
            nOutval = ToLong(sOutPut);
            LV_GBSise.Items[2].SubItems[1].Text = String.Format("{0:#,##0}", nOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LOPRICE", 0);      //시가    
            fOutval = ToLong(sOutPut) / 10000.0f;
            LV_GBSise.Items[3].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LHPRICE", 0);      //고가
            fOutval = ToLong(sOutPut) / 10000.0f;
            LV_GBSise.Items[4].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LLPRICE", 0);      //저가
            fOutval = ToLong(sOutPut) / 10000.0f;
            LV_GBSise.Items[5].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LULIMITPRICE", 0); //상한가
            fOutval = ToLong(sOutPut) / 10000.0f;
            if (ToLong(sOutPut) == 0)
                LV_GBSise.Items[6].SubItems[1].Text = "";
            else
                LV_GBSise.Items[6].SubItems[1].Text = String.Format(sFormat, fOutval);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbSise, "OutRec1", "LLLIMITPRICE", 0); //하한가
            fOutval = ToLong(sOutPut) / 10000.0f;
            if (ToLong(sOutPut) == 0)
                LV_GBSise.Items[7].SubItems[1].Text = "";
            else
                LV_GBSise.Items[7].SubItems[1].Text = String.Format(sFormat, fOutval);
        }

        //================================
        // 해외주식 호가 시세조회 응답결과 처리
        //================================
        private void OnGetData_GBHoga()
        {
            String sOutPut = "";
            long nOutVal = 0;
            double fOutval = 0.0f;

            String sJmCode = TB_GBJmCode.Text;
            if (sJmCode.Length == 0) return;

            String sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sJmCode, 11);
            String sFormat = "{0:F" + sPlaces + "}";

            long nBuyTotQty = 0, nSellTotQty = 0, nNetBuyQty = 0;
            String sPrevPrc = "", sCurrPrc = "", sHoga = "";

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LLSTCPRICE", 0);    //전일종가
            fOutval = ToLong(sOutPut) / 10000.0f;
            sPrevPrc = fOutval.ToString();

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LCPRICE", 0);       //현재가
            fOutval = ToLong(sOutPut) / 10000.0f;
            sCurrPrc = fOutval.ToString();

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LTOTOFFERPRE", 0);   //총매도잔량변동수량
            nOutVal = ToLong(sOutPut);
            LV_GBHogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LTOTOFFER", 0);    //총매도수량
            nSellTotQty = ToLong(sOutPut);
            LV_GBHogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,##0}", nSellTotQty);

            //sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_GBHoga, "OutRec1", "LTIME", 0);        //시간
            //sOutPut.Trim();
            //if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
            //String sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
            //LV_GBHogaTot.Items[0].SubItems[3].Text = sTime;

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LTOTBID", 0);      //총매수수량
            nBuyTotQty = ToLong(sOutPut);
            LV_GBHogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,##0}", nBuyTotQty);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", "LTOTBIDPRE", 0);   //총매수잔량변동수량
            nOutVal = ToLong(sOutPut);
            LV_GBHogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,##0}", nOutVal);

            nNetBuyQty = nBuyTotQty - nSellTotQty;
            LV_GBHogaTot.Items[0].SubItems[3].Text = String.Format("{0:#,###}", nNetBuyQty);   //순매수수량

            String sOutRecItem; //아웃풋 아이템명
            String sIdx;        //아웃풋 아이템명 순번
            int nAskRow;
            int nBidRow;
            nAskRow = 9;
            nBidRow = 0;

            //1호가 ~ 10호가 처리
            for (int i = 1; 1 <= 10; i++)
            {
                //2호가부터 Row위치 변경
                if (i > 1)
                {
                    nAskRow = nAskRow - 1;
                    nBidRow = nBidRow + 1;
                }

                sIdx = i.ToString();
                sOutRecItem = "LOFFER" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매도 호가
                fOutval = ToLong(sOutPut) / 10000.0f;
                sHoga = fOutval == 0.0f ? "" : String.Format(sFormat, fOutval);  // 0 표시 안함
                LV_GBMedoHoga.Items[nAskRow].SubItems[3].Text = sHoga;

                sOutRecItem = "LOFFERREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매도 잔량
                nOutVal = ToLong(sOutPut);
                LV_GBMedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LOFFERRESTPRE" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매도직전잔량
                nOutVal = ToLong(sOutPut);
                LV_GBMedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBID" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매수 호가
                fOutval = ToLong(sOutPut) / 10000.0f;
                sHoga = fOutval == 0.0f ? "" : String.Format(sFormat, fOutval); // 0 표시 안함
                LV_GBMesuHoga.Items[nBidRow].SubItems[3].Text = sHoga;

                sOutRecItem = "LBIDREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매수 잔량
                nOutVal = ToLong(sOutPut);
                LV_GBMesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBIDRESTPRE" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbHoga, "OutRec1", sOutRecItem, 0); //매수직전잔량
                nOutVal = ToLong(sOutPut);
                LV_GBMesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nOutVal);
            }
        }

        //================================
        // 해외주식 체결 시세조회 응답결과 처리
        //================================
        private void OnGetData_GBChe()
        {
            String sOutPut = "";
            long nOutVal = 0;
            double fOutval = 0.0f;

            String sTime = "";

            long nDataCnt = 0;
            nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt(g_sTrcode_gbChe, "OutRec1"); //조회건수
            if (nDataCnt == 0) return;

            String sJmCode = TB_GBJmCode.Text;
            if (sJmCode.Length == 0) return;

            String sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sJmCode, 11);
            String sFormat = "{0:F" + sPlaces + "}";

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbChe, "OutRec1", "LTIME", i); //시간
                sOutPut.Trim();
                nOutVal = ToLong(sOutPut);  // Trim 이 안되는 경우가 발생하여 숫자 형변환으로 처리 추가
                sOutPut = nOutVal.ToString();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                item.SubItems.Add(sTime);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Che, "OutRec1", "LCPRICE", i); //현재가(체결가)
                fOutval = ToLong(sOutPut) / 10000.0f;
                item.SubItems.Add(String.Format(sFormat, fOutval));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_Che, "OutRec1", "LNETVOLUME", i); //체결량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,###}", nOutVal));

                LV_GBCheSise.Items.Add(item);
            }
        }

        private void Btn_GBSetReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제

            String sJmCode = TB_GBJmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(91:뉴욕, 92:아멕스, 93:나스닥, 94:상해, 95:심천, 96:홍콩)
            if (nMarketGb < 91 || nMarketGb > 96)
            {
                MessageBox.Show("해외주식 종목코드를 확인 해주세요.");
                return;
            }

            axChampionCommAgent1.RegisterReal(230, sJmCode);    //해외주식 체결시세
            axChampionCommAgent1.RegisterReal(231, sJmCode);    //해외주식 호가
            g_bSetReal = true;
        }

        private void Btn_GBUnReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;

            //개별 실시간 해제
            //String sJmCode = TB_GBJmCode.Text;
            //if (sJmCode.Length == 0)
            //{
            //    MessageBox.Show("종목코드를 입력 해주세요.");
            //    return;
            //}
            //axChampionCommAgent1.UnRegisterReal(230, sJmCode);
            //axChampionCommAgent1.UnRegisterReal(231, sJmCode);
        }

        //================================
        // 해외주식 호가 시세 실시간 수신 처리([231]G01)
        //================================
        private void OnGetRealData_GBHoga(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            double fRealVal = 0.0f;
            String sRealItem = ""; //실시간 아이템명
            String sIdx = "";      //실시간 아이템명 순번

            long nBuyTotQty = 0, nSellTotQty = 0, nNetBuyQty = 0;

            String sJmCode = axChampionCommAgent1.GetRealOutputData(nPBID, "SCODE");     //거래소코드(4)+심볼(16)
            if (sJmCode.Length == 0) return;

            String sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sJmCode, 11);
            String sFormat = "{0:F" + sPlaces + "}";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOT_PREOFFERCHA");   //총매도호가직전잔량
            nRealVal = ToLong(sRealData);
            LV_GBHogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,##0}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFERREST");    //총매도호가잔량
            nSellTotQty = ToLong(sRealData);
            LV_GBHogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,##0}", nSellTotQty);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTBIDREST");      //총매수호가잔량
            nBuyTotQty = ToLong(sRealData);
            LV_GBHogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,##0}", nBuyTotQty);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOT_PREBIDCHA");   //총매수호가직전잔량
            nRealVal = ToLong(sRealData);
            LV_GBHogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,##0}", nRealVal);

            nNetBuyQty = nBuyTotQty - nSellTotQty;
            LV_GBHogaTot.Items[0].SubItems[3].Text = String.Format("{0:#,##0}", nNetBuyQty);   //순매수수량

            int nAskRow = 9;
            int nBidRow = 0;

            String sHoga = "";
            for (int i = 1; 1 <= 10; i++)
            {
                //2호가부터 Row위치 변경
                if (i > 1)
                {
                    nAskRow = nAskRow - 1;
                    nBidRow = nBidRow + 1;
                }

                sIdx = i.ToString();

                sRealItem = "LPREOFFERCHA" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도직전잔량
                nRealVal = ToLong(sRealData);
                LV_GBMedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "LOFFERREST" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 잔량
                nRealVal = ToLong(sRealData);
                LV_GBMedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "LOFFER" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 호가
                fRealVal = ToLong(sRealData) / 10000.0f;
                sHoga = fRealVal == 0.0f ? "" : String.Format(sFormat, fRealVal);  // 0 표시 안함
                LV_GBMedoHoga.Items[nAskRow].SubItems[3].Text = sHoga;

                sRealItem = "LBID" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 호가
                fRealVal = ToLong(sRealData) / 10000.0f;
                sHoga = fRealVal == 0.0f ? "" : String.Format(sFormat, fRealVal);  // 0 표시 안함
                LV_GBMesuHoga.Items[nBidRow].SubItems[3].Text = sHoga;

                sRealItem = "LBIDREST" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 잔량
                nRealVal = ToLong(sRealData);
                LV_GBMesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "LPREBIDCHA" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수직전잔량
                nRealVal = ToLong(sRealData);
                LV_GBMesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nRealVal);
            }
        }

        //================================
        // 해외주식 체결 시세 실시간 수신 처리([230]G00)
        //================================
        private void OnGetRealData_GBChe(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            double fRealVal = 0.0f;
            String sTime = "";      //호가 시간
            String sJmCode = "";    //종목코드

            int nRowCnt = LV_GBCheSise.Items.Count;
            if (nRowCnt > 19)
                LV_GBCheSise.Items.RemoveAt(nRowCnt - 1);

            ListViewItem item = null;
            item = new ListViewItem();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "SCODE");      //거래소코드(4)+심볼(16)
            sJmCode = sRealData.Trim();
            if (sJmCode.Length == 0) return;

            String sPlaces = axChampionCommAgent1.GetOverseaStockInfo(sJmCode, 11);
            String sFormat = "{0:F" + sPlaces + "}";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LLOCTIME");      //현지시간
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);
            item.SubItems.Add(sTime);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCPRICE");      //현재가
            fRealVal = ToLong(sRealData) / 10000.0f;
            item.SubItems.Add(String.Format(sFormat, fRealVal));
            LV_GBSise.Items[0].SubItems[1].Text = String.Format(sFormat, fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCURVOLUME");   //체결량
            nRealVal = ToLong(sRealData);
            item.SubItems.Add(String.Format("{0:#,##0}", nRealVal));

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LDIFF");        //전일대비
            fRealVal = ToLong(sRealData) / 10000.0f;
            item.SubItems.Add(String.Format(sFormat, fRealVal));
            LV_GBSise.Items[1].SubItems[1].Text = String.Format(sFormat, fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LVOLUME");      //누적거래량
            nRealVal = ToLong(sRealData);
            LV_GBSise.Items[2].SubItems[1].Text = String.Format("{0:#,##0}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LOPRICE");      //시가
            fRealVal = ToLong(sRealData) / 10000.0f;
            item.SubItems.Add(String.Format(sFormat, fRealVal));
            LV_GBSise.Items[3].SubItems[1].Text = String.Format(sFormat, fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LHPRICE");      //고가
            fRealVal = ToLong(sRealData) / 10000.0f;
            item.SubItems.Add(String.Format(sFormat, fRealVal));
            LV_GBSise.Items[4].SubItems[1].Text = String.Format(sFormat, fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LLPRICE");      //저가
            fRealVal = ToLong(sRealData) / 10000.0f;
            item.SubItems.Add(String.Format(sFormat, fRealVal));
            LV_GBSise.Items[5].SubItems[1].Text = String.Format(sFormat, fRealVal);

            LV_GBCheSise.Items.Insert(0, item);
        }

        private void Btn_GBKwansim_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sFidNums = "153,154,000,002,003,004,011,012,013,014,015"; //Fid조회 항목 리스트
            String sFidCodes = "0537AAPL,0537AMZN,0537MSFT,0214000002,0214000333,0215600050,0215601288,010400700,010409988";    //Fid조회 종목코드 리스트

            byte chGubun = Convert.ToByte(',');
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = axChampionCommAgent1.RequestPortfolioFid(nRqId, g_sTrcode_gbKwansim, sFidNums, sFidCodes, chGubun, 10);
            if (nRtn < 1)
            {
                MessageBox.Show("관심종목 FID조회 요청 실패.");
            }
        }

        private void TB_GBAccPwd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (g_bLoginYN == false)
                {
                    MessageBox.Show("로그인 상태를 확인 바랍니다.");
                    return;
                }

                Requset_GBAccInfo();  //계좌 예수금 조회
                Btn_GBCheList_Click(null, null);  //해외주식 체결/미체결 내역 조회
                Requset_GBJanList("");  //해외주식 잔고내역 조회
            }
        }

        private void Btn_GBBuyOrd_Click(object sender, EventArgs e)
        {
            SendBSOrderGB(true);
        }

        private void Btn_GBSellOrd_Click(object sender, EventArgs e)
        {
            SendBSOrderGB(false);
        }

        private void Btn_GBModifyOrd_Click(object sender, EventArgs e)
        {
            SendMCOrderGB(true);
        }

        private void Btn_GBCancelOrd_Click(object sender, EventArgs e)
        {
            SendMCOrderGB(false);
        }

        //================================
        // 해외주식 매수/매도 주문 전송(OTD6101U)
        //================================
        private void SendBSOrderGB(bool bBuy)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sJmCode = TB_GBOrdJmCode.Text;   //종목코드
            String sAccNo = TB_GBAccNo.Text;        //계좌번호
            String sAccPwd = TB_GBAccPwd.Text;      //계좌비번
            String sOrdType = CB_GBOrdType.Text;    //주문유형
            String sOrdQty = UD_GBOrdQty.Text;      //주문수량
            String sOrdPrc = TB_GBOrdPrc.Text;      //주문가격
            String sExgName = CB_OrdExgCode.Text;   //거래소명
            String sTradeGb = bBuy ? "20" : "10";   //매매구분(20:매수, 10:매도)

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(91:뉴욕, 92:아멕스, 93:나스닥, 94:상해, 95:심천, 96:홍콩)
            if (nMarketGb < 91 || nMarketGb > 96)
            {
                MessageBox.Show("해외주식 종목코드를 확인 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            if (sExgName.Trim().Length == 0)
            {
                MessageBox.Show("거래소를 선택 해주세요.");
                return;
            }

            String sExgCode = "";
            if (sExgName == "미국")
                sExgCode = "020";
            else if (sExgName == "중국상해")
                sExgCode = "014";
            else if (sExgName == "중국심천")
                sExgCode = "018";
            else if (sExgName == "홍콩")
                sExgCode = "001";

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            if (sOrdType == "지정가")
                sOrdTypeCode = "010";
            else if (sOrdType == "시장가")
                sOrdTypeCode = "020";
            else if (sOrdType == "MOO") //장개시 시장가
                sOrdTypeCode = "720";
            else if (sOrdType == "MOC") //장마감 시장가
                sOrdTypeCode = "740";
            else if (sOrdType == "LOO") //장개시 지정가
                sOrdTypeCode = "710";
            else if (sOrdType == "LOC") //장마감 지정가
                sOrdTypeCode = "730";
            else if (sOrdType == "TWAP") //시간분할주문
                sOrdTypeCode = "750";
            else if (sOrdType == "VWAP") //수량분할주문
                sOrdTypeCode = "760";

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "AC_PWD", sAccPwd);    //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "EXG_COD", sExgCode);           //거래소코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ITEM_COD", sJmCode);           //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ORD_Q", sOrdQty);              //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "FGST_ORD_UPR", sOrdPrc);       //해외증권주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "BUY_SEL_TR_TCD", sTradeGb);    //매매구분(10:매수, 20:매도)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "FGST_BNS_TCD", sOrdTypeCode);  //해외증권주문유형구분
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ORD_COND_TCD", "0");           //주문조건구분("0" 으로 고정)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "EMC_ORD_YN", "N");             //비상주문여부("N" 으로 고정)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbBSOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "해외주식 매수/매도 주문전송 실패");
                return;
            }
        }

        //================================
        // 해외주식 정정/취소 주문 전송
        //================================
        private void SendMCOrderGB(bool bModify)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sJmCode = TB_GBOrdJmCode.Text;   //종목코드
            String sAccNo = TB_GBAccNo.Text;        //계좌번호
            String sAccPwd = TB_GBAccPwd.Text;      //계좌비번
            String sExgName = CB_OrdExgCode.Text;   //거래소명
            String sOrdType = CB_GBOrdType.Text;    //주문유형
            String sOrdQty = UD_GBOrdQty.Text;      //주문수량
            String sOrdPrc = TB_GBOrdPrc.Text;      //주문가격
            String sOrgOrdNo = TB_GBOrgOrdNo.Text;  //원주문번호
            String sTradeGb = bModify ? "20" : "30";  //매매구분(20:정정, 30:취소)

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(91:뉴욕, 92:아멕스, 93:나스닥, 94:상해, 95:심천, 96:홍콩)
            if (nMarketGb < 91 || nMarketGb > 96)
            {
                MessageBox.Show("해외주식 종목코드를 확인 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            if (sOrgOrdNo.Trim().Length == 0)
            {
                MessageBox.Show("원주문번호를 입력 해주세요.");
                return;
            }

            if (sExgName.Trim().Length == 0)
            {
                MessageBox.Show("거래소구분을 선택 해주세요.");
                return;
            }

            String sExgCode = "";
            if (sExgName == "미국")
                sExgCode = "020";
            else if (sExgName == "중국상해")
                sExgCode = "014";
            else if (sExgName == "중국심천")
                sExgCode = "018";
            else if (sExgName == "홍콩")
                sExgCode = "001";

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            if (sOrdType == "지정가")
                sOrdTypeCode = "010";
            else if (sOrdType == "시장가")
                sOrdTypeCode = "020";
            else if (sOrdType == "MOO")
                sOrdTypeCode = "720";
            else if (sOrdType == "MOC")
                sOrdTypeCode = "740";
            else if (sOrdType == "LOO")
                sOrdTypeCode = "710";
            else if (sOrdType == "LOC")
                sOrdTypeCode = "730";
            else if (sOrdType == "TWAP")
                sOrdTypeCode = "750";
            else if (sOrdType == "VWAP")
                sOrdTypeCode = "760";

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "ACNO", sAccNo);           //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "AC_PWD", sAccPwd);        //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "OORD_NO", sOrgOrdNo);          //원주문번호
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "ORD_MDFY_CNCL_TCD", sTradeGb); //매매구분(20:정정, 30:취소)    
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "PAT_ALL_TCD", "20");           //일부/전부 구분("20"으로 고정)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "EXG_COD", sExgCode);           //거래소코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "ITEM_COD", sJmCode);           //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "ORD_Q", "0");                  //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "FGST_ORD_UPR", sOrdPrc);       //주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "FGST_BNS_TCD", sOrdTypeCode);  //주문유형구분  
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMCOrder, "InRec1", "ORD_COND_TCD", "0");           //주문조건구분("0" 으로 고정)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "EMC_ORD_YN", "N");             //비상주문여부("N" 으로 고정)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbMCOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "해외주식 정정/취소 주문전송 실패");
                return;
            }
        }

        private void Btn_GBAccInfo_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }
            Requset_GBAccInfo();
        }

        private void Btn_GBCheList_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            if (Rdo_Che.Checked == true)
                Requset_GBCheList("");
            else
                Requset_GBMiCheList("");
        }

        private void Btn_GBCheNext_Click(object sender, EventArgs e)
        {
            if (Rdo_Che.Checked == true)
                Requset_GBCheList(g_sNextKey_gbChe);
            else
                Requset_GBMiCheList(g_sNextKey_gbChe);
        }

        private void Btn_GBJanList_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            Requset_GBJanList("");
        }

        private void Btn_GBJanNext_Click(object sender, EventArgs e)
        {
            Requset_GBJanList(g_sNextKey_gbJan);
        }

        //================================
        //해외주식 예수금 조회(OCA1725Q)
        //================================
        private void Requset_GBAccInfo()
        {
            String sAccNo; //계좌번호
            String sAccPwd; //계좌비번
            sAccNo = TB_GBAccNo.Text;
            sAccPwd = TB_GBAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();

            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbAccInfo, "InRec1", "ACNO", sAccNo);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbAccInfo, "InRec1", "AC_PWD", sAccPwd);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbAccInfo, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "해외주식 예수금 조회 요청 실패");
                return;
            }
        }

        //================================
        // 해외주식 체결내역 조회
        //================================
        private void Requset_GBCheList(String sNextKey)
        {
            g_sNextKey_gbChe = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_gbChe.Trim().Length == 0)
            {
                LV_GBCheList.Items.Clear();   //잔고내역 초기화
            }

            String sAccNo; //계좌번호
            String sAccPwd; //계좌비번
            sAccNo = TB_GBAccNo.Text;
            sAccPwd = TB_GBAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            String sExgCode = "";
            if (CB_CheExgCode.Text == "전체")
                sExgCode = "%";
            else if (CB_CheExgCode.Text == "미국")
                sExgCode = "020";
            else if (CB_CheExgCode.Text == "중국상해")
                sExgCode = "014";
            else if (CB_CheExgCode.Text == "중국심천")
                sExgCode = "018";
            else if (CB_CheExgCode.Text == "홍콩")
                sExgCode = "001";

            if (sExgCode.Trim().Length == 0)
            {
                MessageBox.Show("거래소를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();

            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "ACNO", sAccNo);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "AC_PWD", sAccPwd);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            String sOrderDate = DT_GBCheDate.Value.ToString("yyyyMMdd");
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "ORD_DT", sOrderDate);  //주문일자
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "BUY_SEL_TR_TCD", "%"); //조회구분(%:전체, 10:매도, 20:매수)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "ORD_RJT_YN", "%");     //주문거부여부(%:전체, N:완료, Y:거부)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbCheList, "InRec1", "EXG_COD", sExgCode);   //거래소코드(%:전체, 020:미국, 014:중국상해, 018:중국심천, 001:홍콩)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbCheList, g_sNextKey_gbChe, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "해외주식 체결내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 해외주식 미체결내역 조회
        //================================
        private void Requset_GBMiCheList(String sNextKey)
        {
            g_sNextKey_gbChe = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_gbChe.Trim().Length == 0)
            {
                LV_GBCheList.Items.Clear();   //잔고내역 초기화
            }

            String sAccNo; //계좌번호
            String sAccPwd; //계좌비번
            sAccNo = TB_GBAccNo.Text;
            sAccPwd = TB_GBAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            String sExgCode = "";
            if (CB_CheExgCode.Text == "전체")
                sExgCode = "%";
            else if (CB_CheExgCode.Text == "미국")
                sExgCode = "020";
            else if (CB_CheExgCode.Text == "중국상해")
                sExgCode = "014";
            else if (CB_CheExgCode.Text == "중국심천")
                sExgCode = "018";
            else if (CB_CheExgCode.Text == "홍콩")
                sExgCode = "001";

            if (sExgCode.Trim().Length == 0)
            {
                MessageBox.Show("거래소를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();

            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMiCheList, "InRec1", "ACNO", sAccNo);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMiCheList, "InRec1", "AC_PWD", sAccPwd);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            String sOrderDate = DT_GBCheDate.Value.ToString("yyyyMMdd");
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMiCheList, "InRec1", "EXG_COD", sExgCode);     //거래소코드(%:전체, 020:미국, 014:중국상해, 018:중국심천, 001:홍콩)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMiCheList, "InRec1", "ITEM_COD", "%");         //종목코드(%:전체)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbMiCheList, "InRec1", "BUY_SEL_TR_TCD", "%");   //조회구분(%:전체, 10:매도, 20:매수)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbMiCheList, g_sNextKey_gbChe, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "해외주식 미체결내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 해외주식 잔고내역 조회
        //================================
        private void Requset_GBJanList(String sNextKey)
        {
            g_sNextKey_gbJan = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_gbJan.Trim().Length == 0)
            {
                LV_GBJanList.Items.Clear();   //잔고내역 초기화
            }

            String sAccNo;    //계좌번호
            String sAccPwd;   //계좌비번
            sAccNo = TB_GBAccNo.Text;
            sAccPwd = TB_GBAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbJanList, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbJanList, "InRec1", "AC_PWD", sAccPwd);    //계좌 비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbJanList, "InRec1", "CMSN_ICLN_YN", "N");         //수수료포함여부(Y:포함, N:미포함)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbJanList, g_sNextKey_gbJan, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "잔고내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 해외주식 계좌 예수금 조회 응답
        //================================
        private void OnGetData_GBAccInfo()
        {
            String sOutPut = "";
            long nOutVal = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbAccInfo, "OutRec1", "AC_TDA", 0); //계좌총예수금
            nOutVal = ToLong(sOutPut);
            LV_AccInfo.Items[0].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbAccInfo, "OutRec1", "MNYO_ABLE_A", 0); //출금가능금액
            nOutVal = ToLong(sOutPut);
            LV_AccInfo.Items[1].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbAccInfo, "OutRec1", "CSH_DFA", 0); //현금미수금
            nOutVal = ToLong(sOutPut);
            LV_AccInfo.Items[2].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbAccInfo, "OutRec1", "CHCK_A", 0); //수표금액
            nOutVal = ToLong(sOutPut);
            LV_AccInfo.Items[3].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbAccInfo, "OutRec1", "ETC_LND_A", 0); //기타대여금액
            nOutVal = ToLong(sOutPut);
            LV_AccInfo.Items[4].SubItems[1].Text = String.Format("{0:#,##0}", nOutVal);
        }

        //================================
        // 해외주식 체결미체결내역 조회 응답
        //================================
        private void OnGetData_GBCheList(String sTrCode, String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_GBCheNext.Enabled = true;
                g_sNextKey_gbChe = sNextKey;
            }
            else
            {
                Btn_GBCheNext.Enabled = false;
                g_sNextKey_gbChe = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            String sOrdTypeNm = "";     //주문유형
            String sTradeGb = "";       //매매구분
            String sOrdTime = "";       //주문시간

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "ITEM_NM", i); //종목명
                item.SubItems.Add(sOutPut.Trim());

                //매매구분(매수,매도)(첨부문서참조)
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "BUY_SEL_TR_TCD ", i);
                sTradeGb = (sOutPut == "10") ? "매도" : "매수";
                item.SubItems.Add(sTradeGb);

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "FGST_ORD_UPR", i);  //해외증권주문단가
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "ORD_Q", i);    //주문수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "FRGN_NCLSN_Q", i);   //해외미체결수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "ORD_NO", i);   //주문번호
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "OORD_NO", i);  //원주문번호
                item.SubItems.Add(sOutPut.Trim());

                //주문유형(첨부문서참조)
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "FGST_BNS_TCD ", i);
                if (sOutPut == "020")
                    sOrdTypeNm = "시장가";
                else if (sOutPut == "720")
                    sOrdTypeNm = "MOO";
                else if (sOutPut == "740")
                    sOrdTypeNm = "MOC";
                else if (sOutPut == "710")
                    sOrdTypeNm = "LOO";
                else if (sOutPut == "730")
                    sOrdTypeNm = "LOC";
                else if (sOutPut == "750")
                    sOrdTypeNm = "TWAP";
                else if (sOutPut == "760")
                    sOrdTypeNm = "VWAP";
                else
                    sOrdTypeNm = "지정가";

                item.SubItems.Add(sOrdTypeNm);

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "ORD_TMD", i); //주문시간
                sOutPut.Trim();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sOrdTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);

                item.SubItems.Add(sOrdTime);
                LV_GBCheList.Items.Add(item);
            }
        }

        //================================
        // 해외주식 잔고내역 조회 응답
        //================================
        private void OnGetData_GBJanList(String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_GBJanNext.Enabled = true;
                g_sNextKey_gbJan = sNextKey;
            }
            else
            {
                Btn_GBJanNext.Enabled = false;
                g_sNextKey_gbJan = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            String sExgGubun = "";

            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "ITEM_NM", i);      //종목명
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "HLDG_Q", i);    //보유수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "SEL_ABLE_Q", i);   //매도가능수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "BUY_UPR", i);      //매입단가
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "FRGN_STK_CLPR", i);   //현재가
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "EV_PL_SUM_A", i);      //평가손익
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "BNS_BAL_EA", i);       //평가금액
                item.SubItems.Add(sOutPut.Trim());

                //sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "ERN_R", i);        //수익률
                //sRate = string.Format("{0:0.00}", ToDouble(sOutPut));
                //item.SubItems.Add(sRate);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbJanList, "OutRec2", "FRGN_STK_MKT_TCD", i);       //해외주식시장구분코드
                if (sOutPut.Trim() == "01")
                    sExgGubun = "메인보드";
                else if (sOutPut.Trim() == "03")
                    sExgGubun = "NASD";
                else if (sOutPut.Trim() == "06")
                    sExgGubun = "상해";
                else if (sOutPut.Trim() == "07")
                    sExgGubun = "심천";
                else if (sOutPut.Trim() == "08")
                    sExgGubun = "뉴욕";
                else if (sOutPut.Trim() == "09")
                    sExgGubun = "나스닥";
                else if (sOutPut.Trim() == "10")
                    sExgGubun = "아맥스";

                item.SubItems.Add(sExgGubun);
                LV_GBJanList.Items.Add(item);
            }
        }

        //================================
        // 해외주식 주문 체결/미체결 실시간 통보 처리([191]RGC)
        //================================
        private void OnGetRealData_GBCheList(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;

            String sOrdNo = "";
            String sJmCode = "";
            String sJmName = "";
            String sOrdTypeNm = "";
            String sTradeGb = "";
            String sTime = "";
            double fOrdPrc = 0.0f;
            long nOrdQty = 0;
            long nCheQty = 0;
            long nMiCheQty = 0;

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORD_NO");        //주문번호
            nRealVal = ToLong(sRealData);   // 앞에 "0"제거
            sOrdNo = nRealVal.ToString();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ITEM_COD");      //종목코드
            sJmCode = sRealData.Trim();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "HNAME");      //종목명
            sJmName = sRealData.Trim();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BUY_SEL_TR_TCD");   //매매구분(매수,매도)
            sRealData.Trim();
            sTradeGb = (sRealData == "1") ? "매도" : "매수";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JPRC");        //주문가격
            fOrdPrc = ToDouble(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JQTY");      //주문수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHEQTY");      //체결수량
            nCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MICHEQTY");      //미체결수량
            nMiCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORD_BNS_TCD");        //매매구분
            sRealData.Trim();
            if (sRealData == "010")
                sOrdTypeNm = "지정가";
            else if (sRealData == "020")
                sOrdTypeNm = "시장가";
            else if (sRealData == "720")
                sOrdTypeNm = "MOO";
            else if (sRealData == "740")
                sOrdTypeNm = "MOC";
            else if (sRealData == "710")
                sOrdTypeNm = "LOO";
            else if (sRealData == "730")
                sOrdTypeNm = "LOC";
            else if (sRealData == "750")
                sOrdTypeNm = "TWAP";
            else if (sRealData == "760")
                sOrdTypeNm = "VWAP";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHETIME");        //주문/체결시간
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문번호 : " + sOrdNo + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매매구분 : " + sTradeGb + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문유형 : " + sOrdTypeNm + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문가격 : " + String.Format("{0:#,##0.0000}", fOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결수량 : " + String.Format("{0:#,###}", nCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "미체결수량 : " + String.Format("{0:#,###}", nMiCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결시간 : " + sTime;

            MessageBox.Show(sRealMsg, "[주문 체결/미체결 실시간 통보 확인]");

        }

        //================================
        // 해외주식 잔고 실시간 통보 처리([205]RGJ)
        //================================
        private void OnGetRealData_GBJanList(short nPBID)
        {
            String sRealData = "";

            String sJmCode = "";
            String sJmName = "";
            long nJanQty = 0;   //잔고수량(보유수량)
            long nOrdQty = 0;   //주문가능수량
            long nOrdPrc = 0;   //매입단가
            long nCurPrc = 0;   //현재가
            long nSonick = 0;   //평가손익

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ITEM_COD");  //종목코드(표준코드)
            sRealData.Trim();
            sJmCode = axChampionCommAgent1.GetShCode(sRealData);    //단축코드로 변환

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "HNAME");  //종목명
            sJmName = sRealData.Trim();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BQTY");      //보유수량
            nJanQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORDGAQTY");  //주문가능수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BUYAMT");    //매입단가
            nOrdPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "PRICE");     //현재가
            nCurPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ESTSONIK");  //평가손익
            nSonick = ToLong(sRealData);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "보유수량 : " + String.Format("{0:#,###}", nJanQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문가능수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매입단가 : " + String.Format("{0:#,###}", nOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "현 재 가 : " + String.Format("{0:#,###}", nCurPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "평가손익 : " + String.Format("{0:#,###}", nSonick) + System.Environment.NewLine;
            MessageBox.Show(sRealMsg, "[잔고 실시간 통보 확인]");
        }

        private void Rdo_Che_CheckedChanged(object sender, EventArgs e)
        {
            DT_GBCheDate.Enabled = true;
        }

        private void Rdo_MiChe_CheckedChanged(object sender, EventArgs e)
        {
            DT_GBCheDate.Enabled = false;
        }

        //================================
        // 선물옵션
        //================================
        private void TB_FOJmCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Btn_FOSearch_Click(null, null);
            }
        }

        private void Btn_FOSearch_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;

            String sJmCode = TB_FOJmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(24:선물, 25:옵션)
            if (nMarketGb < 24 || nMarketGb > 25)
            {
                MessageBox.Show("선물옵션 종목코드를 확인 해주세요.");
                return;
            }

            Requset_FOSise(sJmCode);  //선물옵션 호가+현재가 시세 조회
            Requset_FOChe(sJmCode);   //선물옵션 체결 시세 조회
        }

        //================================
        // 선물옵션 현재가 시세 조회
        //================================
        private void Requset_FOSise(String sJmCode)
        {
            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(24:선물, 25:옵션)
            string sTrCode = nMarketGb == 24 ? g_sTrcode_FSise : g_sTrcode_OSise;

            int nRqId = axChampionCommAgent1.CreateRequestID();
            axChampionCommAgent1.SetTranInputData(nRqId, sTrCode, "InRec1", "SCODE", sJmCode);
            int nRtn = axChampionCommAgent1.RequestTran(nRqId, sTrCode, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("선물옵션 현재가 조회 요청 실패");
            }
        }

        //================================
        // 선물옵션 체결 시세 조회
        //================================
        private void Requset_FOChe(String sJmCode)
        {
            LV_FOCheSise.Items.Clear();

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(24:선물, 25:옵션)
            string sTrCode = nMarketGb == 24 ? g_sTrcode_FChe : g_sTrcode_OChe;

            int nRqId = axChampionCommAgent1.CreateRequestID();

            axChampionCommAgent1.SetTranInputData(nRqId, sTrCode, "InRec1", "SCODE", sJmCode);
            axChampionCommAgent1.SetTranInputData(nRqId, sTrCode, "InRec1", "LTIME", "0");

            int nRtn = axChampionCommAgent1.RequestTran(nRqId, sTrCode, "", 20);
            if (nRtn < 1)
            {
                MessageBox.Show("선물옵션 체결시세 조회 요청 실패");
            }
        }

        //================================
        // 선물옵션 현재가 시세조회 응답결과 처리
        //================================
        private void OnGetData_FOSise(string sTrCode)
        {
            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "SHNAME", 0);        //종목명
            LB_FOJmName.Text = sOutPut.Trim();

            string sPrevPrc = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LLSTCPRICE", 0);      //전일종가
            string sCurrPrc = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LCPRICE", 0);         //현재가

            // 현재가
            fOutVal = ToLong(sCurrPrc) / 100.0f;
            LV_FOSise.Items[0].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LDIFF", 0);        //전일대비
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[1].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LVOLUME", 0);      //거래량
            nOutVal = ToLong(sOutPut);
            LV_FOSise.Items[2].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LOPRICE", 0);      //시가    
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[3].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LHPRICE", 0);      //고가
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[4].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LLPRICE", 0);      //저가
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[5].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LULIMITPRICE", 0); //상한가
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[6].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LLLIMITPRICE", 0); //하한가
            fOutVal = ToLong(sOutPut) / 100.0f;
            LV_FOSise.Items[7].SubItems[1].Text = String.Format("{0:#,##0.00}", fOutVal);

            // 호가 하단 총합계
            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LOFFERTOTCNT", 0);    //매도총호가건수
            nOutVal = ToLong(sOutPut);
            LV_FOHogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LTOTOFFERREST", 0);   //매도총잔량
            nOutVal = ToLong(sOutPut);
            LV_FOHogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LTIME", 0);         //시간
            sOutPut.Trim();
            if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
            string sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
            LV_FOHogaTot.Items[0].SubItems[3].Text = sTime;

            //sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LTOTBIDOFFERDIFF", 0);//총순매수잔량
            //nOutVal = ToLong(sOutPut);
            //LV_FOHogaTot.Items[0].SubItems[3].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LTOTBIDREST", 0);     //매수총잔량
            nOutVal = ToLong(sOutPut);
            LV_FOHogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,###}", nOutVal);

            sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", "LBIDTOTCNT", 0);      //매수총호가건수
            nOutVal = ToLong(sOutPut);
            LV_FOHogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,###}", nOutVal);

            String sOutRecItem; //아웃풋 아이템명
            String sIdx;        //아웃풋 아이템명 순번
            int nAskRow;
            int nBidRow;
            nAskRow = 9;
            nBidRow = 0;

            //1호가 ~ 5호가 처리
            for (int i = 1; 1 <= 5; i++)
            {
                //2호가부터 Row위치 변경
                if (i > 1)
                {
                    nAskRow = nAskRow - 1;
                    nBidRow = nBidRow + 1;
                }

                sIdx = i.ToString();
                sOutRecItem = "LOFFER" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매도 호가
                fOutVal = ToLong(sOutPut) / 100.0f;
                if (ToLong(sOutPut) == 0)
                    LV_FOMedoHoga.Items[nAskRow].SubItems[3].Text = "";
                else
                    LV_FOMedoHoga.Items[nAskRow].SubItems[3].Text = String.Format("{0:#,##0.00}", fOutVal);

                sOutRecItem = "LOFFERREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매도 잔량
                nOutVal = ToLong(sOutPut);
                LV_FOMedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "WOFFER" + sIdx + "CNT";
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매도호가건수
                nOutVal = ToLong(sOutPut);
                LV_FOMedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "LBID" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매수 호가
                fOutVal = ToLong(sOutPut) / 100.0f;
                if (ToLong(sOutPut) == 0)
                    LV_FOMesuHoga.Items[nBidRow].SubItems[3].Text = "";
                else
                    LV_FOMesuHoga.Items[nBidRow].SubItems[3].Text = String.Format("{0:#,##0.00}", fOutVal);

                sOutRecItem = "LBIDREST" + sIdx;
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매수 잔량
                nOutVal = ToLong(sOutPut);
                LV_FOMesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nOutVal);

                sOutRecItem = "WBID" + sIdx + "CNT";
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec1", sOutRecItem, 0); //매수호가건수
                nOutVal = ToLong(sOutPut);
                LV_FOMesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nOutVal);
            }
        }

        //================================
        // 선물옵션 체결 시세조회 응답결과 처리
        //================================
        private void OnGetData_FOChe(string sTrCode)
        {
            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;
            String sTime = "";

            long nDataCnt = 0;
            nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt(sTrCode, "OutRec2"); //조회건수
            if (nDataCnt == 0)
                return;

            LV_FOCheSise.Items.Clear();

            ListViewItem item = null;

            if (nDataCnt > 20)
                nDataCnt = 20;  //최대 20건까지만 표시한다.

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();
                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "LTIME", i); //시간
                sOutPut.Trim();
                nOutVal = ToLong(sOutPut);  // Trim 이 안되는 경우가 발생하여 숫자 형변환으로 처리 추가
                sOutPut = nOutVal.ToString();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                item.SubItems.Add(sTime);

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "LCPRICE", i); //현재가(체결가)
                fOutVal = ToLong(sOutPut) / 100.0f;
                item.SubItems.Add(String.Format("{0:#,##0.00}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(sTrCode, "OutRec2", "LCURVOLUME", i); //체결량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,###}", nOutVal));

                LV_FOCheSise.Items.Add(item);
            }
        }

        private void Btn_FOSetReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제

            String sJmCode = TB_FOJmCode.Text;
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(24:선물, 25:옵션)
            if (nMarketGb < 24 || nMarketGb > 25)
            {
                MessageBox.Show("선물옵션 종목코드를 확인 해주세요.");
                return;
            }

            if (nMarketGb == 24)
            {
                axChampionCommAgent1.RegisterReal(51, sJmCode); //지수선물 호가 실시간 등록
                axChampionCommAgent1.RegisterReal(65, sJmCode); //지수선물 체결시세 실시간 등록
            }
            else
            {
                axChampionCommAgent1.RegisterReal(52, sJmCode); //지수옵션 호가 실시간 등록
                axChampionCommAgent1.RegisterReal(66, sJmCode); //지수옵션 체결시세 실시간 등록
            }

            g_bSetReal = true;
        }

        private void Btn_FOUnReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
            g_bSetReal = false;

            // 개별 실시간 해제
            //int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(24:선물, 25:옵션)
            //if (nMarketGb < 24 || nMarketGb > 25)
            //{
            //    MessageBox.Show("선물옵션 종목코드를 확인 해주세요.");
            //    return;
            //}
            //
            //if (nMarketGb == 24)
            //{
            //    axChampionCommAgent1.UnRegisterReal(51, sJmCode); //지수선물 호가 실시간 등록
            //    axChampionCommAgent1.UnRegisterReal(65, sJmCode); //지수선물 체결시세 실시간 등록
            //}
            //else
            //{
            //    axChampionCommAgent1.UnRegisterReal(52, sJmCode); //지수옵션 호가 실시간 등록
            //    axChampionCommAgent1.UnRegisterReal(66, sJmCode); //지수옵션 체결시세 실시간 등록
            //}
        }

        //================================
        // 선물옵션 호가시세 실시간 수신 처리([51]F01/[52]O01)
        //================================
        private void OnGetRealData_FOHoga(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            double fRealVal = 0.0f;
            String sRealItem = ""; //실시간 아이템명
            String sIdx = "";      //실시간 아이템명 순번
            String sTime = "";     //호가 시간

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFERCNT");   //매도 총 호가건수
            nRealVal = ToLong(sRealData);
            LV_FOHogaTot.Items[0].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFER");    //매도총잔량
            nRealVal = ToLong(sRealData);
            LV_FOHogaTot.Items[0].SubItems[2].Text = String.Format("{0:#,###}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTIME");    //호가시각
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);
            LV_FOHogaTot.Items[0].SubItems[3].Text = sTime;

            //sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTBIDOFFERDIFF");    //총순매수잔량
            //nRealVal = ToLong(sRealData);
            //LV_FOHogaTot.Items[0].SubItems[3].Text = String.Format("{0:#,###}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTOFFER");      //매수총잔량
            nRealVal = ToLong(sRealData);
            LV_FOHogaTot.Items[0].SubItems[4].Text = String.Format("{0:#,###}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTOTBIDCNT");   //매수 총 호가건수
            nRealVal = ToLong(sRealData);
            LV_FOHogaTot.Items[0].SubItems[5].Text = String.Format("{0:#,###}", nRealVal);

            int nAskRow = 9;
            int nBidRow = 0;

            for (int i = 1; 1 <= 10; i++)
            {
                //2호가부터 Row위치 변경
                if (i > 1)
                {
                    nAskRow = nAskRow - 1;
                    nBidRow = nBidRow + 1;
                }

                sIdx = i.ToString();

                sRealItem = "LOFFER" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 호가
                fRealVal = ToLong(sRealData) / 100.0f;
                if (ToLong(sRealData) == 0)
                    LV_FOMedoHoga.Items[nAskRow].SubItems[3].Text = "";
                else
                    LV_FOMedoHoga.Items[nAskRow].SubItems[3].Text = String.Format("{0:#,##0.00}", fRealVal);

                sRealItem = "LOFFERREST" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도 잔량
                nRealVal = ToLong(sRealData);
                LV_FOMedoHoga.Items[nAskRow].SubItems[2].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "WOFFER" + sIdx + "CNT";
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매도호가건수
                nRealVal = ToLong(sRealData);
                LV_FOMedoHoga.Items[nAskRow].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "LBID" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 호가
                fRealVal = ToLong(sRealData) / 100.0f;
                if (ToLong(sRealData) == 0)
                    LV_FOMesuHoga.Items[nBidRow].SubItems[3].Text = "";
                else
                    LV_FOMesuHoga.Items[nBidRow].SubItems[3].Text = String.Format("{0:#,##0.00}", fRealVal);

                sRealItem = "LBIDREST" + sIdx;
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수 잔량
                nRealVal = ToLong(sRealData);
                LV_FOMesuHoga.Items[nBidRow].SubItems[4].Text = String.Format("{0:#,###}", nRealVal);

                sRealItem = "WBID" + sIdx + "CNT";
                sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, sRealItem);       //매수호가건수
                nRealVal = ToLong(sRealData);
                LV_FOMesuHoga.Items[nBidRow].SubItems[5].Text = String.Format("{0:#,###}", nRealVal);
            }
        }

        //================================
        // 선물옵션 체결시세 실시간 수신 처리([65]F00/[66]O00)
        //================================
        private void OnGetRealData_FOChe(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;
            double fRealVal = 0.0f;
            String sTime = "";     //호가 시간

            int nRowCnt = LV_FOCheSise.Items.Count;
            if (nRowCnt > 19)
                LV_FOCheSise.Items.RemoveAt(nRowCnt - 1);

            ListViewItem item = null;
            item = new ListViewItem();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LTIME");        //시간
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);
            item.SubItems.Add(sTime);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCPRICE");      //현재가
            fRealVal = ToLong(sRealData) / 100.0f;
            item.SubItems.Add(String.Format("{0:#,##0.00}", fRealVal));
            LV_FOSise.Items[0].SubItems[1].Text = String.Format("{0:#,##0.00}", fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LCURVOLUME");   //체결량
            nRealVal = ToLong(sRealData);
            item.SubItems.Add(String.Format("{0:#,###}", nRealVal));

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LDIFF");        //전일대비
            fRealVal = ToLong(sRealData) / 100.0f;
            LV_FOSise.Items[1].SubItems[1].Text = String.Format("{0:#,##0.00}", fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LVOLUME");      //누적거래량
            nRealVal = ToLong(sRealData);
            LV_FOSise.Items[2].SubItems[1].Text = String.Format("{0:#,###}", nRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LOPRICE");      //시가
            fRealVal = ToLong(sRealData) / 100.0f;
            LV_FOSise.Items[3].SubItems[1].Text = String.Format("{0:#,##0.00}", fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LHPRICE");      //고가
            fRealVal = ToLong(sRealData) / 100.0f;
            LV_FOSise.Items[4].SubItems[1].Text = String.Format("{0:#,##0.00}", fRealVal);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "LLPRICE");      //저가
            fRealVal = ToLong(sRealData) / 100.0f;
            LV_FOSise.Items[5].SubItems[1].Text = String.Format("{0:#,##0.00}", fRealVal);

            LV_FOCheSise.Items.Insert(0, item);
        }

        private void TB_FOAccPwd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (g_bLoginYN == false)
                {
                    MessageBox.Show("로그인 상태를 확인 바랍니다.");
                    return;
                }

                Requset_FOCheList("");  //선물옵션 체결/미체결 내역 조회
                Requset_FOJanList("");  //선물옵션 잔고내역 조회
            }
        }

        private void Btn_FOBuyOrd_Click(object sender, EventArgs e)
        {
            SendBSOrderFO(true);  // T:Buy, F:Sell
        }

        private void Btn_FOSellOrd_Click(object sender, EventArgs e)
        {
            SendBSOrderFO(false);  // T:Buy, F:Sell
        }

        private void Btn_FOModifyOrd_Click(object sender, EventArgs e)
        {
            SendMCOrderFO(true);  // T:Modify, F:Cancel
        }

        private void Btn_FOCancelOrd_Click(object sender, EventArgs e)
        {
            SendMCOrderFO(false);  // T:Modify, F:Cancel
        }

        //================================
        // 선물옵션 매수/매도 주문 전송
        //================================
        private void SendBSOrderFO(bool bBuy)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sAccNo = "";     //계좌번호
            String sAccPwd = "";    //계좌비번
            String sOrdType = "";   //주문유형
            String sOrdQty = "";    //주문수량
            String sOrdPrc = "";    //주문가격
            String sTradeGb = "";   //매매구분(20:매수, 10:매도)
            String sJmCode = "";    //종목코드

            //매매구분(20:매수, 10:매도)
            if (bBuy == true)
                sTradeGb = "20";
            else
                sTradeGb = "10";

            sJmCode = TB_FOOrdJmCode.Text;
            sAccNo = TB_FOAccNo.Text;
            sAccPwd = TB_FOAccPwd.Text;
            sOrdType = CB_FOOrdType.Text;
            sOrdQty = UD_FOOrdQty.Text;
            sOrdPrc = TB_FOOrdPrc.Text;

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 24 || nMarketGb > 25)
            {
                MessageBox.Show("선물옵션 종목코드를 확인 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            String sOrdCond = "";

            if (sOrdType == "지정가")
            {
                sOrdTypeCode = "010";
                sOrdCond = "0";
            }
            else if (sOrdType == "시장가")
            {
                sOrdTypeCode = "020";
                sOrdCond = "0";
            }
            else if (sOrdType == "조건부지정가")
            {
                sOrdTypeCode = "030";
                sOrdCond = "0";
            }
            else if (sOrdType == "최유리지정가")
            {
                sOrdTypeCode = "040";
                sOrdCond = "0";
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "AC_PWD", sAccPwd);    //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ITEM_COD", sJmCode);           //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ORD_Q", sOrdQty);              //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ORD_UPR", sOrdPrc);            //주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "BUY_SEL_TR_TCD", sTradeGb);    //매매구분(10:매수, 20:매도)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ORD_BNS_TCD", sOrdTypeCode);   //주문유형구분
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOBSOrder, "InRec1", "ORD_COND_TCD", sOrdCond);      //주문조건구분

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_FOBSOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "선물옵션 매수/매도 주문전송 실패");
                return;
            }
        }

        //================================
        // 선물옵션 정정/취소 주문 전송
        //================================
        private void SendMCOrderFO(bool bModify)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            String sAccNo = "";     //계좌번호
            String sAccPwd = "";    //계좌비번
            String sOrdType = "";   //주문유형
            String sOrdQty = "";    //주문수량
            String sOrdPrc = "";    //주문가격
            String sTradeGb = "";   //매매구분(20:정정, 30:취소)
            String sJmCode = "";    //종목코드
            String sOrgOrdNo = "";  //원주문번호

            //매매구분(20:정정, 30:취소)
            if (bModify == true)
                sTradeGb = "20";
            else
                sTradeGb = "30";

            sJmCode = TB_FOOrdJmCode.Text;
            sAccNo = TB_FOAccNo.Text;
            sAccPwd = TB_FOAccPwd.Text;
            sOrdType = CB_FOOrdType.Text;
            sOrdQty = UD_FOOrdQty.Text;
            sOrdPrc = TB_FOOrdPrc.Text;
            sOrgOrdNo = TB_FOOrgOrdNo.Text;

            if (sJmCode.Trim().Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            int nMarketGb = axChampionCommAgent1.GetMarketKubun(sJmCode, "");       // 종목 시장구분 조회(1:코스피, 2:코스닥,...)
            if (nMarketGb < 24 || nMarketGb > 25)
            {
                MessageBox.Show("선물옵션 종목코드를 입력 해주세요.");
                return;
            }

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            if (sOrdType.Trim().Length == 0)
            {
                MessageBox.Show("주문유형을 선택 해주세요.");
                return;
            }

            if (ToInt(sOrdQty) == 0)
            {
                MessageBox.Show("주문수량을 입력 해주세요.");
                return;
            }

            if (sOrgOrdNo.Trim().Length == 0)
            {
                MessageBox.Show("원주문번호를 입력 해주세요.");
                return;
            }

            //주문유형 코드값 변환
            String sOrdTypeCode = "";
            String sOrdCond = "";

            if (sOrdType == "지정가")
            {
                sOrdTypeCode = "010";
                sOrdCond = "0";
            }
            else if (sOrdType == "시장가")
            {
                sOrdTypeCode = "020";
                sOrdCond = "0";
            }
            else if (sOrdType == "조건부지정가")
            {
                sOrdTypeCode = "030";
                sOrdCond = "0";
            }
            else if (sOrdType == "최유리지정가")
            {
                sOrdTypeCode = "040";
                sOrdCond = "0";
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ACNO", sAccNo);           //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "AC_PWD", sAccPwd);        //계좌비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "OORD_NO", sOrgOrdNo);          //원주문번호
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ITEM_COD", sJmCode);           //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ORD_Q", sOrdQty);              //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ORD_UPR", sOrdPrc);            //주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ORD_MDFY_CNCL_TCD", sTradeGb); //정정, 취소 구분(20:정정, 30:취소)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "BUY_SEL_TR_TCD", "20");        //매수, 매도 구분(10:매수, 20:매도)(테스트용 이므로 '매수정정'으로 고정)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "PAT_ALL_TCD", "20");           //일부, 전부 구분(10:일부, 20:전부)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ORD_BNS_TCD", sOrdTypeCode);   //주문유형구분  
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOMCOrder, "InRec1", "ORD_COND_TCD", sOrdCond);      //주문조건구분

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_FOMCOrder, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "선물옵션 정정/취소 주문전송 실패");
                return;
            }
        }

        private void Btn_FOCheList_Click(object sender, EventArgs e)
        {
            Requset_FOCheList("");
        }

        private void Btn_FOCheNext_Click(object sender, EventArgs e)
        {
            Requset_FOCheList(g_sNextKey_FOChe);
        }

        private void Btn_FOJanList_Click(object sender, EventArgs e)
        {
            Requset_FOJanList("");
        }

        private void Btn_FOJanNext_Click(object sender, EventArgs e)
        {
            Requset_FOJanList(g_sNextKey_FOJan);
        }

        //================================
        // 선물옵션 체결/미체결 내역 조회 요청
        //================================
        private void Requset_FOCheList(String sNextKey)
        {
            g_sNextKey_FOChe = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_FOChe.Trim().Length == 0)
            {
                LV_FOCheList.Items.Clear();   //체결/미체결내역 초기화
            }

            String sAccNo; //계좌번호
            String sAccPwd; //계좌비번
            sAccNo = TB_FOAccNo.Text;
            sAccPwd = TB_FOAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();

            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "ACNO", sAccNo);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "AC_PWD", sAccPwd);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "SORT_TURN_IO1CD", "2");    //주문번호를 기준으로 정순/역순 조회(1:정순, 2:역순)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "CLSN_TP_IO1CD", "%");      //체결구분코드(%:전체, 1:체결, 2:미체결)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "BUY_SEL_TR_TCD", "%");     //조회구분(%:전체, 10:매도, 20:매수)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "ITEM_TP_IO1CD", "%");      //종목구분코드(%:전체, 1:선물, 2:Call, 3:Put, 4:스프레드)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOCheList, "InRec1", "ITEM_COD", "%");           //종목코드(%:전체)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_FOCheList, g_sNextKey_FOChe, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "선물옵션 체결내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 선물옵션 잔고내역 조회 요청
        //================================
        private void Requset_FOJanList(String sNextKey)
        {
            g_sNextKey_FOJan = sNextKey;

            //연속조회키값이 없으면 첫조회로 리스트를 초기화 한다.
            if (g_sNextKey_FOJan.Trim().Length == 0)
            {
                LV_FOJanList.Items.Clear();   //체결/미체결내역 초기화
            }

            String sAccNo;    //계좌번호
            String sAccPwd;   //계좌비번
            sAccNo = TB_FOAccNo.Text;
            sAccPwd = TB_FOAccPwd.Text;

            if (sAccNo.Trim().Length == 0)
            {
                MessageBox.Show("계좌번호를 입력 해주세요.");
                return;
            }

            if (sAccPwd.Trim().Length == 0)
            {
                MessageBox.Show("계좌 비밀번호를 입력 해주세요.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOJanList, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌번호 입력 에러");
                return;
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_FOJanList, "InRec1", "AC_PWD", sAccPwd);    //계좌 비밀번호
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "계좌 비밀번호 에러");
                return;
            }

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_FOJanList, g_sNextKey_FOJan, 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "선물옵션 잔고내역 조회 요청 실패");
                return;
            }

            return;
        }

        //================================
        // 선물옵션 체결/미체결 내역 조회 응답결과 처리
        //================================
        private void OnGetData_FOCheList(String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_FOCheNext.Enabled = true;
                g_sNextKey_FOChe = sNextKey;
            }
            else
            {
                Btn_FOCheNext.Enabled = false;
                g_sNextKey_FOChe = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;
            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            String sOrdTypeNm = "";     //주문유형
            String sOrdTime = "";       //주문시간

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ITEM_NM", i); //종목명
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "TP_NM ", i);  //매도매수구분 + 정정취소구분 ex) “매도전부정정”
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ORD_UPR", i);  //주문가격
                fOutVal = ToDouble(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0.00}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ORD_Q", i);    //주문수량
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ORD_RQ", i);   //미체결수량
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ORD_NO", i);   //주문번호
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "OORD_NO", i);  //원주문번호
                item.SubItems.Add(sOutPut.Trim());

                //주문유형(10:지정가, 20:시장가, 30:조건부지정가, 40:최유리지정가, 50:최우선지정가)(첨부문서참조)
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "NMPR_ORD_BNS_TCD ", i);
                if (sOutPut == "010")
                    sOrdTypeNm = "지정가";
                else if (sOutPut == "020")
                    sOrdTypeNm = "시장가";
                else if (sOutPut == "030")
                    sOrdTypeNm = "조건부지정가";
                else if (sOutPut == "040")
                    sOrdTypeNm = "최유리지정가";
                item.SubItems.Add(sOrdTypeNm);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOCheList, "OutRec2", "ORD_TMD", i); //주문시간
                sOutPut.Trim();
                if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                sOrdTime = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                item.SubItems.Add(sOrdTime);

                LV_FOCheList.Items.Add(item);
            }
        }


        //================================
        // 선물옵션 잔고내역 조회 응답결과 처리
        //================================
        private void OnGetData_FOJanList(String sNextGb, String sNextKey)
        {
            if (sNextGb == "6" || sNextGb == "7") //다음조회있음
            {
                Btn_FOJanNext.Enabled = true;
                g_sNextKey_FOJan = sNextKey;
            }
            else
            {
                Btn_FOJanNext.Enabled = false;
                g_sNextKey_FOJan = "";
            }

            String sOutPut = "";
            long nOutVal = 0;
            double fOutVal = 0.0f;

            int nDataCnt = 0;
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec1", "RECNM", 0);  //레코드수
            nDataCnt = ToInt(sOutPut);

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "ITEM_NM", i);      //종목명
                item.SubItems.Add(sOutPut.Trim());

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "FUOP_USCTR_Q", i);//보유수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "LQDT_ABLE_Q", i); //청산가능수량
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "USCTR_UPR", i);   //매입단가
                fOutVal = ToDouble(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0.00}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "FUOP_CRPR", i);   //현재가
                fOutVal = ToDouble(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0.00}", fOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "PCHS_A", i);      //매입금액
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "PL_A", i);        //평가손익
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_FOJanList, "OutRec2", "FUOP_EA", i);     //평가금액
                nOutVal = ToLong(sOutPut);
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                LV_FOJanList.Items.Add(item);
            }
        }

        //================================
        // 선물옵션 주문 체결/미체결 실시간 통보 처리([193]RFC)
        //================================
        private void OnGetRealData_FOCheList(short nPBID)
        {
            String sRealData = "";
            long nRealVal = 0;

            String sOrdNo = "";
            String sJmCode = "";
            String sJmName = "";
            String sOrdTypeNm = "";
            String sTradeGb = "";
            String sTime = "";
            double fOrdPrc = 0.0f;
            long nOrdQty = 0;
            long nCheQty = 0;
            long nMiCheQty = 0;

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ORDNO");        //주문번호
            nRealVal = ToLong(sRealData);   // 앞에 "0"제거
            sOrdNo = nRealVal.ToString();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "EXPCODE");      //종목코드(표준코드)
            sJmCode = sRealData.Trim();
            //sJmCode = axChampionCommAgent1.GetShCode(sRealData);    //단축코드로 변환

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "HNAME30");      //종목명
            sJmName = sRealData.Trim();
            //sJmName = axChampionCommAgent1.GetNameByCode(sJmCode); //종목코드로 종목명 찾기

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "F_TRDE_TP");   //매매구분(매수,매도)
            sRealData.Trim();
            sTradeGb = (sRealData == "1") ? "매도" : "매수";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JPRC");        //주문가격
            fOrdPrc = ToDouble(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JQTY");      //주문수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHEQTY");      //체결수량
            nCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MICHEQTY");      //미체결수량
            nMiCheQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "MEMEGB");        //매매구분
            sRealData.Trim();
            if (sRealData == "010")
                sOrdTypeNm = "지정가";
            else if (sRealData == "020")
                sOrdTypeNm = "시장가";
            else if (sRealData == "030")
                sOrdTypeNm = "조건부지정가";
            else if (sRealData == "040")
                sOrdTypeNm = "최유리지정가";
            else if (sRealData == "050")
                sOrdTypeNm = "최우선지정가";

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "CHETIME");        //주문/체결시간
            sRealData.Trim();
            if (sRealData.Length == 5) sRealData = "0" + sRealData;
            sTime = sRealData.Substring(0, 2) + ":" + sRealData.Substring(2, 2) + ":" + sRealData.Substring(4, 2);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문번호 : " + sOrdNo + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매매구분 : " + sTradeGb + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문유형 : " + sOrdTypeNm + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "주문가격 : " + String.Format("{0:#,##0.00}", fOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결수량 : " + String.Format("{0:#,###}", nCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "미체결수량 : " + String.Format("{0:#,###}", nMiCheQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "체결시간 : " + sTime;

            MessageBox.Show(sRealMsg, "[선물옵션 주문 체결/미체결 실시간 통보 확인]");
        }

        //================================
        // 선물옵션 잔고 실시간 통보 처리([194]RFJ)
        //================================
        private void OnGetRealData_FOJanList(short nPBID)
        {
            String sRealData = "";

            String sJmCode = "";
            String sJmName = "";
            long nJanQty = 0;   //잔고수량(보유수량)
            long nOrdQty = 0;   //주문가능수량
            double fOrdPrc = 0.0f;   //매입단가
            long nCurPrc = 0;   //현재가
            long nSonick = 0;   //평가손익

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "ITEM_COD");  //종목코드(표준코드)
            sRealData.Trim();
            sJmCode = axChampionCommAgent1.GetShCode(sRealData);    //단축코드로 변환

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "HNAME30");  //종목명
            sJmName = sRealData.Trim();

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "BQTY");      //보유수량
            nJanQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "JQTY");  //청산가능수량
            nOrdQty = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "PAMT");    //매입단가(평균매입지수)
            fOrdPrc = ToDouble(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "PRICE");     //현재가
            nCurPrc = ToLong(sRealData);

            sRealData = axChampionCommAgent1.GetRealOutputData(nPBID, "TRSONIK");  //실현손익
            nSonick = ToLong(sRealData);

            String sRealMsg = "";
            sRealMsg = "종    목 : " + sJmName + "(" + sJmCode + ")" + System.Environment.NewLine;
            sRealMsg = sRealMsg + "보유수량 : " + String.Format("{0:#,###}", nJanQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "청산가능수량 : " + String.Format("{0:#,###}", nOrdQty) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "매입단가 : " + String.Format("{0:#,##0.00}", fOrdPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "현 재 가 : " + String.Format("{0:#,###}", nCurPrc) + System.Environment.NewLine;
            sRealMsg = sRealMsg + "실현손익 : " + String.Format("{0:#,###}", nSonick) + System.Environment.NewLine;
            MessageBox.Show(sRealMsg, "[선물옵션 잔고 실시간 통보 확인]");
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (g_sLoginId.Length == 0 || g_bLoginYN == false)
                return;

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제

            switch (TabControl1.SelectedIndex)
            {
                case 0:
                    TB_JmCode.Select();
                    break;
                case 1:
                    Btn_Kwansim.Select();
                    break;
                case 2:
                    TB_AccNo.Select();
                    axChampionCommAgent1.RegisterReal(191, g_sLoginId);  //국내주식 주문/체결 통보
                    axChampionCommAgent1.RegisterReal(192, g_sLoginId);  //국내주식 잔고 통보
                    break;
                case 3:
                    TB_GBJmCode.Select();
                    break;
                case 4:
                    Btn_GBKwansim.Select();
                    break;
                case 5:
                    TB_GBAccNo.Select();
                    axChampionCommAgent1.RegisterReal(204, g_sLoginId);  //해외주식 주문/체결 통보
                    axChampionCommAgent1.RegisterReal(205, g_sLoginId);  //해외주식 잔고 통보
                    break;
                case 6:
                    TB_FOJmCode.Select();
                    break;
                case 7:
                    TB_FOAccNo.Select();
                    axChampionCommAgent1.RegisterReal(193, g_sLoginId);  //선물옵션 주문/체결 통보
                    axChampionCommAgent1.RegisterReal(194, g_sLoginId);  //선물옵션 잔고 통보
                    break;
                case 8:
                    Btn_007.Select();
                    break;
            }

        }


        //================================
        // 조건검색
        //================================

        // 조건검색 리스트 조회 요청
        private void Btn_007_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_007, "InRec1", "SUSERID", g_sLoginId);       //사용자ID
            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_007, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "조건검색 리스트 조회 요청 실패");
                return;
            }
        }

        private void OnGetData_007()
        {
            LV_007List.Items.Clear();

            String sOutPut = "";
            long nDataCnt = 0;
            nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt(g_sTrcode_007, "OutRec2"); //조회건수
            if (nDataCnt == 0)
            {
                MessageBox.Show("조회 내용이 없습니다.");
                return;
            }

            ListViewItem item = null;
            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "SDATE", i); //저장날짜
                sOutPut.Trim();
                //sTemp = sOutPut.Substring(0, 4) + "/" + sOutPut.Substring(4, 2) + "/" + sOutPut.Substring(6, 2);
                //item.SubItems.Add(sTemp);
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "STIME", i); //저장시간
                sOutPut.Trim();
                //nOutVal = ToLong(sOutPut);  // Trim 이 안되는 경우가 발생하여 숫자 형변환으로 처리 추가
                //sOutPut = nOutVal.ToString();
                //if (sOutPut.Length == 5) sOutPut = "0" + sOutPut;
                //sTemp = sOutPut.Substring(0, 2) + ":" + sOutPut.Substring(2, 2) + ":" + sOutPut.Substring(4, 2);
                //item.SubItems.Add(sTemp);
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "LSEQ", i); //시퀀스번호
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "SCONTNAME", i); //조건명
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "SGROUPNAME", i); //그룹명
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_007, "OutRec2", "SMEMO", i); //조건설명
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                LV_007List.Items.Add(item);
            }
        }


        private void LV_007List_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            ListViewItem lvItem = lv.FocusedItem;

            bool bSelectd = lvItem.Selected;
            if (bSelectd == false)
                return;

            string sDate = lvItem.SubItems[1].Text;     //저장날짜
            string sTime = lvItem.SubItems[2].Text;     //저장시간
            string sSeqNo = lvItem.SubItems[3].Text;    //시퀀스번호
            string sName = lvItem.SubItems[4].Text;     //조건명
            string sEditCnt = TB_EditCnt.Text;
            string sEditList = TB_EditList.Text;
            g_nLastIdx = lvItem.Index;

            if (sDate.Trim().Length > 0 && sTime.Trim().Length > 0 && sSeqNo.Trim().Length > 0)
            {
                // 선택한 리스트의 데이터를 표시.
                LV_028List.Items[0].SubItems[1].Text = sDate;
                LV_028List.Items[1].SubItems[1].Text = sTime;
                LV_028List.Items[2].SubItems[1].Text = sSeqNo;
                LV_028List.Items[3].SubItems[1].Text = sName;

                Requset_msvf027(sDate, sTime, sSeqNo, sEditCnt, sEditList);
            }
            else
                MessageBox.Show("조건 리스트에서 조회할 항목을 선택해 주세요");
        }

        private void Btn_027_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            string sDate = LV_028List.Items[0].SubItems[1].Text;
            string sTime = LV_028List.Items[1].SubItems[1].Text;
            string sSeqNo = LV_028List.Items[2].SubItems[1].Text;
            string sName = LV_028List.Items[3].SubItems[1].Text;
            string sEditCnt = TB_EditCnt.Text;
            string sEditList = TB_EditList.Text;

            if (sDate.Trim().Length > 0 && sTime.Trim().Length > 0 && sSeqNo.Trim().Length > 0)
            {
                // 선택한 리스트의 데이터를 표시.
                LV_028List.Items[0].SubItems[1].Text = sDate;
                LV_028List.Items[1].SubItems[1].Text = sTime;
                LV_028List.Items[2].SubItems[1].Text = sSeqNo;
                LV_028List.Items[3].SubItems[1].Text = sName;

                Requset_msvf027(sDate, sTime, sSeqNo, sEditCnt, sEditList);
            }
            else
                MessageBox.Show("조건 리스트에서 항목을 선택해 주세요");
        }

        // 조건검색(OpenApi용) 조회 요청
        private void Requset_msvf027(string sDate, string sTime, string sSeqNo, string sEditCnt, string sEditList)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "SUSERID", g_sLoginId);        //사용자ID
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "SDATE", sDate);               //저장날짜
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "STIME", sTime);               //저장시간
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "LSEQ", sSeqNo);               //시퀀스번호
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "LFIELDEDITCNT", sEditCnt);    //항목편집개수
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_027, "InRec1", "SFILEDLIST", sEditList);      //항목편집리스트(FID)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_027, "", 20);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "조건검색(OpenApi용) 조회 요청 실패");
            }

            return;
        }

        private void OnGetData_027()
        {
            String sOutPut = "";
            long nOutVal = 0;
            long nDataCnt = 0;
            nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt(g_sTrcode_027, "OutRec2"); //조회건수
            if (nDataCnt == 0)
                return;

            LV_027List.Items.Clear();

            ListViewItem item = null;

            for (int i = 0; i < nDataCnt; i++)
            {
                item = new ListViewItem();
                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SCODE", i); //종목코드
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SHNAME", i); //종목명
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SPRICE", i); //현재가
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SOPEN", i); //시가
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SHIGH", i); //고가
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SLOW", i); //저가
                nOutVal = ToLong(sOutPut.Trim());
                item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                // sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SPREPRICE", i); //기준가
                // nOutVal = ToLong(sOutPut.Trim());
                // item.SubItems.Add(String.Format("{0:#,##0}", nOutVal));

                sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_027, "OutRec2", "SRESULTFILED", i); //결과필드
                sOutPut.Trim();
                item.SubItems.Add(sOutPut);

                LV_027List.Items.Add(item);
            }
        }

        // 조건실시간등록(OpenApi용) 조회 요청
        private void Requset_msvf028(string sDate, string sTime, string sSeqNo, string sGubun, string sDelSeq)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn = 0;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "SUSERID", g_sLoginId);    //사용자ID
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "SDATE", sDate);           //저장날짜
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "STIME", sTime);           //저장시간
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "LSEQ", sSeqNo);           //시퀀스번호
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "SGUBUN", sGubun);         //등록해제구분코드
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_028, "InRec1", "SDELSEQ", sDelSeq);       //해제시실시간등록번호

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_028, "", 1);
            if (nRtn < 1)
            {
                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                MessageBox.Show(g_sMsg.Trim(), "조건실시간등록(OpenApi용) 조회 요청 실패");
                return;
            }

            return;
        }

        private void OnGetData_028()
        {
            String sOutPut = "";
            String sRealGubun = "";
            String sRealKey = "";
            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SSIGNAL", 0);      //신호구분코드
            sOutPut.Trim();

            sRealGubun = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SGUBUN", 0);    //등록해제구분코드
            sRealGubun.Trim();
            LV_028List.Items[4].SubItems[1].Text = sRealGubun;

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SRETURNCODE", 0);  //결과구분코드
            sOutPut.Trim();

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SREGTIME", 0);     //설정시간
            sOutPut.Trim();
            LV_028List.Items[6].SubItems[1].Text = sOutPut;

            sRealKey = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SREGSEQ", 0);      //설정번호    
            sRealKey.Trim();
            LV_028List.Items[5].SubItems[1].Text = sRealKey;

            sOutPut = axChampionCommAgent1.GetTranOutputData(g_sTrcode_028, "OutRec1", "SERRMSG", 0);      //에러메시지
            sOutPut.Trim();

            if (sRealGubun == "E")  // 실시간 등록 응답값이면
            {
                //실시간 등록전 기존에 등록된 키가 있으면 실시간 해제부터 한다.
                long nOldRealKey = ToLong(g_sOldRealKey);
                if (nOldRealKey > 0)
                    axChampionCommAgent1.UnRegisterReal(154, g_sOldRealKey);  //조건검색 실시간 해제

                //실시간 등록
                long nRealKey = ToLong(sRealKey);
                if (nRealKey > 0)
                {
                    String sRKey;
                    sRKey = String.Format("{0:0000}", nRealKey);
                    axChampionCommAgent1.RegisterReal(154, sRKey);  //조건검색 실시간 등록
                    g_sOldRealKey = sRKey;
                }
            }
            else
            {
                //실시간 해제
                long nRealKey = ToLong(sRealKey);
                String sRKey;
                sRKey = String.Format("{0:0000}", nRealKey);
                if (nRealKey > 0)
                {
                    axChampionCommAgent1.UnRegisterReal(154, sRKey);  //조건검색 실시간 해제
                    g_sOldRealKey = "";
                }
            }
        }

        private void Btn_CondiSetReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            string sDate = LV_028List.Items[0].SubItems[1].Text;
            string sTime = LV_028List.Items[1].SubItems[1].Text;
            string sSeqNo = LV_028List.Items[2].SubItems[1].Text;
            string sName = LV_028List.Items[3].SubItems[1].Text;
            string sGubun = "E";    // 실시간 등록 구분
            string sDelSeq = "";

            if (sDate.Trim().Length > 0 && sTime.Trim().Length > 0 && sSeqNo.Trim().Length > 0)
            {
                Requset_msvf028(sDate, sTime, sSeqNo, sGubun, sDelSeq);
                g_bSetReal = true;
            }
            else
                MessageBox.Show("조건 리스트에서 항목을 선택해 주세요");
        }

        private void Btn_CondiClearReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            string sDate = LV_028List.Items[0].SubItems[1].Text;
            string sTime = LV_028List.Items[1].SubItems[1].Text;
            string sSeqNo = LV_028List.Items[2].SubItems[1].Text;
            string sName = LV_028List.Items[3].SubItems[1].Text;
            string sGubun = "D";    // 실시간 해제 구분
            string sDelSeq = LV_028List.Items[5].SubItems[1].Text;  // 실시간 해제 설정번호

            if (sDate.Trim().Length > 0 && sTime.Trim().Length > 0 && sSeqNo.Trim().Length > 0)
            {
                if (sDelSeq.Trim().Length > 0)
                {
                    Requset_msvf028(sDate, sTime, sSeqNo, sGubun, sDelSeq);
                    g_bSetReal = false;
                }
                else
                    MessageBox.Show("실시간 해제 대상이 아닙니다.");
            }
            else
                MessageBox.Show("조건 리스트에서 항목을 선택해 주세요");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false || g_sLoginId.Length == 0)
                return;

            String sAcc, sReturn;
            int nCnt = axChampionCommAgent1.GetAccCnt();

            if (nCnt < 1)
            {
                MessageBox.Show("조회 가능한 계좌정보가 없습니다.");
            }
            else
            {
                sAcc = axChampionCommAgent1.GetAccInfo();
                sReturn = nCnt + "개 계좌 조회\r\n[ " + sAcc + " ]\r\n\n※계좌번호는 ';' 로 분리되어있습니다.";
                MessageBox.Show(sReturn);
            }
        }

        private void btn_MultiReal_Click(object sender, EventArgs e)
        {
            if (g_bLoginYN == false)
            {
                MessageBox.Show("로그인 상태를 확인 바랍니다.");
                return;
            }

            axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제

            String sJmCode = "KR7014910004;KR7047770003;KR7270520000;KR7307750000;KR7347000002;";
            if (sJmCode.Length == 0)
            {
                MessageBox.Show("종목코드를 입력 해주세요.");
                return;
            }

            axChampionCommAgent1.RegisterReal(1, sJmCode);  //주식 종목 우선호가
            axChampionCommAgent1.RegisterReal(21, sJmCode); //주식,ELW 종목 체결시세
            g_bSetReal = true;
            g_bMultiReal = true;
        }

        private void P(object x)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.fff"), x);
        }

        private void DynamicInitializeComponent()
        {
            this.axChampionCommAgent1.OnGetTranData += new AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetTranDataEventHandler(this.axChampionCommAgent1_OnGetTranData);
            this.axChampionCommAgent1.OnGetFidData += new AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetFidDataEventHandler(this.axChampionCommAgent1_OnGetFidData);
            this.axChampionCommAgent1.OnGetRealData += new AxChampionCommAgentLib._DChampionCommAgentEvents_OnGetRealDataEventHandler(this.axChampionCommAgent1_OnGetRealData);
        }

    } // End [public partial class Form1]
}
*/