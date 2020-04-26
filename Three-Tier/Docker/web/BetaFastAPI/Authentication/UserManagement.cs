using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BetaFastAPI.Utilities;
using System.Data;
using BetaFastAPI.Model;
using BetaFastAPI.Security;
using BetaFastAPI.Exceptions;

namespace BetaFastAPI.Authentication
{
    class UserManagement
    {
        private readonly string _usersTableName = "users";
        private string _connectionString;

        public UserManagement(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool UserExists(string username)
        {
            string commandText = "SELECT 1 FROM " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool UserExists(int userID)
        {
            string commandText = "SELECT 1 FROM " + _usersTableName + " WHERE UserID = " + userID + ";";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public int TotalUsers()
        {
            string commandText = "SELECT COUNT(*) FROM " + _usersTableName + ";";
            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    return Int32.Parse(contents.Rows[0][0].ToString());
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public int TotalActiveUsers()
        {
            string commandText = "SELECT count(*) FROM " + _usersTableName + " WHERE Active=1" + ";";
            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    return Int32.Parse(contents.Rows[0][0].ToString());
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public int TotalInactiveUsers()
        {
            string commandText = "SELECT count(*) FROM " + _usersTableName + " WHERE Active=0" + ";";
            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    return Int32.Parse(contents.Rows[0][0].ToString());
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void AddUser(string lastName, string firstName, string username, string password, Role role)
        {
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentNullException("LastName");
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentNullException("FirstName");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password");

            if (UserExists(username))
            {
                throw new UserExistsException("Username " + username + " already exists");
            }

            string salt = Hashing.CreateSalt(16);
            string hashedPassword = Hashing.GenerateSaltedHash(password, salt);

            int newUserID = TotalUsers() + 1;

            string commandText = "INSERT INTO " + _usersTableName + " (UserID, LastName, FirstName, Username, Password, Salt, Role, Active) VALUES ('" + newUserID + "','" + lastName + "','" + firstName + "','" + username + "','" + hashedPassword + "','" + salt + "','" + (int) role + "'," + "1" + ")";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            if (!UserExists(username))
            {
                throw new UserDoesNotExistException("User " + username + " does not exist");
            }

            string commandText = "DELETE FROM " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void DeactivateUser(int userID)
        {
            if (!UserExists(userID))
            {
                throw new UserDoesNotExistException("User " + userID + " does not exist");
            }

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 0 WHERE UserID = " + userID + ";";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void DeactivateUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            if (!UserExists(username))
            {
                throw new UserDoesNotExistException("User " + username + " does not exist");
            }

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 0 WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void ReactivateUser(int userID)
        {
            if (!UserExists(userID))
            {
                throw new UserDoesNotExistException("User " + userID + " does not exist");
            }

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 1 WHERE UserID = " + userID + ";";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void ReactivateUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            if (!UserExists(username))
            {
                throw new UserDoesNotExistException("User " + username + " does not exist");
            }

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 1 WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool IsActive(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            string commandText = "SELECT * from " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        if (contents.Rows[0]["Active"].ToString().Equals("True") || contents.Rows[0]["Active"].ToString().Equals("1"))
                        {
                            return true;
                        }
                        else if (contents.Rows[0]["Active"].ToString().Equals("False") || contents.Rows[0]["Active"].ToString().Equals("0"))
                        {
                            return false;
                        }
                        else
                        {
                            throw new Exception("Status could not be parsed.");
                        }
                    }
                    else
                    {
                        throw new UserDoesNotExistException("User " + username + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool IsAdmin(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            string commandText = "SELECT * FROM " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        if (Enum.TryParse(contents.Rows[0]["Role"].ToString(), out Role currentRole))
                        {
                            return currentRole == Role.Admin;
                        }
                        else
                        {
                            throw new Exception("Role could not be parsed.");
                        }
                    }
                    else
                    {
                        throw new UserDoesNotExistException("User " + username + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool IsAdmin(int userID)
        {
            string commandText = "SELECT * from " + _usersTableName + " where UserID = " + userID;

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        if (Enum.TryParse(contents.Rows[0]["Role"].ToString(), out Role currentRole))
                        {
                            return currentRole == Role.Admin;
                        }
                        else
                        {
                            throw new Exception("Role could not be parsed.");
                        }
                    }
                    else
                    {
                        throw new UserDoesNotExistException("User " + userID + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool CorrectPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password");

            string commandText = "SELECT * FROM " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        string hashedPassword = contents.Rows[0]["Password"].ToString();
                        string salt = contents.Rows[0]["Salt"].ToString();

                        if (hashedPassword.Equals(Hashing.GenerateSaltedHash(password, salt)))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new UserDoesNotExistException("User " + username + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public bool CorrectPassword(int userID, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password");
            }

            string commandText = "SELECT * FROM " + _usersTableName + " WHERE UserID = " + userID;

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        string hashedPassword = contents.Rows[0]["Password"].ToString();
                        string salt = contents.Rows[0]["Salt"].ToString();

                        if (hashedPassword.Equals(Hashing.GenerateSaltedHash(password, salt)))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new UserDoesNotExistException("User " + userID + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public UserModel GetUser(int userID)
        {
            string commandText = "SELECT * FROM " + _usersTableName + " WHERE UserID = " + userID;

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    UserModel output = null;
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        output = new UserModel(
                        Int32.Parse(contents.Rows[0]["UserID"].ToString()),
                        contents.Rows[0]["Lastname"].ToString(),
                        contents.Rows[0]["Firstname"].ToString(),
                        contents.Rows[0]["Username"].ToString(),
                        (Role)contents.Rows[0]["Role"],
                        ("True" == contents.Rows[0]["Active"].ToString())
                        );
                        return output;
                    }
                    else
                    {
                        return output;
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public UserModel GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username");
            }

            string commandText = "SELECT * FROM " + _usersTableName + " WHERE Username = '" + username + "';";

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    UserModel output = null;
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        output = new UserModel(
                        Int32.Parse(contents.Rows[0]["UserID"].ToString()),
                        contents.Rows[0]["Lastname"].ToString(),
                        contents.Rows[0]["Firstname"].ToString(),
                        contents.Rows[0]["Username"].ToString(),
                        (Role)contents.Rows[0]["Role"],
                        ("True" == contents.Rows[0]["Active"].ToString())
                        );
                        return output;
                    }
                    else
                    {
                        return output;
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public List<UserModel> GetAllUsers()
        {
            string commandText = "SELECT * FROM " + _usersTableName;

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    List<UserModel> output = new List<UserModel>();
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        for (int i = 0; i < contents.Rows.Count; i++)
                        {
                            output.Add(new UserModel(
                                Int32.Parse(contents.Rows[i]["UserID"].ToString()),
                                contents.Rows[i]["Lastname"].ToString(),
                                contents.Rows[i]["Firstname"].ToString(),
                                contents.Rows[i]["Username"].ToString(),
                                (Role)contents.Rows[i]["Role"],
                                ("True" == contents.Rows[i]["Active"].ToString())));
                        }

                        return output;
                    }
                    else
                    {
                        throw new UserDoesNotExistException("No users found");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void UpdateUsername(string currentUsername, string newUsername)
        {
            if (string.IsNullOrEmpty(currentUsername))
                throw new ArgumentNullException("CurrentUsername");

            if (string.IsNullOrEmpty(currentUsername))
                throw new ArgumentNullException("NewUsername");


            if (!UserExists(currentUsername))
                throw new UserDoesNotExistException("User " + currentUsername + " does not exist");

            if (UserExists(newUsername))
                throw new UserExistsException();

            string commandText = "UPDATE " + _usersTableName + " SET Username = '" + newUsername + "' WHERE Username = '" + currentUsername + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void UpdateFirstName(string username, string newFirstName)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            if (string.IsNullOrEmpty(newFirstName))
                throw new ArgumentNullException("NewFirstName");


            if (!UserExists(username))
                throw new UserDoesNotExistException("User " + username + " does not exist");

            string commandText = "UPDATE " + _usersTableName + " SET FirstName = '" + newFirstName + "' WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void UpdateLastName(string username, string newLastName)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            if (string.IsNullOrEmpty(newLastName))
                throw new ArgumentNullException("NewLastName");


            if (!UserExists(username))
                throw new UserDoesNotExistException("User " + username + " does not exist");

            string commandText = "UPDATE " + _usersTableName + " SET LastName = '" + newLastName + "' WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void UpdatePassword(string username, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            if (string.IsNullOrEmpty(currentPassword))
                throw new ArgumentNullException("CurrentPassword");

            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException("NewPassword");

            if (!UserExists(username))
                throw new UserDoesNotExistException("User " + username + " does not exist");

            string newSalt = Hashing.CreateSalt(16);
            string newHashedPassword = Hashing.GenerateSaltedHash(newPassword, newSalt);

            try
            {
                if (CorrectPassword(username, currentPassword))
                {
                    string commandText = "UPDATE " + _usersTableName + " SET Salt = '" + newSalt + "' , Password = '" + newHashedPassword + "' WHERE Username = '" + username + "';";

                    DatabaseUtility.ModifyDatabase(_connectionString, commandText);
                }
                else
                {
                    throw new IncorrectPasswordException();
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void UpdateRole(string username, Role newRole)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            if (!UserExists(username))
                throw new UserDoesNotExistException("User " + username + " does not exist");

            string commandText = "UPDATE " + _usersTableName + " SET Role = " + newRole + " WHERE Username = '" + username + "';";

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }
    }
}