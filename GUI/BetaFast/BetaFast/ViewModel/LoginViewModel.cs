using BetaFast.Command;
using BetaFast.Authentication;
using BetaFast.ViewModel.Interfaces;
using System.Windows.Input;
using BetaFast.Model;
using System;
using System.Threading.Tasks;
using System.Security;
using BetaFast.Exceptions;

namespace BetaFast.ViewModel
{
    public class LoginViewModel : ViewModelBase, IViewModel
    {
        private ICommand _submitCredentials;
        private ICommand _register;
        private Creds _credentials;
        private int _incorrectLoginAttempts;
        private string _message;
        private string _messageColor;

        public SecureString Password
        {
            get { return _credentials.Password; }
            set
            {
                if (_credentials.Password != value)
                {
                    _credentials.Password = value.Copy();
                    OnPropertyChanged("Password");
                }
            }
        }

        public string Username
        {
            get { return _credentials.Username; }
            set
            {
                if (_credentials.Username != value)
                {
                    _credentials.Username = value;
                    OnPropertyChanged("Username");
                }
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

        public LoginViewModel()
        {
            _credentials = new Creds { Username = string.Empty, Password = new SecureString() };
            _incorrectLoginAttempts = 0;
            Mediator.Mediator.Subscribe("ClearLogin", this.ClearPage);
        }

        private void ClearPage(object obj)
        {
            ClearPage();
        }

        // This will only verify the status code. A user can be redirected to the post-login page by modifying the response. 
        // However, this will only present authorization bypasses for which View is loaded and not any data returned from the
        // server nor any modification sent to the server.
        private async Task<bool> IsLoggedIn()
        {
            try
            {
                await SecureStringRequests.SecureStringRequests.SecureStringLogin(Username, Password, Logon.Login);
                return true;
            }
            catch (ServerException e)
            {
                ClearForm();
                MessageColor = "Red";
                Message = e.Message;
                return false;
            }
            catch (LoginException e)
            {
                _incorrectLoginAttempts++;
                ClearForm();
                MessageColor = "Red";
                Message = e.Message;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ClearForm();
                MessageColor = "Red";
                Message = UNKNOWN;
                return false;
            }
        }

        private async Task<bool> IsAdmin()
        {
            try
            {
                return await Logon.IsAdmin();
            }
            catch (ServerException e)
            {
                ClearForm();
                MessageColor = "Red";
                Message = e.Message;
                throw new Exception();
            }
            catch (UserDoesNotExistException)
            {
                ClearForm();
                MessageColor = "Red";
                Message = ROLE_ERROR;
                throw new Exception();
            }
            catch (Exception)
            {
                ClearForm();
                MessageColor = "Red";
                Message = ROLE_ERROR;
                throw new Exception();
            }
        }

        private void ClearForm()
        {
            Username = string.Empty;
            Password.Dispose();
            Password = new SecureString();
        }

        private void ClearPage()
        {
            ClearForm();
            Message = string.Empty;
        }

        private bool IsFormComplete()
        {
            return ((Password.Length != 0) && (Username != string.Empty));
        }

        public ICommand SubmitCredentials
        {
            get
            {
                return _submitCredentials ?? (_submitCredentials = new RelayCommand(async x =>
                {
                    if (!IsFormComplete())
                    {
                        MessageColor = "Red";
                        Message = INCOMPLETE;
                    }
                    else
                    {
                        if (await IsLoggedIn())
                        {
                            try
                            {
                                if (await IsAdmin())
                                {
                                    Mediator.Mediator.Notify("SetAdmin", "Visible");
                                }
                                else
                                {
                                    Mediator.Mediator.Notify("SetAdmin", "Hidden");
                                }
                                ClearPage();
                                MessageColor = "Green";
                                Message = "Success! Loading movies . . .";
                                Mediator.Mediator.Notify("GoToHome", "");
                            }
                            catch
                            {
                                // Role could not be determined
                                MessageColor = "Red";
                                Message = "An unknown error occurred.";
                                await Logout.LogoutAsync();
                            }
                        }
                    }
                }));
            }
        }

        public ICommand Register
        {
            get
            {
                return _register ?? (_register = new RelayCommand(x =>
                {
                    ClearPage();
                    Mediator.Mediator.Notify("GoToRegister", "");
                }));
            }
        }

        private const string INCOMPLETE = "A username and password are required.";
        private const string UNKNOWN = "An unknown error occurred.";
        private const string ROLE_ERROR = "Error: Role could not be determined.";
    }
}