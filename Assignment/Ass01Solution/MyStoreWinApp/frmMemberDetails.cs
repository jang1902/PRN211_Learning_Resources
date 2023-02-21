using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStoreWinApp
{
    public partial class frmMemberDetails : Form
    {


        public frmMemberDetails()
        {
            InitializeComponent();
        }

        public IMemberRepository memberRepository { get; set; }
        public bool action { get; set; } // true = add, false = update
        public MemberObject memberInfo { get; set; }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var CheckEmail = new EmailAddressAttribute();
                if (!CheckEmail.IsValid(txtEmail.Text))
                {
                    throw new Exception("Wrong Email!");
                }
    

                MemberObject member = new MemberObject
                {
                    MemberID = memberInfo.MemberID,
                    MemberName = txtMemberName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    City = txtCity.Text,
                    Country = txtCountry.Text
                };
                memberRepository.UpdateMember(member);
                MessageBox.Show("Update successfully!!", "Update member", MessageBoxButtons.OK);
                txtMemberName.Text = member.MemberName;
                txtEmail.Text = member.Email;
                txtPassword.Text = member.Password;
                txtCity.Text = member.City;
                txtCountry.Text = member.Country;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update member", MessageBoxButtons.OK);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var CheckEmail = new EmailAddressAttribute();
                if (!CheckEmail.IsValid(txtEmail.Text))
                {
                    throw new Exception("Wrong Email!");
                }
              

                MemberObject member = new MemberObject
                {
                    MemberID = int.Parse(txtMemberID.Text),
                    MemberName = txtMemberName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    City = txtCity.Text,
                    Country = txtCountry.Text
                };
                memberRepository.AddMember(member);
                MessageBox.Show("Add successfully!!", "Add new member", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new member", MessageBoxButtons.OK);
            }
        }

        private void frmMemberDetails_Load(object sender, EventArgs e)
        {
            if (action == true)
            {
                btnAdd.Visible = true;
                btnUpdate.Enabled = false;

            }
            else
            {
                btnAdd.Enabled = false;
                btnUpdate.Visible = true;
                txtMemberID.Enabled = false;

                txtMemberID.Text = memberInfo.MemberID.ToString();
                txtMemberName.Text = memberInfo.MemberName;
                txtEmail.Text = memberInfo.Email;
                txtPassword.Text = memberInfo.Password;
                txtCity.Text = memberInfo.City;
                txtCountry.Text = memberInfo.Country;
            }
        }
    }
}
