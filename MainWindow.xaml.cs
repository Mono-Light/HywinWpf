using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Messager.Responses;
using Messager.Api;
using Messager.Encryption;

namespace Messager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pw = "";
        string us = "";
        bool isTyping;
        bool isLoggingIn;

        static TextBlock errorLabel;
        public static MainWindow Current;

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
            Main();
        }

        public static void ShowErrorMessage(string message)
        {
            errorLabel.Text = message;
        }

        async void Main()
        {
            errorLabel = ErrorMessage;
            WebClient.Initialize("http://localhost:3500");
            await LocalUser.GetEndKeys();
            
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            string text = box.Text.Trim();

            if (text.Length < 1 || box.Foreground == Brushes.DimGray) box.Text = "";
            isTyping = true;
            box.Foreground = Brushes.White;
            box.FontStyle = FontStyles.Normal;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            string text = box.Text.Trim();
            isTyping = false;
            if (text.Length < 1)
            {
                if (box.Name == "UsernameInput") box.Text = "gustavo@28";
                if (box.Name == "PasswordInput") box.Text = "mypw1234";
                box.Foreground = Brushes.DimGray;
                box.FontStyle = FontStyles.Italic;
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            string text = box.Text.Trim();
            if (!isTyping) return;

            if (box.Name == "UsernameInput")
                us = text;
            else if (box.Name == "PasswordInput")
                pw = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           /* MinWidth = SystemParameters.PrimaryScreenWidth / 3;
            MinHeight = SystemParameters.PrimaryScreenHeight / 3;

            GridLength width = new GridLength(MinWidth / 8);
            int col = Grid.GetColumn(LoginButton);

            MainGrid.ColumnDefinitions[col].Width = width;
            MainGrid.ColumnDefinitions[col+1].Width = width;


            // Set Distance between the inputs
            int row = 4;

            MainGrid.RowDefinitions[row].MaxHeight = MinHeight / 17;*/

        }
        
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (isLoggingIn)
            {
                ShowErrorMessage("Already logging in, please wait.");
                return;
            }
            if (us.Length == 0)
            {
                ShowErrorMessage("No username given!");
                return;
            }
            if (pw.Length == 0)
            {
                ShowErrorMessage("No password given!");
                return;
            }

            isLoggingIn = true;

            bool r = await LocalUser.TryLogin(us, pw);
            if (r) Content = new Resources.Pages.InitPage();
            isLoggingIn = false;
        }


        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Console.WriteLine("NO");
        }

    }
}
