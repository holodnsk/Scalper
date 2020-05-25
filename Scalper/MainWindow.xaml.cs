using System;
using System.Windows;
using StockSharp.Algo;

namespace Scalper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Hide();
            
            //ShowLogMessage("MainWindow");
            
            
        }

        public void ShowLogMessage(String message)
        {
            systemLog.Text = systemLog.Text + "\n" + message;
        }
    }
}
