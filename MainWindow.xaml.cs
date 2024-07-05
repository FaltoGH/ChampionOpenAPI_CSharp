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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // https://stackoverflow.com/a/1926796/14367566
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private void Btn_VerChk_Click(object sender, EventArgs e)
        {
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
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("프로그램의 위치를 찾지 못했습니다.");
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
            catch (Win32Exception)
            {
                return;
            }
            this.button.IsEnabled = false;
        }

        private void OnVersionCheckSuccess()
        {
            if (!versionCheckSuccessFlag)
            {
                // you only execute once
                versionCheckSuccessFlag = true;
                this.grid.Children.Remove(this.button);
                loginForm = new LoginForm(g_nVersionCheck);
                this.grid.Children.Add(loginForm);
            }
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
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.loginForm?.axChampionCommAgent1?.Dispose();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.Escape)
            {
                OnVersionCheckSuccess();
            }
        }
    }
}
