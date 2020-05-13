using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BetaFast.Command;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.Utilities;
using BetaFast.ViewModel.Base;

namespace BetaFast.ViewModel
{
    class MovieManagementViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _add;
        private ICommand _delete;
        private ICommand _browse;
        private ICommand _refresh;

        private string _message;
        private string _messageColor;
        private string _fileMessage;
        private string _fileMessageColor;

        private string _title;
        private string _director;
        private string _year;
        private int _yearInt;
        private string _price;
        private decimal _priceDecimal;
        private string _description;
        private BitmapImage _poster;

        private byte[] _posterByteArray;
        private string _posterFileName;

        private ObservableCollection<Movie> _moviesList;

        private Movie _selectedMovie;

        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                OnPropertyChanged("SelectedMovie");
            }
        }

        public ObservableCollection<Movie> MoviesList
        {
            get { return _moviesList; }
            set
            {
                _moviesList = value;
                OnPropertyChanged("MoviesList");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public string Director
        {
            get { return _director; }
            set
            {
                if (_director != value)
                {
                    _director = value;
                    OnPropertyChanged("Director");
                }
            }
        }

        public string Year
        {
            get { return _year; }
            set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged("Year");
                }
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public BitmapImage Poster
        {
            get { return _poster; }
            set
            {
                _poster = value;
                OnPropertyChanged("Poster");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public string MessageColor
        {
            get { return _messageColor; }
            set
            {
                _messageColor = value;
                OnPropertyChanged("MessageColor");
            }
        }

        public string FileMessage
        {
            get { return _fileMessage; }
            set
            {
                _fileMessage = value;
                OnPropertyChanged("FileMessage");
            }
        }

        public string FileMessageColor
        {
            get { return _fileMessageColor; }
            set
            {
                _fileMessageColor = value;
                OnPropertyChanged("FileMessageColor");
            }
        }

        private void SetMessage(string text, string color)
        {
            Message = text;
            MessageColor = color;
        }

        private void SetFileMessage(string text, string color)
        {
            FileMessage = text;
            FileMessageColor = color;
        }

        private void ClearFileMessage()
        {
            FileMessage = string.Empty;
        }

        public MovieManagementViewModel()
        {
            SetMessage(INITIAL, INITIAL_COLOR);
            Mediator.Mediator.Subscribe("RefreshMovieManagement", this.RefreshMovieManagement);
            Mediator.Mediator.Subscribe("GoToLogin", this.ClearPage);
            Mediator.Mediator.Subscribe("OnGoBack", this.ClearPage);
            Mediator.Mediator.Subscribe("MovieCache", this.MovieCache);

            _posterFileName = string.Empty;
            _posterByteArray = null;
        }

        private void MovieCache(object obj)
        {
            MoviesList = (ObservableCollection<Movie>)obj;
        }

        private void RefreshMovieManagement(object obj)
        {
            GetAllMovies();
        }

        private void GetAllMovies()
        {
            try
            {
                Task t = Task.Run(async () =>
                {
                    MoviesList = await MovieManagement.MovieManagement.GetAllMoviesAsync(MoviesList);
                    Mediator.Mediator.Notify("MovieCache", MoviesList);
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                if (!t.Wait(ts))
                {
                    Debug.WriteLine("Timeout.");
                    SetMessage("A timeout occurred.", ERROR_COLOR);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                SetMessage("An error occurred retrieving movies.", ERROR_COLOR);
            }
        }

        private void RemoveMovie(Movie movie)
        {
            MoviesList.Remove(movie);
        }

        private void RemoveMovie(string title)
        {
            for (int i = 0; i < MoviesList.Count; i++)
            {
                if (MoviesList[i].Title.Equals(title))
                {
                    MoviesList.RemoveAt(i);
                    break;
                }
            }
        }

        private void AddMovie(Movie movie)
        {
            MoviesList.Add(movie);
        }

        private void ClearPage(object obj)
        {
            ClearPage();
        }

        private void ClearPage()
        {
            SetMessage(INITIAL, INITIAL_COLOR);
            Title = string.Empty;
            Director = string.Empty;
            Year = string.Empty;
            Description = string.Empty;
            ClearFile();
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Director = string.Empty;
            Year = string.Empty;
            Description = string.Empty;
            ClearFile();
        }

        private void ClearFile()
        {
            _posterByteArray = null;
            _posterFileName = string.Empty;
            ClearFileMessage();
        }

        private bool IsFileSelected()
        {
            return ((_posterByteArray != null) && (_posterFileName != string.Empty));
        }

        private async Task AddMovie()
        {
            try
            {
                if (IsFileSelected())
                {
                    BitmapImage image = ImageUtility.ByteArrayToBitmapImage(_posterByteArray);
                    Poster poster = new Poster(ImageUtility.GetImageFormat(_posterFileName), ImageUtility.PosterURLFromFilename(_posterFileName), _posterFileName, image);
                    await MovieManagement.MovieManagement.AddMovieAsync(new Movie(Title, Description, Director, _yearInt, _priceDecimal, poster));
                    AddMovie(new Movie(Title, Description, Director, _yearInt, _priceDecimal, poster));
                    ClearPage();
                }
                else
                {
                    SetMessage("Select a poster file.", ERROR_COLOR);
                }
            }
            catch (ServerException e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
            catch (Exception e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
        }

        private async Task DeleteMovie()
        {
            try
            {
                await MovieManagement.MovieManagement.DeleteMovieAsync(SelectedMovie.Title);
                RemoveMovie(SelectedMovie.Title);
                SetMessage(DELETED, SUCCESS_COLOR);
            }
            catch (ServerException)
            {
                SetMessage("A server error occurred", ERROR_COLOR);
            }
            catch (Exception e)
            {
                SetMessage(e.Message, ERROR_COLOR);
            }
        }

        private bool IsFormComplete()
        {
            if ((!string.IsNullOrEmpty(Title)) && (!string.IsNullOrEmpty(Director)) && (!string.IsNullOrEmpty(Year)) && (!string.IsNullOrEmpty(Description)) && IsFileSelected())
            {
                // Validate form values
                if (Year.Contains("-"))
                {
                    SetMessage("Year is invalid.", ERROR_COLOR);
                    return false;
                }

                if (!Int32.TryParse(Year, out _yearInt))
                {
                    SetMessage("Year is invalid.", ERROR_COLOR);
                    return false;
                }

                if ((_yearInt < 1888) || (_yearInt > DateTime.Now.Year))
                {
                    SetMessage("We only rent movies that are currently released. Please check the date.", ERROR_COLOR);
                    return false;
                }

                if (Price.Contains("-"))
                {
                    SetMessage("Price is invalid.", ERROR_COLOR);
                    return false;
                }

                if (!(Price.Length == 4))
                {
                    SetMessage("Price must be in format \"x.xx\".", ERROR_COLOR);
                    return false;
                }

                if (!Price.Contains(".") || !Price.Substring(1, 1).Contains("."))
                {
                    SetMessage("Price must be in format \"x.xx\".", ERROR_COLOR);
                    return false;
                }

                if (!Decimal.TryParse(Price, out _priceDecimal))
                {
                    SetMessage("Price is invalid.", ERROR_COLOR);
                    return false;
                }

                if (!(_priceDecimal > 0))
                {
                    SetMessage("Price is invalid.", ERROR_COLOR);
                    return false;
                }

                return true;
            }
            else
            {
                SetMessage(ERROR, ERROR_COLOR);
                return false;
            }
        }

        private void SelectFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _posterFileName = Path.GetFileName(openFileDialog.FileName);
                    _posterByteArray = File.ReadAllBytes(openFileDialog.FileName);
                    SetFileMessage(SELECTED, SUCCESS_COLOR);
                }
            }
        }

        public ICommand Add
        {
            get
            {
                return _add ?? (_add = new RelayCommand(async x =>
                {
                    if (IsFormComplete())
                    {
                        await AddMovie();
                    }
                }));
            }
        }

        public ICommand Delete
        {
            get
            {
                return _delete ?? (_delete = new RelayCommand(async x =>
                {
                    if (SelectedMovie == null)
                    {
                        SetMessage("No movie is selected to delete.", ERROR_COLOR);
                    }
                    else
                    {
                        await DeleteMovie();
                    }
                }));
            }
        }

        public ICommand Browse
        {
            get
            {
                return _browse ?? (_browse = new RelayCommand(x =>
                {
                    SelectFile();
                }));
            }
        }

        public ICommand Refresh
        {
            get
            {
                return _refresh ?? (_refresh = new RelayCommand(x =>
                {
                    GetAllMovies();
                }));
            }
        }

        private const string CREATED = "Movie successfully created.";
        private const string DELETED = "Movie successfully deleted.";
        private const string ERROR = "All fields are required.";
        private const string INITIAL = "Enter information for a new movie.";

        private const string SUCCESS_COLOR = "green";
        private const string ERROR_COLOR = "red";
        private const string INITIAL_COLOR = "black";

        private const string SELECTED = "File selected";
    }
}
