using BetaFast.Model.Interfaces;

namespace BetaFast.Model
{
    public class Movie : IModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public Poster Poster { get; set; }

        public Movie(string title, string description, string director, int year, decimal price, Poster poster)
        {
            Title = title;
            Description = description;
            Director = director;
            Year = year;
            Price = price;
            Poster = poster;
        }
    }
}
