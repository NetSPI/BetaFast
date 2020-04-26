using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BetaFastAPI.Model
{
    public class MovieUpload
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public IFormFile Poster { get; set; }
    }
}
