namespace BetaFast.Model
{
    public class PaymentDetails
    {
        public PaymentCard Card { get; set; }
        public int ZipCode { get; set; }

        public PaymentDetails(string nameOnCard, string issuer, long cardNumber, int cvc, string expiryDate, int zipCode)
        {
            Card = new PaymentCard(nameOnCard, issuer, cardNumber, cvc, expiryDate);
            ZipCode = zipCode;
        }

        public PaymentDetails(PaymentCard card, int zipCode)
        {
            Card = card;
            ZipCode = zipCode;
        }
    }
}
