using BaseballBandit.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace BaseballBandit.Classes
{
    public static class Order
    {
        public static List<int> OrderNum {  get; set; } = new List<int>();
        public static List<int> UserId { get; set; } = new List<int>();
        public static List<string?> shippingAddress {  get; set; } = new List<string?>();
        public static List<double> Total { get; set; } = new List<double>();
        public static List<DateTime> OrderDate { get; set; } = new List<DateTime>();
        public static List<bool> Refunded { get; set; } = new List<bool>();

        public static List<int> PaymentID { get; set; } = new List<int>();

        public static void InitializeOrderLog (BaseballBanditContext context)
        {
            string sql = $"Select * From OrderLog WHERE UserID = {User.UserID}";
            var orders = context.OrderLogs.FromSqlRaw(sql).ToList();

            for(int i = 0; i < orders.Count; i++)
            {
                InitializeOrderLog(context, i);
            }

            return;
        }
        public static void InitializeOrderLog(BaseballBanditContext context, int num)
        {
            string sql = $"Select * From OrderLog WHERE UserID = {User.UserID}";
            var orders = context.OrderLogs.FromSqlRaw(sql).ToList();

            OrderNum.Add(orders[num].OrderNum);
            UserId.Add(orders[num].UserId);
            shippingAddress.Add(orders[num].ShippingAddress);
            Total.Add(orders[num].Total);
            OrderDate.Add(orders[num].OrderDate);
            Refunded.Add(orders[num].Refunded);
            PaymentID.Add(orders[num].PaymentID);

            return;
        }
        public static bool FinalizeOrder(int PaymentId, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        Insert into OrderLog(UserID, ShippingAddress, Total, OrderDate, Refunded, PaymentID)
	                    Values(@UserId, @ShippingAddress, @Total, @OrderDate, @Refunded, @PaymentID)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", User.UserID);
                        cmd.Parameters.AddWithValue("@ShippingAddress", User.Address + ", " + User.AddressCity + ", " + User.AddressState + ", " + User.AddressZip);
                        cmd.Parameters.AddWithValue("@Total", CartClass.SubTotal);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Refunded", false);
                        cmd.Parameters.AddWithValue("@PaymentID", PaymentId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                string sql = $"Select * from OrderLog WHERE UserID = {User.UserID}";
                var userOrders = context.OrderLogs.FromSqlRaw(sql).ToList();

                OrderNum.Add(userOrders[userOrders.Count() - 1].OrderNum);
                UserId.Add(User.UserID);
                shippingAddress.Add(User.Address + ", " + User.AddressCity + ", " + User.AddressState + ", " + User.AddressZip);
                Total.Add(CartClass.SubTotal);
                OrderDate.Add(DateTime.Now);
                Refunded.Add(false);
                PaymentID.Add(PaymentId);

                if (OrderedProducts(context, userOrders[userOrders.Count() - 1].OrderNum))
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }

        }
        public static bool OrderedProducts(BaseballBanditContext context, int orderNum)
        {
            try
            {
                for (int i = 0; i < CartClass.productIds.Count; i++)
                {
                    using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                    {
                        string updateQuery = @"
                        Insert into OrderedProducts(FkOrderNum, ProductNum, Quantity, SellerID, ProductPrice)
	                    Values(@OrderNum, @ProductNum, @Quantity, @SellerID, @ProductPrice)";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@OrderNum", orderNum);
                            cmd.Parameters.AddWithValue("@ProductNum", CartClass.productIds[i]);
                            cmd.Parameters.AddWithValue("@Quantity", CartClass.Quantity[i]);
                            cmd.Parameters.AddWithValue("@SellerID", CartClass.SellerID[i]);
                            cmd.Parameters.AddWithValue("@ProductPrice", CartClass.productPrice[i]);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                if(CartClass.ClearCart(context))
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public static bool RefundOrder(int OrderNum, BaseballBanditContext context)
        {
            if(UpdateOrderDB(OrderNum, context))
            {
                for(int i = 0; i < Order.OrderNum.Count; i++)
                {
                    if (Order.OrderNum[i] == OrderNum)
                    {
                        Refunded[i] = true;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool UpdateOrderDB(int OrderNum, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                                        UPDATE OrderLog
                                        SET Refunded=@Refunded
                                        WHERE OrderNum=@OrderNum";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderNum", OrderNum);
                        cmd.Parameters.AddWithValue("@Refunded", true);

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

        public static bool LogoutOrders()
        {
            OrderNum.Clear();
            UserId.Clear();
            shippingAddress.Clear();
            Total.Clear();
            OrderDate.Clear();
            Refunded.Clear();
            PaymentID.Clear();
            return true;
        }
    }
}
