using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity.Validation;
using System.IO;

namespace Amirhome.Models
{
    public class DataEncryptor
    {
        TripleDESCryptoServiceProvider symm;

        #region Factory
        public DataEncryptor()
        {
            this.symm = new TripleDESCryptoServiceProvider();
            this.symm.Padding = PaddingMode.PKCS7;
        }
        public DataEncryptor(TripleDESCryptoServiceProvider keys)
        {
            this.symm = keys;
        }

        public DataEncryptor(byte[] key, byte[] iv)
        {
            this.symm = new TripleDESCryptoServiceProvider();
            this.symm.Padding = PaddingMode.PKCS7;
            this.symm.Key = key;
            this.symm.IV = iv;
        }

        #endregion

        #region Properties
        public TripleDESCryptoServiceProvider Algorithm
        {
            get { return symm; }
            set { symm = value; }
        }
        public byte[] Key
        {
            get { return symm.Key; }
            set { symm.Key = value; }
        }
        public byte[] IV
        {
            get { return symm.IV; }
            set { symm.IV = value; }
        }

        #endregion

        #region Crypto

        public byte[] Encrypt(byte[] data) { return Encrypt(data, data.Length); }
        public byte[] Encrypt(byte[] data, int length)
        {
            try
            {
                // Create a MemoryStream.
                var ms = new MemoryStream();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                var cs = new CryptoStream(ms,
                    symm.CreateEncryptor(symm.Key, symm.IV),
                    CryptoStreamMode.Write);

                // Write the byte array to the crypto stream and flush it.
                cs.Write(data, 0, length);
                cs.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = ms.ToArray();

                // Close the streams.
                cs.Close();
                ms.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine("A cryptographic error occured: {0}", ex.Message);
            }
            return null;
        }

        public string EncryptString(string text)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(text)));
        }

        public byte[] Decrypt(byte[] data) { return Decrypt(data, data.Length); }
        public byte[] Decrypt(byte[] data, int length)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream ms = new MemoryStream(data);

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cs = new CryptoStream(ms,
                    symm.CreateDecryptor(symm.Key, symm.IV),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] result = new byte[length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                cs.Read(result, 0, result.Length);
                return result;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine("A cryptographic error occured: {0}", ex.Message);
            }
            return null;
        }

        public string DecryptString(string data)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data))).TrimEnd('\0');
        }

        #endregion

    }
    public class UserManager
    {
        public UserAccouunt getUserByID(int id)
        {
            UserAccouunt user = null;
            using (var context = new AmirhomeEntities())
            {
                user = (from U in context.UserAccouunts.Include("UserAccouuntsRole")
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
            List<UserAccouunt> res;
            string passkey = encodePassword(password);
            using (var context = new AmirhomeEntities())
            {
                res = (from U in context.UserAccouunts
                       where U.Email == email && U.Passkey == passkey
                        select U).ToList();
            }
            if (res.Count > 0)
            {
                if (!res[0].Approved.Value)
                    return -1;
                return res.First().ID;
            }
            return -1;
        }

        public bool deleteUser(int id)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    UserAccouunt acc = new UserAccouunt { ID = id };
                    context.UserAccouunts.Attach(acc);
                    context.UserAccouunts.Remove(acc);
                    context.Entry(acc).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool updateUser(UserAccouunt user)
        {
            try
            {
                user.Passkey = encodePassword(user.Passkey);
                using (var context = new AmirhomeEntities())
                {
                    context.UserAccouunts.Attach(user);
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool refreshLastOnline(int id)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    UserAccouunt res = (from U in context.UserAccouunts
                                        where U.ID == id
                                        select U).First();
                    res.LastTimeOnline = DateTime.Now;
                    context.UserAccouunts.Attach(res);
                    context.Entry(res).State = System.Data.Entity.EntityState.Modified;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
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
                    context.Entry(user).State = System.Data.Entity.EntityState.Added;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }
        public int getNumOfUsersRegisteredAfter(DateTime date)
        {
            int count = 0;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    count = (from U in context.UserAccouunts
                             where U.CreateDate.Value >= date
                             select U).Count();
                }
                return count;
            }
            catch
            {
                return 0;
            }
        }
        public List<UserAccouunt> getAllUser()
        {
            List<UserAccouunt> users = null;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    users = (from U in context.UserAccouunts
                             select U).ToList();
                }
                return users;
            }
            catch
            {
                return null;
            }
        }
    }
}