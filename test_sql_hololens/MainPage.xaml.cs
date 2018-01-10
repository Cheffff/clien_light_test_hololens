using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using test_sql_hololens.Page;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace test_sql_hololens
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>

    public sealed partial class MainPage //: Page
    {

        private const int PBKDF2IterCount = 1000; // default for Rfc2898DeriveBytes
        private const int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private const int SaltSize = 128 / 8; // 128 bits
        //private Account  _account;
       // private UserAccount _account;

        private bool _isExistingLocalAccount;

        public MainPage()
        {
            //this.InitializeComponent();
            this.InitializeComponent();
            //Loaded += MainPage_Loaded;
        }
       

        private void PassportSignInButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            SignInPassportAsync();
        }

 
        private void ForgetButtonTextBlock_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            Frame.Navigate(typeof(Inscription));
        }


        private void RegisterButtonTextBlock_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            Frame.Navigate(typeof(help));
        }
        private async void SignInPassportAsync()
        {
            string email = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (HasUser(email, password))
            {

                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["loginSessionKey"] = email;
                Frame.Navigate(typeof(Menu));
            }
            else
            {
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
            }


            //if (_isExistingLocalAccount)
            //{
            //    if (await MicrosoftPassportHelper.GetPassportAuthenticationMessageAsync(_account))
            //    {
            //        Frame.Navigate(typeof(Welcome), _account);
            //        //Frame.Navigate(typeof(Menu), _account);
            //    }
            //}
            //else if (AuthService.AuthService.Instance.ValidateCredentials(UsernameTextBox.Text, PasswordBox.Password))
            //{
            //    Guid userId = AuthService.AuthService.Instance.GetUserId(UsernameTextBox.Text);

            //    if (userId != Guid.Empty)
            //    {
            //        bool isSuccessful = await MicrosoftPassportHelper.CreatePassportKeyAsync(userId, UsernameTextBox.Text);
            //        if (isSuccessful)
            //        {
            //            Debug.WriteLine("Successfully signed in with Windows Hello!");
            //            _account = AuthService.AuthService.Instance.GetUserAccount(userId);
            //            Frame.Navigate(typeof(Welcome), _account);
            //        }
            //        else
            //        {
            //            AuthService.AuthService.Instance.PassportRemoveUser(userId);

            //            ErrorMessage.Text = "Account Creation Failed";
            //        }
            //    }
            //}
            //else
            //{
            //    ErrorMessage.Text = "Invalid Credentials";
            //}
        }

        public bool HasUser(string email, string password)
        {
            string GetUsersQuery = "SELECT [PasswordHash], [UserName] FROM [dbo].[AspNetUsers] WHERE [Email] = '" + email + "'";

            try
            {
                using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
                {
                    // Open connection
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetUsersQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string hashedPassword = reader.GetString(0);
                                    if (VerifyHashedPassword(hashedPassword, password))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        ErrorMessage.Text = "Username or password was incorrect!";
                                        return false;
                                    }
                                }
                                else
                                {
                                    ErrorMessage.Text = "No such account exists!";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return false;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) text hash.

            if (hashedPasswordBytes.Length != (1 + SaltSize + PBKDF2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
            {
                // Wrong length or version header.
                return false;
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            var storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2IterCount))
            {
                generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);
            }
            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

    }
}