﻿using System;
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
using System.Windows.Shapes;

namespace CommonWindows.View
{
    /// <summary>
    /// Логика взаимодействия для SerialPortSettingsView.xaml
    /// </summary>
    public partial class SerialPortSettingsView
    {        
        public SerialPortSettingsView()
        {
            InitializeComponent();            
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
