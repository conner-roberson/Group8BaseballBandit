using Azure.Identity;
using BaseballBandit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using System.Data;
using BaseballBandit.Classes;

namespace BaseballBandit.Classes
{
    public static class User
    {
        public static string? UserName;

        public static int UserID;

        public static string? Email;

        public static string? FirstName;

        public static string? LastName;

        public static bool? Admin;

        public static bool? Seller;

        public static string? Address;

        public static string? AddressCity;

        public static string? AddressState;

        public static int AddressZip;

        public static List<long> CardNumber = new List<long>();

        public static List<int> ExpirationMonth = new List<int>();

        public static List<int> ExpirationYear = new List<int>();

        public static List<int> CardCVC = new List<int>();

        public static List<int> PaymentID = new List<int>();

        public static bool Login(string UserID, string Password, BaseballBanditContext context)
        {
            string sql = $"Exec LoginUser {UserID}";
            var check = context.Users.FromSqlRaw(sql).ToList();

            string realPass = check[0].HashedPass;

            bool CheckPass = BCrypt.Net.BCrypt.EnhancedVerify(Password, realPass);

            if (check.Count == 0 || CheckPass == false)
            {
                return false;
            }
            else
            {
                User.UserID = check[0].UserId;
                UserName = check[0].UserName;
                Email = check[0].Email;
                FirstName = check[0].FirstName;
                LastName = check[0].LastName;
                Admin = check[0].Admin;
                Seller = check[0].Seller;
                Address = check[0].Address;
                AddressCity = check[0].AddressCity;
                AddressState = check[0].AddressState;
                AddressZip = check[0].AddressZip;

                
                Order.InitializeOrderLog(context);
                if(check[0].Admin == false && check[0].Seller == false)
                {
                    CartClass.InitializeCart(check[0].UserId, context);
                    GetPaymentInfo(context);
                }

                return true;
                
            }
        }
        public static string HashPassword(string Password) 
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(Password, 13);
            return passwordHash;
        }
        public static bool Register(Models.User user, BaseballBanditContext context)
        {
            try
            {
                string hashedPass = HashPassword(user.HashedPass);
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
        public static bool DeleteUser(int userId, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        DELETE FROM UserData WHERE UserID=@userId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public static bool DeleteSeller(int userId, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        DELETE FROM Inventory WHERE SellerId=@userId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public static bool DeleteProduct(int productId, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        DELETE FROM Inventory WHERE ProductID=@productId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@productId", productId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public static void GetPaymentInfo(BaseballBanditContext context)
        {
            string sql = $"Select * from PaymentInformation where UserID = {User.UserID}";
            var payments = context.PaymentInfo.FromSqlRaw(sql).ToList();

            for(int i = 0; i < payments.Count(); i++)
            {
                CardNumber.Add(payments[i].CardNumber);
                ExpirationMonth.Add(payments[i].ExpirationMonth);
                ExpirationYear.Add(payments[i].ExpirationYear);
                CardCVC.Add(payments[i].CardCVC);
                PaymentID.Add(payments[i].PaymentID);
            }
        }
        public static bool LogoutUser()
        {
            UserName = null;
            UserID = 0;
            Email = null;
            FirstName = null;
            LastName = null;
            Admin = false;
            Seller = false;
            Address = null;
            AddressCity = null;
            AddressState = null;
            AddressZip = 0;
            CardNumber.Clear();
            ExpirationMonth.Clear();
            ExpirationYear.Clear();
            CardCVC.Clear();
            PaymentID.Clear();
            return true;
        }

        public static bool AddPayment(long CardNum, int ExpirationMon, int ExpirationYr, int CVC)
        {
            CardNumber.Add(CardNum);
            ExpirationMonth.Add(ExpirationMon);
            ExpirationYear.Add(ExpirationYr);
            CardCVC.Add(CVC);
            PaymentID.Add(0);
            return true;
        }
    }
}
