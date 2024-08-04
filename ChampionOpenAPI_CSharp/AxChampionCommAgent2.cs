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
                __agent.OnGetTranData += __agent_OnGetTranData;
                System.Windows.Forms.Application.Run();
            })
            { IsBackground = true, Name = "axt" };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            if (!are.WaitOne(0x3f3f3f3f)) throw new TimeoutException();
            int ret = __agent.CommLogin(__versionCheckValue.Get(), userID, pwd, certPwd);
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
                try
                {
                    __agent?.CommTerminate(1);
                }
                catch (COMException e)
                {
                    Console.WriteLine("Dispose: " + e);
                }
                __agent?.Dispose();
            }
        }

        private const string OutRec1 = "OutRec1";
        private void __agent_OnGetTranData_gbday()
        {
            int nDataCnt = __agent.GetTranOutputRowCnt(gbday, OutRec1);
            for (int i = 0; i < nDataCnt; i++)
            {
                string ldate = __agent.GetTranOutputData(gbday, OutRec1, "LDATE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "CPCHECK", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LDIFF", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LDIFFRATIO", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LCPRICE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LVOLUME", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LVALUE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LOPRICE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LHPRICE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LLPRICE", i);
                __agent.GetTranOutputData(gbday, OutRec1, "LBPRICE", i);
                Console.WriteLine(ldate);
            }
        }

        private void __agent_OnGetTranData(object sender, _DChampionCommAgentEvents_OnGetTranDataEvent e)
        {
            string sTrCode = __agent.GetCommRecvOptionValue(0); // TR 코드
            if (sTrCode == gbday)
            {
                __agent_OnGetTranData_gbday();
            }
        }

        private const string gbday = "gbday";
        private const string InRec1 = "InRec1";
        /// <summary>
        /// 해외주식 일별 정보
        /// </summary>
        /// <param name="strSCODE">종목코드|20|거래소코드(4)+심볼(16)</param>
        /// <param name="strCTP">수정주가여부|1|0:미적용 1:적용</param>
        public gbdays[] gbdayf(string strSCODE, string strCTP)
        {
            int nRqID = __agent.CreateRequestID();
            __agent.SetTranInputData(nRqID, gbday, InRec1, "SCODE", strSCODE);
            __agent.SetTranInputData(nRqID, gbday, InRec1, "CTP", strCTP);
            int nRtn = __agent.RequestTran(nRqID, gbday, "", 20);
            if (nRtn < 1)
            {
                return null;
            }
            return null;
        }

        public string GetAccInfo()
        {
            return __agent.GetAccInfo();
        }

    }

}
