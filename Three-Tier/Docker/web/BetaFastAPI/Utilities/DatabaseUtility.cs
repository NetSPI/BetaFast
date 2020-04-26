using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using BetaFastAPI.Exceptions;

namespace BetaFastAPI.Utilities
{
    public static class DatabaseUtility
    {
        public static bool IsServerAvailable(SqlConnection connection)
        {
            try
            {
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        public static DataTable QueryDatabase(string connectionString, string query, SqlParameter parameter = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameter != null)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (DataTable contents = new DataTable("Contents"))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(contents);
                                return contents;
                            }
                        }
                    }
                }
                else
                {
                    throw new ServerUnavailableException();
                }
            }
        }

        public static DataTable QueryDatabase(string connectionString, string query, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        using (DataTable contents = new DataTable("Contents"))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(contents);
                                return contents;
                            }
                        }
                    }
                }
                else
                {
                    throw new ServerUnavailableException();
                }
            }
        }

        public static void ModifyDatabase(string connectionString, string query, SqlParameter parameter = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameter != null)
                        {
                            command.Parameters.Add(parameter);
                        }
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    throw new ServerUnavailableException();
                }
            }
        }

        public static void ModifyDatabase(string connectionString, string query, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (IsServerAvailable(connection))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    throw new ServerUnavailableException();
                }
            }
        }
    }
}
