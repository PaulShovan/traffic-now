using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.Helpers
{
    public class PasswordHasher
    {
        public string GetHashedPassword(string password)
        {
            try
            {
                var data = Encoding.ASCII.GetBytes(password);
                var sha1 = new SHA1CryptoServiceProvider();
                var sha1data = sha1.ComputeHash(data);
                var hashedPassword = new ASCIIEncoding().GetString(sha1data);
                return hashedPassword;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
