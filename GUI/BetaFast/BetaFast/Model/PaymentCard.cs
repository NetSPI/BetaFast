namespace BetaFast.Model
{
    public class PaymentCard
    {
        public string NameOnCard { get; set; }
        public string Issuer { get; set; }
        public long CardNumber { get; set; }
        public int CVC { get; set; }
        public string ExpiryDate { get; set; }

        public PaymentCard(string nameOnCard, string issuer, long cardNumber, int cvc, string expiryDate)
        {
            NameOnCard = nameOnCard;
            Issuer = issuer;
            CardNumber = cardNumber;
            CVC = cvc;
            ExpiryDate = expiryDate;
        }
    }
}
