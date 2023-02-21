using System;
using System.ComponentModel.DataAnnotations;


namespace BusinessObject
{
    public class MemberObject
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; }
        private string email;

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                var CheckEmail = new EmailAddressAttribute();

                if (CheckEmail.IsValid(Email) == true)
                {
                    email = value;
                }
                else
                {
                    throw new Exception("Wrong email format");
                }

            }
        }

        public string Password { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }
}