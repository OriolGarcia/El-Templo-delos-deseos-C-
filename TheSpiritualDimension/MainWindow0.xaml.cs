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
using System.Windows.Shapes;

namespace TheSpiritualDimension
{
    /// <summary>
    /// Interaction logic for MainWindow0.xaml
    /// </summary>
    public partial class MainWindow0 : Window
    {
        public MainWindow0()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {



            if (!File.Exists("Security.ETD"))
            {
                Navegador.Content = new ValidarLicencia();
                
                //Bloqueo = true;
                return;
                App.licenceOk = false;
                //App.home = this;
            }
            else
            {

                try
                {




                    String text = File.ReadAllText("Security.ETD");

                    string desencriptado = CheckLicencia.Desencriptar(text);
                    if (String.IsNullOrEmpty(desencriptado))
                    {
                        // Bloqueo = true;
                        return;
                    }
                    string[] separado = desencriptado.Replace("\r\n", "\n").Split("\n".ToCharArray());
                    App.Email = separado[0];
                    //App.main = this;
                    //App.idUser = separado[8];

                    CheckLicencia checkLic = new CheckLicencia();

                    DateTime lim;
                    string fecha = separado[6];
                    if (fecha == "0001-01-01 00:00:00")
                    {
                        lim = new DateTime(0);
                    }
                    else
                    {
                        lim = DateTime.ParseExact(separado[6], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (checkLic.checkLicence(false, separado[0], separado[2], separado[4], lim))
                    {
                        App.licenceOk = true;


                        //Bloqueo = false;
                        Navegador.Content = new Home();


                    }
                    else
                    {
                        // Bloqueo = true;
                        Navegador.Content = new ValidarLicencia();
                       
                        App.licenceOk = false;
                    }

                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }


        }


        public void DesbloquearPantalla()
        {

            Navegador.Content = new Home();
        }
        public void BloquearPantalla()
        {

             Navegador.Content = new ValidarLicencia();
           
        }
    }
}