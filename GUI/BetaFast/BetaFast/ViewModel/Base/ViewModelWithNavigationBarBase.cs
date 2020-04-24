using BetaFast.Authentication;
using BetaFast.Command;
using BetaFast.ViewModel.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace BetaFast.ViewModel.Base
{
    public abstract class ViewModelWithNavigationBarBase : INotifyPropertyChanged, IViewModelAuthenticated
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static ICommand _back;
        private static ICommand _home;
        private static ICommand _cart;
        private static ICommand _settings;
        private static ICommand _movies;
        private static ICommand _users;
        private static ICommand _signOut;

        private static string _adminVisibility;
        private static string _homeVisiblity;

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }

        public string AdminVisibility
        {
            get { return _adminVisibility; }
            set
            {
                _adminVisibility = value;
                OnPropertyChanged("AdminVisibility");
            }
        }

        public string HomeVisibility
        {
            get { return _homeVisiblity; }
            set
            {
                _homeVisiblity = value;
                OnPropertyChanged("HomeVisibility");
            }
        }

        public ViewModelWithNavigationBarBase()
        {
            _adminVisibility = "Hidden";
            _homeVisiblity = "Visible";
            Mediator.Mediator.Subscribe("SetAdmin", this.OnAdmin);
            Mediator.Mediator.Subscribe("SetHome", this.OnHome);
        }

        private void OnAdmin(Object obj)
        {
            AdminVisibility = (string)obj;
        }

        private void OnHome(Object obj)
        {
            HomeVisibility = (string)obj;
        }

        public static ICommand Back
        {
            get
            {
                return _back ?? (_back = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoBack", "");
                }));
            }
        }

        public static ICommand Home
        {
            get
            {
                return _home ?? (_home = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToHome", "");
                }));
            }
        }

        public static ICommand Movies
        {
            get
            {
                return _movies ?? (_movies = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToMovieManagement", "");
                }));
            }
        }

        public static ICommand Users
        {
            get
            {
                return _users ?? (_users = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToUserManagement", "");
                }));
            }
        }

        public static ICommand Cart
        {
            get
            {
                return _cart ?? (_cart = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToShoppingCart", "");
                }));
            }
        }

        public static ICommand Settings
        {
            get
            {
                return _settings ?? (_settings = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToSettings", "");
                }));
            }
        }

        public static ICommand SignOut
        {
            get
            {
                return _signOut ?? (_signOut = new RelayCommand(async x =>
                {
                    try
                    {
                        await Logout.LogoutAsync();
                        Mediator.Mediator.Notify("GoToLogin", "");
                    }
                    catch
                    {
                        Mediator.Mediator.Notify("GoToLogin", "");
                        Console.WriteLine("Sign out error");
                    }
                }));
            }
        }
    }
}
