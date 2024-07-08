using AxChampionCommAgentLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChampionOpenAPI_CSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int g_nVersionCheck;
        private string g_sOpenAPI_PATH;
        private LoginForm loginForm;
        private bool versionCheckSuccessFlag;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
#if DEBUG
            if(e.Key == Key.Escape)
            {
                OnVersionCheckSuccess();
            }
#endif
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // https://stackoverflow.com/a/1926796/14367566
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private void Btn_VerChk_Click(object sender, EventArgs e)
        {
            this.Btn_VerChk.IsEnabled = false;

            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.OpenSubKey("Software\\EugeneFN\\Champion", true);

            if (regkey != null)
            {
                Object objVal = regkey.GetValue("PATH");
                if (objVal != null)
                {
                    g_sOpenAPI_PATH = Convert.ToString(objVal);
                }
                else
                {
                    System.Windows.MessageBox.Show("OpenApi의 위치를 찾지 못했습니다.");
                    this.Btn_VerChk.IsEnabled = true;
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("프로그램의 위치를 찾지 못했습니다.");
                this.Btn_VerChk.IsEnabled = true;
                return;
            }

            Directory.SetCurrentDirectory(g_sOpenAPI_PATH);

            String sRunPath = g_sOpenAPI_PATH + "\\ChampionOpenAPIVersionProcess.exe";
            RunVersionCheckProcess(sRunPath);
        }

        // 프로그램 핸들 찾기(버전처리)
        private void RunVersionCheckProcess(string file)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = file;

            // https://stackoverflow.com/questions/1556182/finding-the-handle-to-a-wpf-window
            string arguments = "/" + new WindowInteropHelper(this).Handle;

            startInfo.Arguments = arguments;

            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                this.Btn_VerChk.IsEnabled = true;
            }
        }

        private void OnVersionCheckSuccess()
        {
            if (!this.versionCheckSuccessFlag)
            {
                // you only execute once
                this.versionCheckSuccessFlag = true;
                this.grid.Children.Clear();

                this.loginForm = new LoginForm();
                this.loginForm.LoginSuccess += LoginForm_LoginSuccess;
                this.grid.Children.Add(loginForm);
                this.loginForm.SetNVersionCheck(g_nVersionCheck);
            }
        }

        private void assert(bool v)
        {
            if (!v) throw new Exception();
        }

        private void LoginForm_LoginSuccess(object sender, EventArgs e)
        {
            assert(this.grid.Children.Count == 1);
            this.grid.Children.Clear();
            assert(this.grid.Children.Count == 0);

            LogoutForm f = new LogoutForm(loginForm.g_sLoginId, loginForm.axChampionCommAgent1);
            f.LogoutEugeneFN += OnLogoutEugeneFN;
            this.grid.Children.Add(f);
        }

        private void OnLogoutEugeneFN(object sender, EventArgs e)
        {
            this.grid.Children.Clear();
            this.grid.Children.Add(loginForm);
        }

        // 윈도우 메세지 수신(버전처리)
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            try
            {
                if (msg == 7422)  // 버전처리완료 메세지
                {
                    if ((int)lParam == 1)
                        g_nVersionCheck = (int)wParam;
                    else
                        g_nVersionCheck = 0;

                    if (g_nVersionCheck > 0)
                    {
                        OnVersionCheckSuccess();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                this.Btn_VerChk.IsEnabled = true;
            }
            return IntPtr.Zero;
        }
    }
}
