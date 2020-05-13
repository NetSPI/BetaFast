using BetaBank.Command;
using BetaBank.Model;
using BetaBank.ViewModel.Interfaces;
using System;
using System.Data.SqlClient;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BetaBank.ViewModel
{
    class RegisterViewModel : ViewModelBase, IViewModel
    {
        private ICommand _back;
        private ICommand _submitForm;
        private readonly Registration _registration;
        private string _errorMessage;
        private string _successMessage;
        private string _quote;

        byte[] Key = new byte[] { 0x72, 0x65, 0x70, 0x6c, 0x61, 0x63, 0x65, 0x72, 0x72, 0x65, 0x70, 0x6c, 0x61, 0x63, 0x65, 0x72 };

        public int PreviousPageIndex
        {
            get;
            set;
        }

        public SecureString Password
        {
            get { return _registration.Password; }
            set
            {
                if (_registration.Password != value)
                {
                    _registration.Password = value.Copy();
                    OnPropertyChanged("Password");
                }
            }
        }

        public string Username
        {
            get { return _registration.Username; }
            set
            {
                if (_registration.Username != value)
                {
                    _registration.Username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string FirstName
        {
            get { return _registration.FirstName; }
            set
            {
                if (_registration.FirstName != value)
                {
                    _registration.FirstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string LastName
        {
            get { return _registration.LastName; }
            set
            {
                if (_registration.LastName != value)
                {
                    _registration.LastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                _successMessage = value;
                OnPropertyChanged("SuccessMessage");
            }
        }

        public string Quote
        {
            get { return _quote; }
            set
            {
                _quote = value;
                OnPropertyChanged("Quote");
            }
        }

        public RegisterViewModel()
        {
            Mediator.Mediator.Subscribe("GoToRegister", this.GetQuote);
            _registration = new Registration
            {
                Username = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Password = new SecureString()
            };

            // Default quote
            Quote = @"You're the type of business person who doesn't even need to know how business works. Because you figured it all out.";
        }

        private void ClearPage()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password.Dispose();
            Password = new SecureString();
        }

        private void ClearForm()
        {
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password.Dispose();
            Password = new SecureString();
        }

        private void GetQuote(object obj)
        {
            Quote = Quotes.Quotes.GetQuote();
        }

        private bool IsFormComplete()
        {
            return ((Password.Length != 0) && (Username != string.Empty) && (FirstName != string.Empty) && (LastName != string.Empty));
        }

        public ICommand Back
        {
            get
            {
                return _back ?? (_back = new RelayCommand(x =>
                {
                    ClearPage();
                    Mediator.Mediator.Notify("GoBack", "");
                }));
            }
        }

        public ICommand SubmitForm
        {
            get
            {
                return _submitForm ?? (_submitForm = new RelayCommand(x =>
                {
                    if (!IsFormComplete())
                    {
                        ErrorMessage = INCOMPLETE;
                    }
                    else
                    {
                        CreateUserStoredProcedure();
                    }
                }));
            }
        }

        private void CreateUserStoredProcedure()
        {
            try
            {
                string encryptedPassword = Security.BetaEncryption.Encrypt(Key, Utilities.SecureStringUtility.SecureStringToString(Password));
                SQL.StoredProcedures.CreateUser(LastName, FirstName, Username, encryptedPassword);
                ClearPage();
                SuccessMessage = SUCCESS;
            }
            catch (SqlException e)
            {
                int errorNumber = e.Number;
                if (errorNumber >= 50000)
                {
                    ErrorMessage = e.Message;
                }
                else
                {
                    ErrorMessage = "An error occurred.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred.";
            }
        }

        private const string SUCCESS = "Thanks for joining!";
        private const string INCOMPLETE = "All fields are required.";
    }
}