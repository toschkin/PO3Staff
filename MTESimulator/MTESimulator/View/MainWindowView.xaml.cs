using System.Windows;
using System.Windows.Input;
using MTESimulator.Utils;
using MTESimulator.ViewModel;

namespace MTESimulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public MainWindowView()
        {
            InitializeComponent();
        }
        private void MainWindowView_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.No ==
                MessageBox.Show("Выйти из программы?", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                e.Cancel = true;
            else
            {
                MainWindowViewModel.DisconnectCommand.Execute(null);
            }
        }        
    }
}
