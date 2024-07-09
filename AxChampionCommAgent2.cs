using AxChampionCommAgentLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ChampionOpenAPI_CSharp
{
    public class AxChampionCommAgent2 : AxChampionCommAgent, IChampionCommAgent
    {
        private string apiAgentModulePath;
        private string gbcode_cod;
        private List<string> codeList;
        private Action<int> versionCheckCallback;
        private bool versionCheckAttempted;

        private class Control2 : Form
        {
            private Action<Message> wndProc;
            public Control2(Action<Message> wndProc)
            {
                this.wndProc = wndProc;
            }
            protected override void WndProc(ref Message m)
            {
                wndProc?.Invoke(m);
                base.WndProc(ref m);
            }
        }
        
        public AxChampionCommAgent2()
        {
            this.BeginInit();

            Control2 parent = new Control2(wndProc);
            parent.Controls.Add(this);

#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif
            this.EndInit();
#if DEBUG
            Console.WriteLine("[DEBUG] [AxChampionCommAgent2..ctor.EndInit] " + sw.Elapsed);
            sw = null;
#endif
        }

        public override string GetApiAgentModulePath()
        {
            if(!string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                return apiAgentModulePath;
            }

            apiAgentModulePath = base.GetApiAgentModulePath();
            if (!string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                return apiAgentModulePath;
            }

            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.OpenSubKey("Software\\EugeneFN\\Champion", true);
            if (regkey == null)
            {
                throw new KeyNotFoundException("프로그램의 위치를 찾지 못했습니다.");
            }
            
            Object objVal = regkey.GetValue("PATH");
            if (objVal == null)
            {
                throw new DirectoryNotFoundException("OpenApi의 위치를 찾지 못했습니다.");
            }

            apiAgentModulePath = Convert.ToString(objVal);
            if (string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                throw new NullReferenceException();
            }

            return apiAgentModulePath;
        }

        public IReadOnlyList<string> GetCodeList()
        {
            if (gbcode_cod == null)
            {
                gbcode_cod = Path.Combine(GetApiAgentModulePath(), "mst", "gbcode.cod");
                if (!File.Exists(gbcode_cod))
                {
                    throw new FileNotFoundException();
                }
            }
            if (codeList == null)
            {
                codeList = new List<string>();
                string[] allLines = File.ReadAllLines(gbcode_cod);
                foreach (var line in allLines)
                {
                    string code = line.Substring(0, 20);
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        code = code.Trim();
                        this.codeList.Add(code);
                    }
                }
            }
            return codeList;
        }

        // 프로그램 핸들 찾기(버전처리)
        private void RunVersionCheckProcess(string file)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = file;

            IntPtr handle = Parent.Handle;
            if (handle.ToInt32() <= 0)
            {
                throw new InvalidActiveXStateException();
            }

#if DEBUG
            Console.WriteLine("[DEBUG] handle="+handle);
#endif

            string arguments = "/" + handle;

            startInfo.Arguments = arguments;

            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";
            Process.Start(startInfo);
        }

        // 윈도우 메세지 수신(버전처리)
        void wndProc(Message m)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] [{0}] {1}", DateTime.Now.ToString("HH:mm:ss.fff"), m);
#endif

            if (m.Msg == 0x1cfe)  // 버전처리완료 메세지
            {
            Console.WriteLine("[DEBUG] [{0}] Version process done.", DateTime.Now.ToString("HH:mm:ss.fff"));

                int g_nVersionCheck;
                if ((int)m.LParam == 1)
                    g_nVersionCheck = (int)m.WParam;
                else
                    g_nVersionCheck = 0;

                if (g_nVersionCheck > 0)
                {
                    versionCheckCallback?.Invoke(g_nVersionCheck);
                    versionCheckCallback = null;
                }
            }
        }

        public void VersionCheck(Action<int> versionCheckCallback)
        {
            if (versionCheckAttempted)
            {
                throw new InvalidOperationException();
            }

            this.versionCheckCallback = versionCheckCallback;

            string path = GetApiAgentModulePath();
            Directory.SetCurrentDirectory(path);

            String sRunPath = Path.Combine(path, "ChampionOpenAPIVersionProcess.exe");
            RunVersionCheckProcess(sRunPath);

            versionCheckAttempted = true;
        }

        public override int CommLogin(int nVersionPassKey, string sUserID, string sPwd, string sCertPwd)
        {
            if(nVersionPassKey <= 0)
                throw new ArgumentOutOfRangeException();
            return base.CommLogin(nVersionPassKey, sUserID, sPwd, sCertPwd);
        }

        public override int CommLoginPartner(int nVersionPassKey, string sUserID, string sPwd, string sCertPwd, string sPartnerCode)
        {
            if (nVersionPassKey <= 0)
                throw new ArgumentOutOfRangeException();
            return base.CommLoginPartner(nVersionPassKey, sUserID, sPwd, sCertPwd, sPartnerCode);
        }
    }
}
