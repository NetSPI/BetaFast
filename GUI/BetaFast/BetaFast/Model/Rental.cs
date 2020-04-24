using BetaFast.Model.Interfaces;

namespace BetaFast.Model
{
    public class Rental : IModel
    {
        public string Username { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Rental(string username, string title, int quantity, decimal price)
        {
            Username = username;
            Title = title;
            Quantity = quantity;
            Price = price;
        }
    }
}
