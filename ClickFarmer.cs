using Farmer;
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
    class ClickFarmer
    {

        public static IWebDriver driver;

        public static bool isRunning = false;

        public static bool useOpera = false;
        public static bool useEdge = false;

        public static void FarmSoundCloud(string songsOrArtists)
        {
            isRunning = true;
            bool useOpera = false;

            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            List<string> songsOrArtistsList = songsOrArtists.Split('\n').ToList();

            string exePath = ".\\chromedriver.exe";

            ExtractResource(exePath, useOpera);

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


        public static void FarmSpotify(string media)
        {
            isRunning = true;

            string exePath;

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


            media = File.ReadAllText(".\\media.txt");

            List<string> mediaList = media.Split('\n').ToList();

            if (useOpera)
            {
                exePath = ".\\operadriver.exe";
            }
            else if (useEdge)
            {
                exePath = ".\\msedgedriver.exe";
            }
            else
            {
                exePath = ".\\chromedriver.exe";
            }

            ExtractResource(exePath, useOpera);

            GetDriver(useOpera, useEdge);

            driver.Navigate().GoToUrl("https://accounts.spotify.com/en/login?continue=https:%2F%2Fopen.spotify.com%2F");

            //ObjectRepo.spotify_LogIn.click(driver);

            string username = File.ReadAllText(".\\username.txt");
            string password = "Penis911!";

            ObjectRepo.spotify_UserNameBox.waitForVisible(driver, 30);
            ObjectRepo.spotify_UserNameBox.SetValue(driver, username);
            ObjectRepo.spotify_PassWordBox.SetValue(driver, password);
            ObjectRepo.spotify_LogInButton.click(driver);

            Log("Logged in as " + username + ". Waiting 10 seconds.");
            Thread.Sleep(10000);

            if (ObjectRepo.spotify_inCorrectUserNamePWError.isVisible(driver))
            {
                string path = ".\\BadUser.txt";
                File.AppendAllLines(path, new[] { username });
                Thread.Sleep(5000);
                driver.Close();
                Thread.Sleep(2000);
                Process.GetCurrentProcess().Kill();
            }

            foreach (string currentMedia in mediaList)
            {

                // Play playlist on repeat
                if (currentMedia.StartsWith("playlist"))
                {
                    string url = currentMedia.Substring(9);
                    Thread.Sleep(2000);
                    driver.Navigate().GoToUrl(url);

                    Log("Playing playlist at " + url);

                    ObjectRepo.spotify_Play.waitForVisible(driver, 20);

                    Thread.Sleep(1000);

                    while (true)
                    {
                        if (ObjectRepo.spotify_EnableRepeat.isVisible(driver))
                        {
                            ObjectRepo.spotify_EnableRepeat.click(driver);
                        }

                        ObjectRepo.spotify_nextButton.scrollTo(driver);

                        if (ObjectRepo.spotify_Play.isVisible(driver))
                        {
                            ObjectRepo.spotify_Play.click(driver);
                        }

                        Thread.Sleep(2000);

                        while (ObjectRepo.spotify_playButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_playButton.click(driver);
                        }

                        PlayTimeWait();
                        ObjectRepo.spotify_nextButton.click(driver);
                        CheckForSpinny(driver);
                        HandleAds(driver);
                        Thread.Sleep(2000);
                    }
                }

                // Shuffle artist discography
                if (currentMedia.StartsWith("artist"))
                {
                    string artist = currentMedia.Substring(7);

                    Log("Shuffling artist discography for " + artist);

                    ObjectRepo.spotify_Search.waitForVisible(driver, 30);
                    ObjectRepo.spotify_Search.click(driver);
                    ObjectRepo.spotify_SearchBar.SetValue(driver, artist);
                    PageObject artistPage = ObjectRepo.getArtistObject(artist);
                    artistPage.waitForVisible(driver, 30);
                    artistPage.click(driver);

                    while (true)
                    {
                        if (ObjectRepo.spotify_EnableRepeat.isVisible(driver))
                        {
                            ObjectRepo.spotify_EnableRepeat.click(driver);
                        }

                        if (ObjectRepo.spotify_shuffleButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_shuffleButton.click(driver);
                        }

                        ObjectRepo.spotify_nextButton.scrollTo(driver);

                        while (ObjectRepo.spotify_playButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_playButton.click(driver);
                        }

                        PlayTimeWait();
                        ObjectRepo.spotify_nextButton.click(driver);
                        CheckForSpinny(driver);
                        Thread.Sleep(500);
                        HandleAds(driver);
                    }
                }

                // Play song on repeat
                if (currentMedia.StartsWith("song"))
                {
                    string songUrl = currentMedia.Substring(5);
                    Log("Playing song at " + songUrl);
                    driver.Navigate().GoToUrl(songUrl);
                    Thread.Sleep(2000);
                    ObjectRepo.spotify_backButton.click(driver);
                    Thread.Sleep(2000);
                    ObjectRepo.spotify_backButton.click(driver);
                    Thread.Sleep(2000);
                    ObjectRepo.spotify_backButton.click(driver);
                    while (true)
                    {
                        while (ObjectRepo.spotify_playButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_playButton.click(driver);
                            Thread.Sleep(2000);
                        }

                        PlayTimeWait();
                        driver.Navigate().Refresh();
                        ObjectRepo.spotify_Play.waitForVisible(driver, 20);
                        CheckForSpinny(driver);
                        ObjectRepo.spotify_backButton.click(driver);
                        HandleAds(driver);
                    }
                }
            }

            isRunning = false;
        }

        private static void PlayTimeWait()
        {
            int playWholeSongRandom = new Random().Next(1, 10);
            string playInterval;
            int randomWaitTime = 0;

            try
            {
                playInterval = File.ReadLines(".\\config.txt").First().Substring(13);
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

            if (playWholeSongRandom == 7 || playInterval.Equals("full"))
            {
                if (!playInterval.Equals("dev"))
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
            else
            {
                Log("Playing " + songTitle + " for " + randomWaitTime + " seconds");
                Thread.Sleep(randomWaitTime * 1000);
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
                driver.Navigate().Refresh();
                Thread.Sleep(1000);
            }
        }

        public static void CheckForSpinny(IWebDriver driver)
        {
            if (ObjectRepo.spotify_SpinnyPause.isVisible(driver))
            {
                Log("Uh oh! The spinny showed up! Refreshing page.");
                Thread.Sleep(10000);
                driver.Navigate().Refresh();
                Thread.Sleep(5000);
            }
        }

        private static void ExtractResource(string path, bool useOpera)
        {
            try
            {
                byte[] bytes;
                if (useOpera)
                {
                    bytes = Properties.Resources.operadriver;
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

    }
}
