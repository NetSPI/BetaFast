using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Model;
using BetaFastAPI.Utilities;

namespace BetaFastAPI.Movies
{
    public class MovieManagement
    {
        private string _moviesTableName = "movies";
        private string _rentalTableName = "rentals";
        private string _connectionString;
        private string _postersPath;

        public MovieManagement(string connectionString, string postersPath)
        {
            _connectionString = connectionString;
            _postersPath = postersPath;
        }
        public Movie GetMovieByTitle(string title)
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
                        Movie movie = new Movie();
                        movie.Title = title;
                        movie.Description = contents.Rows[0]["Description"].ToString();
                        movie.Director = contents.Rows[0]["Director"].ToString();
                        movie.PosterFile = contents.Rows[0]["PosterFile"].ToString();
                        movie.Year = int.Parse(contents.Rows[0]["Year"].ToString());
                        movie.Price = Decimal.Parse(contents.Rows[0]["Price"].ToString());

                        return movie;
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

        public List<Movie> GetAllMovies()
        {
            string commandText = "SELECT * FROM " + _moviesTableName;

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText))
                {
                    if (contents != null && contents.Rows.Count > 0)
                    {
                        List<Movie> movies = new List<Movie>();

                        for (int i = 0; i < contents.Rows.Count; i++)
                        {
                            Movie movie = new Movie();
                            movie.Title = contents.Rows[i]["Title"].ToString();
                            movie.Description = contents.Rows[i]["Description"].ToString();
                            movie.Director = contents.Rows[i]["Director"].ToString();
                            movie.PosterFile = contents.Rows[i]["PosterFile"].ToString();
                            movie.Year = int.Parse(contents.Rows[i]["Year"].ToString());
                            movie.Price = decimal.Parse(contents.Rows[i]["Price"].ToString());

                            movies.Add(movie);
                        }
                        return movies;
                    }
                    else
                    {
                        throw new MovieDoesNotExistException("No movies found");
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

        private bool MovieFileExists(string file)
        {
            string commandText = "SELECT 1 FROM " + _moviesTableName + " WHERE PosterFile = @file";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@file",
                Value = file
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

        public async Task AddMovieAsync(string title, string description, string director, int year, decimal price, IFormFile posterFile)
        {
            string poster = posterFile.FileName;

            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("Title");
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("Description");
            if (string.IsNullOrEmpty(director))
                throw new ArgumentNullException("Director");
            if (string.IsNullOrEmpty(poster))
                throw new ArgumentNullException("Poster");

            // Check if format is xx.xx
            if ((decimal.Round(price, 2) != price) && (Math.Floor(Math.Log10((double)decimal.Truncate(price)) + 1) > 1))
            {
                throw new ArgumentException("Price must be in the format x.xx");
            }

            if (price < 0)
            {
                throw new ArgumentException("Invalid price");
            }

            if ((year < 1888) || (year > DateTime.Now.Year))
            {
                throw new ArgumentException("We only rent movies that are currently released. Please check the date.");
            }

            if (!Security.InputValidation.IsValidFileName(posterFile.FileName))
            {
                throw new InvalidFileException();
            }

            if (MovieExists(title))
            {
                throw new MovieExistsException("Movie " + title + " already exists");
            }

            if (MovieFileExists(poster))
            {
                throw new MovieExistsException("Poster with the name " + poster + " already exists");
            }

            string commandText = "INSERT INTO " + _moviesTableName + " (Title, Description, Director, Year, Price, PosterFile) VALUES (@title,@description,@director,@year,@price,@poster)";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@title", SqlDbType.VarChar) {Value = title},
                 new SqlParameter("@description", SqlDbType.VarChar) {Value = description},
                 new SqlParameter("@director", SqlDbType.VarChar) {Value = director},
                 new SqlParameter("@year", SqlDbType.Int) {Value = year},
                 new SqlParameter("@price", SqlDbType.Decimal) {Value = price},
                 new SqlParameter("@poster", SqlDbType.VarChar) {Value = poster}
            };

            try
            {
                string filePath = GetPosterPath(posterFile.FileName);

                if (System.IO.File.Exists(filePath))
                {
                    throw new InvalidFileException();
                }

                DatabaseUtility.ModifyDatabase(_connectionString, commandText, parameters);

                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await posterFile.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    // Filewrite failed - delete database entry. Return 500.
                    DeleteMovie(title);
                    throw new Exception();
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        public void DeleteMovie(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("Title");
            }

            if (!MovieExists(title))
            {
                throw new MovieDoesNotExistException("Movie " + title + " does not exist");
            }

            // Try and delete the poster file. Not found? That's fine. Error deleting? Throw exception.
            Movie movie = GetMovieByTitle(title);
            string filePath = GetPosterPath(movie.PosterFile);

            if (!MultipleMoviesUsePoster(movie.PosterFile))
            {
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
            }

            // Delete from Movies table
            string commandText = "DELETE FROM " + _moviesTableName + " WHERE Title = @title;";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@title",
                Value = title
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText, param);
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }

            // Delete from Rental table
            string commandText2 = "DELETE FROM " + _rentalTableName + " WHERE Title = @title;";

            SqlParameter param2 = new SqlParameter
            {
                ParameterName = "@title",
                Value = title
            };

            try
            {
                DatabaseUtility.ModifyDatabase(_connectionString, commandText2, param2);
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
        }

        private bool MultipleMoviesUsePoster(string fileName)
        {
            string commandText = "SELECT * FROM " + _moviesTableName + " WHERE PosterFile = @file";
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@file",
                Value = fileName
            };

            try
            {
                using (DataTable contents = DatabaseUtility.QueryDatabase(_connectionString, commandText, param))
                {
                    if (contents.Rows.Count > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (ServerUnavailableException)
            {
                throw new ServerUnavailableException();
            }
        }

        private string GetPosterPath(string file)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), _postersPath);
            return System.IO.Path.Combine(folderPath, file);
        }
    }
}