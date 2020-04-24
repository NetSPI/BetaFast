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
            string commandText = "SELECT 1 FROM " + _usersTableName + " WHERE Username = @username";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@username",
                Value = username
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, param))
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
            string commandText = "SELECT 1 FROM " + _usersTableName + " WHERE UserID = @userID";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@userID",
                Value = userID
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, param))
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
            string commandText = "SELECT COUNT(*) FROM " + _usersTableName;
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
            string commandText = "SELECT count(*) FROM " + _usersTableName + " WHERE Active=1";
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
            string commandText = "SELECT count(*) FROM " + _usersTableName + " WHERE Active=0";
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

            string commandText = "INSERT INTO " + _usersTableName + " (UserID, LastName, FirstName, Username, Password, Salt, Role, Active) VALUES (@userID,@lastName,@firstName,@username,@password,@salt,@role,@active)";

            int newUserID = TotalUsers() + 1;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@userID", SqlDbType.Int) {Value = newUserID},
                 new SqlParameter("@lastName", SqlDbType.VarChar) {Value = lastName},
                 new SqlParameter("@firstName", SqlDbType.VarChar) {Value = firstName},
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                 new SqlParameter("@password", SqlDbType.VarChar) {Value = hashedPassword},
                 new SqlParameter("@salt", SqlDbType.VarChar) {Value = salt},
                 new SqlParameter("@role", SqlDbType.Int) {Value = (int) role},
                 new SqlParameter("@active", SqlDbType.Bit) {Value = 1}
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
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

            string commandText = "DELETE FROM " + _usersTableName + " WHERE Username = @username;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@username",
                Value = username
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
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

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 0 WHERE UserID = @userID;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@userID",
                Value = userID
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
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

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 0 WHERE Username = @username;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@username",
                Value = username
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
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

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 1 WHERE UserID = @userID;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@userID",
                Value = userID
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
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

            string commandText = "UPDATE " + _usersTableName + "; SET Active = 1 WHERE Username = @username;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@username",
                Value = username
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
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

            string commandText = "SELECT * from " + _usersTableName + " where username = @username";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
            };
            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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

            string commandText = "SELECT * from " + _usersTableName + " where username = @username";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
            };
            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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
            string commandText = "SELECT * from " + _usersTableName + " where UserID = @userID";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@userID", SqlDbType.VarChar) {Value = userID},
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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

            string commandText = "SELECT * from " + _usersTableName + " where username = @username";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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

            string commandText = "SELECT * from " + _usersTableName + " where UserID = @userID";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@userID", SqlDbType.Int) {Value = userID},
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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
            string commandText = "SELECT * from " + _usersTableName + " where UserID = @userID";
            SqlParameter parameter = new SqlParameter("@userID", SqlDbType.VarChar) { Value = userID };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameter))
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

            string commandText = "SELECT * from " + _usersTableName + " where Username = @username";
            SqlParameter parameter = new SqlParameter("@username", SqlDbType.VarChar) { Value = username };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameter))
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
            string commandText = "SELECT * from " + _usersTableName;

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

            string commandText = "UPDATE " + _usersTableName + " SET Username = @newUsername WHERE Username = @currentUsername;";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@newUsername", SqlDbType.VarChar) {Value = newUsername},
                 new SqlParameter("@currentUsername", SqlDbType.VarChar) {Value = currentUsername}
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
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

            string commandText = "UPDATE " + _usersTableName + " SET FirstName = @newFirstName WHERE Username = @username;";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@newFirstName", SqlDbType.VarChar) {Value = newFirstName},
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username}
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
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

            string commandText = "UPDATE " + _usersTableName + " SET LastName = @newLastName WHERE Username = @username;";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@newLastName", SqlDbType.VarChar) {Value = newLastName},
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username}
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
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
                    string commandText = "UPDATE " + _usersTableName + " SET Salt = @newSalt , Password = @newHashedPassword WHERE Username = @username;";

                    List<SqlParameter> parameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@newSalt", SqlDbType.VarChar) {Value = newSalt},
                        new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                        new SqlParameter("@newHashedPassword", SqlDbType.VarChar) {Value = newHashedPassword}
                    };
                    DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
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

            string commandText = "UPDATE " + _usersTableName + " SET Role = @role WHERE Username = @username;";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@role", SqlDbType.VarChar) {Value = newRole},
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username}
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }
    }
}