using System.Windows;

namespace Life
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
