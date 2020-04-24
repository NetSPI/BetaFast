using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Model;
using BetaFastAPI.Utilities;

namespace BetaFastAPI.Rentals
{
    public class RentalManagement
    {
        private string _rentalTableName = "rentals";
        private string _moviesTableName = "movies";
        private string _connectionString;

        public RentalManagement(string connectionString)
        {
            _connectionString = connectionString;
        }

        private bool RentalExists(string username, string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("Title");

            string commandText = "SELECT 1 FROM " + _rentalTableName + " WHERE Username = @username AND Title = @title";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                 new SqlParameter("@title", SqlDbType.VarChar) {Value = title}
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
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

        public List<Rental> GetCart(string username)
        {
            string commandText = "SELECT * FROM " + _rentalTableName + " WHERE Username = @username";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@username",
                Value = username
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, param))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        List<Rental> rentals = new List<Rental>();

                        for (int i = 0; i < contents.Rows.Count; i++)
                        {
                            Rental rental = new Rental();
                            rental.Username = contents.Rows[i]["Username"].ToString();
                            rental.Title = contents.Rows[i]["Title"].ToString();
                            rental.Quantity = int.Parse(contents.Rows[i]["Quantity"].ToString());
                            rental.Price = decimal.Parse(contents.Rows[i]["Price"].ToString());

                            rentals.Add(rental);
                        }
                        return rentals;
                    }
                    else
                    {
                        throw new UserDoesNotExistException("No rentals found");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void AddRental(string username, string title, int quantity)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("Title");
            if (quantity < 0)
                throw new ArgumentOutOfRangeException("Quantity");

            decimal price;

            try
            {
                price = GetMoviePrice(title);
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
            catch (MovieDoesNotExistException e)
            {
                throw e;
            }

            if (MovieExists(title))
            {
                try
                {
                    if (RentalExists(username, title))
                    {
                        string commandText = "UPDATE " + _rentalTableName + " SET Quantity = Quantity + @quantity WHERE Username = @username AND Title = @title;";

                        List<SqlParameter> parameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                        new SqlParameter("@title", SqlDbType.VarChar) {Value = title},
                        new SqlParameter("@quantity", SqlDbType.Int) {Value = quantity}
                    };

                        DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
                    }
                    else
                    {
                        string commandText = "INSERT INTO " + _rentalTableName + " (Username, Title, Quantity, Price) VALUES (@username,@title,@quantity,@price)";

                        List<SqlParameter> parameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                        new SqlParameter("@title", SqlDbType.VarChar) {Value = title},
                        new SqlParameter("@quantity", SqlDbType.Int) {Value = quantity},
                        new SqlParameter("@price", SqlDbType.Decimal) {Value = price}
                    };

                        DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
                    }
                }
                catch (ServerUnavailableException e)
                {
                    throw e;
                }
            }
            else
            {
                throw new MovieDoesNotExistException();
            }
        }

        public void RemoveRental(string username, string title, int quantity)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("Title");

            int currentQuantity = GetQuantity(username, title);

            if ((quantity < 0) || (quantity > currentQuantity))
                throw new ArgumentOutOfRangeException("Quantity");

            try
            {
                if (quantity == currentQuantity)
                {
                    if (!RentalExists(username, title))
                    {
                        throw new RentalDoesNotExistException("Rental " + title + " does not exist");
                    }

                    string commandText = "DELETE FROM " + _rentalTableName + " WHERE Username = @username AND Title = @title;";

                    List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                    new SqlParameter("@title", SqlDbType.VarChar) {Value = title}
                };

                    DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
                }
                else
                {
                    string commandText = "UPDATE " + _rentalTableName + " SET Quantity = Quantity - @quantity WHERE Username = @username AND Title = @title;";

                    List<SqlParameter> parameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                        new SqlParameter("@title", SqlDbType.VarChar) {Value = title},
                        new SqlParameter("@quantity", SqlDbType.Int) {Value = quantity}
                    };

                    DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);
                }
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
        }

        public void EmptyCart(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            try
            {
                string commandText = "DELETE FROM " + _rentalTableName + " WHERE Username = @username;";

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@username",
                    Value = username
                };

                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
        }

        private int GetQuantity(string username, string title)
        {
            string commandText = "SELECT * FROM " + _rentalTableName + " WHERE Username = @username AND Title = @title";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@username", SqlDbType.VarChar) {Value = username},
                new SqlParameter("@title", SqlDbType.VarChar) {Value = title}
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, parameters))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        return int.Parse(contents.Rows[0]["Quantity"].ToString());
                    }
                    else
                    {
                        throw new RentalDoesNotExistException("Rental " + title + " for " + username + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public decimal GetMoviePrice(string title)
        {
            string commandText = "SELECT * FROM " + _moviesTableName + " WHERE Title = @title";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@title",
                Value = title
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, param))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        return decimal.Parse(contents.Rows[0]["Price"].ToString());
                    }
                    else
                    {
                        throw new MovieDoesNotExistException("Movie " + title + " does not exist");
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        private bool MovieExists(string title)
        {
            string commandText = "SELECT 1 FROM " + _moviesTableName + " WHERE Title = @title";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@title",
                Value = title
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
    }
}
