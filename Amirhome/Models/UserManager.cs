using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity.Validation;

namespace Amirhome.Models
{
    public class UserManager
    {
        public UserAccouunt getUserByID(int id)
        {
            UserAccouunt user = null;
            using (var context = new AmirhomeEntities())
            {
                user = (from U in context.UserAccouunts
                        where U.ID == id
                        select U).FirstOrDefault();
            }
            return user;
        }
        public UserAccouunt getUserByEmail(string email)
        {
            UserAccouunt user = null;
            using (var context = new AmirhomeEntities())
            {
                user = (from U in context.UserAccouunts
                        where U.Email == email
                        select U).FirstOrDefault();
            }
            return user;
        }
        public void changePasswordForUser(int id, string old_password, string new_password)
        {
            return;
        }
        private string encodePassword(string password)
        {
            var data = Encoding.UTF8.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1password = sha1.ComputeHash(data);
            return HexStringFromBytes(sha1password);
        }
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
        public int authenticateUser(string email, string password)
        {
            List<int> res;
            string passkey = encodePassword(password);
            using (var context = new AmirhomeEntities())
            {
                res = (from U in context.UserAccouunts
                       where U.Email == email && U.Passkey == passkey
                        select U.ID).ToList();
            }
            if (res.Count > 0)
                return res.First();
            return -1;
        }

        public bool createNewUser(UserAccouunt user)
        {
            if (getUserByEmail(user.Email) != null)
            {
                return false;
            }
            user.Passkey = encodePassword(user.Passkey);
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    context.UserAccouunts.Add(user);
                    context.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

    }
}