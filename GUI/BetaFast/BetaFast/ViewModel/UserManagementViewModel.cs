using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using BetaFast.Command;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.ViewModel.Base;

namespace BetaFast.ViewModel
{
    class UserManagementViewModel : ViewModelWithNavigationBarBase
    {
        private ICommand _add;
        private ICommand _delete;
        private ICommand _refresh;

        private string _message;
        private string _messageColor;

        private Role _role;
        private string _username;
        private SecureString _password;
        private string _firstName;
        private string _lastName;

        private ObservableCollection<Account> _usersList;

        private Account _selectedUser;

        public Account SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public ObservableCollection<Account> UsersList
        {
            get { return _usersList; }
            set
            {
                _usersList = value;
                OnPropertyChanged("UsersList");
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value.Copy();
                    OnPropertyChanged("Password");
                }
            }
        }

        public Role Role
        {
            get { return _role; }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged("Role");
                }
            }
        }

        public IEnumerable<Role> Roles
        {
            get
            {
                return Enum.GetValues(typeof(Role))
                    .Cast<Role>();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged("LastName");
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

        private void SetMessage(string text, string color)
        {
            Message = text;
            MessageColor = color;
        }

        public UserManagementViewModel()
        {
            SetMessage(INITIAL, INITIAL_COLOR);
            _password = new SecureString();
            Mediator.Mediator.Subscribe("RefreshUserManagement", this.RefreshUserManagement);
            Mediator.Mediator.Subscribe("GoToLogin", this.ClearPage);
            Mediator.Mediator.Subscribe("OnGoBack", this.ClearPage);
        }

        private void GetAllUsers()
        {
            try
            {
                Task t = Task.Run(async () =>
                {
                    _usersList = await AccountManagement.AccountManagement.GetAllAccountsAsync();
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                if (!t.Wait(ts))
                {
                    Debug.WriteLine("Timeout.");
                    SetMessage("A timeout occurred.", ERROR_COLOR);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                SetMessage("An error occurred retrieving movies.", ERROR_COLOR);
            }
        }

        private void RefreshUserManagement(object obj)
        {
            GetAllUsers();
        }

        private void RemoveUser(Account account)
        {
            UsersList.Remove(account);
        }

        private void RemoveUser(string username)
        {
            for(int i=0; i < UsersList.Count; i++)
            {
                if(UsersList[i].Username.Equals(username))
                {
                    UsersList.RemoveAt(i);
                    break;
                }
            }
        }

        private void AddUser(Account account)
        {
            UsersList.Add(account);
        }

        private void ClearPage(object obj)
        {
            ClearPage();
        }

        private void ClearPage()
        {
            SetMessage(INITIAL, INITIAL_COLOR);
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

        private async Task Register()
        {
            try
            {
                if(Role == Role.Admin)
                {
                    await SecureStringRequests.SecureStringRequests.SecureStringRegistration(Username, FirstName, LastName, Password, AccountManagement.AccountManagement.RegisterAdminAsync);
                }
                else if (Role == Role.User)
                {
                    await SecureStringRequests.SecureStringRequests.SecureStringRegistration(Username, FirstName, LastName, Password, AccountManagement.AccountManagement.RegisterUserAsync);
                }
                else
                {
                    throw new Exception("Invalid Role");
                }

                AddUser(new Account(LastName, FirstName, Username, Role, true));
                ClearPage();
                SetMessage(REGISTERED, SUCCESS_COLOR);
            }
            catch (UserExistsException e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
            catch (ServerException e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
            catch (Exception e)
            {
                ClearForm();
                SetMessage(e.Message, ERROR_COLOR);
            }
        }

        private async Task Deactivate()
        {
            try
            {
                await AccountManagement.AccountManagement.DeleteAsync(SelectedUser.Username);
                RemoveUser(SelectedUser.Username);
                SetMessage(DEACTIVATED, SUCCESS_COLOR);
            }
            catch (UserExistsException e)
            {
                SetMessage(e.Message, ERROR_COLOR);
            }
            catch (ServerException e)
            {
                SetMessage(e.Message, ERROR_COLOR);
            }
            catch (Exception e)
            {
                SetMessage(e.Message, ERROR_COLOR);
            }
        }

        private bool IsFormComplete()
        {
            return ((Password.Length != 0) && (Username != string.Empty) && (FirstName != string.Empty) && (LastName != string.Empty) && ((Role == Role.Admin) || (Role == Role.User)));
        }

        public ICommand Add
        {
            get
            {
                return _add ?? (_add = new RelayCommand(async x =>
                {
                    if (!IsFormComplete())
                    {
                        SetMessage(ERROR, ERROR_COLOR);
                    }
                    else
                    {
                        await Register();
                    }
                }));
            }
        }

        public ICommand Delete
        {
            get
            {
                return _delete ?? (_delete = new RelayCommand(async x =>
                {
                    if (SelectedUser == null)
                    {
                        SetMessage("No account is selected to delete.", ERROR_COLOR);
                    }
                    else
                    {
                        await Deactivate();
                    }
                }));
            }
        }

        public ICommand Refresh
        {
            get
            {
                return _refresh ?? (_refresh = new RelayCommand(x =>
                {
                    GetAllUsers();
                }));
            }
        }

        private const string REGISTERED = "User successfully created.";
        private const string DEACTIVATED = "User successfully deleted.";
        private const string ERROR = "All fields are required.";
        private const string INITIAL = "Enter information for a new user.";

        private const string SUCCESS_COLOR = "green";
        private const string ERROR_COLOR = "red";
        private const string INITIAL_COLOR = "black";
    }
}
