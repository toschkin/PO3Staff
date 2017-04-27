using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MTESimulator.ViewModel;

namespace MTESimulator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Setup Quick Converter.
            // Add the System namespace so we can use primitive types (i.e. int, etc.).
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(UInt16));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Parity));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Colors));
            // Add the System.Windows namespace so we can use Visibility.Collapsed, etc.
            QuickConverter.EquationTokenizer.AddNamespace(typeof(System.Windows.Visibility));

            var mw = new MainWindowView();
            mw.DataContext = mw.MainWindowViewModel = new MainWindowViewModel(mw);
            
            mw.Show();
        }
    }
}
