using BetaBank.View.Interfaces;
using System;
using System.Windows;

namespace BetaBank.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, IMainView
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = System.Windows.SizeToContent.Width;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}