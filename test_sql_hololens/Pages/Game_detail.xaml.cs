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
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

using Windows.UI.Popups;
using System.Collections.ObjectModel;
using Windows.Networking.BackgroundTransfer;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading;
using Windows.System;
using MySql.Data.MySqlClient;
using System.Data.Common;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace test_sql_hololens.Page
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Game_detail //: Page
    {
    
        public string _entityId;
        public int int_id;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _entityId = e.Parameter as string;
            int_id = int.Parse(_entityId);
            Getgame((App.Current as App).ConnectionString);
            title.Text = jeu.Titre;

        }
        /*
        private void toto(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DL_chess), jeu.url); // a delete avec le chargement direct dans la page
        }
        */
        public class Game
        {
            public string Titre { get; set; }
            public string description { get; set; }
            public string url { get; set; }



        }
        Game jeu = new Game();

        ///get data
        ///


        public Game Getgame(string connectionString)
        {
            // string GetUsersQuery = "select [name], [description] from [dbo].[games] where [id_game] = "+ int_id;
            string GetUsersQuery = "select [name], [description], [url_game] from [dbo].[games] where [id_game] ='" + int_id + "'";
            //GetUsersQuery.Parameters.Add(new SqlParameter("@int_id", int_id));


            Debug.WriteLine(GetUsersQuery);

            Debug.WriteLine("apres requete");



            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Open connection
                    Debug.WriteLine("try");

                    conn.Open();
                    Debug.WriteLine("open");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetUsersQuery;
                            Debug.WriteLine(cmd.CommandText);

                            //cmd.Parameters.Add(new SqlParameter("@int_id", _entityId));
                            Debug.WriteLine("using 1");
                            Debug.WriteLine(_entityId);



                            using (SqlDataReader reader = cmd.ExecuteReader())

                            {
                                Debug.WriteLine("using 2");

                                // read through all rows
                                while (reader.Read())
                                {
                                    Debug.WriteLine("reader");
                                    jeu.Titre = reader.GetString(0);
                                    jeu.description = reader.GetString(1);
                                    jeu.url = reader.GetString(2);
                                    Debug.WriteLine("requete sql result");
                                    Debug.WriteLine(jeu.Titre);
                                }
                            }
                        }
                    }
                }
                return jeu;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }


        public Game_detail()
        {
            this.InitializeComponent();
            //Getgame((App.Current as App).ConnectionString);

            //title.SetBinding(TextBlock.TextProperty, "fuck yout");
            //title.Text = jeu.Titre;
            //StudentsList.ItemsSource = jeu;
        }


        /// <summary>
        /// Menu slipt

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuButton1_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Menu));
        }

        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(Welcome));
        }



        /// End Split
        /// Telechargement
        /// Start
        /// 


        string fileName = "UWP.zip"; // a changer en fonction de la bdd
        string urlName = "http://eip.epitech.eu/2018/virtualdeck/Epicture-UWP-master.zip";


        DownloadOperation downloadOperation;
        CancellationTokenSource cancellationToken;
        BackgroundDownloader backgroundDownloader = new BackgroundDownloader();

        public StorageFolder folder;

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            FolderPicker folderPicker = new FolderPicker();
            folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.Thumbnail;
            folderPicker.FileTypeFilter.Add("*");

            // StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            folder = await folderPicker.PickSingleFolderAsync();

            //  Debug.WriteLine(folder.Path);

            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                downloadOperation = backgroundDownloader.CreateDownload(new Uri(urlName), file);
                Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);
                cancellationToken = new CancellationTokenSource();
                btnDownload.IsEnabled = false;
                btnCancel.IsEnabled = true;
                btnPauseResume.IsEnabled = true;

                Debug.WriteLine("file");
                Debug.WriteLine(file.Path);
                Debug.WriteLine(file.Name);
                Debug.WriteLine(file.FolderRelativeId);






                try
                {
                    txtStatus.Text = "Initializing...";
                    await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                }
                catch (TaskCanceledException)
                {
                    txtStatus.Text = "Download cancelled";
                    await downloadOperation.ResultFile.DeleteAsync();
                    btnPauseResume.Content = "Resume";
                    btnCancel.IsEnabled = false;
                    btnPauseResume.IsEnabled = false;
                    btnDownload.IsEnabled = true;
                    downloadOperation = null;

                }
            }

        }

        /// <summary>
        /// Check folder file fonction
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private async Task<bool> IsFolderExists(string folderName)
        {
            try
            {
                StorageFolder file = await ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public async Task<bool> isFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        /// <summary>
        /// END
        /// </summary>



        private async void openfolder(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(Welcome));
            //await Windows.System.Launcher.LaunchFileAsync(folder);
            await Launcher.LaunchFolderAsync(folder);
            //await Windows.System.Launcher.LaunchFolderAsync(folder);

        }

        private void btnPauseResume_Click(object sender, RoutedEventArgs e)
        {
            if (btnPauseResume.Content.ToString().ToLower().Equals("pause"))
            {
                try
                {
                    downloadOperation.Pause();
                }
                catch (InvalidOperationException)
                {

                }
            }
            else
            {
                try
                {
                    downloadOperation.Resume();
                }
                catch (InvalidOperationException)
                { }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<DownloadOperation> downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            if (downloads.Count > 0)
            {
                downloadOperation = downloads.First();
                cancellationToken = new CancellationTokenSource();
                Progress<DownloadOperation> progress = new Progress<DownloadOperation>(progressChanged);
                btnDownload.IsEnabled = false;
                btnCancel.IsEnabled = true;
                btnPauseResume.IsEnabled = true;
                try
                {
                    txtStatus.Text = "Initializing...";
                    await downloadOperation.AttachAsync().AsTask(cancellationToken.Token, progress);
                }
                catch (TaskCanceledException)
                {
                    txtStatus.Text = "Download cancelled";
                    await downloadOperation.ResultFile.DeleteAsync();
                    btnPauseResume.Content = "Resume";
                    btnCancel.IsEnabled = false;
                    btnPauseResume.IsEnabled = false;
                    btnDownload.IsEnabled = true;
                    downloadOperation = null;

                }
            }
        }

        private void progressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
            txtProgress.Text = String.Format(" {2}% ({0}/{1}kb).",
                downloadOperation.Progress.BytesReceived / 1024,
                downloadOperation.Progress.TotalBytesToReceive / 1024, progress);

            ProgressBarDownload.Value = progress;
            switch (downloadOperation.Progress.Status)
            {
                case BackgroundTransferStatus.Running:
                    {
                        txtStatus.Text = "Downloading...";
                        btnPauseResume.Content = "Pause";
                        break;
                    }
                case BackgroundTransferStatus.PausedByApplication:
                    {
                        txtStatus.Text = "Downloading paused";
                        btnPauseResume.Content = "Resume";
                        break;
                    }
                case BackgroundTransferStatus.PausedCostedNetwork:
                    {
                        txtStatus.Text = "Downloading paused because of metered connection";
                        btnPauseResume.Content = "Resume";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        txtStatus.Text = "No network detected. Please check your internet connection";

                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        txtStatus.Text = "An error occured while downloading.";

                        break;
                    }
            }
            if (progress >= 100)
            {
                txtStatus.Text = "Download Completed";
                btnCancel.IsEnabled = false;
                btnPauseResume.IsEnabled = false;
                btnDownload.IsEnabled = true;
                downloadOperation = null;
            }

        }




    }
}
