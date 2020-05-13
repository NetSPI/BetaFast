using BetaFast.Content;
using BetaFast.Controllers;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BetaFast.MovieManagement
{
    public static class MovieManagement
    {
        public static async Task AddMovieAsync(Movie movie)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent("---------------" + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds))
            {
                content.Add(new StringContent(movie.Title), "Title");
                content.Add(new StringContent(movie.Description), "Description");
                content.Add(new StringContent(movie.Director), "Director");
                content.Add(new StringContent(movie.Year.ToString()), "Year");
                content.Add(new StringContent(movie.Price.ToString()), "Price");
                content.Add(new StreamContent(new MemoryStream(ImageUtility.BitmapImageToByteArray(movie.Poster.Image, movie.Poster.Format))), "Poster", movie.Poster.FileName);

                using (HttpResponseMessage response = await MovieController.AddMovie(content))
                {

                    int status = (int)response.StatusCode;
                    string body = await response.Content.ReadAsStringAsync();

                    if (status == 200)
                    {
                        return;
                    }
                    else if (status == 400)
                    {
                        throw new Exception(body);
                    }
                    else if (status == 401)
                    {
                        throw new UnauthenticatedException();
                    }
                    else if (status == 403)
                    {
                        throw new UnauthorizedException();
                    }
                    else if (status == 500)
                    {
                        throw new ServerException();
                    }
                    else
                    {
                        throw new Exception("An uncaught error occurred.");
                    }
                }
            }
        }

        public static async Task DeleteMovieAsync(string title)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Title", title } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await MovieController.DeleteMovie(encodedContent))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    return;
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 403)
                {
                    throw new UnauthorizedException();
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("An uncaught error occurred.");
                }
            }
        }

        public static async Task<Movie> GetMovieAsync(string title)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Title", title } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);

            using (HttpResponseMessage response = await MovieController.GetMovie(encodedContent))
            {
                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    MovieResponse jsonBody = JsonConvert.DeserializeObject<MovieResponse>(body);

                    try
                    {
                        return await MovieResponseToMovie(jsonBody);
                    }
                    catch (ServerException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else if (status == 400)
                {
                    throw new ArgumentException(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 404)
                {
                    throw new MovieDoesNotExistException("Movie not found");
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("Uncaught status code");
                }
            }
        }
        public static async Task<ObservableCollection<Movie>> GetAllMoviesAsync(ObservableCollection<Movie> cache)
        {
            using (HttpResponseMessage response = await MovieController.GetAllMovies())
            {
                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    ObservableCollection<MovieResponse> jsonBody = JsonConvert.DeserializeObject<ObservableCollection<MovieResponse>>(body);

                    try
                    {
                        return await MovieResponseToMovie(jsonBody, cache);
                    }
                    catch (ServerException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 404)
                {
                    return new ObservableCollection<Movie>();
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("An error occurred");
                }
            }
        }

        public static async Task<ObservableCollection<Movie>> GetAllMoviesAsync()
        {
            ObservableCollection<Movie> cache = new ObservableCollection<Movie>();
            return await GetAllMoviesAsync(cache);
        }

        private static async Task<Poster> GetPoster(string filename)
        {
            try
            {
                BitmapImage image = await MoviePosters.GetMoviePosterAsync(filename);
                ImageFormat format = ImageUtility.GetImageFormat(filename);
                Uri uri = ImageUtility.PosterURLFromFilename(filename);

                return new Poster(format, uri, filename, image);
            }
            catch (ServerException e)
            {
                throw e;
            }
            catch (FileNotFoundException e)
            {
                Uri fileNotFoundUri = new Uri("pack://application:,,,/Images/file-not-found.jpg");
                BitmapImage image = new BitmapImage(fileNotFoundUri);
                image.Freeze();
                return new Poster(ImageFormat.Jpeg, fileNotFoundUri, "file-not-found.jpg", image);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static async Task<Movie> MovieResponseToMovie(MovieResponse response)
        {
            Poster poster = await GetPoster(response.PosterFile);
            return new Movie(response.Title, response.Description, response.Director, response.Year, response.Price, poster);
        }

        private static async Task<ObservableCollection<Movie>> MovieResponseToMovie(ObservableCollection<MovieResponse> response, ObservableCollection<Movie> cache)
        {
            ObservableCollection<Movie> movies = new ObservableCollection<Movie>();

            for (int i = 0; i < response.Count; i++)
            {
                Movie cachedMovie = GetMovieFromList(response[i].Title, cache);
                if (cachedMovie != null)
                {
                    movies.Add(cachedMovie);
                }
                else
                {
                    Poster poster = await GetPoster(response[i].PosterFile);
                    movies.Add(new Movie(response[i].Title, response[i].Description, response[i].Director, response[i].Year, response[i].Price, poster));
                }
            }

            return movies;
        }

        private static Movie GetMovieFromList(string title, ObservableCollection<Movie> list)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Title.Equals(title))
                    {
                        return list[i];
                    }
                }
            }
            return null;
        }
    }
}
