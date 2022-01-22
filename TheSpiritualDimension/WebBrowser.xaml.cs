using System;
using System.Collections.Generic;
using System.IO;
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

namespace TheSpiritualDimension
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : Page
    {

        string page = "https://www.supremappagency.com/eltemplodelosdeseos/";
        public WebBrowser()
        {
            InitializeComponent();


        }
        public WebBrowser(string page)
        {
            InitializeComponent();
            this.page = page;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.webView.Navigate(page);
        }


        private void webView_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {

            if (e.Uri.AbsoluteUri == "https://supremappagency.com/eltemplodelosdeseos/gracias-prueba.html" || e.Uri.AbsoluteUri == "https://supremappagency.com/eltemplodelosdeseos/thank-you.php")
            {
                System.Threading.Thread.Sleep(10000);
                NavigationService nav = NavigationService.GetNavigationService(this);
                ValidarLicencia r1 = new ValidarLicencia();
                nav.Navigate(r1);


            }


        }
    }
}
