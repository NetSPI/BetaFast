using System.Security;
using BetaFast.Model.Interfaces;

namespace BetaFast.Model
{
    class Registration : IModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; }
    }
}
