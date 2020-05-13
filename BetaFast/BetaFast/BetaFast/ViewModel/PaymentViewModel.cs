using BetaFast.Command;
using BetaFast.Exceptions;
using BetaFast.Utilities;
using BetaFast.ViewModel.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BetaFast.ViewModel
{
    public class PaymentViewModel : ViewModelWithNavigationBarBase
    {
        private string _nameOnCard;
        private string _cardNumber;
        private string _cvc;
        private string _expiryDate;
        private string _zipCode;

        private string _currentUsername;

        private string _message;

        private decimal _totalPrice;

        private long _cardNumberLong;
        private int _cvcInt;
        private int _zipCodeInt;

        private ICommand _pay;
        private ICommand _delete;
        private ICommand _load;

        private bool _isSelected;

        private bool _isUsernameValid;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public string NameOnCard
        {
            get { return _nameOnCard; }
            set
            {
                if (_nameOnCard != value)
                {
                    _nameOnCard = value;
                    OnPropertyChanged("NameOnCard");
                }
            }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    OnPropertyChanged("CardNumber");
                }
            }
        }

        public string CVC
        {
            get { return _cvc; }
            set
            {
                if (_cvc != value)
                {
                    _cvc = value;
                    OnPropertyChanged("CVC");
                }
            }
        }

        public string ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                if (_expiryDate != value)
                {
                    _expiryDate = value;
                    OnPropertyChanged("ExpiryDate");
                }
            }
        }

        public string ZipCode
        {
            get { return _zipCode; }
            set
            {
                if (_zipCode != value)
                {
                    _zipCode = value;
                    OnPropertyChanged("ZipCode");
                }
            }
        }

        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged("TotalPrice");
                }
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public PaymentViewModel()
        {
            _currentUsername = string.Empty;
            Mediator.Mediator.Subscribe("CurrentUsername", this.SetCurrentUsername);
            Mediator.Mediator.Subscribe("TotalPrice", this.GetTotalPrice);
        }

        private void SetCurrentUsername(object obj)
        {
            _currentUsername = (string)obj;

            if (IsUsernameValid(_currentUsername))
            {
                _isUsernameValid = true;
            }
            else
            {
                _isUsernameValid = false;
            }
        }

        private void LoadPaymentInfo()
        {
            if (_isUsernameValid)
            {
                if (File.Exists(GetPaymentDetailsPath()))
                {
                    try
                    {
                        byte[] paymentDetailsBytes = File.ReadAllBytes(GetPaymentDetailsPath());
                        string[] details = System.Text.Encoding.UTF8.GetString(paymentDetailsBytes).Split('\n');
                        if (details.Length != 5)
                        {
                            Message = "Saved payment details were invalid.";
                            return;
                        }
                        else
                        {
                            NameOnCard = details[0];
                            CardNumber = details[1];
                            ExpiryDate = details[2];
                            CVC = details[3];
                            ZipCode = details[4];
                        }
                    }
                    catch
                    {
                        Message = "An error occurred loading payment information.";
                        return;
                    }
                }
                else
                {
                    Message = "No saved payment details were found.";
                }
            }
            else
            {
                Message = "Payment details could not be retrieved.";
            }
        }

        private void SavePaymentInfo()
        {
            if (_isUsernameValid)
            {
                if (IsFormValid())
                {
                    string contents = NameOnCard + "\n" + CardNumber + "\n" + ExpiryDate + "\n" + CVC + "\n" + ZipCode;
                    try
                    {
                        FileInfo file = new FileInfo(GetPaymentDetailsPath());
                        file.Directory.Create();
                        File.WriteAllBytes(GetPaymentDetailsPath(), System.Text.Encoding.UTF8.GetBytes(contents));
                        FileUtility.SetReadAllUsers(GetPaymentDetailsPath());
                    }
                    catch
                    {
                        Message = "An error occurred saving payment information.";
                    }
                }
            }
            else
            {
                Message = "Payment details could not be saved.";
            }
        }

        private void DeletePaymentInfo()
        {
            if (_isUsernameValid)
            {
                if (File.Exists(GetPaymentDetailsPath()))
                {
                    File.Delete(GetPaymentDetailsPath());
                }
            }
            else
            {
                Message = "Payment details could not be deleted.";
            }
        }

        private bool IsUsernameValid(string username)
        {
            if (!string.IsNullOrEmpty(username) && Security.InputValidation.IsAlphaNumeric(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetPaymentDetailsPath()
        {
            if (_isUsernameValid)
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BetaFast\PaymentDetails\" + _currentUsername + ".txt");
                return fileName;
            }
            else
            {
                Message = "Payment details could not be saved.";
                return string.Empty;
            }
        }

        private void GetTotalPrice(object obj)
        {
            TotalPrice = (decimal)obj;
        }

        private void ClearPage()
        {
            NameOnCard = string.Empty;
            CardNumber = string.Empty;
            CVC = string.Empty;
            ExpiryDate = string.Empty;
            ZipCode = string.Empty;
            Message = string.Empty;
            TotalPrice = 0.00M;
        }

        private bool IsFormValid()
        {
            if ((!string.IsNullOrEmpty(NameOnCard)) && (!string.IsNullOrEmpty(CardNumber)) && (!string.IsNullOrEmpty(CVC)) && (!string.IsNullOrEmpty(ExpiryDate)) && (!string.IsNullOrEmpty(ZipCode)))
            {
                Regex nameRegex = new Regex(@"^[\w\s]*$");
                Regex monthRegex = new Regex(@"^(0[1-9]|1[0-2])$");
                Regex yearRegex = new Regex(@"^(19|20)\d{2}$");

                if (!nameRegex.IsMatch(NameOnCard))
                {
                    Message = "Name is invalid";
                    return false;
                }

                if ((CardNumber.Length < 15) || (CardNumber.Length > 16))
                {
                    Message = "Credit card number length is invalid";
                    return false;
                }

                if (!long.TryParse(CardNumber, out _cardNumberLong))
                {
                    Message = "Credit card number is invalid";
                    return false;
                }

                if (!CVC.Length.Equals(3))
                {
                    Message = "CVC is inavlid";
                    return false;
                }

                if (!Int32.TryParse(CVC, out _cvcInt))
                {
                    Message = "CVC is invalid";
                    return false;
                }

                if (ExpiryDate.Contains("/"))
                {
                    string month = ExpiryDate.Split('/')[0];
                    string year = ExpiryDate.Split('/')[1];

                    if ((!monthRegex.IsMatch(month)) || (!yearRegex.IsMatch(year)))
                    {
                        Message = "Invalid expiration date";
                        return false;
                    }
                    else
                    {
                        if ((!Int32.TryParse(month, out int monthInt)) || (!Int32.TryParse(year, out int yearInt)))
                        {
                            Message = "Invalid expiration date";
                            return false;
                        }
                    }
                }
                else
                {
                    Message = "Invalid expiration date";
                    return false;
                }

                if (!ZipCode.Length.Equals(5))
                {
                    Message = "Zip Code length is invalid";
                    return false;
                }

                if (!Int32.TryParse(ZipCode, out _zipCodeInt))
                {
                    Message = "Zip Code is invalid";
                    return false;
                }

                Message = string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand Load
        {
            get
            {
                return _load ?? (_load = new RelayCommand(x =>
                {
                    LoadPaymentInfo();
                }));
            }
        }

        public ICommand Delete
        {
            get
            {
                return _delete ?? (_delete = new RelayCommand(x =>
                {
                    DeletePaymentInfo();
                }));
            }
        }

        public ICommand Pay
        {
            get
            {
                return _pay ?? (_pay = new RelayCommand(x =>
                {
                    if (IsFormValid())
                    {
                        try
                        {
                            Task t = Task.Run(async () =>
                            {
                                await ShoppingCart.ShoppingCartManagement.CheckoutAsync(NameOnCard, _cardNumberLong, _cvcInt, ExpiryDate, _zipCodeInt, TotalPrice);
                            });
                            TimeSpan ts = TimeSpan.FromMilliseconds(10000);
                            if (!t.Wait(ts))
                            {
                                Debug.WriteLine("Timeout.");
                                MessageBox.Show("A timeout occurred");
                            }
                            if (IsSelected)
                            {
                                SavePaymentInfo();
                            }
                            ClearPage();
                            Mediator.Mediator.Notify("GoToPaymentConfirmation", "");
                        }
                        catch (ArgumentException e)
                        {
                            Debug.WriteLine("Argument exception: " + e.Message);
                            MessageBox.Show("An error occurred");
                        }
                        catch (ServerException)
                        {
                            Debug.WriteLine("A server exception occurred");
                            MessageBox.Show("A server exception occurred");
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("An exception occurred: " + e.Message);
                            MessageBox.Show("An error occurred");
                        }
                    }
                }));
            }
        }
    }
}