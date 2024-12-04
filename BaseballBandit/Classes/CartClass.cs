using BaseballBandit.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BaseballBandit.Classes
{
    public static class CartClass
    {
        public static int FkUserId { get; set; }
        public static double SubTotal { get; set; }
        public static int NumItems { get; set; } = 0;

        public static List<int> SellerID { get; set; } = new List<int>();
        public static List<double> productPrice { get; set; } = new List<double>();
        public static List<int> productIds { get; set; } = new List<int>();
        public static List<int> Quantity { get; set; } = new List<int>();

        public static void InitializeCart(int UserId, BaseballBanditContext context)
        {
            string sql = $"Exec InitializeCart {UserId}";
            var check = context.Carts.FromSqlRaw(sql).ToList();

            string moreSql = $"Select * from Inventory";
            var inventory = context.Inventories.FromSqlRaw(moreSql).ToList();
            FkUserId = User.UserID;

            if (check.Count > 0)
            { 
                int j = 0;
                for (int i = 0; i < inventory.Count && j < check.Count; i++)
                {
                    if (inventory[i].ProductId == check[j].ProductId)
                    {
                        productIds.Add(check[j].ProductId);
                        Quantity.Add(check[j].Quantity);
                        SellerID.Add(inventory[i].SellerId);
                        productPrice.Add(inventory[i].ProductPrice);
                        NumItems += Quantity[j];
                        SubTotal += inventory[i].ProductPrice;
                        j++;
                        i = -1;
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }
        public static bool AddToCart(int ProductId, BaseballBanditContext context)
        {
            string sql = $"Select * From Inventory Where ProductID = {ProductId}";
            var Product = context.Inventories.FromSqlRaw(sql).ToList();

            if (Product.Count == 0)
            {
                return false;
            }

            double productPriceValue = Product[0].ProductPrice;

            bool alreadyInCart = false;
            bool success = false;

            for (int i = 0; i < productIds.Count; i++)
            {
                if (productIds[i] == ProductId)
                {
                    Quantity[i] += 1;
                    alreadyInCart = true;
                    success = ChangeQuantity(context, ProductId, Quantity[i]);
                }
            }
            if (!alreadyInCart)
            {
                productIds.Add(ProductId);
                Quantity.Add(1);
                SellerID.Add(Product[0].SellerId);
                productPrice.Add(Product[0].ProductPrice);
                success = AddToDbCart(context);
            }

            SubTotal += productPriceValue;
            NumItems += 1;

            return success;
        }
        public static bool AddToDbCart(BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        Insert into Cart(FkUserID, ProductId, Quantity)
	                    Values(@FkUserId, @ProductId, @Quantity)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@FkUserId", FkUserId);
                        cmd.Parameters.AddWithValue("@ProductId", productIds.Last());
                        cmd.Parameters.AddWithValue("@Quantity", Quantity.Last());

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
        public static bool RemoveFromCart(int ProductId, BaseballBanditContext context)
        {
            string sql = $"Select * From Inventory Where ProductID = {ProductId}";
            var ProductPrice = context.Inventories.FromSqlRaw(sql).ToList();

            if (ProductPrice.Count == 0)
            {
                return false;
            }

            double productPriceValue = ProductPrice[0].ProductPrice;
            bool success = false;

            for (int i = 0; i < productIds.Count; i++)
            {
                if (productIds[i] == ProductId && Quantity[i] > 1)
                {
                    Quantity[i] -= 1;
                    success = ChangeQuantity(context, productIds[i], Quantity[i]);
                    break;
                }
                else if (productIds[i] == ProductId && Quantity[i] == 1)
                {
                    success = RemoveFromDbCart(context, productIds[i]);
                    productIds.RemoveAt(i);
                    Quantity.RemoveAt(i);
                    SellerID.RemoveAt(i);
                    break;
                }
            }

            SubTotal -= productPriceValue;
            NumItems -= 1;

            return success;
        }
        public static bool RemoveAllFromCart(int ProductId, BaseballBanditContext context)
        {
            string sql = $"Select * From Inventory Where ProductID = {ProductId}";
            var ProductPrice = context.Inventories.FromSqlRaw(sql).ToList();

            if (ProductPrice.Count == 0)
            {
                return false;
            }

            double productPriceValue = ProductPrice[0].ProductPrice;
            bool success = false;
            int totalRemove = 0;
            double totalPrice = 0;
            for (int i = 0; i < productIds.Count; i++)
            {
                if (productIds[i] == ProductId)
                {
                    success = RemoveFromDbCart(context, productIds[i]);
                    productIds.RemoveAt(i);
                    totalRemove = Quantity[i];
                    totalPrice = Quantity[i] * productPriceValue;
                    Quantity.RemoveAt(i);
                    break;
                }
            }

            SubTotal -= totalPrice;
            NumItems -= totalRemove;

            return success;
        }
        public static bool RemoveFromDbCart(BaseballBanditContext context, int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        DELETE FROM Cart WHERE FkUserId=@FkUserId AND ProductId=@ProductId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@FkUserId", FkUserId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);

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
        public static bool ChangeQuantity(BaseballBanditContext context, int productId, int quantity)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"UPDATE Cart
                                            SET Quantity = @Quantity
                                            WHERE FkUserId=@FkUserId AND ProductId=@ProductId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@FkUserId", FkUserId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);

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
        public static bool ClearCart(BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        DELETE FROM Cart WHERE FkUserId=@FkUserId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@FkUserId", FkUserId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                SubTotal = 0;
                NumItems = 0;
                productIds.Clear();
                Quantity.Clear();
                productPrice.Clear();
                SellerID.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public static bool LogoutCart()
        {
            SubTotal = 0;
            NumItems = 0;
            productIds.Clear();
            Quantity.Clear();
            productPrice.Clear();
            SellerID.Clear();
            return true;
        }
    }
}
