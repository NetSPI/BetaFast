using BetaFast.Command;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.ViewModel.Interfaces;
using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BetaFast.ViewModel
{
    class RegisterViewModel : ViewModelBase, IViewModel
    {
        private ICommand _back;
        private ICommand _submitForm;
        private readonly Registration _registration;
        private string _errorMessage;
        private string _successMessage;
        private string _quote;

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
        }

        public RegisterViewModel()
        {
            Mediator.Mediator.Subscribe("GoToRegister", this.GetQuote);
            _registration = new Registration {
                Username = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Password = new SecureString()
            };

            // Default quote
            _quote = "\"I'm having a BetaBlast with BetaFast\" - Eric Gruber, Security Consultant of the Stars";
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
            try
            {
                Task t = Task.Run(async () =>
                {
                    _quote = await Content.RegistrationQuotes.GetQuoteAsync();
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(1000);
                if (!t.Wait(ts))
                    Debug.WriteLine("Timeout.");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task Register()
        {
            try
            {
                await SecureStringRequests.SecureStringRequests.SecureStringRegistration(Username, FirstName, LastName, Password, AccountManagement.AccountManagement.RegisterUserAsync);
                ClearPage();
                SuccessMessage = SUCCESS;
            }
            catch (UserExistsException e)
            {
                ClearForm();
                SuccessMessage = string.Empty;
                ErrorMessage = e.Message;
            }
            catch (ServerException e)
            {
                ClearForm();
                SuccessMessage = string.Empty;
                ErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                ClearForm();
                SuccessMessage = string.Empty;
                ErrorMessage = e.Message;
            }
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
                return _submitForm ?? (_submitForm = new RelayCommand(async x =>
                {
                    if (!IsFormComplete())
                    {
                        ErrorMessage = INCOMPLETE;
                    }
                    else
                    {
                        if (Security.InputValidation.IsAlphaNumeric(Username))
                        {
                            await Register();
                        }
                        else
                        {
                            ErrorMessage = "Username must only contain alphanumeric characters.";
                        }
                    }
                }));
            }
        }

        private const string SUCCESS = "Thanks for joining!";
        private const string INCOMPLETE = "All fields are required.";
    }
}
