namespace BaseballBandit.Models
{
    public class PaymentInformation
    {
        public int UserId { get; set; }

        public long CardNumber { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear {  get; set; }

        public int CardCVC { get; set; }

        public int PaymentID { get; set; }
    }
}
