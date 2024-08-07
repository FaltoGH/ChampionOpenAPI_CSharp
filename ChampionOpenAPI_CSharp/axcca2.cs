using AxChampionCommAgentLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ChampionOpenAPI_CSharp
{

    public class axcca2 : IDisposable
    {
        private delegate void WndProcDelegate(ref Message m);
        private class RelayWndProcForm : Form
        {
            private readonly WndProcDelegate __wndProc;
            public RelayWndProcForm(WndProcDelegate wndProc)
            {
                __wndProc = wndProc;
            }
            protected override void WndProc(ref Message m)
            {
                __wndProc?.Invoke(ref m);
                base.WndProc(ref m);
            }
        }

        private int g_nVersionCheck;
        private bool m_bIsVersionChecked;

        private void WndProc(ref Message m)
        {
            if (m.Msg == 0x1cfe) // version check success
            {
                if ((int)m.LParam == 1)
                    g_nVersionCheck = (int)m.WParam;
                else
                    g_nVersionCheck = 0;

                if (g_nVersionCheck > 0)
                    m_bIsVersionChecked = true;
            }
        }

        private string __s2359132;
        public string GetApiAgentModulePath()
        {
            if (string.IsNullOrWhiteSpace(__s2359132))
            {
                RegistryKey regkey = Registry.CurrentUser;
                regkey = regkey.OpenSubKey("Software\\EugeneFN\\Champion");
                if (regkey == null)
                {
                    throw new KeyNotFoundException();
                }
                object objVal = regkey.GetValue("PATH");
                if (objVal == null)
                {
                    throw new DirectoryNotFoundException();
                }
                __s2359132 = objVal.ToString();
                if (string.IsNullOrWhiteSpace(__s2359132))
                {
                    throw new NullReferenceException();
                }
            }
            return __s2359132;
        }

        private bool __b1421533;
        public void VersionCheck()
        {
            new Thread(() =>
            {
                if (__b1421533)
                    throw new InvalidOperationException();
                string path = GetApiAgentModulePath();
                Directory.SetCurrentDirectory(path);
                String sRunPath = Path.Combine(path, "ChampionOpenAPIVersionProcess.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = sRunPath;
                RelayWndProcForm form = new RelayWndProcForm(WndProc);
                IntPtr handle = form.Handle;
                if (handle.ToInt32() <= 0)
                    throw new ArgumentOutOfRangeException();
                string arguments = "/" + handle;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                __b1421533 = true;
                System.Windows.Forms.Application.Run();
            })
            { IsBackground = true, Name = "wndproct" }.Start();
            while (!m_bIsVersionChecked)
                Thread.Sleep(127);
        }

        private AxChampionCommAgent axChampionCommAgent1;
        private bool m_bIsLoginSuccess;
        private string m_sLoginedUserID;
        public int Login(string userID, string pwd, string certPwd)
        {
            if (g_nVersionCheck <= 0 || !m_bIsVersionChecked)
                throw new InvalidOperationException();
            using (AutoResetEvent are = new AutoResetEvent(false))
            {
                Thread t = new Thread(() =>
                {
                    axChampionCommAgent1 = new AxChampionCommAgent();
                    axChampionCommAgent1.BeginInit();
                    new Control().Controls.Add(axChampionCommAgent1);
                    axChampionCommAgent1.EndInit();
                    are.Set();
                    axChampionCommAgent1.OnGetTranData += axChampionCommAgent1_OnGetTranData;
                    System.Windows.Forms.Application.Run();
                })
                { IsBackground = true, Name = "axt" };
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                if (!are.WaitOne(0x3f3f3f3f)) throw new TimeoutException();
            }
            int ret = axChampionCommAgent1.CommLogin(g_nVersionCheck, userID, pwd, certPwd);
            if (ret == 0)
            {
                m_sLoginedUserID = userID;
                m_bIsLoginSuccess = true;
            }
            return ret;
        }

        public string GetLastErrMsg()
        {
            return axChampionCommAgent1.GetLastErrMsg();
        }

        private string __s3519352;
        private string GetGbcodeCod()
        {
            if (__s3519352 == null)
            {
                __s3519352 = Path.Combine(GetApiAgentModulePath(), "mst", "gbcode.cod");
                if (!File.Exists(__s3519352))
                    throw new FileNotFoundException();
            }
            return __s3519352;
        }

        private List<string> __ls3259135;
        public List<string> GetCodeList()
        {
            if (__ls3259135 == null)
            {
                __ls3259135 = new List<string>();
                string[] allLines = File.ReadAllLines(GetGbcodeCod());
                foreach (var line in allLines)
                {
                    string code = line.Substring(0, 20);
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        code = code.Trim();
                        __ls3259135.Add(code);
                    }
                }
            }
            return __ls3259135;
        }

        public void Dispose()
        {
            if (axChampionCommAgent1 != null)
            {
                if (m_bIsLoginSuccess)
                    axChampionCommAgent1?.CommLogout(m_sLoginedUserID);
                axChampionCommAgent1?.AllUnRegisterReal();
                try
                {
                    axChampionCommAgent1?.CommTerminate(1);
                }
                catch (COMException e)
                {
                    Console.WriteLine("Dispose: catched exception: " + e);
                }
                axChampionCommAgent1?.Dispose();
            }
        }

        private List<gbday_struct> m_gbday_structs;
        private readonly AutoResetEvent m_gbday_are = new AutoResetEvent(false);
        private void axChampionCommAgent1_OnGetTranData_gbday()
        {
            int nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt("gbday", "OutRec1");
            m_gbday_structs = new List<gbday_struct>();
            for (int i = 0; i < nDataCnt; i++)
            {
                gbday_struct g;
                g.ldate = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LDATE", i);
                g.cpcheck = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "CPCHECK", i);
                g.ldiff = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LDIFF", i);
                g.ldiffratio = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LDIFFRATIO", i);
                g.lcprice = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LCPRICE", i);
                g.lvolume = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LVOLUME", i);
                g.lvalue = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LVALUE", i);
                g.loprice = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LOPRICE", i);
                g.lhprice = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LHPRICE", i);
                g.llprice = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LLPRICE", i);
                g.lbprice = axChampionCommAgent1.GetTranOutputData("gbday", "OutRec1", "LBPRICE", i);
                m_gbday_structs.Add(g);
            }
            m_gbday_are.Set();
        }

        private string m_sOrdNo;
        private readonly AutoResetEvent m_gbBSOrder_are = new AutoResetEvent(false);
        private void axChampionCommAgent1_OnGetTranData_gbBSOrder()
        {
            string sOrdNo = axChampionCommAgent1.GetTranOutputData(g_sTrcode_gbBSOrder, "OutRec1", "ORD_NO", 0);   //주문번호
            if (!string.IsNullOrWhiteSpace(sOrdNo))
            {
                m_sOrdNo = sOrdNo;
            }
            else
            {
                m_sOrdNo = null;
            }
            m_gbBSOrder_are.Set();
        }

        private string m_sNextKey;
        private void axChampionCommAgent1_OnGetTranData(object sender, _DChampionCommAgentEvents_OnGetTranDataEvent e)
        {
            string sTrCode = axChampionCommAgent1.GetCommRecvOptionValue(0); // TR 코드
            string sNextGb = axChampionCommAgent1.GetCommRecvOptionValue(1);    // 이전/다음 조회구분(0:없음, 4:다음없음, 5:다음없음, 6:다음있음, 7:다음있음)
            if (sNextGb == "6" || sNextGb == "7")
            {
                m_sNextKey = axChampionCommAgent1.GetCommRecvOptionValue(2);    // 연속조회키
            }
            else
            {
                m_sNextKey = null;
            }
            string sMsg = axChampionCommAgent1.GetCommRecvOptionValue(4);    // 응답 메세지
            string sSubMsg = axChampionCommAgent1.GetCommRecvOptionValue(5);    // 부가 메세지
            string sErrCode = axChampionCommAgent1.GetCommRecvOptionValue(7);    // 에러여부
            Console.WriteLine("OnGetTranData: sTrCode={0} sNextGb={1} sNextKey={2} sMsg={3} sSubMsg={4} sErrCode={5}",
                sTrCode, sNextGb, m_sNextKey, sMsg, sSubMsg, sErrCode);//null safe

            if (sTrCode == "gbday")
            {
                axChampionCommAgent1_OnGetTranData_gbday();
            }
            else if(sTrCode == g_sTrcode_gbBSOrder)
            {
                axChampionCommAgent1_OnGetTranData_gbBSOrder();
            }
        }

        private void SetTranInputDatas(int nRqId, string strTrCode, string strRecName, params string[] strItemValues)
        {
            for (sbyte i = 0; i < strItemValues.Length - 1; i += 2)
            {
                axChampionCommAgent1.SetTranInputData(nRqId, strTrCode, strRecName, strItemValues[i], strItemValues[i + 1]);
            }
        }

        /// <summary>
        /// 해외주식 일별 정보
        /// </summary>
        /// <param name="strSCODE">종목코드|20|거래소코드(4)+심볼(16)</param>
        /// <param name="strCTP">수정주가여부|1|0:미적용 1:적용</param>
        /// <returns>List of gbday_struct and sNextKey.</returns>
        public ValueTuple2<List<gbday_struct>,string> gbdayf(string strSCODE, string strCTP, short nRequestCount, string sNextKey)
        {
            int nRqID = axChampionCommAgent1.CreateRequestID();
            SetTranInputDatas(nRqID, "gbday", "InRec1", "SCODE", strSCODE, "CTP", strCTP);
            m_gbday_are.Reset();
            int nRtn = axChampionCommAgent1.RequestTran(nRqID, "gbday", sNextKey, nRequestCount);
            if (nRtn < 1)
            {
                throw new Exception();
            }
            if (!m_gbday_are.WaitOne(0x3f3f3f3f))
            {
                throw new TimeoutException();
            }
            return new ValueTuple2<List<gbday_struct>, string>(m_gbday_structs, m_sNextKey);
        }

        private void SleepBeforeRequest()
        {
            Thread.Sleep(200);
        }

        /// <summary>
        /// 해외주식 일별 정보
        /// </summary>
        /// <param name="strSCODE">종목코드|20|거래소코드(4)+심볼(16)</param>
        /// <param name="strCTP">수정주가여부|1|0:미적용 1:적용</param>
        /// <returns>List of gbday_struct and sNextKey.</returns>
        public ValueTuple2<List<gbday_struct>, string> gbdayf2(string strSCODE, string strCTP, short nRequestCount)
        {
            ValueTuple2<List<gbday_struct>, string> ret = gbdayf(strSCODE, strCTP, nRequestCount, null);

            while (ret.Item1.Count < nRequestCount && ret.Item2 != null)
            {
                SleepBeforeRequest();
                ValueTuple2<List<gbday_struct>, string> ret2 = gbdayf(strSCODE, strCTP, nRequestCount, ret.Item2);
                ret.Item1.AddRange(ret2.Item1);
                ret.Item2 = ret2.Item2;
            }

            return ret;
        }

        private Account[] __a325196;
        public Account[] GetAccounts()
        {
            if (__a325196 == null)
            {
                string accInfo = axChampionCommAgent1.GetAccInfo();
                string[] accNos = accInfo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                __a325196 = Array.ConvertAll(accNos, x => new Account(x));
            }
            return __a325196;
        }

        private const string g_sTrcode_gbBSOrder = "OTD6101U";    //해외주식 매수/매도 주문전송 TrCode

        /// <summary>
        /// 해외주식 매수/매도 주문 전송(OTD6101U)
        /// </summary>
        /// <param name="account">계좌</param>
        /// <param name="sExgCode">거래소코드</param>
        /// <param name="sJmCode">종목코드</param>
        /// <param name="sOrdQty">주문수량</param>
        /// <param name="sOrderPrice">해외증권주문단가</param>
        /// <param name="bTradeGb">매매구분(false:매수, true:매도)</param>
        /// <param name="ordType">해외증권주문유형구분</param>
        /// <returns>주문번호</returns>
        public string SendBSOrderGB(Account account, string sExgCode, string sJmCode,
            string sOrdQty, string sOrderPrice, bool bTradeGb, OrderType ordType)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn;

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ACNO", account.Number);       //계좌번호
            if (nRtn < 1) //계좌번호 입력 에러
            {
                throw new Exception();
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "AC_PWD", account.Password);    //계좌비밀번호
            if (nRtn < 1) //계좌 비밀번호 에러
            {
                throw new Exception();
            }

            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "EXG_COD", sExgCode);           //거래소코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ITEM_COD", sJmCode);           //종목코드
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ORD_Q", sOrdQty);              //주문수량
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "FGST_ORD_UPR", sOrderPrice);       //해외증권주문단가
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "BUY_SEL_TR_TCD", bTradeGb?"20":"10");    //매매구분(10:매수, 20:매도)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "FGST_BNS_TCD", ordType.Code);  //해외증권주문유형구분
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ORD_COND_TCD", "0");           //주문조건구분("0" 으로 고정)
            axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "EMC_ORD_YN", "N");             //비상주문여부("N" 으로 고정)

            nRtn = axChampionCommAgent1.RequestTran(nRqId, g_sTrcode_gbBSOrder, "", 20);
            if (nRtn < 1) //해외주식 매수/매도 주문전송 실패
            {
                throw new Exception();
            }

            if (!m_gbBSOrder_are.WaitOne(0x3f3f3f3f))
            {
                throw new TimeoutException();
            }

            return m_sOrdNo;
        }

    }

}
