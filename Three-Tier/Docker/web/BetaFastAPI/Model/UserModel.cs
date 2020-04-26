namespace BetaFastAPI.Model
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public UserModel(int userid, string lastname, string firstname, string username, Role role, bool isactive)
        {
            UserID = userid;
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
