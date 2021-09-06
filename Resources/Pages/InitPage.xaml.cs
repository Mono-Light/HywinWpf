using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Messager.Extensions;

namespace Messager.Resources.Pages
{
    /// <summary>
    /// Interaktionslogik für MainPage.xaml
    /// </summary>
    public partial class InitPage : Page
    {
        public InitPage()
        {
            InitializeComponent();
            Main();
        }

        private async void Main()
        {
            int percentage = 0;
            int multiplier = 1;
            int state = 0;

            while (percentage < 100)
            {
                percentage += 1 * multiplier;
                state++;
                if (state >= 4)
                {
                    multiplier += 1 * multiplier;
                    state = 0;
                }
                LoadingBar.SetPercent(percentage);
                await Task.Delay(200);
            }

            MainWindow.Current.Content = new MainPage();
        }

    }
}
