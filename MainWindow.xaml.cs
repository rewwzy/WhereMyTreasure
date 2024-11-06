using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhereMyTreasure.Utils;
using Point = System.Drawing.Point;

namespace WhereMyTreasure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<string> devices = KAutoHelper.ADBHelper.GetDevices();
            foreach (var deviceID in devices)
            {
                AutoHelper helper = new AutoHelper(deviceID);

                if (!deviceID.ToLower().Contains("emulator"))
                {
                    helper.Login();
                    //helper.Delay(15);
                    helper.CheckForRate();
                    //helper.Delay(5);
                    helper.Move(new Point(75, -25), 10000);
                }

            }
        }
    }
}
