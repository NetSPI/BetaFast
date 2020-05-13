using BetaFastAPI.Exceptions;
using BetaFastAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BetaFastAPI.Security
{
    public class EncryptionDataManagement
    {
        private readonly string _saltTableName = "salts";
        private string _connectionString;

        public EncryptionDataManagement(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Salt is stored as Base64 string
        public string GetSalt(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            string commandText = "SELECT * from " + _saltTableName + " where username = @username";
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
                        string salt = contents.Rows[0]["Salt"].ToString();

                        return salt;
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

        private bool UserExists(string username)
        {
            string commandText = "SELECT 1 FROM " + _saltTableName + " WHERE Username = @username";
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

        // Salt is stored as Base64 string
        public void SetSalt(string username, string salt)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException("Salt");

            if (UserExists(username))
            {
                string commandText = "UPDATE " + _saltTableName + " SET Salt = @salt WHERE Username = @username;";

                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@salt", SqlDbType.VarChar) {Value = salt},
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

            else
            {
                string commandText = "INSERT INTO " + _saltTableName + " (Username, Salt) VALUES (@username,@salt)";

                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                    new SqlParameter("@salt", SqlDbType.VarChar) {Value = salt}
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
}
