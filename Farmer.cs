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
    class Farmer
    {

        public static IWebDriver driver;

        public static bool isRunning = false;

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

            bool useOpera = false;
            bool useEdge = false;

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
                File.AppendAllLines(path, new[] { username });
                Thread.Sleep(5000);
                driver.Close();
                Process.GetCurrentProcess().Kill();
            }

            foreach (string currentMedia in mediaList)
            {

                // Play playlist on repeat
                if (currentMedia.StartsWith("playlist"))
                {
                    string url = currentMedia.Substring(9);
                    driver.Navigate().GoToUrl(url);

                    Log("Playing playlist at " + url);

                    ObjectRepo.spotify_PlaylistPlay.waitForVisible(driver, 20);

                    if (ObjectRepo.spotify_EnableRepeat.isVisible(driver))
                    {
                        ObjectRepo.spotify_EnableRepeat.click(driver);
                    }

                    while (true)
                    {
                        ObjectRepo.spotify_nextButton.scrollTo(driver);

                        if (ObjectRepo.spotify_playButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_playButton.click(driver);
                        }

                        Thread.Sleep(2000);

                        if (ObjectRepo.spotify_playButton.isVisible(driver))
                        {
                            ObjectRepo.spotify_playButton.click(driver);
                        }

                        int playWholeSongRandom = new Random().Next(1, 10);
                        int randomWaitTime = new Random().Next(35, 60);

                        if (playWholeSongRandom == 7)
                        {
                            Log("Playing whole song");
                            string playtime = ObjectRepo.spotify_SongPlayTime.getText(driver);
                            int minutes = Int32.Parse(playtime.Split(':')[0]);
                            int seconds = Int32.Parse(playtime.Split(':')[1]);
                            int waitTime = (minutes * 60) + seconds + 2;
                            Thread.Sleep(waitTime * 1000);
                            //ObjectRepo.spotify_nextButton.click(driver);
                        }
                        else
                        {
                            Log("Playing for " + randomWaitTime + " seconds");
                            Thread.Sleep(randomWaitTime * 1000);
                            ObjectRepo.spotify_nextButton.click(driver);
                        }

                        HandleAds(driver);
                        Thread.Sleep(500);
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

                    if (ObjectRepo.spotify_shuffleButton.isVisible(driver))
                    {
                        ObjectRepo.spotify_shuffleButton.click(driver);
                    }

                    while (true)
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
                        HandleAds(driver);
                    }
                }

                // Play song on repeat
                if (currentMedia.StartsWith("song"))
                {
                    string song = currentMedia.Substring(5);
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

        public static void HandleAds(IWebDriver driver)
        {
            if (ObjectRepo.spotify_Advertisement.isVisible(driver))
            {
                driver.Navigate().Refresh();
                Thread.Sleep(1000);
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
