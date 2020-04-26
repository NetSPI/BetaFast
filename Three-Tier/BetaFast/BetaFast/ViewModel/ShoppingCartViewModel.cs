using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BetaFast.Command;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.ViewModel.Base;

namespace BetaFast.ViewModel
{
    class ShoppingCartViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _delete;
        private ICommand _confirmPayment;

        private ObservableCollection<Movie> _allMovies;
        private ObservableCollection<Movie> _moviesInCart;
        private ObservableCollection<Rental> _rentalsList;

        private decimal _totalPrice;

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

        public ObservableCollection<Movie> MoviesInCart
        {
            get { return _moviesInCart; }
            set
            {
                _moviesInCart = value;
                OnPropertyChanged("MoviesInCart");
            }
        }

        public ObservableCollection<Movie> AllMovies
        {
            get { return _allMovies; }
            set
            {
                _allMovies = value;
                OnPropertyChanged("AllMovies");
            }
        }

        public decimal TotalPrice
        {
            get { return _totalPrice; }
            private set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }

        public ShoppingCartViewModel()
        {
            MoviesInCart = new ObservableCollection<Movie>();
            AllMovies = new ObservableCollection<Movie>();
            _rentalsList = new ObservableCollection<Rental>();
            Mediator.Mediator.Subscribe("RefreshShoppingCart", this.RefreshShoppingCart);
            Mediator.Mediator.Subscribe("MovieCache", this.GetCacheForCart);
        }

        private void GetCacheForCart(object obj)
        {
            AllMovies = (ObservableCollection<Movie>)obj;
        }

        private void RefreshShoppingCart(object obj)
        {
            SelectedMovie = null;
            MoviesInCart.Clear();
            GetAllRentals();
            GetMoviesFromCart();
            GetTotalPrice();
        }

        private void GetAllRentals()
        {
            try
            {
                Task t = Task.Run(async () =>
                {
                    _rentalsList = await ShoppingCart.ShoppingCartManagement.GetCartAsync();
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                if (!t.Wait(ts))
                {
                    Debug.WriteLine("Timeout.");
                    MessageBox.Show("A timeout occurred");
                }
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine("Argument exception: " + e.Message);
                MessageBox.Show("An error occurred");
            }
            catch (ServerException)
            {
                Debug.WriteLine("A server exception occurred");
                MessageBox.Show("A server exception occurred");
            }
            catch(Exception e)
            {
                Debug.WriteLine("An exception occurred: " + e.Message);
                MessageBox.Show("An error occurred");
            }
        }

        private void GetTotalPrice()
        {
            decimal total = 0;
            for (int i = 0; i < MoviesInCart.Count; i++)
            {
                total += MoviesInCart[i].Price;
            }
            TotalPrice = total;
        }

        private void GetMoviesFromCart()
        {
            MoviesInCart.Clear();
            if (_rentalsList == null)
            {
                _rentalsList = new ObservableCollection<Rental>();
                return;
            }
            if (AllMovies.Count == 0)
            {
                for (int i=0; i < _rentalsList.Count; i++)
                {
                    Movie rental = null;
                    try
                    {
                        Task t = Task.Run(async () =>
                        {
                            rental = await MovieManagement.MovieManagement.GetMovieAsync(_rentalsList[i].Title);
                        });
                        TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                        if (!t.Wait(ts))
                        {
                            Debug.WriteLine("Timeout.");
                            MessageBox.Show("A timeout occurred");
                        }
                        if (rental != null)
                        {
                            for (int j = 0; j < _rentalsList[i].Quantity; j++)
                            {
                                MoviesInCart.Add(rental);
                            }
                        }
                        else
                        {
                            MessageBox.Show("An error occurred");
                        }
                        
                    }
                    catch (ArgumentException e)
                    {
                        Debug.WriteLine("Argument exception: " + e.Message);
                        MessageBox.Show("An error occurred");
                    }
                    catch (ServerException)
                    {
                        Debug.WriteLine("A server exception occurred");
                        MessageBox.Show("A server exception occurred");
                    }
                    catch (MovieDoesNotExistException)
                    {
                        Debug.WriteLine("Movie does not exist exception");
                        MessageBox.Show("An error occurred");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("An exception occurred: " + e.Message);
                        MessageBox.Show("An error occurred");
                    }
                }
            }
            else
            {
                for (int i = 0; i < _rentalsList.Count; i++)
                {
                    for (int j = 0; j < AllMovies.Count; j++)
                    {
                        if (AllMovies[j].Title.Equals(_rentalsList[i].Title))
                        {
                            for (int k = 0; k < _rentalsList[i].Quantity; k++)
                            {
                                MoviesInCart.Add(AllMovies[j]);
                            }
                            break;
                        }
                        if (j == AllMovies.Count)
                        {
                            // Rental was not found
                            throw new MovieDoesNotExistException();
                        }
                    }
                }
            }
        }

        public ICommand ConfirmPayment
        {
            get
            {
                return _confirmPayment ?? (_confirmPayment = new RelayCommand(x =>
                {
                    if (MoviesInCart.Count > 0)
                    {
                        Mediator.Mediator.Notify("TotalPrice", TotalPrice);
                        Mediator.Mediator.Notify("GoToPayment", "");
                    }
                }));
            }
        }

        private void DeleteSelectedRentalLocally()
        {
            string title = SelectedMovie.Title;

            for (int i=0; i < MoviesInCart.Count; i++)
            {
                if (MoviesInCart[i].Title.Equals(title))
                {
                    MoviesInCart.Remove(MoviesInCart[i]);
                    break;
                }
            }

            for (int i = 0; i < _rentalsList.Count; i++)
            {
                if (_rentalsList[i].Title.Equals(title))
                {
                    _rentalsList[i].Quantity -= 1;
                    break;
                }
            }

            SelectedMovie = null;

            GetTotalPrice();
        }

        public ICommand Delete
        {
            get
            {
                return _delete ?? (_delete = new RelayCommand(x =>
                {
                    if (SelectedMovie != null)
                    {
                        try
                        {
                            Task t = Task.Run(async () =>
                            {
                                await ShoppingCart.ShoppingCartManagement.RemoveRentalAsync(SelectedMovie.Title, 1);
                            });
                            TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                            if (!t.Wait(ts))
                            {
                                Debug.WriteLine("Timeout.");
                                MessageBox.Show("A timeout occurred");
                            }
                            DeleteSelectedRentalLocally();
                        }
                        catch (ArgumentException e)
                        {
                            Debug.WriteLine("Argument exception: " + e.Message);
                            MessageBox.Show("An error occurred");
                        }
                        catch (ServerException)
                        {
                            Debug.WriteLine("A server exception occurred");
                            MessageBox.Show("A server exception occurred");
                        }
                        catch (MovieDoesNotExistException)
                        {
                            Debug.WriteLine("Movie does not exist exception");
                            MessageBox.Show("An error occurred");
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("An exception occurred: " + e.Message);
                            MessageBox.Show("An error occurred");
                        }
                    }
                }));
            }
        }
    }
}
