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
            return __agent.CommLogin(__versionCheckValue.Get(), userID, pwd, certPwd);
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
            __agent?.Dispose();
        }
    }

}
