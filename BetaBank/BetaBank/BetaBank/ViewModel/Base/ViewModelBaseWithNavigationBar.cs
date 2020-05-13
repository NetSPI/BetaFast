using BetaBank.Command;
using BetaBank.ViewModel.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace BetaBank.ViewModel.Base
{
    public abstract class ViewModelWithNavigationBar : INotifyPropertyChanged, IViewModelAuthenticated
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static ICommand _back;
        private static ICommand _admin;
        private static ICommand _signOut;
        private Guid _sessionID;

        private static string _homeVisiblity;
        private static string _adminVisibility;

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

        public ViewModelWithNavigationBar()
        {
            _adminVisibility = "Hidden";
            Mediator.Mediator.Subscribe("SetAdmin", this.OnAdmin);
            Mediator.Mediator.Subscribe("SetHome", this.OnHome);
            Mediator.Mediator.Subscribe("SessionID", this.SetSessionID);
        }

        private void SetSessionID(Object obj)
        {
            _sessionID = (Guid)obj;
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

        public static ICommand Admin
        {
            get
            {
                return _admin ?? (_admin = new RelayCommand(x =>
                {
                    Mediator.Mediator.Notify("GoToAdmin", "");
                }));
            }
        }

        public ICommand SignOut
        {
            get
            {
                return _signOut ?? (_signOut = new RelayCommand(x =>
                {
                    try
                    {
                        ;
                        SQL.StoredProcedures.DestroySession(_sessionID);
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