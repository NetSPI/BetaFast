using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BetaFast.Authentication;
using BetaFast.Command;
using BetaFast.Model;
using BetaFast.ViewModel.Base;

namespace BetaFast.ViewModel
{
    class HomeViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _search;
        private ICommand _aboutUs;

        private ObservableCollection<Movie> _moviesList;
        private ObservableCollection<Movie> _visibleMoviesList;

        private string _searchText;

        private Movie _selectedMovie;

        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                _selectedMovie = value;
                if (value != null)
                {
                    Mediator.Mediator.Notify("MovieSelected", SelectedMovie);
                    Mediator.Mediator.Notify("GoToMovie", "");
                }
                OnPropertyChanged("SelectedMovie");
            }
        }

        public ObservableCollection<Movie> VisibleMoviesList
        {
            get { return _visibleMoviesList; }
            set
            {
                _visibleMoviesList = value;
                OnPropertyChanged("VisibleMoviesList");
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

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        public HomeViewModel()
        {
            Mediator.Mediator.Subscribe("RefreshHome", this.RefreshHome);
        }

        private void RefreshHome(object obj)
        {
            SelectedMovie = null;
            GetAllMovies();
            VisibleMoviesList = MoviesList;
        }

        private void GetAllMovies()
        {
            try
            {
                Task t = Task.Run(async () =>
                {
                    _moviesList = await MovieManagement.MovieManagement.GetAllMoviesAsync(_moviesList);
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(60000);
                if (!t.Wait(ts))
                {
                    Debug.WriteLine("Timeout.");
                    MessageBox.Show("A timeout occurred");
                    Task t2 = Task.Run(async () =>
                    {
                        await Logout.LogoutAsync();
                    });
                    TimeSpan ts2 = TimeSpan.FromMilliseconds(1000);
                    Mediator.Mediator.Notify("GoToLogin", "");
                }
                if (_moviesList != null)
                {
                    Mediator.Mediator.Notify("MovieCache", _moviesList);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show("An error occurred");
            }
        }

        public ICommand AboutUs
        {
            get
            {
                return _aboutUs ?? (_aboutUs = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToAboutUs", "");
                }));
            }
        }

        public ICommand Search
        {
            get
            {
                return _search ?? (_search = new RelayCommand(x =>
                {
                    ObservableCollection<Movie> placeholder = new ObservableCollection<Movie>();
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        for (int i=0; i < MoviesList.Count; i++)
                        {
                            if (MoviesList[i].Title.ToLower().Contains(SearchText.ToLower()))
                            {
                                placeholder.Add(MoviesList[i]);
                            }
                        }
                        VisibleMoviesList = placeholder;
                    }
                    else
                    {
                        VisibleMoviesList = MoviesList;
                    }
                }));
            }
        }
    }
}
