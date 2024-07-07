using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChampionOpenAPI_CSharp
{
    /// <summary>
    /// Interaction logic for LogoutForm.xaml
    /// </summary>
    public partial class LogoutForm : UserControl
    {
        public event EventHandler LogoutEugeneFN;
        private IChampionCommAgent axChampionCommAgent1;
        private string g_sLoginId;

        public LogoutForm(string g_sLoginId, IChampionCommAgent axChampionCommAgent1)
        {
            InitializeComponent();
            this.g_sLoginId = g_sLoginId;
            this.axChampionCommAgent1 = axChampionCommAgent1;
        }

        private void BTN_logout_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(g_sLoginId))
                return;

            int nRtn = axChampionCommAgent1.CommLogout(g_sLoginId);

            if (nRtn > 0)
            {
                //로그아웃 성공
                //g_bLoginYN = false;
                //Btn_Logout.Enabled = false;
                //Btn_Search.Enabled = false;
                //Btn_SetReal.Enabled = false;
                //Btn_UnReal.Enabled = false;
                //Btn_Kwansim.Enabled = false;

                //Btn_GBSearch.Enabled = false;
                //Btn_GBSetReal.Enabled = false;
                //Btn_GBUnReal.Enabled = false;
                //Btn_GBKwansim.Enabled = false;

                //Btn_FOSearch.Enabled = false;
                //Btn_FOSetReal.Enabled = false;
                //Btn_FOUnReal.Enabled = false;
                
                // Disable "Stay signed in"
                Environment.SetEnvironmentVariable(LoginForm.PWD, string.Empty, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable(LoginForm.CRETPWD, string.Empty, EnvironmentVariableTarget.User);
                
                axChampionCommAgent1.AllUnRegisterReal();   //모든 실시간 해제
                axChampionCommAgent1.CommTerminate(1);   //통신종료

                LogoutEugeneFN?.Invoke(sender, e);
            }
        }
    }
}
