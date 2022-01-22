using System;

using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace TheSpiritualDimension
{
    public class VoiceEffect
    {
        private string vozanterior;        
        
        public int nchars =99999;
        public Boolean CsoGb = true;
        private string mensaje;
        private Home home;
        public bool fin45;
        SpeechSynthesizer reader = new SpeechSynthesizer();
        public int i; bool b;
        public volatile bool _isCurrentlySpeaking = false;
        private int volumen = 100;
        /// <summary>Event handler. Fired when the SpeechSynthesizer object starts speaking asynchronously.</summary>
        private void StartedSpeaking(object sender, SpeakStartedEventArgs e)
        { _isCurrentlySpeaking = true;
            FilaNumeroSagrado itm = (FilaNumeroSagrado)home.dtNumeros.SelectedItem;
            //nchars = itm.Numero.Replace(" ", "").Length;
        }
        /// <summary>Event handler. Fired when the SpeechSynthesizer object finishes speaking asynchronously.</summary>
        private void FinishedSpeaking(object sender, SpeakCompletedEventArgs e)
        { _isCurrentlySpeaking = false; b = false;
            if (fin45)
            {

                int j = home.dtNumeros.SelectedIndex;
                if (j + 1 >= home.dtNumeros.Items.Count)
                {
                    if (home.chkBucle.IsChecked == true)
                    {
                        home.dtNumeros.SelectedIndex = 0;
                        j = 0;

                    }
                    else { return; }
                }
               if (home.pasarsiguiente)
               {   
                    Object obj0 = home.dtNumeros.SelectedItem;
                    home.dtNumeros.SelectedItem = home.dtNumeros.Items[j + 1];
                    FilaNumeroSagrado itm = (FilaNumeroSagrado) home.dtNumeros.SelectedItem;

                    nchars = itm.Numero.Length;


            }
                home.ReproducirSeleccionado();
                i = 0;
                fin45 = false;
                
            }

        }
        private void synth_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            try
            {
                SpeechSynthesizer reader2 = (SpeechSynthesizer)sender;
                reader2.ToString();
                bool isNumeric = long.TryParse(e.Text, out long n);
                if (b == false)
                {
                    if ((isNumeric) && CsoGb == false)
                    {
                        home.txtCuenta.Content = "Grabovoi";
                        i++;
                        //home.AddIndex(i);
                        if (i >= nchars) { fin45 = true; }
                    }

                    else if (isNumeric && CsoGb == true)
                    {

                        i++;
                        home.AddIndex(i);
                        if (i >= 45) { fin45 = true; }
                        else { fin45 = false; }

                    }
                    else { i = 0; fin45 = false; }
                }

                b = true;
            }catch(Exception e1) { MessageBox.Show(e1.Message); }
        }
        private VoiceEffect _instance;
        /// <summary>Gets the singleton instance of the VoiceEffect class.</summary>
        /// <returns>A unique shared instance of the VoiceEffect class.</returns>
        public ReadOnlyCollection<InstalledVoice> GetInstalledVoices() {
            try
            {
                return reader.GetInstalledVoices();

            }
            catch (Exception e) { MessageBox.Show(e.Message); return null; }
        }
        public void Resume()
        {

            reader.Resume();
        }
        public void Pause()
        {

            reader.Pause();
        }
        public void Stop() {
           int vol=  reader.Volume;
            reader.Dispose();
            _isCurrentlySpeaking = false; b = false;
            i = 0;
            reader = new SpeechSynthesizer();
                changeVolume(vol);
            this.init();
        }

        /// <summary>
        /// Constructor. Initializes the class assigning event handlers for the
        /// SpeechSynthesizer object.
        /// </sum
        /// mary>
        /// 
        public void init(string voz, Home home)
        {
            this.home = home;
            reader.SelectVoice(voz);
            vozanterior = voz;
           
        }

        public void init() { init(vozanterior,home); }
        public VoiceEffect()
        {

           
        }
        public void changevoice(string voz)
        {

           reader.Pause();
            reader.SelectVoice(voz);
          reader.Resume();

        }
        public void changeVolume(int volumen) {
            reader.Volume = volumen;
            this.volumen = volumen;

        }
        /// <summary>Speaks stuff.</summary>
        /// <param name="str">The stuff to speak.</param>
        public void startSpeaking(string str)
        {
            reader.Rate = -2; // Voice  effects.
            reader.Volume = volumen;

            // if the reader's currently speaking anything,
            // don't let any incoming prompts overlap
            while (_isCurrentlySpeaking)
            { continue; }
            mensaje = str;
            reader.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(StartedSpeaking);
            reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(FinishedSpeaking);
            reader.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(synth_SpeakProgress);
            reader.SpeakAsync(str);
           ;
            ;
        }

        /// <summary>Creates a new thread to speak stuff into.</summary>
        /// <param name="str">The stuff to read.</param>
        public void createVoiceThread(string str)
        {
            Thread voicethread = new Thread(() => startSpeaking(str)); // Lambda Process
            voicethread.IsBackground = true;
            voicethread.Start();
        }
    }
}
