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
using System.Windows;
using System.Windows.Input;

namespace ClickFarm
{
    class ClickFarmer
    {

        public static IWebDriver driver;

        public static bool isRunning = false;
        public static bool useOpera = false;
        public static bool useEdge = true;


        // The logic for soundcloud. much smaller and more simplistic than Spotify. it just will play a song on repeat, refreshing after a random number of seconds.
        public static void FarmSoundCloud(string media)
        {
            isRunning = true;
            string exePath;
            KillBrowserProcesses();

            // read in media file and split into list on new lines
            media = File.ReadAllText(".\\media.txt");
            List<string> mediaList = media.Split('\n').ToList();

            if (useOpera)
            {
                exePath = ".\\operadriver.exe";
                Log("Using Opera");
            }
            else if (useEdge)
            {
                exePath = ".\\msedgedriver.exe";
                Log("Using Edge");
            }
            else
            {
                exePath = ".\\chromedriver.exe";
                Log("Using Chrome");
            }

            ExtractResource(exePath);

            // assigns value of global driver variable to what browser you are using
            GetDriver(useOpera, useEdge);

            // loop thru the media lines in media.txt
            foreach (string item in mediaList)
            {
                bool isSongLink = item.StartsWith("http");

                if (isSongLink)
                {
                    driver.Navigate().GoToUrl(item);
                    ObjectRepo.soundcloud_PlayButton.waitForVisible(driver, 10);
                    int i = 1;
                    while (i < 1000)
                    {
                        Log("Count: " + i);
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
                    // this code block is incomplete. looks like i started writing some logic for shuffling artists on Soundcloud but then pivoted to working on Spotify.
                    // soundcloud only works now with individual songs.
                    driver.Navigate().GoToUrl("https://www.soundcloud.com");

                    foreach (string artist in mediaList)
                    {
                        ObjectRepo.soundcloud_SearchBar.SetValue(driver, artist);
                        ObjectRepo.soundcloud_SearchButton.click(driver);
                    }
                }
            }

            isRunning = false;
        }

        // All logic for Spotify farming is contained in this method
        public static void FarmSpotify(string media)
        {
            try
            {
                isRunning = true;

                string exePath;

                // Kill driver before running (doesn't always work for some reason)
                KillBrowserProcesses();

                // read in media file and split into list on new lines
                media = File.ReadAllText(".\\media.txt");
                List<string> mediaList = media.Split('\n').ToList();

                if (useOpera)
                {
                    exePath = ".\\operadriver.exe";
                    Log("Using Opera");
                }
                else if (useEdge)
                {
                    exePath = ".\\msedgedriver.exe";
                    Log("Using Edge");
                }
                else
                {
                    exePath = ".\\chromedriver.exe";
                    Log("Using Chrome");
                }

                ExtractResource(exePath);

                // assigns value of global driver variable to what browser you are using
                GetDriver(useOpera, useEdge);

                // go to Spotify login page and wait either 15 or 5 seconds depending on if you are showing Chrome window. Honestly not sure why we are doing this wait here.
                driver.Navigate().GoToUrl("https://accounts.spotify.com/en/login?continue=https:%2F%2Fopen.spotify.com%2F");

                if (ClickFarmer.getConfigValue(ObjectRepo.ShowChromeWindow).Equals("false"))
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                }
                else
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                }

                string username = File.ReadAllText(".\\username.txt");
                string password = File.ReadAllText(".\\password.txt");

                // enter username and password, login
                ObjectRepo.spotify_UserNameBox.waitForVisible(driver, 30);
                ObjectRepo.spotify_UserNameBox.SetValue(driver, username);
                ObjectRepo.spotify_PassWordBox.SetValue(driver, password);
                ObjectRepo.spotify_LogInButton.click(driver);

                Thread.Sleep(3000);

                // add bad users to bad user text file
                if (ObjectRepo.spotify_inCorrectUserNamePWError.isVisible(driver))
                {
                    string badUserPath = ".\\BadUser.txt";
                    File.AppendAllLines(badUserPath, new[] { username });
                    Thread.Sleep(5000);
                    driver.Close();
                    Thread.Sleep(2000);
                    Process.GetCurrentProcess().Kill();
                }


                Log("Logged in as " + username + ". Waiting 10 seconds.");
                string loginSuccessPath = ".\\loginsuccessful.txt";
                File.AppendAllLines(loginSuccessPath, new[] { username });
                Thread.Sleep(10000);

                ObjectRepo.spotify_Search.waitForVisible(driver, 20);

                // loop through each media entry in the list
                foreach (string currentMedia in mediaList)
                {

                    // Play playlist on repeat
                    if (currentMedia.StartsWith("playlist"))
                    {
                        bool firstRun = true;
                        string url = currentMedia.Substring(9);
                        Thread.Sleep(2000);
                        driver.Navigate().GoToUrl(url);

                        Log("Playing playlist at " + url);

                        ObjectRepo.spotfiy_logo.waitForVisible(driver, 20);

                        HandlePoliteBar(driver);

                        Thread.Sleep(1000);

                        LikePlayList();

                        while (true)
                        {
                            if (!firstRun)
                            {
                                EnableRepeat();
                            }

                            // scroll to player controls, press play button
                            ObjectRepo.spotify_playerControls.scrollTo(driver);

                            // play button is a bit finicky sometimes and doesn't remain pressed, so i have this if statement and while loop below to make sure it gets pressed.
                            if (ObjectRepo.spotify_Play.isVisible(driver))
                            {
                                ObjectRepo.spotify_Play.click(driver);
                                Thread.Sleep(2000);
                            }

                            // once the play button is pressed, it becomes the pause button and it is no longer visible to Selenium
                            while (ObjectRepo.spotify_playButton.isVisible(driver))
                            {
                                ObjectRepo.spotify_playButton.click(driver);
                            }

                            // wait the number of seconds determined bny PlayTimeWait(), randomly like the song, hit next, handle ads, spinny, and loop again
                            PlayTimeWait();
                            LikeSong();
                            ObjectRepo.spotify_nextButton.click(driver);
                            Thread.Sleep(2000);
                            HandleAds(driver);
                            CheckForSpinny(driver);
                            firstRun = false;
                        }
                    }

                    // Shuffle artist discography
                    if (currentMedia.StartsWith("artist"))
                    {
                        bool firstRun = true;
                        string artistUrl = currentMedia.Substring(7);

                        Log("Shuffling artist discography for artist at " + artistUrl);

                        driver.Navigate().GoToUrl(artistUrl);

                        while (true)
                        {

                            if (!firstRun)
                            {
                                EnableRepeat();
                                EnableShuffle();
                            }

                            ObjectRepo.spotify_nextButton.scrollTo(driver);

                            HandlePoliteBar(driver);

                            Thread.Sleep(2000);

                            while (ObjectRepo.spotify_Play.isVisible(driver))
                            {
                                ObjectRepo.spotify_Play.click(driver);
                                Thread.Sleep(1000);
                            }

                            while (ObjectRepo.spotify_playButton.isVisible(driver))
                            {
                                ObjectRepo.spotify_playButton.click(driver);
                            }

                            PlayTimeWait();
                            LikeSong();
                            ObjectRepo.spotify_nextButton.click(driver);
                            Thread.Sleep(2000);
                            HandleAds(driver);
                            CheckForSpinny(driver);
                            firstRun = false;
                        }
                    }

                    // Play song on repeat 
                    if (currentMedia.StartsWith("song"))
                    {
                        bool firstRun = true;
                        string songUrl = currentMedia.Substring(5);
                        Log("Playing song at " + songUrl);
                        driver.Navigate().GoToUrl(songUrl);

                        ObjectRepo.spotify_Play.waitForVisible(driver, 10);

                        while (true)
                        {
                            if (!firstRun)
                            {
                                EnableRepeatOne();
                            }

                            HandlePoliteBar(driver);

                            Thread.Sleep(2000);

                            while (ObjectRepo.spotify_Play.isVisible(driver))
                            {
                                ObjectRepo.spotify_Play.click(driver);
                                Thread.Sleep(2000);
                            }

                            EnableRepeatOne();

                            PlayTimeWait();
                            LikeSong();
                            driver.Navigate().Refresh();
                            ObjectRepo.spotify_Play.waitForVisible(driver, 20);
                            HandleAds(driver);
                            CheckForSpinny(driver);
                            firstRun = false;
                        }
                    }
                }

                isRunning = false;
            }
            catch (Exception e)
            {
                Log("Error: " + e.Message);
            }
        }

