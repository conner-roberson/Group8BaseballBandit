namespace BaseballBandit.Models
{
    public class OrderedProductsDetails
    {
        public int FkOrderNum { get; set; }

        public int ProductNum { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }

        public double ProductPrice { get; set; }

        public string ProductType { get; set; } = null!;

        public string ProductColor { get; set; } = null!;

        public int? ProductEquipmentSize { get; set; }

        public string? ProductApparelSize { get; set; }

        public string? ImagePath { get; set; }

        public string? Name { get; set; }

        public string? Brand { get; set; }

        public int SellerId { get; set; }
    }
}
