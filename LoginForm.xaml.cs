using AxChampionCommAgentLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChampionOpenAPI_CSharp
{
    public partial class LoginForm : System.Windows.Controls.UserControl
    {
        public AxChampionCommAgentLib.AxChampionCommAgent axChampionCommAgent1;
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CS0414 // The field 'LoginForm.g_bLoginYN' is assigned but its value is never used
        private bool g_bLoginYN;
#pragma warning restore CS0414 // The field 'LoginForm.g_bLoginYN' is assigned but its value is never used
#pragma warning restore IDE0052 // Remove unread private members
        private string g_sMsg;
        private string g_sLoginId;
        private int g_nVersionCheck;
        private const string USERID = "eugenefn_userid";

        public LoginForm(int g_nVersionCheck)
        {
            InitializeComponent();
            InitComp();
            this.g_nVersionCheck = g_nVersionCheck;

            string s = Environment.GetEnvironmentVariable(USERID, EnvironmentVariableTarget.User);
            if(!string.IsNullOrWhiteSpace(s))
            {
                TB_ID.Text = s;
            }
        }

        private void OnLoginSuccess()
        {
            g_bLoginYN = true;
            //Btn_Logout.Enabled = true;
            //Btn_Search.Enabled = true;
            //Btn_SetReal.Enabled = true;
            //Btn_UnReal.Enabled = true;
            //Btn_Kwansim.Enabled = true;

            //Btn_GBSearch.Enabled = true;
            //Btn_GBSetReal.Enabled = true;
            //Btn_GBUnReal.Enabled = true;
            //Btn_GBKwansim.Enabled = true;

            //Btn_FOSearch.Enabled = true;
            //Btn_FOSetReal.Enabled = true;
            //Btn_FOUnReal.Enabled = true;

            if(CB_saveID.IsChecked == true)
            {
                Environment.SetEnvironmentVariable(USERID, g_sLoginId, EnvironmentVariableTarget.User);
            }
            else
            {
                Environment.SetEnvironmentVariable(USERID, string.Empty, EnvironmentVariableTarget.User);
            }

            System.Windows.MessageBox.Show("로그인 성공");

            new Form1(axChampionCommAgent1, g_sLoginId).ShowDialog();
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            string strID = TB_ID.Text;             // 로그인 아이디
            string strPwd = TB_Pwd.Password;           // 로그인 비밀번호
            string strCretPwd = TB_CretPwd.Password;   // 인증 비밀번호
            //string strPartnerCode = ComCode.Text;       // 제휴사 코드

            if (strID.Length == 0 || strPwd.Length == 0)
            {
                System.Windows.MessageBox.Show("로그인 입력값을 확인 바랍니다.");
                return;
            }

            if (strCretPwd.Length == 0 && !CB_PO.IsChecked.GetValueOrDefault())
            {
                MessageBoxResult MsgRtn = System.Windows.MessageBox.Show(
                    "시세전용으로 로그인 하시겠습니까?\n인증서 비밀번호를 입력하시려면 취소를 누르십시오.",
                    "확인", MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Information);
                if (MsgRtn == MessageBoxResult.Cancel)
                {
                    return;
                }
                CB_PO.IsChecked = true;
            }


            int nRtn;
            //if (ChkPartner.Checked == false)
                nRtn = axChampionCommAgent1.CommLogin(g_nVersionCheck, strID, strPwd, strCretPwd);      // 일반 로그인
            //else
                //nRtn = axChampionCommAgent1.CommLoginPartner(g_nVersionCheck, strID, strPwd, strCretPwd, strPartnerCode);       //제휴사용 로그인

            if (nRtn == 0)
            {
                //로그인 성공
                g_sLoginId = strID;
                OnLoginSuccess();
            }
            else
            {
                //로그인 실패
                g_bLoginYN = false;

                g_sMsg = axChampionCommAgent1.GetLastErrMsg();
                System.Windows.MessageBox.Show(g_sMsg.Trim(), "로그인 실패");
                return;
            }
        }
        
        private void InitComp()
        {
            Stopwatch sw = Stopwatch.StartNew();
            this.axChampionCommAgent1 = new AxChampionCommAgentLib.AxChampionCommAgent();
            ((System.ComponentModel.ISupportInitialize)(this.axChampionCommAgent1)).BeginInit();
            new System.Windows.Forms.GroupBox().Controls.Add(this.axChampionCommAgent1);
            // 
            // axChampionCommAgent1
            // 
            this.axChampionCommAgent1.Enabled = true;
            this.axChampionCommAgent1.Location = new System.Drawing.Point(168, 46);
            this.axChampionCommAgent1.Name = "axChampionCommAgent1";
            this.axChampionCommAgent1.Size = new System.Drawing.Size(46, 22);
            this.axChampionCommAgent1.TabIndex = 16;
            ((System.ComponentModel.ISupportInitialize)(this.axChampionCommAgent1)).EndInit();
            Console.WriteLine("InitComp:" + sw.Elapsed);
        }

        private void QUEST_PO_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("주문 및 은행이체, 계좌간대체, 청약서비스 등 인증서가 반드시 필요한 거래는 이용하실 수 없습니다.", "시세전용으로 로그인");
        }
    }
}
