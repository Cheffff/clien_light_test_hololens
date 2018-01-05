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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace test_sql_hololens
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public class Student
        {
            public int Id { get; set; }
            public string email { get; set; }
            public string Age { get; set; }
        }

        public MainPage()
        {
            //this.InitializeComponent();
            this.InitializeComponent();
            //Loaded += MainPage_Loaded;


            List<Student> list = new List<Student>();

            list = Getgame();
            StudentsList.ItemsSource = list;
        }

        public List<Student> Getgame()
        {
            const string GetUsersQuery = "select [id_game], [name], [description] from [dbo].[games]";

            List<Student> gamelist = new List<Student>();

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
                                    var user = new Student();
                                    // Get the only column value for each row
                                    user.Id = reader.GetInt32(0);
                                    user.email = reader.GetString(1);
                                    user.Age = reader.GetString(2);
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


        private List<Student> listOfStudents = new List<Student>();


        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuButton1_Click(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(Menu));
        }

        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(MainPage));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(Welcome));
        }


        private void Button_Click21(object sender, RoutedEventArgs e)
        {

        }
    }
}
