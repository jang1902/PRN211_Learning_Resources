using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStoreWinApp
{
    public partial class frmMemberManagement : Form
    {
        public frmMemberManagement()
        {
            InitializeComponent();
        }


        private void frmMemberManagement_Load(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            txtMemberID.Enabled = false;
            txtMemberName.Enabled = false;
            txtEmail.Enabled = false;
            txtPassword.Enabled = false;
            txtCity.Enabled = false;
            txtCountry.Enabled = false;
            btnAdd.Enabled = false;
            dgvMemberList.Enabled = false;
            btnLoad.Enabled = true;

        }

        private MemberObject GetMemberInfo()
        {
            MemberObject member = null;
            try
            {
                member = new MemberObject
                {
                    MemberID = int.Parse(txtMemberID.Text),
                    MemberName = txtMemberName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    City = txtCity.Text,
                    Country = txtCountry.Text
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Get Member Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return member;
        }
        public MemberObject loginMember { get; set; }
        IMemberRepository memberRepository = new MemberRepository();

        BindingSource source;
        BindingSource citySource;
        BindingSource countrySource;

        bool search = false;
        bool filter = false;
        IEnumerable<MemberObject> dataSource;
        IEnumerable<MemberObject> searchResult;
        IEnumerable<MemberObject> filterResult;

        IEnumerable<string> countryList;
        Dictionary<string, IEnumerable<string>> cityDictionary;

        private void LoadMemberList()
        {
            try
            {
                
                source = new BindingSource();
                source.DataSource = filter ? filterResult : dataSource;
                if (!filter)
                {
                    countryList = from member in dataSource
                                  where !string.IsNullOrEmpty(member.Country.Trim())
                                  orderby member.Country ascending
                                  select member.Country;
                    countryList = countryList.Distinct();
                    cityDictionary = new Dictionary<string, IEnumerable<string>>();
                    foreach (var country in countryList)
                    {
                        var cityList = from member in dataSource
                                       where !string.IsNullOrEmpty(member.City.Trim()) && (member.Country.Equals(country))
                                       orderby member.City ascending
                                       select member.City;
                        cityList = cityList.Prepend("All");
                        cityList = cityList.Distinct();

                        cityDictionary.Add(country, cityList);
                    }

                    countryList = countryList.Prepend("All");

                    if (dataSource.Count() > 0)
                    {
                        countrySource = new BindingSource();
                        countrySource.DataSource = countryList;
                        cboCountry.DataSource = null;
                        cboCountry.DataSource = countrySource;

                        cboCity.DataBindings.Clear();
                        
                    }
                }
                

                txtMemberID.DataBindings.Clear();
                txtMemberName.DataBindings.Clear();
                txtEmail.DataBindings.Clear();
                txtPassword.DataBindings.Clear();
                txtCity.DataBindings.Clear();
                txtCountry.DataBindings.Clear();

                txtMemberID.DataBindings.Add("Text", source, "MemberID");
                txtMemberName.DataBindings.Add("Text", source, "MemberName");
                txtEmail.DataBindings.Add("Text", source, "Email");
                txtPassword.DataBindings.Add("Text", source, "Password");
                txtCity.DataBindings.Add("Text", source, "City");
                txtCountry.DataBindings.Add("Text", source, "Country");

                dgvMemberList.DataSource = null;
                dgvMemberList.DataSource = source;

                if (dataSource.Count() > 0)
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Member List");
            }
        }

        private void LoadFullList()
        {
            search = false;
            filter = false;
            var members = memberRepository.GetMembersList();
            var memberList = from member in members
                             orderby member.MemberName descending
                             select member;
            dataSource = memberList;
            searchResult = memberList;
            filterResult = memberList;
         
            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            dgvMemberList.Enabled = true;
            btnLoad.Enabled = true;
            
            LoadFullList();
            LoadMemberList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmMemberDetails frmMemberDetails = new frmMemberDetails
            {
                memberRepository = this.memberRepository,
                action = true,
                Text = "Add new member"
            };

            if (frmMemberDetails.ShowDialog() == DialogResult.OK)
            {
                LoadFullList();
                LoadMemberList();
                
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MemberObject member = GetMemberInfo();
         
                if (MessageBox.Show("Do you really want to delete?", "Delete member", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    memberRepository.DeleteMember(member.MemberID);
                    search = false;
                    LoadFullList();
                    LoadMemberList();
                }
            
        }

        private void dgvMemberList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MemberObject member = GetMemberInfo();

            frmMemberDetails frmMemberDetails = new frmMemberDetails
            {
                memberRepository = this.memberRepository,
                action = false,
                memberInfo = member,
                Text = "Update member infomation"
            };

            if (frmMemberDetails.ShowDialog() == DialogResult.OK)
            {
                LoadFullList();

                LoadMemberList();
                
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtSearchValue.Text;
                if (radioID.Checked)
                {
                    int searchID = int.Parse(searchValue);
                    IEnumerable<MemberObject> searchResult = memberRepository.SearchMember(searchID);
                    if (searchResult.Any())
                    {
                        dataSource = searchResult;
                        this.searchResult = searchResult;
                        this.filterResult = searchResult;
                        filter = false;
                        search = true;
                        LoadMemberList();
                    }
                    else
                    {
                        MessageBox.Show("No result found!", "Search member", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else if (radioName.Checked)
                {
                    string searchName = searchValue;
                    IEnumerable<MemberObject> searchResult = memberRepository.SearchMember(searchName);
                    if (searchResult.Any())
                    {
                        dataSource = searchResult;
                        this.searchResult = searchResult;
                        this.filterResult = searchResult;
                        filter = false;
                        search = true;
                        LoadMemberList();
                    }
                    else
                    {
                        MessageBox.Show("No result found!", "Search member", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search member", MessageBoxButtons.OK);
            }
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCountry.DataSource != null)
                {
                    string country = cboCountry.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(country))
                    {
                        IEnumerable<MemberObject> searchResult;
                        if (search)
                        {
                            searchResult = memberRepository.SearchMemberByCountry(country, this.searchResult);
                        }
                        else
                        {
                            searchResult = memberRepository.SearchMemberByCountry(country, this.dataSource);
                        }

                        if (searchResult.Any())
                        {
                            cboCity.DataBindings.Clear();

                            IEnumerable<string> cityList = new List<string>();
                            if (country.Equals("All"))
                            {
                                var keys = cityDictionary.Keys;
                                IEnumerable<string> _cityList;
                                foreach (var key in keys)
                                {
                                    cityDictionary.TryGetValue(key, out _cityList);
                                    if (_cityList.Any())
                                    {
                                        foreach (var _city in _cityList)
                                        {
                                            cityList = cityList.Concat(new List<string>() { _city });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cityDictionary.TryGetValue(country, out cityList);
                            }

                            cityList = cityList.Distinct();

                            citySource = new BindingSource();
                            citySource.DataSource = cityList;
                            cboCity.DataSource = null;
                            cboCity.DataSource = citySource;

                            filterResult = searchResult;
                            filter = true;
                            LoadMemberList();
                        }
                        else
                        {
                            MessageBox.Show("No result found!", "Search member", MessageBoxButtons.OK);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search member", MessageBoxButtons.OK);
            }
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCity.DataSource != null)
                {
                    string city = cboCity.SelectedItem.ToString();
                    
                    string country = cboCountry.Text;

                    if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(country))
                    {
                        IEnumerable<MemberObject> searchResult;
                        if (search)
                        {
                            searchResult = memberRepository.SearchMemberByCity(country, city, this.searchResult);
                        }
                        else
                        {
                            searchResult = memberRepository.SearchMemberByCity(country, city, this.dataSource);
                        }

                        if (searchResult.Any())
                        {
                            filter = true;
                            filterResult = searchResult;
                            LoadMemberList();
                        }
                        else
                        {
                            MessageBox.Show("No result found!", "Search member", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search member", MessageBoxButtons.OK);
            }
        }
    }
}
