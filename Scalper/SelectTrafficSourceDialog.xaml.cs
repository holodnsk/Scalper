using System.IO;
using System.Windows.Controls;

namespace Scalper
{

    public delegate void TrafficSourсeFileSelectedHandler(string fileName);
    public partial class SelectTrafficSourceDialog
    {

        public event TrafficSourсeFileSelectedHandler TrafficSourсeFileSelectedEvent;

        public SelectTrafficSourceDialog()
        {
            InitializeComponent();
            
            FileInfo[] files = new DirectoryInfo(@"traffic").GetFiles();

            foreach (FileInfo file in files)
            {                
                ListView.Items.Add(file);
            }
            Show();
        }        
        

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TrafficSourсeFileSelectedEvent?.Invoke((sender as ListView)?.SelectedItem.ToString());
            Close();
        }
    }
}