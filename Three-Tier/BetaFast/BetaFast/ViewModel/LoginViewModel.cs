using BetaFast.Command;
using BetaFast.Authentication;
using BetaFast.ViewModel.Interfaces;
using System.Windows.Input;
using BetaFast.Model;
using System;
using System.Threading.Tasks;
using System.Security;
using BetaFast.Exceptions;
using BetaFast.Utilities;
using System.Security.Cryptography;
using System.IO;

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

        byte[] Key = new byte[] { 0xB2, 0xA2, 0x53, 0x5F, 0x15, 0x04, 0xAD, 0x8C, 0x33, 0xE1, 0x3F, 0xFA, 0x4C, 0xA0, 0xA4, 0xC5 };
        byte[] IV = new byte[] { 0xB2, 0xA2, 0x53, 0x5F, 0x15, 0x04, 0xAD, 0x8C, 0x33, 0xE1, 0x3F, 0xFA, 0x4C, 0xA0, 0xA4, 0xC5 };

        private bool _isSelected;

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
            _credentials = new Creds
            {
                Username = (string)Microsoft.Win32.Registry.GetValue(credentialsKey, "Username", ""),
                Password = Utilities.SecureStringUtility.StringToSecureString((string)Microsoft.Win32.Registry.GetValue(credentialsKey, "Password", ""))
            };

            _isSelected = false;
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
            string adminPassword = "EIqEWJIODv9easB6iQybuQ==";
            try
            {
                if (Username.Equals("betafastadmin") && Encrypt(SecureStringUtility.SecureStringToString(Password)).Equals(adminPassword))
                {
                    return true;
                }
                else
                {
                    return await Logon.IsAdmin();
                }
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
            catch (Exception e)
            {
                ClearForm();
                MessageColor = "Red";
                Message = ROLE_ERROR;
                throw new Exception();
            }
        }

        private string Encrypt(string plaintext)
        {
            byte[] ciphertextBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                        ciphertextBytes = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(ciphertextBytes);
        }

        private string Decrypt(string ciphertext)
        {
            string plaintext = string.Empty;
            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(ciphertextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private void ClearForm()
        {
            if (!IsSelected)
            {
                Username = string.Empty;
                Password.Dispose();
                Password = new SecureString();
            }
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
							if (IsSelected)
                                {
                                    // Set registry with username and password
                                    Microsoft.Win32.Registry.SetValue(credentialsKey, "Username", Username, Microsoft.Win32.RegistryValueKind.String);
                                    Microsoft.Win32.Registry.SetValue(credentialsKey, "Password", Utilities.SecureStringUtility.SecureStringToString(Password), Microsoft.Win32.RegistryValueKind.String);
                                }
                                else
                                {
                                    // Clear registry
                                    Microsoft.Win32.Registry.SetValue(credentialsKey, "Username", "", Microsoft.Win32.RegistryValueKind.String);
                                    Microsoft.Win32.Registry.SetValue(credentialsKey, "Password", "", Microsoft.Win32.RegistryValueKind.String);
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
	private const string localRoot = "HKEY_CURRENT_USER";
        private const string subkey = "BetaFast";
        private const string credentialsKey = localRoot + "\\" + subkey + "\\" + "Credentials";

    }
}
