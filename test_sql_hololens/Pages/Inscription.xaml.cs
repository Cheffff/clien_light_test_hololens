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

using System.Threading;
using System.Threading.Tasks;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace test_sql_hololens.Page
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Inscription// : Page
    {
        public Inscription()
        {
            this.InitializeComponent();
        }
        Params param;
        private string client_id = "236d67f77afbc1d";
        AutoResetEvent waitForNavComplete;

            private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Uri requestUri = new Uri("http://virtualdeck.azurewebsites.net/");

            // Show the pop up
            HomeContent.Visibility = Visibility.Collapsed;
            MyWebView.Visibility = Visibility.Visible;

            MyWebView.Navigate(requestUri);

            await Task.Run(() => { waitForNavComplete.WaitOne(); });

            waitForNavComplete.Reset();
            MyWebView.Visibility = Visibility.Collapsed;
            HomeContent.Visibility = Visibility.Visible;
        }

        private void MyWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess == true && args.Uri == new Uri("http://conan.akylonia.net/perso.html"))
            {
                waitForNavComplete.Set();

                String[] Results = args.Uri.ToString().Substring(args.Uri.ToString().IndexOf('#') + 1).Split('&');
                string access_token, refresh_token, expires_in, token_type, account_username, account_id;
                access_token = refresh_token = expires_in = token_type = account_username = account_id = null;

                for (int i = 0; i < Results.Length; i++)
                {
                    String[] splits = Results[i].Split('=');
                    switch (splits[0])
                    {
                        case "access_token":
                            access_token = splits[1];
                            break;
                        case "refresh_token":
                            refresh_token = splits[1];
                            break;
                        case "expires_in":
                            expires_in = splits[1];
                            break;
                        case "token_type":
                            token_type = splits[1];
                            break;
                        case "account_username":
                            account_username = splits[1];
                            break;
                        case "account_id":
                            account_id = splits[1];
                            break;
                    }
                }
                if (access_token != null)
                {
                    param = new Params { access_token = access_token, refresh_token = refresh_token, expires_in = expires_in, token_type = token_type, account_username = account_username, account_id = account_id };
                    this.Frame.Navigate(typeof(MainPage), param);
                }
                else
                {
                    textResponse.Text = "Failed to connect.";
                }
            }
            else
            {
                textResponse.Text = "Failed to connect (" + args.WebErrorStatus.ToString() + ").";
            }
        }

        private void MyWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            textResponse.Text = "Failed to navigate.";
        }


        class Params
        {

            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }
            public string account_username { get; set; }
            public string account_id { get; set; }
        }
    }
}
