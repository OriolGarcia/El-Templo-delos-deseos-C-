using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace TheSpiritualDimension
{
    class CheckLicencia
    {
        // string cs = "Host=localhost;Username=postgres;Password=postgres;Database=etd;port=5433";
        string cs = ConfigurationManager.ConnectionStrings["Azure"].ConnectionString;
        string mac;
        DateTime limitdate, now;
        string codeApp = "ETD303033";

        Boolean offline = false;
        private static readonly int tamanyoClave = 32;
        private static readonly int tamanyoVector = 16;

        // Define la palabra clave para el cifrado y
        private static readonly string Clave = "ETD-2020-Licenceogs1017";
        private static readonly string Vector = "Estoes una prueba837723";
        // se convierte el vector y la key a bytes

        public static byte[] Key = UTF8Encoding.UTF8.GetBytes(Clave);
        public static byte[] IV = UTF8Encoding.UTF8.GetBytes(Vector);
        public CheckLicencia() { }

        public bool checkLicence(bool newlicence, string Email, string Key, String macpc, DateTime limitdatefich)
    {

        IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();



            if (CheckEmailKey(Email, Key, macpc))
            {

                foreach (NetworkInterface adapter in nics)
                {

                    if (!newlicence  && offline && (adapter.GetPhysicalAddress().ToString() != macpc || (limitdatefich < DateTime.Now && limitdatefich != new DateTime(0))))
                    {

                        if (File.Exists("Security.ETD"))
                        {
                            File.Delete("Security.ETD");
                        }
                        return false;
                    }



                    else if (!newlicence && offline == true && adapter.GetPhysicalAddress().ToString() == macpc && (limitdatefich >= DateTime.Now || limitdatefich == new DateTime(0)))
                    {
                        return true;
                    }
                    SqlConnection con = new SqlConnection(cs);
                    if (mac == null && newlicence && offline == false)
                    {
                        try
                        {
                            
                            con.Open();

                            string sql = "UPDATE Licencias set mac =@mac  WHERE email='" + Email + "' AND program_key = '" + Key + "' AND id_app='" + codeApp + "'";
                            SqlCommand cmd = new SqlCommand(sql,con);

                            cmd.Parameters.AddWithValue("@mac", adapter.GetPhysicalAddress().ToString());


                            cmd.ExecuteNonQuery();


                            CrearLiceciaLocal(Email, Key, mac,limitdate);

                            con.Close();
                            if (limitdate != new DateTime(0) && limitdate < now)
                                return false;
                            return true;

                        }
                        catch (SqlException ERR)

                        {
                            con.Close();
                            return false;
                        }
                        catch (Exception ex) { con.Close();  return false; }
                    }
                

                    if (adapter.GetPhysicalAddress().ToString() == mac && offline == false)
                    {
                        if (limitdate != new DateTime(0) && limitdate < now)
                            return false;
                        CrearLiceciaLocal(Email, Key, mac, limitdate);
                        return true;


                    };


                }
                return false;
            }
            else {


                if (File.Exists("Security.ETD"))
                {
                    File.Delete("Security.ETD");
                }

                return false; }

    }

        private bool CheckEmailKey(String email, String Key, String macpc)
        {
            SqlConnection con = new SqlConnection(cs);
            
            try
            {

                


                con.Open();

                string sql = "SELECT id_App, email, program_key, mac, limitdate, GETDATE() FROM LICENCIAS WHERE email=@email AND program_key=@Key AND id_app='" + codeApp + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@Key", Key);
                string t = cmd.CommandText;
                SqlDataReader rdr = cmd.ExecuteReader();
                
                if (rdr.Read())
                {


                    try
                    {
                        mac = rdr.GetString(3);
                        if (String.IsNullOrEmpty(mac.Trim(' ')))
                            mac = null;
                        
                    }
                    catch
                    {
                        mac = null;
                    }
                    try
                    {
                        limitdate = rdr.GetDateTime(4);
                    }
                    catch
                    {
                       limitdate = new DateTime(0);
                    }
                    try
                    {
                        now = rdr.GetDateTime(5);
                    }
                    catch
                    {
                        now = new DateTime(0);
                    }
                    con.Close();
                    return true;
                }
                con.Close();
                return false;

            } catch(SqlException ERR) {
                con.Close();
                if ( ERR.Number == 11001)
                                {
                              offline = true;
                                    return true;

                                }
                return false;
        }
            catch(Exception ex) {

                con.Close();

                Console.Write(ex.Message);

                return false;
            }
    }

        public static string Encriptar(String txtPlano)

        {

            Array.Resize(ref Key, tamanyoClave);
            Array.Resize(ref IV, tamanyoVector);

            // se instancia el Rijndael

            Rijndael RijndaelAlg = Rijndael.Create();

            // se establece cifrado

            MemoryStream memoryStream = new MemoryStream();

            // se crea el flujo de datos de cifrado

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                RijndaelAlg.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);

            // se obtine la información a cifrar

            byte[] txtPlanoBytes = UTF8Encoding.UTF8.GetBytes(txtPlano);

            // se cifran los datos

            cryptoStream.Write(txtPlanoBytes, 0, txtPlanoBytes.Length);

            cryptoStream.FlushFinalBlock();

            // se obtinen los datos cifrados

            byte[] cipherMessageBytes = memoryStream.ToArray();

            // se cierra todo

            memoryStream.Close();
            cryptoStream.Close();

            // Se devuelve la cadena cifrada

            return Convert.ToBase64String(cipherMessageBytes);
        }



        public static string Desencriptar(String txtCifrado)
        {

            Array.Resize(ref Key, tamanyoClave);
            Array.Resize(ref IV, tamanyoVector);

            // se obtienen los bytes para el cifrado

            byte[] cipherTextBytes = Convert.FromBase64String(txtCifrado);

            // se almacenan los datos descifrados

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];// Cifrado Rijndael AES con C#
 
			// se crea una instancia del Rijndael			
 
			Rijndael RijndaelAlg = Rijndael.Create();

            // se crean los datos cifrados

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // se descifran los datos

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                RijndaelAlg.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            // se obtienen datos descifrados

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            // se cierra todo

            memoryStream.Close();
            cryptoStream.Close();

            // se devuelve los datos descifrados

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }




        private void CrearLiceciaLocal(String email, String Clave, String MAC, DateTime fechalimite)
    {
            try
            {





                string lines = email + "\r\n" + "\r\n" + Clave + "\r\n" + "\r\n" + MAC + "\r\n" + "\r\n" + fechalimite.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";

                string text = Encriptar(lines);

                System.IO.File.WriteAllText("Security.ETD", text);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }



}

}
