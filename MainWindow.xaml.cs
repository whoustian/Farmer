using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClickFarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string ArtistNames { get; set; }

        public bool SpotifyFarm { get; set; }

        public bool SoundCloudFarm { get; set; }

        TextBoxOutputter outputter;

        // top level interface. bool spotify is defaulted to true, but if you set it to false it will go and use FarmSoundCloud.
        public MainWindow()
        {
            bool spotify = true;
            InitializeComponent();
            DataContext = this;
            try
            {
                if (ClickFarmer.isRunning.Equals(false))
                {
                    if (spotify)
                    {
                        Task.Run(() => { ClickFarmer.FarmSpotify(ArtistNames); });
                    }
                    else
                    {
                        Task.Run(() => { ClickFarmer.FarmSoundCloud(ArtistNames); });
                    }

                }
                else
                {
                    MessageBox.Show("Farmer already running.", "Error Message");
                }

                outputter = new TextBoxOutputter(textBox);
                Console.SetOut(outputter);
            }
            catch
            {
                ClickFarmer.isRunning = false;
            }
        }

        void TimerTick(object state)
        {
            var who = state as string;
            Console.WriteLine(who);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //Farmer.FarmSpotify(ArtistNames);
            //if (SoundCloudFarm)
            //{
            //    try
            //    {
            //        if (Farmer.isRunning.Equals(false))
            //        {
            //            await Task.Run(() => { Farmer.FarmSoundCloud(ArtistNames); });
            //        }
            //        else
            //        {
            //            MessageBox.Show("Farmer already running.", "Error Message");
            //        }

            //    }
            //    catch
            //    {
            //        Farmer.isRunning = false;
            //    }
            //}
            //else if (SpotifyFarm)
            //{
            try
            {
                if (ClickFarmer.isRunning.Equals(false))
                {
                    await Task.Run(() => { ClickFarmer.FarmSpotify(ArtistNames); });
                }
                else
                {
                    MessageBox.Show("Farmer already running.", "Error Message");
                }

            }
            catch
            {
                ClickFarmer.isRunning = false;
            }
            //}
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            SoundCloudFarm = true;
            SpotifyFarm = false;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            SoundCloudFarm = false;
            SpotifyFarm = true;
        }

        private void textBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            textBox.ScrollToEnd();
        }

        private void skip_Click(object sender, RoutedEventArgs e)
        {
            ClickFarmer.Skip();
        }
    }
}
