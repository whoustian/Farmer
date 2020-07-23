﻿using Farmer;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClickFarm
{
    class Farmer
    {

        public static IWebDriver driver;

        public static bool isRunning = false;


        public static void FarmSoundCloud(string songsOrArtists)
        {
            isRunning = true;

            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            List<string> songsOrArtistsList = songsOrArtists.Split('\n').ToList();

            string exePath = ".\\chromedriver.exe";

            ExtractResource(exePath);

            driver = SeleniumWebDriver.SetUpChromeDriver();

            foreach (string item in songsOrArtistsList)
            {
                bool isSongLink = item.StartsWith("http");

                if (isSongLink)
                {
                    driver.Navigate().GoToUrl(item);
                    ObjectRepo.soundcloud_PlayButton.waitForVisible(driver, 10);
                    int i = 1;
                    while (i < 1000)
                    {
                        Console.WriteLine("Count: " + i);
                        Thread.Sleep(3000);
                        ObjectRepo.soundcloud_PlayButton.click(driver);
                        int randomWaitTime = new Random().Next(1000, 10000);
                        Thread.Sleep(randomWaitTime);
                        driver.Navigate().Refresh();
                        ObjectRepo.soundcloud_PlayButton.waitForVisible(driver, 10);
                        i++;
                    }
                }
                else
                {
                    driver.Navigate().GoToUrl("https://www.soundcloud.com");

                    foreach (string artist in songsOrArtistsList)
                    {
                        ObjectRepo.soundcloud_SearchBar.SetValue(driver, artist);
                        ObjectRepo.soundcloud_SearchButton.click(driver);
                    }
                }
            }

            isRunning = false;
        }


        public static void FarmSpotify(string artists)
        {
            isRunning = true;

            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            artists = File.ReadAllText(".\\artist.txt");

            List<string> artistNames = artists.Split('\n').ToList();

            string exePath = ".\\chromedriver.exe";

            ExtractResource(exePath);

            driver = SeleniumWebDriver.SetUpChromeDriver();

            driver.Navigate().GoToUrl("https://open.spotify.com/");

            ObjectRepo.spotify_LogIn.click(driver);

            string username = File.ReadAllText(".\\username.txt");
            string password = "Penis911!";

            ObjectRepo.spotify_UserNameBox.waitForVisible(driver, 30);
            ObjectRepo.spotify_UserNameBox.SetValue(driver, username);
            ObjectRepo.spotify_PassWordBox.SetValue(driver, password);
            ObjectRepo.spotify_LogInButton.click(driver);

            Thread.Sleep(500);

            if (ObjectRepo.spotify_inCorrectUserNamePWError.isVisible(driver))
            {
                string path = ".\\BadUser.txt";
                File.AppendAllLines(path, new[] { "bad user: " + username });
                Thread.Sleep(5000);
                driver.Close();
                Process.GetCurrentProcess().Kill();
            }

            foreach (string artist in artistNames)
            {
                ObjectRepo.spotify_Search.waitForVisible(driver, 30);
                ObjectRepo.spotify_Search.click(driver);
                ObjectRepo.spotify_SearchBar.SetValue(driver, artist);
                PageObject artistPage = ObjectRepo.getArtistObject(artist);
                artistPage.waitForVisible(driver, 30);
                artistPage.click(driver);

                if (ObjectRepo.spotify_shuffleButton.isVisible(driver))
                {
                    ObjectRepo.spotify_shuffleButton.click(driver);
                }

                int i = 0;

                while (i < 100000)
                {
                    ObjectRepo.spotify_nextButton.scrollTo(driver);
                    int randomWaitTime = new Random().Next(35, 60);
                    if (ObjectRepo.spotify_playButton.isVisible(driver))
                    {
                        ObjectRepo.spotify_playButton.click(driver);
                    }
                    Console.WriteLine("Playing for " + randomWaitTime + " seconds");
                    Thread.Sleep(randomWaitTime * 1000);
                    ObjectRepo.spotify_nextButton.click(driver);
                    Thread.Sleep(500);
                }
            }

            isRunning = false;
        }

        public static void cycleTop5()
        {
            for (int j = 1; j <= 5; j++)
            {
                PageObject track = new PageObject(By.XPath("(//*[@loading='lazy'])[" + j + "]"));
                track.waitForVisible(driver, 10);
                track.click(driver);
                int randomWaitTime = new Random().Next(35000, 60000);
                Thread.Sleep(randomWaitTime);
            }
        }

        private static void ExtractResource(string path)
        {
            try
            {
                byte[] bytes = Properties.Resources.chromedriver;
                File.WriteAllBytes(path, bytes);
            }
            catch { }
        }

    }
}
