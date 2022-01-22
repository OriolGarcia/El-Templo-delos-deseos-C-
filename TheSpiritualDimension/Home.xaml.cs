using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace TheSpiritualDimension
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        VoiceEffect voz;

        bool pause, finish;
        public bool pasarsiguiente;
        int index;
        private string Email;
        private double _volume;
        private bool bloqueo = false;
        private string lang;


        public Home()
        {
            InitializeComponent();
            //DataContext = this;
            SetLanguageDictionary();
            cargarCodigos();
            voz = new VoiceEffect();
            voces();

            finish = true;
            pause = true;

            App.home = this;
        }


        private void Activar_Click(object sender, RoutedEventArgs e)
        {
            CheckLicencia checkLic = new CheckLicencia();
            if (checkLic.checkLicence(true, txtemail.Text, txtkey.Text, "", new DateTime(0)))
            {

                DesbloquearPantalla();
                cargarCodigos();
                App.licenceOk = true;
                MessageBox.Show("Producto activado!");

            }

            else
            {
                App.licenceOk = false;
                MessageBox.Show("El producto no está activado. Consulte a ogs1017@gmail.com para activarlo.");
                BloquearPantalla();
            }

        }
        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
           
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {




                
                case "ca":
                case "ca-ES":
                    dict.Source = new Uri("..\\Resources\\Multilanguage.CA.xaml", UriKind.Relative);
                    lang = "ca";
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ca-ES");
                    break;
                case "es-ES":
                case "eu-ES":
                case "es-AR":
                case "es-BO":
                case "es-CL":
                case "es-CO":
                case "es-CR":
                case "es-DO":
                case "es-EC":
                case "es-GT":
                case "es-HN":
                case "es-MX":
                case "es-NI":
                case "es-PA":
                case "es-PE":
                case "es-PR":
                case "es-PY":
                case "es-SV":
                case "es-US":
                case "es-UY":
                case "es-VE":
                    dict.Source = new Uri("..\\Resources\\Multilanguage.xaml", UriKind.Relative);
                    lang = "es";
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("");
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\Multilanguage.EN.xaml",
                                      UriKind.Relative);
                    lang = "en";
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("EN");
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }
       

        public void BloquearPantalla()
        {

            PopupTest.IsOpen = true;
            btAñadir.IsEnabled = false;
            btEditar.IsEnabled = false;
            btEliminar.IsEnabled = false;
            btPlaypause.IsEnabled = false;
            btPlaypause_Copy.IsEnabled = false;
            btsubircodigo.IsEnabled = false;
            btbajarcodigo.IsEnabled = false;


        }

        public void DesbloquearPantalla()
        {

            PopupTest.IsOpen = false;
            btAñadir.IsEnabled = true;
            btEditar.IsEnabled = true;
            btEliminar.IsEnabled = true;
            btPlaypause.IsEnabled = true;
            btPlaypause_Copy.IsEnabled = true;
            btsubircodigo.IsEnabled = true;
            btbajarcodigo.IsEnabled = true;


        }


        List<VoiceInfo> vocesinfo = new List<VoiceInfo>();
        void voces()
        {
           
            int n = 0, i = 0;
            ReadOnlyCollection<InstalledVoice> voces = voz.GetInstalledVoices();
            if (voces != null && cmbVoz.Items.Count==0)
            {
                foreach (InstalledVoice voice in voces)
                {
                    VoiceInfo info = voice.VoiceInfo;
                    vocesinfo.Add(info);
                    cmbVoz.Items.Add(info.Name);
                   

                     if ( lang=="ca" && info.Culture.TwoLetterISOLanguageName == "ca")
                        n = i;
                    if (lang == "es" && info.Culture.TwoLetterISOLanguageName == "es")
                        n = i;
                    if (lang == "en" && info.Culture.TwoLetterISOLanguageName == "en")
                        n = i;
                  
                    i++;
               
                }

                




                cmbVoz.SelectedIndex = n;
                voz.init(vocesinfo.ElementAt(cmbVoz.SelectedIndex).Name, this);
            }
        }

        void synthesizer_SpeakCompleted(object sender, System.Speech.Synthesis.SpeakCompletedEventArgs e)
        {
            index = index + 1;
            if (index / 45 < 46)
            {
                txtCuenta.Content = (index / 45).ToString();
            }
            else
            {
                pause = true;
                finish = true;
            }
        }

        private void btPlaypause_Copy_Click(object sender, RoutedEventArgs e)
        {

            if (finish == true)
            {


                finish = false;
                pause = false;

                int j = dtNumeros.SelectedIndex;

                if (j >= dtNumeros.Items.Count)
                {
                    if (chkBucle.IsChecked == true)
                    {
                        dtNumeros.SelectedIndex = 0;
                        j = 0;

                    }
                    else { return; }
                }
                if (j < 0) { j = 0; }

                dtNumeros.SelectedItem = dtNumeros.Items[j];
                ReproducirSeleccionado();


            }

            else
            {
                voz.Stop();
                pause = true;
                finish = true;
                txtCuenta.Content = Resource.Bendiciones;
            }

        }
        private void BtEliminar_Click(object sender, RoutedEventArgs e)
        {
            int i = dtNumeros.SelectedIndex;


            if (i < 0) { return; }
            List<FilaNumeroSagrado> source = (List<FilaNumeroSagrado>)dtNumeros.ItemsSource;

            FilaNumeroSagrado obj0 = source[i];
            source.Remove(obj0);

            dtNumeros.ItemsSource = null;
            dtNumeros.ItemsSource = source;
            if (i > 0)
                dtNumeros.SelectedItem = dtNumeros.Items[i - 1];
            guardarCodigos();
        }
        private void BtAñadir_Click(object sender, RoutedEventArgs e)
        {


            List<FilaNumeroSagrado> filas = new List<FilaNumeroSagrado>();
            filas = (List<FilaNumeroSagrado>)dtNumeros.ItemsSource;
            var isNumeric = long.TryParse(txtañadir.Text, out long n);
            string CSoGrabovoi;
            if ((bool)CodigoSagrado.IsChecked) CSoGrabovoi = "1"; else CSoGrabovoi = "2";

            if ((isNumeric && CSoGrabovoi == "1") || (Regex.IsMatch(txtañadir.Text, "^\\d+( +\\d+)?") && CSoGrabovoi == "2"))
            {
                FilaNumeroSagrado itm = new FilaNumeroSagrado(txtañadir.Text, txtbeneficio.Text, txtNombre.Text, (bool)CodigoSagrado.IsChecked);
                filas.Add(itm);

            }
            else { return; }

            int tmp = dtNumeros.SelectedIndex; dtNumeros.SelectedIndex = -1;
            dtNumeros.ItemsSource = null;

            dtNumeros.ItemsSource = filas;
            dtNumeros.SelectedIndex = tmp;
            guardarCodigos();


            txtañadir.Text = "";
            txtbeneficio.Text = "";
            txtNombre.Text = "";
        }

        private void guardarCodigos()
        {

            if (!File.Exists(AppContext.BaseDirectory + "\\codigos.txt"))
            {
                System.IO.File.WriteAllText(AppContext.BaseDirectory+"\\codigos.txt", "");
            }



            StreamWriter sw = new StreamWriter(AppContext.BaseDirectory + "\\codigos.txt");
            foreach (FilaNumeroSagrado item in dtNumeros.Items)
            {


                string CSoGrabovoi;
                if ((bool)item.CSoGrabovoi) CSoGrabovoi = "1"; else CSoGrabovoi = "2";
                if (CSoGrabovoi == "1")
                {
                    var isNumeric = long.TryParse(item.Numero, out long n);
                    if ((String.IsNullOrEmpty(item.Numero)) || (isNumeric = false))
                    {
                        continue;
                    }
                    else
                    {
                        if (Regex.IsMatch(item.Numero, "^\\d+( +\\d+)?") == false) continue;
                    }

                }

                sw.WriteLine(item.Numero + "|" + item.beneficio + "|" + item.benefactor + "|" + CSoGrabovoi);
            }
            sw.Close();

        }












        private void cargarCodigos()
        {




            if (!File.Exists(AppContext.BaseDirectory+"\\codigos.txt"))
            {
                System.IO.File.WriteAllText(AppContext.BaseDirectory+"\\codigos.txt", "");
            }

            // Borrar el contenido previo de los controles
            this.dtNumeros.Items.Clear();
            dtNumeros.Items.Clear();

            // Leer el fichero usando la codificación de Windows
            // pero pudiendo usar la marca de tipo de fichero (BOM)
            System.IO.StreamReader sr =
                new System.IO.StreamReader(
                        Directory.GetCurrentDirectory() + "\\codigos.txt",
                        System.Text.Encoding.UTF8,
                        true);

            // Leer el contenido mientras no se llegue al final
            List<FilaNumeroSagrado> filas = new List<FilaNumeroSagrado>();
            while (sr.Peek() != -1)
            {
                // Leer una líena del fichero
                string s = sr.ReadLine();
                string[] line = s.Split('|');
                var isNumeric = long.TryParse(line[0], out long n);
                if ((String.IsNullOrEmpty(s)) || (isNumeric = false))
                {
                    continue;
                }
                ListBoxItem itm = new ListBoxItem();
                itm.Content = s;
                bool CSoGrabovoi;


                if (line.GetLength(0) == 1) { filas.Add(new FilaNumeroSagrado(line[0], null, null, true)); }
                if (line.GetLength(0) == 2) { filas.Add(new FilaNumeroSagrado(line[0], line[1], null, true)); }
                if (line.GetLength(0) == 3) { filas.Add(new FilaNumeroSagrado(line[0], line[1], line[2], true)); }
                if (line.GetLength(0) == 4)
                {
                    if (line[3] == "2") { CSoGrabovoi = false; }
                    else { CSoGrabovoi = true; }
                    filas.Add(new FilaNumeroSagrado(line[0], line[1], line[2], CSoGrabovoi));
                }


                //this.listNumeros.Items.Add(itm);


            }

            dtNumeros.ItemsSource = filas;

            // Cerrar el fichero
            sr.Close();

        }
        public void AddIndex(int i)
        {



            txtCuenta.Content = (i).ToString();


        }

        private void CmbVoz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {




            voz.changevoice(vocesinfo.ElementAt(cmbVoz.SelectedIndex).Name);
            string idioma = vocesinfo.ElementAt(cmbVoz.SelectedIndex).Culture.Name;

            ResourceDictionary dict = new ResourceDictionary();

            if (vocesinfo.ElementAt(cmbVoz.SelectedIndex).Culture.TwoLetterISOLanguageName == "ca")
            {
                dict.Source = new Uri("..\\Resources\\Multilanguage.CA.xaml",
                                     UriKind.Relative);

                lang = "ca";

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ca-ES");
            }

            else if (vocesinfo.ElementAt(cmbVoz.SelectedIndex).Culture.TwoLetterISOLanguageName == "es")
            {
                dict.Source = new Uri("..\\Resources\\Multilanguage.xaml",
                                     UriKind.Relative);

                lang = "es";

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("");
            }
            else
            {
                dict.Source = new Uri("..\\Resources\\Multilanguage.EN.xaml",
                                      UriKind.Relative);
                lang = "en";
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("EN");
            }
            this.Resources.MergedDictionaries.Add(dict);


        }

        private void btsubircodigo_Click(object sender, RoutedEventArgs e)
        {
            int i = dtNumeros.SelectedIndex;


            if (i - 1 < 0) { return; }
            List<FilaNumeroSagrado> source = (List<FilaNumeroSagrado>)dtNumeros.ItemsSource;

            FilaNumeroSagrado obj0 = source[i];
            source.Remove(obj0);
            source.Insert(i - 1, obj0);
            dtNumeros.ItemsSource = null;
            dtNumeros.ItemsSource = source;
            dtNumeros.SelectedItem = dtNumeros.Items[i - 1];
            guardarCodigos();

        }

        private void btbajarcodigo_Click(object sender, RoutedEventArgs e)
        {
            int i = dtNumeros.SelectedIndex;


            if (i - 1 >= dtNumeros.Items.Count) { return; }
            List<FilaNumeroSagrado> source = (List<FilaNumeroSagrado>)dtNumeros.ItemsSource;

            FilaNumeroSagrado obj0 = source[i];
            source.Remove(obj0);
            source.Insert(i + 1, obj0);
            dtNumeros.ItemsSource = null;
            dtNumeros.ItemsSource = source;
            dtNumeros.SelectedItem = dtNumeros.Items[i + 1];
            guardarCodigos();

        }
        public void ReproducirSeleccionado()
        {

            try
            {
                pasarsiguiente = true;
                while (voz._isCurrentlySpeaking)
                { continue; }

                FilaNumeroSagrado item = dtNumeros.Items[dtNumeros.SelectedIndex] as FilaNumeroSagrado;

                String Num = item.Numero;


                var isNumeric = long.TryParse(Num, out long n);

                if (isNumeric || Regex.IsMatch(item.Numero, "^\\d+( +\\d+)?"))
                {
                    txtCuenta.Content = Resource.Bendiciones; ;
                    voz.startSpeaking(Resource.Merelajo);

                    if (!item.beneficio.Trim(' ').Equals(""))
                    {


                        if (item.CSoGrabovoi == true)
                            voz.startSpeaking(Resource.AplicoCodigo + item.beneficio);
                        else if (item.CSoGrabovoi == false)
                            voz.startSpeaking(Resource.AplicoGrabovoi + item.beneficio);

                    }

                    if (!item.benefactor.Trim(' ').Equals(""))
                    {

                        voz.startSpeaking(Resource.beneficiode + item.benefactor + Resource.poderosapresencia);

                    }




                    index = -1;

                    if (item.CSoGrabovoi == false && Regex.IsMatch(item.Numero, "^\\d+( +\\d+)?"))
                    {
                        voz.nchars = item.Numero.Replace(" ", "").Length;
                        voz.CsoGb = false;

                        foreach (char c in (item.Numero))
                        {

                            Num = c.ToString();
                            if (Num == " ") Thread.Sleep(400);
                            voz.startSpeaking(c.ToString());
                            while (voz._isCurrentlySpeaking)
                            { continue; }
                            if (finish == true) { return; }
                        }


                    }
                    else if (isNumeric)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            voz.CsoGb = true;
                            voz.startSpeaking(Num);
                            while (voz._isCurrentlySpeaking)
                            { continue; }
                            if (finish == true)
                            {
                                return;
                            }
                        }


                    }




                    if (BenefYo.IsChecked == false)
                    {

                        voz.startSpeaking(Resource.hasidopara + txtNombre.Text);

                    }
                }
                else
                {

                    voz.startSpeaking(Resource.Porfavorintroduzca);


                }
            }
            catch (Exception err) { }
        }

        private void ListNumeros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pasarsiguiente = false;
        }



        private void BenefYo_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && txtNombre != null)
            {
                if (rb.IsChecked == true)
                {
                    txtNombre.IsEnabled = false;
                    txtNombre.Text = "";

                }
            }
        }

        private void BenefOtro_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && txtNombre != null)
            {
                if (rb.IsChecked == true)
                {
                    txtNombre.IsEnabled = true;
                }
            }
        }

        private void DtNumeros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtNumeros.SelectedIndex < 0 || dtNumeros.SelectedIndex >= dtNumeros.Items.Count)
            {
                if (chkBucle.IsChecked == true)
                {
                    dtNumeros.SelectedIndex = 0;

                }

                else
                {
                    pause = true; finish = true;
                    return;
                }

            }

            if (dtNumeros.SelectedIndex < dtNumeros.Items.Count && dtNumeros.SelectedIndex >= 0)
            {
                pasarsiguiente = false;
                FilaNumeroSagrado itm = (FilaNumeroSagrado)dtNumeros.Items[dtNumeros.SelectedIndex];
                txtañadir.Text = itm.Numero;
                txtbeneficio.Text = itm.beneficio;
                txtNombre.Text = itm.benefactor;
                if (itm.CSoGrabovoi)
                    CodigoSagrado.IsChecked = true;
                else
                    Graboboi.IsChecked = true;
            }
        }

        private void BtEditar_Click(object sender, RoutedEventArgs e)
        {
            List<FilaNumeroSagrado> filas = new List<FilaNumeroSagrado>();
            filas = (List<FilaNumeroSagrado>)dtNumeros.ItemsSource;
            var isNumeric = long.TryParse(txtañadir.Text, out long n);
            int i = dtNumeros.SelectedIndex;
            if (i < 0 || i >= dtNumeros.Items.Count)
            {
                return;
            }
            string CSoGrabovoi = "";
            if ((bool)CodigoSagrado.IsChecked) CSoGrabovoi = "1"; else CSoGrabovoi = "2";

            if ((isNumeric && CSoGrabovoi == "1") || (Regex.IsMatch(txtañadir.Text, "^\\d+( +\\d+)?") && CSoGrabovoi == "2"))
            {

                FilaNumeroSagrado itm = new FilaNumeroSagrado(txtañadir.Text, txtbeneficio.Text, txtNombre.Text, (bool)CodigoSagrado.IsChecked);


                filas[i].Numero = txtañadir.Text;
                filas[i].beneficio = txtbeneficio.Text;
                filas[i].benefactor = txtNombre.Text;
                filas[i].CSoGrabovoi = (bool)CodigoSagrado.IsChecked;
                dtNumeros.SelectedIndex = -1;
                dtNumeros.ItemsSource = null;

                dtNumeros.ItemsSource = filas;
                dtNumeros.SelectedIndex = i;
                guardarCodigos();

            }

        }
        private void VolumChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeBar.IsLoaded)
            {



                voz.changeVolume((int)e.NewValue);
            }
        }

        private void pdf1_Click(object sender, RoutedEventArgs e)
        {
            string filename = "Codigos sagrados numericos completo 3.1.pdf";
            System.Diagnostics.Process.Start(filename);
        }
        private void pdf2_Click(object sender, RoutedEventArgs e)
        {
            string filename2 = "Numeros que Curan Gregori Grabovoi.pdf";
            System.Diagnostics.Process.Start(filename2);
        }
        private void btPlaypause_Click(object sender, RoutedEventArgs e)
        {


            if (pause)
            {
                if (finish == true)
                {
                    finish = false;
                    pause = false;

                    int j = dtNumeros.SelectedIndex;

                    if (j >= dtNumeros.Items.Count)
                    {
                        if (chkBucle.IsChecked == true)
                        {
                            dtNumeros.SelectedIndex = 0;
                            j = 0;

                        }
                        else { return; }
                    }
                    if (j < 0) { j = 0; }
                    dtNumeros.SelectedItem = dtNumeros.Items[j];
                    ReproducirSeleccionado();


                }

                else
                {
                    voz.Resume();
                    pause = false;
                }
            }
            else
            {
                pause = true;
                voz.Pause();
            }
        }
    }
}

