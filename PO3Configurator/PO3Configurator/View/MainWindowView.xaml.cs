using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonWindows.View;
using CommonWindows.ViewModel;
using Microsoft.Win32;
using PO3Configurator.Utils;
using PO3Configurator.ViewModel;
using PO3Core;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace PO3Configurator.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        #region Members       
        private ImageBrush _backgroundBrush = new ImageBrush();
        #endregion

        #region Properties

        public ImageBrush DefaultBackgroundBrush { get; set; }
        
        #endregion

        #region Constructor
        public MainWindowView()
        {
            InitializeComponent();

            if (Registry.CurrentUser.OpenSubKey(Constants.registryAppNode) == null)
                Registry.CurrentUser.CreateSubKey(Constants.registryAppNode);

            
            _backgroundBrush.Opacity = 0.2;

            _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/default.jpg"));
            DefaultBackgroundBrush = new ImageBrush
            {
                ImageSource =
                    new BitmapImage(
                        new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/default.jpg")),
                Opacity = 0.2
            };
            /*
            if (DateTime.Now.Day == 9 && DateTime.Now.Month == 5)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/9may.jpg"));
            if (DateTime.Now.Day == 1 && DateTime.Now.Month == 5)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/1may.jpg"));
            if (DateTime.Now.Day == 1 && DateTime.Now.Month == 6)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/1june.jpg"));
            if (DateTime.Now.Day == 12 && DateTime.Now.Month == 6)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/russian flag.png"));
            if (DateTime.Now.Day == 23 && DateTime.Now.Month == 2)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/23feb.png"));
            if (DateTime.Now.Day == 8 && DateTime.Now.Month == 3)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/8_marta.jpg"));
            if (DateTime.Now.Day == 4 && DateTime.Now.Month == 11)
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/4novem.jpg"));
            if ((DateTime.Now.Day >= 31 && DateTime.Now.Month == 12) && (DateTime.Now.Day < 2 && DateTime.Now.Month == 1))
                _backgroundBrush.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PO3Configurator;component/View/Images/Backgrounds/newyear.jpg"));
                */
            WindowMainDockPannel.Background = _backgroundBrush;            
        }
        #endregion

        #region Event Handlers
                        
        private void DeviceAddress_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (MainMenu.IsKeyboardFocusWithin)
            {
                BindingExpression be = DeviceAddress.GetBindingExpression(TextBox.TextProperty);
                be?.UpdateSource();
                e.Handled = true;
            }
        }        
        private void MainWindowView_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.No ==
                MessageBox.Show("Выйти из программы?\r\nВсе несохраненные данные будут утеряны", Constants.messageBoxTitle, MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                e.Cancel = true;            
        }
        #endregion        
    }
}
