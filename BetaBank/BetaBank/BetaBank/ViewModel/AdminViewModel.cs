using BetaBank.Command;
using BetaBank.ViewModel.Interfaces;
using BetaBank.ViewModel.Base;
using System;
using System.Data.SqlClient;
using System.Windows.Input;

namespace BetaBank.ViewModel
{
    class AdminViewModel : ViewModelWithNavigationBar, IViewModel
    {
        private ICommand _yes;
        private ICommand _no;
        private string _address;
        private string _addressColor;

        private Guid _sessionID;

        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        public string AddressColor
        {
            get { return _addressColor; }
            set
            {
                if (_addressColor != value)
                {
                    _addressColor = value;
                    OnPropertyChanged("AddressColor");
                }
            }
        }

        public AdminViewModel()
        {
            Mediator.Mediator.Subscribe("SessionID", this.SessionID);
        }

        private void SessionID(object obj)
        {
            ClearPage();
            _sessionID = (Guid)obj;
        }

        private void ClearPage()
        {
            Address = string.Empty;
            AddressColor = "black";
        }

        public ICommand Yes
        {
            get
            {
                return _yes ?? (_yes = new RelayCommand(x =>
                {
                    try
                    {
                        AddressColor = "black";
                        Address = SQL.StoredProcedures.GetVaultLocation(_sessionID);
                    }
                    catch (SqlException e)
                    {
                        string errorMessage = e.Message;
                        int errorNumber = e.Number;
                        if (errorNumber >= 50000)
                        {
                            AddressColor = "red";
                            Address = e.Message;
                        }
                        else
                        {
                            AddressColor = "red";
                            Address = "An error occurred.";
                        }
                    }
                    catch (Exception)
                    {
                        AddressColor = "red";
                        Address = "An error occurred.";
                    }
                }));
            }
        }

        public ICommand No
        {
            get
            {
                return _no ?? (_no = new RelayCommand(x =>
                {
                    AddressColor = "black";
                    Address = NOT_TODAY;
                }));
            }
        }

        private const string NOT_TODAY = "You are most welcome.";
    }
}