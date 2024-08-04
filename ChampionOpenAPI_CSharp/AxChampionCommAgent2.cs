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

        private AxChampionCommAgent agent;
        private bool __isLoginSuccess;
        private string __loginedUserID;
        public int Login(string userID, string pwd, string certPwd)
        {
            using (AutoResetEvent are = new AutoResetEvent(false))
            {
                Thread t = new Thread(() =>
                {
                    agent = new AxChampionCommAgent();
                    agent.BeginInit();
                    new Control().Controls.Add(agent);
                    agent.EndInit();
                    are.Set();
                    agent.OnGetTranData += agent_OnGetTranData;
                    System.Windows.Forms.Application.Run();
                })
                { IsBackground = true, Name = "axt" };
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                if (!are.WaitOne(0x3f3f3f3f)) { throw new TimeoutException(); }
            }
            int ret = agent.CommLogin(__versionCheckValue.Get(), userID, pwd, certPwd);
            if (ret == 0)
            {
                __loginedUserID = userID;
                __isLoginSuccess = true;
            }
            return ret;
        }

        public string GetLastErrMsg()
        {
            return agent.GetLastErrMsg();
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
            if (agent != null)
            {
                if (__isLoginSuccess)
                {
                    agent?.CommLogout(__loginedUserID);
                }
                agent?.AllUnRegisterReal();
                try
                {
                    agent?.CommTerminate(1);
                }
                catch (COMException e)
                {
                    Console.WriteLine("Dispose: catched exception: " + e);
                }
                agent?.Dispose();
            }
        }

        private const string OutRec1 = "OutRec1";
        private gbdays[] gbdayss;
        private readonly AutoResetEvent gbdayare = new AutoResetEvent(false);
        private void agent_OnGetTranData_gbday()
        {
            int nDataCnt = agent.GetTranOutputRowCnt(gbday, OutRec1);
            gbdayss = new gbdays[nDataCnt];
            for (int i = 0; i < nDataCnt; i++)
            {
                gbdayss[i].ldate = agent.GetTranOutputData(gbday, OutRec1, "LDATE", i);
                gbdayss[i].cpcheck = agent.GetTranOutputData(gbday, OutRec1, "CPCHECK", i);
                gbdayss[i].ldiff = agent.GetTranOutputData(gbday, OutRec1, "LDIFF", i);
                gbdayss[i].ldiffratio = agent.GetTranOutputData(gbday, OutRec1, "LDIFFRATIO", i);
                gbdayss[i].lcprice = agent.GetTranOutputData(gbday, OutRec1, "LCPRICE", i);
                gbdayss[i].lvolume = agent.GetTranOutputData(gbday, OutRec1, "LVOLUME", i);
                gbdayss[i].lvalue = agent.GetTranOutputData(gbday, OutRec1, "LVALUE", i);
                gbdayss[i].loprice = agent.GetTranOutputData(gbday, OutRec1, "LOPRICE", i);
                gbdayss[i].lhprice = agent.GetTranOutputData(gbday, OutRec1, "LHPRICE", i);
                gbdayss[i].llprice = agent.GetTranOutputData(gbday, OutRec1, "LLPRICE", i);
                gbdayss[i].lbprice = agent.GetTranOutputData(gbday, OutRec1, "LBPRICE", i);
            }
            gbdayare.Set();
        }

        private void agent_OnGetTranData(object sender, _DChampionCommAgentEvents_OnGetTranDataEvent e)
        {
            string sTrCode = agent.GetCommRecvOptionValue(0); // TR 코드
            if (sTrCode == gbday)
            {
                agent_OnGetTranData_gbday();
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
            int nRqID = agent.CreateRequestID();
            agent.SetTranInputData(nRqID, gbday, InRec1, "SCODE", strSCODE);
            agent.SetTranInputData(nRqID, gbday, InRec1, "CTP", strCTP);
            gbdayare.Reset();
            int nRtn = agent.RequestTran(nRqID, gbday, "", 20);
            if (nRtn < 1)
            {
                return null;
            }
            if (!gbdayare.WaitOne(0x3f3f3f3f))
            {
                throw new TimeoutException();
            }
            return gbdayss;
        }

        public string GetAccInfo()
        {
            return agent.GetAccInfo();
        }

    }

}
