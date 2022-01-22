using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace TheSpiritualDimension
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string Email;
        public static Home home;
        public static bool licenceOk;

        void App_Exit(object sender, ExitEventArgs e)
        { 
            
            
            if(licenceOk)
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["Azure"].ConnectionString;
                    string mac="no-mac";
                SqlConnection con = new SqlConnection(cs);
                con.Open();
                    IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface adapter in nics)
                    {
                        mac = adapter.GetPhysicalAddress().ToString();
                        break;
                    }


                            string sql = "DELETE FROM Codigos where email=@email;";
                SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@email", Email);

                //    cmd.Parameters.AddWithValue("@email", mac);


                    cmd.ExecuteNonQuery();
                int i = 0;
                foreach (FilaNumeroSagrado item in home.dtNumeros.Items)
                {




                        sql = "INSERT INTO Codigos(email,Numero,uso,dedicado_a,CS_Graboboi,orden,fecha) VALUES(@email,@num,@uso,@dedicado_a,@CsGraboboi,@orden,GETDATE());";
                               cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@email", Email);
                    //    cmd.Parameters.AddWithValue("@email", mac);
                        cmd.Parameters.AddWithValue("@num", item.Numero);
                    cmd.Parameters.AddWithValue("@uso", item.beneficio);
                    cmd.Parameters.AddWithValue("@dedicado_a", item.benefactor);
                        cmd.Parameters.AddWithValue("@CsGraboboi", item.CSoGrabovoi);
                        cmd.Parameters.AddWithValue("@orden", i);


                    cmd.ExecuteNonQuery();
                        i++;

                }
                con.Close();

            }
            catch (Exception Ex) { string ms = Ex.ToString(); }
        }

    

    }
}
