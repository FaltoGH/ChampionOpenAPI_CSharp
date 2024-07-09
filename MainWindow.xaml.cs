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
        private LoginForm loginForm;
        private bool versionCheckSuccessFlag;
        private IChampionCommAgent agent;

        public MainWindow()
        {
            InitializeComponent();
            agent = new AxChampionCommAgent2();
        }

        private void Btn_VerChk_Click(object sender, EventArgs e)
        {
            this.Btn_VerChk.IsEnabled = false;
            try
            {
                agent.VersionCheck(OnVersionCheckSuccess);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                this.Btn_VerChk.IsEnabled = true;
            }
        }

        private void OnVersionCheckSuccess(int x)
        {
            if (!this.versionCheckSuccessFlag)
            {
                // you only execute once
                this.versionCheckSuccessFlag = true;
                this.grid.Children.Clear();

                this.loginForm = new LoginForm();
                this.loginForm.LoginSuccess += LoginForm_LoginSuccess;
                this.grid.Children.Add(loginForm);
                this.loginForm.SetNVersionCheck(x, agent);
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
    }
}
