using System;
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

namespace TheSpiritualDimension
{
    /// <summary>
    /// Lógica de interacción para Activar.xaml
    /// </summary>
    public partial class Activar : Window
    {
        public Activar()
        {
            InitializeComponent();
        }



        private void Activar_Click(object sender, RoutedEventArgs e)
        {
            CheckLicencia checkLic = new CheckLicencia();
            if (checkLic.checkLicence(true, txtemail.Text, txtkey.Text, "", new DateTime(0)))
            {

                MessageBox.Show("Producto activado!");


            }

            else
            {
                MessageBox.Show("El producto no está activado. Consulte a ogs1017@gmail.com para activarlo.");
            }
        }
    }
}
