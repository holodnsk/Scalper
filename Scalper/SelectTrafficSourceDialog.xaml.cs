using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Scalper
{
    public partial class SelectTrafficSourceDialog : Window
    {
        public SelectTrafficSourceDialog()
        {
            InitializeComponent();
        }

        public SelectTrafficSourceDialog(string trafficfilelist)
        {
            
        }

        private IEnumerable trafficFileList;
        public void ShowItems(IEnumerable trafficFileList)
        {
            this.trafficFileList = trafficFileList;
        }
    }
}