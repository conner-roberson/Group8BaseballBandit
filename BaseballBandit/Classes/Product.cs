using BaseballBandit.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BaseballBandit.Classes
{
    public class Product
    {
        public static bool EditProduct(Inventory product, BaseballBanditContext context)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("server=(localdb)\\localDB;database=BaseballBandit;Integrated Security=True; ConnectRetryCount=0; Encrypt=True; TrustServerCertificate=True"))
                {
                    string updateQuery = @"UPDATE Inventory SET 
                                            ProductPrice=@ProductPrice,
                                            ProductType=@ProductType, 
                                            ProductColor=@ProductColor, 
                                            ProductEquipmentSize=@ProductEquipmentSize,
                                            ProductApparelSize=@ProductApparelSize, 
                                            ImagePath=@ImagePath, 
                                            Brand=@Brand, 
                                            Name=@Name, 
                                            SellerId=@SellerId
                                            WHERE ProductID=@ProductId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@ProductType", product.ProductType);
                        cmd.Parameters.AddWithValue("@ProductColor", product.ProductColor);
                        cmd.Parameters.AddWithValue("@ProductEquipmentSize", product.ProductEquipmentSize);
                        cmd.Parameters.AddWithValue("@ProductApparelSize", product.ProductApparelSize);
                        cmd.Parameters.AddWithValue("@ImagePath", product.ImagePath);
                        cmd.Parameters.AddWithValue("@Brand", product.Brand);
                        cmd.Parameters.AddWithValue("@Name", product.Name);
                        cmd.Parameters.AddWithValue("@SellerId", product.SellerId);
                        cmd.Parameters.AddWithValue("@ProductID", product.ProductId);

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
