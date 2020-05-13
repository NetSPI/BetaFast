using BetaBank.Command;
using BetaBank.ViewModel.Interfaces;
using BetaBank.ViewModel.Base;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace BetaBank.ViewModel
{
    class HomeViewModel : ViewModelWithNavigationBar, IViewModel
    {
        private ICommand _yes;
        private ICommand _no;
        private ICommand _withdraw;
        private string _welcome;
        private string _balanceAmount;
        private string _balanceColor;
        private string _withdrawAmount;
        private string _remainingBalance;
        private string _remainingBalanceColor;
        private string _username;

        private Guid _sessionID;

        public string Welcome
        {
            get { return _welcome; }
            set
            {
                if (_welcome != value)
                {
                    _welcome = value;
                    OnPropertyChanged("Welcome");
                }
            }
        }

        public string BalanceAmount
        {
            get { return _balanceAmount; }
            set
            {
                if (_balanceAmount != value)
                {
                    _balanceAmount = value;
                    OnPropertyChanged("BalanceAmount");
                }
            }
        }

        public string BalanceColor
        {
            get { return _balanceColor; }
            set
            {
                if (_balanceColor != value)
                {
                    _balanceColor = value;
                    OnPropertyChanged("BalanceColor");
                }
            }
        }

        public string WithdrawAmount
        {
            get { return _withdrawAmount; }
            set
            {
                if (_withdrawAmount != value)
                {
                    _withdrawAmount = value;
                    OnPropertyChanged("WithdrawAmount");
                }
            }
        }

        public string RemainingBalance
        {
            get { return _remainingBalance; }
            set
            {
                if (_remainingBalance != value)
                {
                    _remainingBalance = value;
                    OnPropertyChanged("RemainingBalance");
                }
            }
        }

        public string RemainingBalanceColor
        {
            get { return _remainingBalanceColor; }
            set
            {
                if (_remainingBalanceColor != value)
                {
                    _remainingBalanceColor = value;
                    OnPropertyChanged("RemainingBalanceColor");
                }
            }
        }

        public HomeViewModel()
        {
            Mediator.Mediator.Subscribe("GoToHome", this.RefreshHome);
            Mediator.Mediator.Subscribe("SessionID", this.GetSessionID);
            Mediator.Mediator.Subscribe("Username", this.GetUsername);
        }

        private void RefreshHome(object obj)
        {
            ClearPage();
        }

        private void GetUsername(object obj)
        {
            _username = (string)obj;
            SetWelcomeMessage(_username);
        }

        private void GetSessionID(object obj)
        {
            _sessionID = (Guid)obj;
        }

        private void SetWelcomeMessage(string username)
        {
            Welcome = "Welcome, " + username;
        }

        private void ClearPage()
        {
            Welcome = string.Empty;
            BalanceAmount = string.Empty;
            BalanceColor = "black";
            WithdrawAmount = string.Empty;
            RemainingBalance = FEATURE_DISABLED;
            RemainingBalanceColor = "red";
            _username = string.Empty;
        }

        public ICommand Yes
        {
            get
            {
                return _yes ?? (_yes = new RelayCommand(x =>
                {
                    try
                    {
                        string balance = SQL.StoredProcedures.GetBalance(_sessionID);
                        if (balance.Contains("-"))
                        {
                            BalanceColor = "red";
                        }
                        else if (balance[1].Equals('0'))
                        {
                            BalanceColor = "black";
                        }
                        else
                        {
                            BalanceColor = "green";
                        }

                        BalanceAmount = balance;
                    }
                    catch (SqlException e)
                    {
                        string errorMessage = e.Message;
                        int errorNumber = e.Number;
                        if (errorNumber >= 50000)
                        {
                            MessageBox.Show(e.Message);
                        }
                        else
                        {
                            MessageBox.Show("An error occurred.");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error occurred.");
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
                    BalanceColor = "black";
                    BalanceAmount = CERTAINLY_NOT;
                }));
            }
        }

        public ICommand Withdraw
        {
            get
            {
                return _withdraw ?? (_withdraw = new RelayCommand(x =>
                {
                    if (!string.IsNullOrEmpty(WithdrawAmount) && IsValidMoney(WithdrawAmount))
                    {
                        try
                        {
                            // Jim - my son said your code didn't make sense. He wrote his own WithdrawBalance, and I thought it was better. What even is a stored procedure?
                            // I took the liberty of commenting out your "code"
                            //SQL.StoredProcedures.WithdrawBalance(decimal.Parse(WithdrawAmount), _sessionID);
                            SQL.StoredProcedures.WithdrawBalance(decimal.Parse(WithdrawAmount), _username);
                            RemainingBalanceColor = "green";
                            RemainingBalance = "Your remaining funds amount to " + SQL.StoredProcedures.GetBalance(_sessionID) + ".";
                        }
                        catch (SqlException e)
                        {
                            string errorMessage = e.Message;
                            int errorNumber = e.Number;
                            if (errorNumber >= 50000)
                            {
                                RemainingBalance = e.Message;
                            }
                            else
                            {
                                RemainingBalance = e.Message;
                            }
                            RemainingBalanceColor = "red";
                        }
                        catch (Exception)
                        {
                            RemainingBalance = "An error occurred.";
                            RemainingBalanceColor = "red";
                        }
                    }
                    else
                    {
                        RemainingBalance = "Invalid input format";
                        RemainingBalanceColor = "red";
                    }
                }));
            }
        }

        private bool IsValidMoney(string money)
        {
            return decimal.TryParse(money, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out decimal num);
        }

        private const string FEATURE_DISABLED = "This feature is under construction! We apologize for the inconvenience.";
        private const string CERTAINLY_NOT = "Our sincerest apologies!";
    }
}