using BaseballBandit.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BaseballBandit.Classes
{
    public static class CartClass
    {
        public static int FkUserId { get; set; }
        public static double? SubTotal { get; set; } = 0;
        public static int? NumTotalItems { get; set; } = 0;
        public static int? Item1Id { get; set; }
        public static int? NumItem1 { get; set; }
        public static int? Item2Id { get; set; }
        public static int? NumItem2 { get; set; }
        public static int? Item3Id { get; set; }
        public static int? NumItem3 { get; set; }
        public static int? Item4Id { get; set; }
        public static int? NumItem4 { get; set; }
        public static int? Item5Id { get; set; }
        public static int? NumItem5 { get; set; }
        public static int? Item6Id { get; set; }
        public static int? NumItem6 { get; set; }
        public static int? Item7Id { get; set; }
        public static int? NumItem7 { get; set; }
        public static int? Item8Id { get; set; }
        public static int? NumItem8 { get; set; }
        public static int? Item9Id { get; set; }
        public static int? NumItem9 { get; set; }
        public static int? Item10Id { get; set; }
        public static int? NumItem10 { get; set; }
        public static List<int?> productIds { get; set; } = new List<int?>();

        public static void InitializeCart(int UserId, BaseballBanditContext context)
        {
            string sql = $"Exec InitializeCart {UserId}";
            var check = context.Carts.FromSqlRaw(sql).ToList();

            if (check.Count > 0)
            {
                FkUserId = check[0].FkUserId;
                SubTotal = check[0].SubTotal ?? 0;
                NumTotalItems = check[0].NumTotalItems ?? 0;
                Item1Id = check[0].Item1Id ?? null;
                NumItem1 = check[0].NumItem1 ?? 0;
                Item2Id = check[0].Item2Id ?? null;
                NumItem2 = check[0].NumItem2 ?? 0;
                Item3Id = check[0].Item3Id ?? null;
                NumItem3 = check[0].NumItem3 ?? 0;
                Item4Id = check[0].Item4Id ?? null;
                NumItem4 = check[0].NumItem4 ?? 0;
                Item5Id = check[0].Item5Id ?? null;
                NumItem5 = check[0].NumItem5 ?? 0;
                Item6Id = check[0].Item6Id ?? null;
                NumItem6 = check[0].NumItem6 ?? 0;
                Item7Id = check[0].Item7Id ?? null;
                NumItem7 = check[0].NumItem7 ?? 0;
                Item8Id = check[0].Item8Id ?? null;
                NumItem8 = check[0].NumItem8 ?? 0;
                Item9Id = check[0].Item9Id ?? null;
                NumItem9 = check[0].NumItem9 ?? 0;
                Item10Id = check[0].Item10Id ?? null;
                NumItem10 = check[0].NumItem10 ?? 0;

                if (check[0].Item1Id != null)
                {
                    productIds.Add(check[0].Item1Id);
                }
                if (check[0].Item2Id != null)
                {
                    productIds.Add(check[0].Item2Id);
                }
                if (check[0].Item3Id != null)
                {
                    productIds.Add(check[0].Item3Id);
                }
                if (check[0].Item4Id != null)
                {
                    productIds.Add(check[0].Item4Id);
                }
                if (check[0].Item5Id != null)
                {
                    productIds.Add(check[0].Item5Id);
                }
                if (check[0].Item6Id != null)
                {
                    productIds.Add(check[0].Item6Id);
                }
                if (check[0].Item7Id != null)
                {
                    productIds.Add(check[0].Item7Id);
                }
                if (check[0].Item8Id != null)
                {
                    productIds.Add(check[0].Item8Id);
                }
                if (check[0].Item9Id != null)
                {
                    productIds.Add(check[0].Item9Id);
                }
                if (check[0].Item10Id != null)
                {
                    productIds.Add(check[0].Item10Id);
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
            var ProductPrice = context.Inventories.FromSqlRaw(sql).ToList();

            if (ProductPrice.Count == 0)
            {
                return false;
            }

            double productPriceValue = ProductPrice[0].ProductPrice;

            if (Item1Id == null)
            {
                Item1Id = ProductId;
                NumItem1 += 1;
            }
            else if (Item2Id == null)
            {
                Item2Id = ProductId;
                NumItem2 += 1;
            }
            else if (Item3Id == null)
            {
                Item3Id = ProductId;
                NumItem3 += 1;
            }
            else if (Item4Id == null)
            {
                Item4Id = ProductId;
                NumItem4 += 1;
            }
            else if (Item5Id == null)
            {
                Item5Id = ProductId;
                NumItem5 += 1;
            }
            else if (Item6Id == null)
            {
                Item6Id = ProductId;
                NumItem6 += 1;
            }
            else if (Item7Id == null)
            {
                Item7Id = ProductId;
                NumItem7 += 1;
            }
            else if (Item8Id == null)
            {
                Item8Id = ProductId;
                NumItem8 += 1;
            }
            else if (Item9Id == null)
            {
                Item9Id = ProductId;
                NumItem9 += 1;
            }
            else if (Item10Id == null)
            {
                Item10Id = ProductId;
                NumItem10 += 1;
            }
            else
            {
                return false;
            }



            SubTotal += productPriceValue;
            NumTotalItems += 1;
            productIds.Add(ProductId);

            return CartClass.UpdateCart(context);


        }

        public static bool UpdateCart(BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"
                        UPDATE Cart
                        SET 
                        SubTotal = @SubTotal, 
                        NumTotalItems = @NumTotalItems, 
                        Item1Id = @Item1Id, NumItem1 = @NumItem1, 
                        Item2Id = @Item2Id, NumItem2 = @NumItem2, 
                        Item3Id = @Item3Id, NumItem3 = @NumItem3, 
                        Item4Id = @Item4Id, NumItem4 = @NumItem4, 
                        Item5Id = @Item5Id, NumItem5 = @NumItem5, 
                        Item6Id = @Item6Id, NumItem6 = @NumItem6, 
                        Item7Id = @Item7Id, NumItem7 = @NumItem7, 
                        Item8Id = @Item8Id, NumItem8 = @NumItem8, 
                        Item9Id = @Item9Id, NumItem9 = @NumItem9, 
                        Item10Id = @Item10Id, NumItem10 = @NumItem10
                        WHERE 
                        FkUserId = @FkUserId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@FkUserId", FkUserId);
                        cmd.Parameters.AddWithValue("@SubTotal", SubTotal);
                        cmd.Parameters.AddWithValue("@NumTotalItems", NumTotalItems);
                        cmd.Parameters.AddWithValue("@Item1Id", Item1Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem1", NumItem1 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item2Id", Item2Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem2", NumItem2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item3Id", Item3Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem3", NumItem3 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item4Id", Item4Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem4", NumItem4 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item5Id", Item5Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem5", NumItem5 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item6Id", Item6Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem6", NumItem6 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item7Id", Item7Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem7", NumItem7 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item8Id", Item8Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem8", NumItem8 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item9Id", Item9Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem9", NumItem9 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Item10Id", Item10Id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumItem10", NumItem10 ?? (object)DBNull.Value);

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

            if (Item10Id == ProductId)
            {
                Item10Id = null;
                NumItem10 -= 1;
            }
            else if (Item9Id == ProductId)
            {
                Item9Id = null;
                NumItem9 -= 1;
            }
            else if (Item8Id == ProductId)
            {
                Item8Id = null;
                NumItem8 -= 1;
            }
            else if (Item7Id == ProductId)
            {
                Item7Id = null;
                NumItem7 -= 1;
            }
            else if (Item6Id == ProductId)
            {
                Item6Id = null;
                NumItem6 -= 1;
            }
            else if (Item5Id == ProductId)
            {
                Item5Id = null;
                NumItem5 -= 1;
            }
            else if (Item4Id == ProductId)
            {
                Item4Id = null;
                NumItem4 -= 1;
            }
            else if (Item3Id == ProductId)
            {
                Item3Id = null;
                NumItem3 -= 1;
            }
            else if (Item2Id == ProductId)
            {
                Item2Id = null;
                NumItem2 -= 1;
            }
            else if (Item1Id == ProductId)
            {
                Item1Id = null;
                NumItem1 -= 1;
            }
            else
            {
                return false;
            }


            SubTotal -= productPriceValue;
            NumTotalItems -= 1;
            productIds.Remove(ProductId);

            return CartClass.UpdateCart(context);
        }
    }
}
