using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using BetaFast.Command;
using BetaFast.ViewModel.Base;

namespace BetaFast.ViewModel
{
    class SettingsViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _update;

        private SecureString _currentPassword;
        private SecureString _newPassword;
        private SecureString _confirmPassword;

        private string _message;
        private string _messageColor;

        public SecureString CurrentPassword
        {
            get { return _currentPassword; }
            set
            {
                if (_currentPassword != value)
                {
                    _currentPassword = value.Copy();
                    OnPropertyChanged("CurrentPassword");
                }
            }
        }

        public SecureString NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value.Copy();
                    OnPropertyChanged("NewPassword");
                }
            }
        }

        public SecureString ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value.Copy();
                    OnPropertyChanged("ConfirmPassword");
                }
            }
        }

        public SettingsViewModel()
        {
            _currentPassword = new SecureString();
            _newPassword = new SecureString();
            _confirmPassword = new SecureString();
            Mediator.Mediator.Subscribe("GoToLogin", this.ClearPage);
            Mediator.Mediator.Subscribe("OnGoBack", this.ClearPage);
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

        private void SetMessage(string text, string color)
        {
            Message = text;
            MessageColor = color;
        }

        private void ClearPage(object obj)
        {
            ClearPage();
        }

        private void ClearForm()
        {
            CurrentPassword.Dispose();
            CurrentPassword = new SecureString();
            NewPassword.Dispose();
            NewPassword = new SecureString();
            ConfirmPassword.Dispose();
            ConfirmPassword = new SecureString();
        }

        private void ClearPage()
        {
            CurrentPassword.Dispose();
            CurrentPassword = new SecureString();
            NewPassword.Dispose();
            NewPassword = new SecureString();
            ConfirmPassword.Dispose();
            ConfirmPassword = new SecureString();
            SetMessage(INITIAL, INITIAL_COLOR);
        }

        private bool IsFormComplete()
        {
            return ((CurrentPassword.Length != 0) && (NewPassword.Length != 0) && (ConfirmPassword.Length != 0));
        }

        private async Task UpdatePassword()
        {
            try
            {
                await SecureStringRequests.SecureStringRequests.SecureStringUpdatePassword(CurrentPassword, NewPassword, ConfirmPassword, AccountManagement.AccountManagement.UpdatePassword);
                ClearForm();
                SetMessage(SUCCESS, SUCCESS_COLOR);
            }
            catch (Exception e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
        }

        public ICommand Update
        {
            get
            {
                return _update ?? (_update = new RelayCommand(async x =>
                {
                    if (IsFormComplete())
                    {
                        await UpdatePassword();
                    }
                    else
                    {
                        SetMessage(FIELDS_ERROR, ERROR_COLOR);
                    }
                }));
            }
        }

        private const string FIELDS_ERROR = "All fields are required.";
        private const string SUCCESS = "Password successfully updated.";
        private const string INITIAL = "";

        private const string SUCCESS_COLOR = "green";
        private const string ERROR_COLOR = "red";
        private const string INITIAL_COLOR = "black";
    }
}
