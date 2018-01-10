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
using test_sql_hololens.Page;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace test_sql_hololens.Page
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Menu //: Page
    {
        public Menu()
        {
            this.InitializeComponent();
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //            Frame.Navigate(typeof(Jeu));
            Frame.Navigate(typeof(List_game));

        }

        private void MenuButton1_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Menu));
        }

        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Compte));
        }


        private void MenuButton21_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(help));
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
           // Frame.Navigate(typeof(Welcome));
        }

    }
}

