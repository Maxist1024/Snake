using Snake.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Snake.UI
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly int SIZE = 10;
        public readonly MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
            mainWindowViewModel.MainGrid = grid;
            InitBoard();
            mainWindowViewModel.InitSnake();
            mainWindowViewModel.InitFood();
        }

        void InitBoard()
        {
            for (int i = 0; i < grid.Width / SIZE; i++)
            {
                ColumnDefinition columnDefinitions = new ColumnDefinition();
                columnDefinitions.Width = new GridLength(SIZE);
                grid.ColumnDefinitions.Add(columnDefinitions);
            }
            for (int j = 0; j < grid.Height / SIZE; j++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(SIZE);
                grid.RowDefinitions.Add(rowDefinition);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!mainWindowViewModel.dispatcherTimer.IsEnabled)
                mainWindowViewModel.dispatcherTimer.Start();

            if (e.Key == Key.Left)
            {
                mainWindowViewModel.DirectionX = -1;
                mainWindowViewModel.DirectionY = 0;
            }

            if (e.Key == Key.Right)
            {
                mainWindowViewModel.DirectionX = 1;
                mainWindowViewModel.DirectionY = 0;
            }

            if (e.Key == Key.Up)
            {
                mainWindowViewModel.DirectionX = 0;
                mainWindowViewModel.DirectionY = -1;
            }

            if (e.Key == Key.Down)
            {
                mainWindowViewModel.DirectionX = 0;
                mainWindowViewModel.DirectionY = 1;
            }
        }

    }
}
