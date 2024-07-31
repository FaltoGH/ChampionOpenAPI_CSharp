using AxChampionCommAgentLib;
using ChampionOpenAPI_CSharp.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ChampionOpenAPI_CSharp
{

    public class AxChampionCommAgent2 : IDisposable
    {
        private const short VERSION_CHECKED = 0x1cfe;
        private void WndProc(ref Message m)
        {
            Console.WriteLine("message: " + m);
            if (m.Msg == VERSION_CHECKED)
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
        private readonly Concurrent<int> __versionCheckValue = new Concurrent<int>();
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
                WndProcForm form = new WndProcForm(WndProc);
                IntPtr handle = form.Handle;
                if (handle.ToInt32() <= 0)
                {
                    throw new InvalidCastException();
                }
                Console.WriteLine("handle: " + handle);
                string arguments = "/" + handle;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                __b1421533 = true;
                System.Windows.Forms.Application.Run();
            })
            { IsBackground = true }.Start();
            __versionCheckValue.WaitWhile(x => x == 0);
        }

        private AxChampionCommAgent __agent;
        private bool __isLoginSuccess;
        private string __loginedUserID;
        public int Login(string userID, string pwd, string certPwd)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            Thread t = new Thread(() =>
            {
                __agent = new AxChampionCommAgent();
                __agent.BeginInit();
                new Control().Controls.Add(__agent);
                __agent.EndInit();
                are.Set();
                System.Windows.Forms.Application.Run();
            })
            { IsBackground = true };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            are.WaitOne(0x3f3f3f3f);
            int ret= __agent.CommLogin(__versionCheckValue.Get(), userID, pwd, certPwd);
            if (ret == 0)
            {
                __loginedUserID = userID;
                __isLoginSuccess = true;
            }
            return ret;
        }

        public string GetLastErrMsg()
        {
            return __agent.GetLastErrMsg();
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
            if (__agent != null)
            {
                if (__isLoginSuccess)
                {
                    __agent?.CommLogout(__loginedUserID);
                }
                __agent?.AllUnRegisterReal();
                __agent?.CommTerminate(1);
                __agent?.Dispose();
            }
        }

        public string gbdayf(string strSCODE, string strCTP)
        {
            int nRqID = __agent.CreateRequestID();
            __agent.SetTranInputData(nRqID, "gbday", "InRec1", "SCODE", strSCODE);
            __agent.SetTranInputData(nRqID, "gbday", "InRec1", "CTP", strCTP);

            string strIsBenefit;   //수익증권여부 
            string strPrevOrNext;  //이전,다음 데이터 조회 구분
            string strPrevNextKey; //이전,다음 구분값
            string strScreenNo;    //화면 번호
            string strTranType;    //Tran 구분값
            int nRequestCount;  //최대 조회 데이터 건수

            strIsBenefit = "N"; //수익증권여부 'N'입력
            strPrevOrNext = "0";    //기본:0, 이전:1, 다음:2
            strPrevNextKey = "";    //기본은 키값이 없음
            strScreenNo = "1000";   //임시로 1000을 입력
            strTranType = "1";  //Tran = 1, Fid = 2
            nRequestCount = 20;   //임시로 20개을 입력

            __agent.RequestTran(nRqID, "gbday", strIsBenefit, strPrevOrNext, strPrevNextKey, strScreenNo, strTranType, nRequestCount);





            /****************************************************************************************
             개발 편의를 위한 CommAgent 조회응답 OnGetTranData 이벤트 함수처리 MFC 소스 템플릿
             O U T - P U T
             ****************************************************************************************/
            int nRqID = nRequestId; //서버로 부터 받은 RqID 값
            LPCTSTR sData = pBlock; //서버로 부터 받은 데이터 포인터값
            int nDataLen = nBlockLength;    //서버로 부터 받은 데이터 길이값

            CStringArray strArrLDATE;       //일자
            CStringArray strArrCPCHECK;     //전일대비부호
            CStringArray strArrLDIFF;       //전일대비
            CStringArray strArrLDIFFRATIO;      //대비율
            CStringArray strArrLCPRICE;     //현재가
            CStringArray strArrLVOLUME;     //누적거래량
            CStringArray strArrLVALUE;      //누적거래대금
            CStringArray strArrLOPRICE;     //시가
            CStringArray strArrLHPRICE;     //고가
            CStringArray strArrLLPRICE;     //저가
            CStringArray strArrLBPRICE;     //기준가


            데이터 건수를 얻어온다.
int nDataCnt = m_CommAgent.GetTranOutputRowCnt(gbday, OutRec1);

            for (int nPos = 0; nPos < nDataCnt; nPos++)
            {
                strArrLDATE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LDATE, nPos));
                strArrCPCHECK.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, CPCHECK, nPos));
                strArrLDIFF.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LDIFF, nPos));
                strArrLDIFFRATIO.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LDIFFRATIO, nPos));
                strArrLCPRICE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LCPRICE, nPos));
                strArrLVOLUME.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LVOLUME, nPos));
                strArrLVALUE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LVALUE, nPos));
                strArrLOPRICE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LOPRICE, nPos));
                strArrLHPRICE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LHPRICE, nPos));
                strArrLLPRICE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LLPRICE, nPos));
                strArrLBPRICE.Add(m_CommAgent.GetTranOutputData(gbday, OutRec1, LBPRICE, nPos));
            }


            m_CommAgent.ReleaseRqId(nRqID);	//서버로 부터 받은 RqID 해제한다.
        }
    }

}
