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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ChampionOpenAPI_CSharp
{

    public class axcca2 : IDisposable
    {
        private void WndProc(ref Message m)
        {
            Console.WriteLine("message: " + m);
            if (m.Msg == 0x1cfe) // version check success
            {
                int g_nVersionCheck;
                if ((int)m.LParam == 1)
                    g_nVersionCheck = (int)m.WParam;
                else
                    g_nVersionCheck = 0;

                if (g_nVersionCheck > 0)
                {
                    __versionCheckValue.Set(g_nVersionCheck);
                }
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
        private readonly conc<int> __versionCheckValue = new conc<int>();
        public void VersionCheck()
        {
            new Thread(() =>
            {
                if (__b1421533)
                {
                    throw new InvalidOperationException();
                }
                string path = GetApiAgentModulePath();
                Directory.SetCurrentDirectory(path);
                String sRunPath = Path.Combine(path, "ChampionOpenAPIVersionProcess.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = sRunPath;
                relaywp form = new relaywp(WndProc);
                IntPtr handle = form.Handle;
                if (handle.ToInt32() <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                string arguments = "/" + handle;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                __b1421533 = true;
                System.Windows.Forms.Application.Run();
            })
            { IsBackground = true, Name = "wndproct" }.Start();
            __versionCheckValue.WaitWhile(x => x == 0);
        }

        private AxChampionCommAgent axChampionCommAgent1;
        private bool __isLoginSuccess;
        private string __loginedUserID;
        public int Login(string userID, string pwd, string certPwd)
        {
            using (AutoResetEvent are = new AutoResetEvent(false))
            {
                Thread t = new Thread(() =>
                {
                    axChampionCommAgent1 = new AxChampionCommAgent();
                    axChampionCommAgent1.BeginInit();
                    new Control().Controls.Add(axChampionCommAgent1);
                    axChampionCommAgent1.EndInit();
                    are.Set();
                    axChampionCommAgent1.OnGetTranData += agent_OnGetTranData;
                    System.Windows.Forms.Application.Run();
                })
                { IsBackground = true, Name = "axt" };
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                if (!are.WaitOne(0x3f3f3f3f)) { throw new TimeoutException(); }
            }
            int ret = axChampionCommAgent1.CommLogin(__versionCheckValue.Get(), userID, pwd, certPwd);
            if (ret == 0)
            {
                __loginedUserID = userID;
                __isLoginSuccess = true;
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
                {
                    throw new FileNotFoundException();
                }
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
                if (__isLoginSuccess)
                {
                    axChampionCommAgent1?.CommLogout(__loginedUserID);
                }
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

        private List<gbdays> gbdayss;
        private readonly AutoResetEvent gbdayare = new AutoResetEvent(false);
        private void agent_OnGetTranData_gbday()
        {
            int nDataCnt = axChampionCommAgent1.GetTranOutputRowCnt("gbday", "OutRec1");
            gbdayss = new List<gbdays>();
            for (int i = 0; i < nDataCnt; i++)
            {
                gbdays g;
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
                gbdayss.Add(g);
            }
            gbdayare.Set();
        }

        private string sNextKey;
        private void agent_OnGetTranData(object sender, _DChampionCommAgentEvents_OnGetTranDataEvent e)
        {
            string sTrCode = axChampionCommAgent1.GetCommRecvOptionValue(0); // TR 코드
            string sNextGb = axChampionCommAgent1.GetCommRecvOptionValue(1);    // 이전/다음 조회구분(0:없음, 4:다음없음, 5:다음없음, 6:다음있음, 7:다음있음)
            if (sNextGb == "6" || sNextGb == "7")
            {
                sNextKey = axChampionCommAgent1.GetCommRecvOptionValue(2);    // 연속조회키
            }
            else
            {
                sNextKey = null;
            }
            string sMsg = axChampionCommAgent1.GetCommRecvOptionValue(4);    // 응답 메세지
            string sSubMsg = axChampionCommAgent1.GetCommRecvOptionValue(5);    // 부가 메세지
            string sErrCode = axChampionCommAgent1.GetCommRecvOptionValue(7);    // 에러여부
            Console.WriteLine("OnGetTranData: sTrCode={0} sNextGb={1} sNextKey={2} sMsg={3} sSubMsg={4} sErrCode={5}",
                sTrCode, sNextGb, sNextKey, sMsg, sSubMsg, sErrCode);

            if (sTrCode == "gbday")
            {
                agent_OnGetTranData_gbday();
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
        public gbdayfr gbdayf(string strSCODE, string strCTP, short nRequestCount, string sNextKey)
        {
            gbdayfr ret = new gbdayfr();
            int nRqID = axChampionCommAgent1.CreateRequestID();
            SetTranInputDatas(nRqID, "gbday", "InRec1", "SCODE", strSCODE, "CTP", strCTP);
            gbdayare.Reset();
            int nRtn = axChampionCommAgent1.RequestTran(nRqID, "gbday", sNextKey, nRequestCount);
            ret.nRtn = nRtn;
            if (nRtn < 1)
            {
                Console.WriteLine("error: gbdayf return " + nRtn);
                return ret;
            }

            if (!gbdayare.WaitOne(0x3f3f3f3f))
            {
                throw new TimeoutException();
            }

            ret.gbdayss = this.gbdayss;
            ret.sNextKey = this.sNextKey;
            ret.success = true;
            return ret;
        }

        /// <summary>
        /// 해외주식 일별 정보
        /// </summary>
        /// <param name="strSCODE">종목코드|20|거래소코드(4)+심볼(16)</param>
        /// <param name="strCTP">수정주가여부|1|0:미적용 1:적용</param>
        public gbdayfr gbdayf2(string strSCODE, string strCTP, short nRequestCount)
        {
            gbdayfr ret = gbdayf(strSCODE, strCTP, nRequestCount, null);

            while (ret.Count < nRequestCount && ret.sNextKey != null)
            {
                //call gbdayf
                gbdayfr ret2 = gbdayf(strSCODE, strCTP, nRequestCount, ret.sNextKey);
                if (!ret2.success)
                {
                    break;
                }
                ret.gbdayss.AddRange(ret2.gbdayss);
                ret.nRtn = ret2.nRtn;
                ret.sNextKey = ret2.sNextKey;
                Thread.Sleep(127);
            }

            return ret;
        }

        public string GetAccInfo()
        {
            return axChampionCommAgent1.GetAccInfo();
        }

        private const string g_sTrcode_gbBSOrder = "OTD6101U";    //해외주식 매수/매도 주문전송 TrCode

        /// <summary>
        /// 해외주식 매수/매도 주문 전송(OTD6101U)
        /// </summary>
        /// <param name="sAccNo"></param>
        /// <param name="sAccPwd"></param>
        /// <param name="sExgCode">거래소코드</param>
        /// <param name="sJmCode">종목코드</param>
        /// <param name="sOrdQty">주문수량</param>
        /// <param name="sOrdPrc">해외증권주문단가</param>
        /// <param name="sTradeGb">매매구분(10:매수, 20:매도)</param>
        /// <param name="sOrdTypeCode">해외증권주문유형구분</param>
        public string SendOrder(string sAccNo,
            string sAccPwd, string sExgCode, string sJmCode,
            string sOrdQty, string sOrdPrc, string sTradeGb, string sOrdTypeCode)
        {
            int nRqId = axChampionCommAgent1.CreateRequestID();
            int nRtn;
            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "ACNO", sAccNo);       //계좌번호
            if (nRtn < 1) //계좌번호 입력 에러
            {
                return axChampionCommAgent1.GetLastErrMsg();
            }

            nRtn = axChampionCommAgent1.SetTranInputData(nRqId, g_sTrcode_gbBSOrder, "InRec1", "AC_PWD", sAccPwd);    //계좌비밀번호
            if (nRtn < 1) //계좌 비밀번호 에러
            {
                return axChampionCommAgent1.GetLastErrMsg();
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
            if (nRtn < 1) //해외주식 매수/매도 주문전송 실패
            {
                return axChampionCommAgent1.GetLastErrMsg();
            }

            return null;
        }

    }

}
