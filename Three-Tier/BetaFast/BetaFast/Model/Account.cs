using BetaFast.Model.Interfaces;

namespace BetaFast.Model
{
    public class Account : IModel
    {
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public Account(string lastname, string firstname, string username, Role role, bool isactive)
        {
            Lastname = lastname;
            Firstname = firstname;
            Username = username;
            Role = role;
            IsActive = isactive;
        }
    }

    public enum Role
    {
        Admin,
        User
    };
}
