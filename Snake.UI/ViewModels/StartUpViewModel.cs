using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Snake.UI.ViewModels
{
    public class StartUpViewModel:ViewModelBase
    {
        public StartUpViewModel()
        {
            PlayGameCommand = new DelegateCommand<Window>(OnPlayGameExecute);
        }

        public ICommand PlayGameCommand { get; }

        private void OnPlayGameExecute(Window window)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
