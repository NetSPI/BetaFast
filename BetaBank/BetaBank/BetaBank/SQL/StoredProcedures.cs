using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace BetaBank.SQL
{
    public static class StoredProcedures
    {
        public static string GetBalance(Guid sessionID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetBalance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters.Add("@totalBalance", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;

                    cmd.Parameters["@ID"].Value = sessionID;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string balance = (string)cmd.Parameters["@totalBalance"].Value;
                    conn.Close();
                    return balance;
                }
            }
        }

        public static void WithdrawBalance(decimal withdrawal, Guid sessionID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("WithdrawBalance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Withdrawal", SqlDbType.Money);
                    cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);

                    cmd.Parameters["@Withdrawal"].Value = new SqlMoney(withdrawal);
                    cmd.Parameters["@ID"].Value = sessionID;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
 
        public static void WithdrawBalance(decimal withdrawal, string username)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("WithdrawBalance", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE BetaBankBalances SET Balance = Balance - " + new SqlMoney(withdrawal).ToString() + " WHERE Username = '" + username + "';";

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, username.Length);
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar, password.Length);
                    cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

                    cmd.Parameters["@Username"].Value = username;
                    cmd.Parameters["@Password"].Value = password;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Guid sessionID = (Guid)cmd.Parameters["@GUID"].Value;
                    Mediator.Mediator.Notify("SessionID", sessionID);
                    conn.Close();
                }
            }
        }

        public static void DestroySession(Guid sessionID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DestroySession", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@ID"].Value = sessionID;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void CreateUser(string lastName, string firstName, string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CreateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar, lastName.Length);
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, firstName.Length);
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, username.Length);
                    cmd.Parameters.Add("@Password", SqlDbType.VarChar, password.Length);

                    cmd.Parameters["@LastName"].Value = lastName;
                    cmd.Parameters["@FirstName"].Value = firstName;
                    cmd.Parameters["@Username"].Value = username;
                    cmd.Parameters["@Password"].Value = password;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static string GetVaultLocation(Guid sessionID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["betabase"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetVaultLocation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters.Add("@address", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;

                    cmd.Parameters["@ID"].Value = sessionID;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string address = (string)cmd.Parameters["@address"].Value;
                    conn.Close();
                    return address;
                }
            }
        }
    }
}
