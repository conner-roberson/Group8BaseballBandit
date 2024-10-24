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

            return;
        }
        public static bool FinalizeOrder(double SubTotal, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        Insert into OrderLog(UserID, ShippingAddress, Total, OrderDate, Refunded)
	                    Values(@UserId, @ShippingAddress, @Total, @OrderDate, @Refunded)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", User.UserID);
                        cmd.Parameters.AddWithValue("@ShippingAddress", User.Address + ", " + User.AddressCity + ", " + User.AddressState + ", " + User.AddressZip);
                        cmd.Parameters.AddWithValue("@Total", SubTotal);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Refunded", false);

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
                Total.Add(SubTotal);
                OrderDate.Add(DateTime.Now);
                Refunded.Add(false);

                if (OrderedProducts(context))
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
        public static bool OrderedProducts(BaseballBanditContext context)
        {
            try
            {
                for (int i = 0; i < CartClass.productIds.Count; i++)
                {
                    using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                    {
                        string updateQuery = @"
                        Insert into OrderedProducts(FkOrderNum, ProductNum, Quantity)
	                    Values(@OrderNum, @ProductNum, @Quantity)";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@OrderNum", OrderNum.Last());
                            cmd.Parameters.AddWithValue("@ProductNum", CartClass.productIds[i]);
                            cmd.Parameters.AddWithValue("@Quantity", CartClass.Quantity[i]);

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
    }
}
