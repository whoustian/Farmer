﻿using System;
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //Farmer.FarmSpotify(ArtistNames);
            try
            {
                if (Farmer.isRunning.Equals(false))
                {
                    await Task.Run(() => { Farmer.FarmSoundCloud(ArtistNames); });
                }
                else
                {
                    MessageBox.Show("Downloader already running.", "Error Message");
                }

            }
            catch
            {
                Farmer.isRunning = false;
            }
        }
    }
}
