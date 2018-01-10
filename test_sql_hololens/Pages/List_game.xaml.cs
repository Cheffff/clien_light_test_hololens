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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace test_sql_hololens.Page
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class List_game// : Page
    {

        public class Game
        {
            public int Id { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
        }
        public List_game()
        {
            this.InitializeComponent();


            List<Game> list = new List<Game>();

            list = Getgame();
            StudentsList.ItemsSource = list;
        }

        public List<Game> Getgame()
        {
            const string GetUsersQuery = "select [id_game], [name], [description] from [dbo].[games]";

            List<Game> gamelist = new List<Game>();

            try
            {

                using (SqlConnection conn = new SqlConnection(@"Server=tcp:virtual-deck.database.windows.net,1433;Initial Catalog=VirtualDeckStoreDb;Persist Security Info=False;User ID=admin_virtualdeck;Password=$pass_teamtek2017;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetUsersQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())

                            {
                                // read through all rows
                                while (reader.Read())
                                {
                                    var user = new Game();
                                    // Get the only column value for each row
                                    user.Id = reader.GetInt32(0);
                                    user.name = reader.GetString(1);
                                    user.desc = reader.GetString(2);
                                    gamelist.Add(user);
                                }
                            }
                        }
                    }

                    return gamelist;
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }


        private List<Game> listOfStudents = new List<Game>();


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
            Frame.Navigate(typeof(Compte));
        }


        private void Button_Click21(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;

            String id_test = id.ToString();
            Frame.Navigate(typeof(Game_detail), id_test);

        }
    }
}
