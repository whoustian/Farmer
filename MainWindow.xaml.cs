﻿using System;
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

        public string Media { get; set; }

        public bool SpotifyFarm { get; set; }

        public bool SoundCloudFarm { get; set; }

        TextBoxOutputter outputter;

        // top level interface. bool spotify on line 37 determines if you are using spotify or soundcloud.
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
                        Task.Run(() => { ClickFarmer.FarmSpotify(Media); });
                    }
                    else
                    {
                        Task.Run(() => { ClickFarmer.FarmSoundCloud(Media); });
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

        // Much of what's down here is no longer used, just pieces of the UI that used to exist and don't anymore. Wouldn't delete any of it though without retesting.
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
                    await Task.Run(() => { ClickFarmer.FarmSpotify(Media); });
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
