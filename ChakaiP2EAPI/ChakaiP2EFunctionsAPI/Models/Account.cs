using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChakaiP2EFunctionsAPI.Utilities;

namespace ChakaiP2EFunctionsAPI.Models
{
    internal class Account : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [NoSelect]
        public string PasswordHash { get; set; }
        [NoSelect]
        public string PasswordSalt { get; set; }

        private string _password;
        [NoInsert, NoUpdate, NoSelect]
        public string Password { get { return _password; } set { _password = value; CreatePasswordHashAndSalt(); } }
        [NoInsert, NoUpdate, NoSelect]
        public string RegistrationKey { get; set; }

        private void CreatePasswordHashAndSalt()
        {
            //Create the salt
            PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt(12);

            //Hash the password with the salt
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, PasswordSalt);
        }
    }
}
