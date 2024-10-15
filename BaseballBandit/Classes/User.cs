using Azure.Identity;
using BaseballBandit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using System.Data;

namespace BaseballBandit.Classes
{
    public class User
    {
        private readonly BaseballBanditContext _context;

        public User(BaseballBanditContext context)
        {
            _context = context;
        }

        public string? UserName;

        public int? UserID;

        public string? Password;

        public string? Email;

        public string? FirstName;

        public string? LastName;

        public bool? Admin;

        public bool? Seller;

        public string? Address;

        public string? AddressCity;

        public string? AddressState;

        public int AddressZip;

        public bool Login(string UserID, string Password)
        {
            string sql = $"Exec LoginUser {UserID}";
            var check = _context.Users.FromSqlRaw(sql).ToList();
            if(check.Count == 0)
            {
                return false;
            }
            else
            {
                string realPass = check[0].HashedPass;

                bool CheckPass = BCrypt.Net.BCrypt.EnhancedVerify(Password, realPass);

                if (CheckPass)
                {
                    User currentUser = new User(_context)
                    {
                        UserID = check[0].UserId,
                        UserName = check[0].UserName,
                        Password = check[0].HashedPass,
                        Email = check[0].Email,
                        FirstName = check[0].FirstName,
                        LastName = check[0].LastName,
                        Admin = check[0].Admin,
                        Seller = check[0].Seller,
                        Address = check[0].Address,
                        AddressCity = check[0].AddressCity,
                        AddressState = check[0].AddressState,
                        AddressZip = check[0].AddressZip,
                    };
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string HashPassword(string Password) 
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(Password, 13);
            return passwordHash;
        }

        public bool Register(User user)
        {
            try
            {
                string hashedPass = HashPassword(user.Password);
                SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True");
                SqlCommand cmd = new SqlCommand("RegisterUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@hashedPass", hashedPass);
                cmd.Parameters.AddWithValue("@Admin", user.Admin);
                cmd.Parameters.AddWithValue("@Seller", user.Seller);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Address", user.Address);
                cmd.Parameters.AddWithValue("@AddressCity", user.AddressCity);
                cmd.Parameters.AddWithValue("@AddressState", user.AddressState);
                cmd.Parameters.AddWithValue("@AddressZip", user.AddressZip);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
