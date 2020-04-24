using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Diagnostics;

namespace BetaFast.Utilities
{
    public static class DatabaseUtility
    {
        public static string GetConnectionStringByName(string name)
        {
            string returnValue = null;

            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[name];

            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }


        public static bool IsServerAvailable(SqlConnection connection)
        {
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error connecting to server.");
                Debug.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        public static DataTable QueryDatabase(string connectionStringName, string query, SqlParameter parameter = null)
        {
            DataTable contents = new DataTable("Contents");
            using (SqlConnection connection = new SqlConnection(GetConnectionStringByName(connectionStringName)))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    if (parameter != null)
                    {
                        command.Parameters.Add(parameter);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(contents);
                }
            }
            return contents;
        }

        public static DataTable QueryDatabase(string connectionStringName, string query, List<SqlParameter> parameters)
        {
            DataTable contents = new DataTable("Contents");
            using (SqlConnection connection = new SqlConnection(GetConnectionStringByName(connectionStringName)))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(contents);
                }
            }
            return contents;
        }

        public static void ModifyDatabase(string connectionStringName, string query, SqlParameter parameter = null)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionStringByName(connectionStringName)))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    if (parameter != null)
                    {
                        command.Parameters.Add(parameter);
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ModifyDatabase(string connectionStringName, string query, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionStringByName(connectionStringName)))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
