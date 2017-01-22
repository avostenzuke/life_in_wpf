using System;
using System.Windows;
using System.Windows.Media;
using Life.ViewModel;
using System.Configuration;

namespace Life
{
    /// <summary> 
    /// View - отвечает за отображение информации пользователю. 
    /// В текущей архитектуре View является каким-либо элементом управления WPF. 
    /// View взаимодействует с ViewModel при помощи механизма привязки данных WPF (data binding).
    /// Подписка на события элементов управления так же производится при помощи привязки данных к
    /// свойствам ViewModel, реализующим стандартный интерфейс команды, как правило ICommand.
    /// Наличие прямого доступа из View к Model не оговаривается.
    /// </summary> 
    public partial class MainWindow : Window
    {

        #region Fields

        private static readonly int borderSize = Convert.ToInt32(ConfigurationManager.AppSettings["borderSize"]);
        private static readonly int rectangleSidesLength = Convert.ToInt32(ConfigurationManager.AppSettings["rectangleSidesLength"]);
        private static readonly int rowCount = Convert.ToInt32(ConfigurationManager.AppSettings["rowCount"]);
        private static readonly int columnCount = Convert.ToInt32(ConfigurationManager.AppSettings["columnCount"]);
        private static readonly int updateSleepMs = Convert.ToInt32(ConfigurationManager.AppSettings["updateSleepMs"]);

        #endregion

        #region Constructors

        public MainWindow()
        {
            Brush v = Brushes.Black;
            InitializeComponent();
            MainWindowViewModel mwvm = new MainWindowViewModel(borderSize, rectangleSidesLength, rowCount, columnCount, updateSleepMs, Brushes.LimeGreen, Brushes.Black);
            DataContext = mwvm;
        }

        #endregion
    }
}
