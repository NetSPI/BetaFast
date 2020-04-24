using System;
using System.Threading.Tasks;
using BetaFast.Model;
using System.Windows.Input;
using BetaFast.Command;
using BetaFast.ViewModel.Base;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;
using BetaFast.Exceptions;

namespace BetaFast.ViewModel
{
    public class MovieViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _rent;

        private Movie _movie;
       
        public string Title
        {
            get { return _movie.Title; }
        }

        public string Description
        {
            get { return _movie.Description; }
        }

        public string Director
        {
            get { return _movie.Director; }
        }

        public int Year
        {
            get { return _movie.Year; }
        }

        public decimal Price
        {
            get { return _movie.Price; }
        }

        public Movie Movie
        {
            get { return _movie; }
            set
            {
                _movie = value;
                OnPropertyChanged("Movie");
            }
        }

        public BitmapImage Poster
        {
            get { return _movie.Poster.Image; }
        }

        public MovieViewModel()
        {
            Mediator.Mediator.Subscribe("MovieSelected", this.MovieSelected);
        }

        private void MovieSelected(object obj)
        {
            Movie = (Movie)obj;
        }

        public ICommand Rent
        {
            get
            {
                return _rent ?? (_rent = new RelayCommand(x =>
                {
                    AddRental();
                    Mediator.Mediator.Notify("GoToShoppingCart", "");
                }));
            }
        }

        private void AddRental()
        {
            try
            {
                Task t = Task.Run(async () =>
                {
                    await ShoppingCart.ShoppingCartManagement.AddRentalAsync(Title, 1);
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
            catch (RentalDoesNotExistException)
            {
                Debug.WriteLine("Rental does not exist exception");
                MessageBox.Show("An error occurred");
            }
            catch (ServerException)
            {
                Debug.WriteLine("A server exception occurred");
                MessageBox.Show("A server exception occurred");
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception occurred: " + e.Message);
                MessageBox.Show("An error occurred");
            }
        }
    }
}
