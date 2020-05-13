using System.Windows.Input;
using System;
using System.Security;
using BetaBank.ViewModel.Interfaces;
using BetaBank.Model;
using BetaBank.Command;
using System.Data.SqlClient;
using BetaBank.Utilities;

namespace BetaBank.ViewModel
{
    public class LoginViewModel : ViewModelBase, IViewModel
    {
        private ICommand _submitCredentials;
        private ICommand _register;
        private Creds _credentials;
        private string _message;
        private string _messageColor;

        byte[] Key = new byte[] { 0x72, 0x65, 0x70, 0x6c, 0x61, 0x63, 0x65, 0x72, 0x72, 0x65, 0x70, 0x6c, 0x61, 0x63, 0x65, 0x72 };

        private Guid _sessionID;

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
            _credentials = new Creds();
            Password = new SecureString();
            Mediator.Mediator.Subscribe("ClearLogin", this.ClearPage);
            Mediator.Mediator.Subscribe("SessionID", this.SetSessionID);
        }

        private void SetSessionID(object obj)
        {
            _sessionID = (Guid)obj;
        }

        private void ClearPage(object obj)
        {
            ClearPage();
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

        private bool IsAdmin()
        {
            string adminPassword = "IRUCBQ8EVEtKVVFsYWNlcg==";

            if (Username.Equals("The_Chairman") && Security.BetaEncryption.Encrypt(Key, SecureStringUtility.SecureStringToString(Password)).Equals(adminPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand SubmitCredentials
        {
            get
            {
                return _submitCredentials ?? (_submitCredentials = new RelayCommand(x =>
                {
                    if (!IsFormComplete())
                    {
                        MessageColor = "Red";
                        Message = INCOMPLETE;
                    }
                    else
                    {
                        LoginStoredProcedure();
                        if (_sessionID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            if (IsAdmin())
                            {
                                Mediator.Mediator.Notify("SetAdmin", "Visible");
                            }
                            else
                            {
                                Mediator.Mediator.Notify("SetAdmin", "Hidden");
                            }
                            Mediator.Mediator.Notify("GoToHome", "");
                            Mediator.Mediator.Notify("Username", Username);
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

        private void LoginStoredProcedure()
        {
            try
            {
                string encryptedPassword = Security.BetaEncryption.Encrypt(Key, SecureStringUtility.SecureStringToString(Password));
                SQL.StoredProcedures.Login(Username, encryptedPassword);
            }
            catch (SqlException e)
            {
                int errorNumber = e.Number;
                if (errorNumber >= 50000)
                {
                    Message = e.Message;
                }
                else
                {
                    Message = UNKNOWN;
                }
                MessageColor = "red";
            }
            catch (Exception)
            {
                Message = UNKNOWN;
                MessageColor = "red";
            }
        }

        private const string INCOMPLETE = "A username and password are required.";
        private const string UNKNOWN = "An unknown error occurred.";
    }
}