        private static void KillBrowserProcesses()
        {
            if (useOpera)
            {
                Process[] operaDriverProcesses = Process.GetProcessesByName("operadriver");

                foreach (var operaDriverProcess in operaDriverProcesses)
                {
                    operaDriverProcess.Kill();
                }
            }
            else if (useEdge)
            {
                Process[] edgeDriverProcesses = Process.GetProcessesByName("msedgedriver");

                foreach (var edgeDriverProcess in edgeDriverProcesses)
                {
                    edgeDriverProcess.Kill();
                }
            }
            else
            {
                Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    chromeDriverProcess.Kill();
                }
            }
        }

        private static void EnableShuffle()
        {
            while (ObjectRepo.spotify_shuffleButton.isVisible(driver))
            {
                ObjectRepo.spotify_shuffleButton.click(driver);
                Thread.Sleep(1000);
            }
        }

        private static void EnableRepeat()
        {
            int i = 0;
            while (!ObjectRepo.spotify_EnableRepeatOne.isVisible(driver))
            {
                if (ObjectRepo.spotify_EnableRepeat.isVisible(driver))
                {
                    ObjectRepo.spotify_EnableRepeat.click(driver);
                }
                Thread.Sleep(1000);
                if (ObjectRepo.spotify_DisableRepeat.isVisible(driver))
                {
                    ObjectRepo.spotify_DisableRepeat.click(driver);
                }
                Thread.Sleep(1000);
                i++;
                if (i == 10)
                {
                    Log("Encountered repeat bug.");
                    driver.Close();
                    Thread.Sleep(10000);
                    FarmSpotify("");
                    break;
                }
            }
        }

        private static void EnableRepeatOne()
        {
            int i = 0;
            while (!ObjectRepo.spotify_DisableRepeat.isVisible(driver) && i <= 200)
            {
                if (ObjectRepo.spotify_EnableRepeat.isVisible(driver))
                {
                    ObjectRepo.spotify_EnableRepeat.click(driver);
                }
                Thread.Sleep(3000);
                if (ObjectRepo.spotify_EnableRepeatOne.isVisible(driver))
                {
                    ObjectRepo.spotify_EnableRepeatOne.click(driver);
                }
                Thread.Sleep(3000);
                i++;
                if (i == 50)
                {
                    // there was sometimes a bug with pressing repeat where it wouldn't work, so i have it try 50 times and then restart the driver.
                    Log("Encountered repeat bug.");
                    driver.Close();
                    Thread.Sleep(10000);
                    FarmSpotify("");
                    break;
                }
            }
        }

        // Method which determines how long to wait for a song based on the config value PlayInterval, and whole song mode being enabled
        private static void PlayTimeWait()
        {
            string playInterval;
            int randomWaitTime = 0;

            try
            {
                playInterval = getConfigValue(ObjectRepo.PlayInterval);
            }
            catch
            {
                playInterval = "1";
            }


            if (playInterval.Equals("dev"))
            {
                randomWaitTime = 5;
            }
            else if (Int32.Parse(playInterval) == 1)
            {
                randomWaitTime = new Random().Next(35, 60);
            }
            else if (Int32.Parse(playInterval) == 2)
            {
                randomWaitTime = new Random().Next(60, 120);
            }

            string songTitle = ObjectRepo.spotify_songTitle.getText(driver);


            int playWholeSongRandom = new Random().Next(1, 20);

            // whole song mode
            if (playWholeSongRandom == 7 || playInterval.Equals("full"))
            {
                if (!playInterval.Equals("dev"))
                {
                    if (getConfigValue(ObjectRepo.PlayFullSongs).Equals("true"))
                    {
                        Log("Playing whole song of " + songTitle);
                        string playtime = ObjectRepo.spotify_SongPlayTime.getText(driver);
                        int minutes = Int32.Parse(playtime.Split(':')[0]);
                        int seconds = Int32.Parse(playtime.Split(':')[1]);
                        int waitTime = (minutes * 60) + seconds + 2;
                        Thread.Sleep(waitTime * 1000);
                        //ObjectRepo.spotify_nextButton.click(driver);
                    }
                }
            }
            else
            {
                Log("Playing " + songTitle + " for " + randomWaitTime + " seconds");
                Thread.Sleep(randomWaitTime * 1000);
            }
        }

        public static string getConfigValue(string key)
        {
            try
            {
                string value = "";
                string[] configLines = File.ReadAllLines(".\\config.txt");

                foreach (string i in configLines)
                {
                    if (i.StartsWith(key))
                    {
                        value = i.Split(':')[1];
                        return value;
                    }
                }
                return "";
            }

            catch
            {
                return "";
            }
        }

        private static void GetDriver(bool useOpera, bool useEdge)
        {
            if (useOpera)
            {
                driver = SeleniumWebDriver.SetUpOperaDriver();
            }
            else if (useEdge)
            {
                driver = SeleniumWebDriver.SetUpEdgeDriver();
            }
            else
            {
                driver = SeleniumWebDriver.SetUpChromeDriver();
            }
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

        public static void HandleAds(IWebDriver driver)
        {
            if (ObjectRepo.spotify_Advertisement.isVisible(driver))
            {
                Log("Encountered advertisement. Refreshing.");
                driver.Navigate().Refresh();
                Thread.Sleep(10000);
                ObjectRepo.spotify_playerControls.waitForVisible(driver, 20);
            }
        }

        // This is an annoying bar that sometimes pops up at the bottom of the screen, just a method checking for it and getting rid of it if needed. 
        public static void HandlePoliteBar(IWebDriver driver)
        {
            if (ObjectRepo.spotify_politeBar.isVisible(driver))
            {
                Log("Encountered bottom taskbar. Handling.");
                ObjectRepo.spotify_politeBarbutton.click(driver);
                Thread.Sleep(3000);
            }
        }

        public static void CheckForSpinny(IWebDriver driver)
        {
            if (ObjectRepo.spotify_SpinnyPause.isVisible(driver))
            {
                Log("Uh oh! The dreaded spinny! Refreshing page in 10 seconds.");
                Thread.Sleep(10000);
                driver.Navigate().Refresh();
                Thread.Sleep(5000);
            }
        }

        // writes the driver.exe file 
        private static void ExtractResource(string path)
        {
            try
            {
                byte[] bytes;
                if (useOpera)
                {
                    bytes = Properties.Resources.operadriver;
                }
                if (useEdge)
                {
                    bytes = Properties.Resources.msedgedriver;
                }
                else
                {
                    bytes = Properties.Resources.chromedriver;
                }
                File.WriteAllBytes(path, bytes);
            }
            catch { }
        }

        public static void Log(string content)
        {
            Console.WriteLine(content);
            File.AppendAllText(".\\log.txt", DateTime.Now + ": " + content + Environment.NewLine);
        }

        public static void Skip()
        {
            if (ObjectRepo.spotify_nextButton.isVisible(driver))
            {
                ObjectRepo.spotify_nextButton.click(driver);
                Log("Skipped song.");
            }
            else
            {
                MessageBox.Show("Cannot skip - Skip button not visible.", "Error Message");
            }
        }

        public static void LikeSong()
        {
            // generate a random number one thru 10
            int random = new Random().Next(1, 10);
            if (random == 7)
            {
                if (ObjectRepo.spotify_likeSong.isVisible(driver))
                {
                    Log("Liking current song.");
                    ObjectRepo.spotify_likeSong.click(driver);
                    Thread.Sleep(2000);
                }
            }
        }

        public static void LikePlayList()
        {
            int random = new Random().Next(1, 10);
            if (random == 7)
            {
                if (ObjectRepo.spotify_likePlaylist.isVisible(driver))
                {
                    Log("Liking current playlist.");
                    ObjectRepo.spotify_likePlaylist.click(driver);
                    Thread.Sleep(2000);
                }
            }
        }

    }
}
