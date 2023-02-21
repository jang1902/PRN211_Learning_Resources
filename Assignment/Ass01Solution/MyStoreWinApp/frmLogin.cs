using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStoreWinApp
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
        IMemberRepository memberRepository = new MemberRepository();


        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            MemberObject member = memberRepository.Login(email, password);

            if (member == null)
            {
                MessageBox.Show("Login failed!", "Login");
            }
            else
            {
                if (member.MemberID== 77777777)
                {
                    frmMemberManagement frmMemberManagement = new frmMemberManagement
                    {
                        loginMember = member,
                    };
                    frmMemberManagement.Show();
                }
                else
                {
                    frmMemberDetails frmMemberDetails = new frmMemberDetails
                    {
                        Text = "Member Details",
                        memberInfo = member,
                        action = false,
                        memberRepository = memberRepository
                    };



                    frmMemberDetails.Show();
                }
            }

      
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
