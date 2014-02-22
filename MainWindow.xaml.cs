using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace FeedNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Splasher.Splash = new SplashScreen();
            Splasher.ShowSplash();
            Thread newThread = new Thread(Splasher.Progress);
            newThread.Start();
            InitializeComponent();
            Thread.Sleep(3000);
            newThread.Abort();
            Splasher.CloseSplash();
        }
    }
}
