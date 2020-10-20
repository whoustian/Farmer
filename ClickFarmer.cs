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
using System.Windows;
using System.Windows.Input;

namespace ClickFarm
{
    class ClickFarmer
    {

        public static IWebDriver driver;

        // HI

        public static bool isRunning = false;

        public static bool useOpera = false;
        public static bool useEdge = false;

        public static void FarmSoundCloud(string media)
        {
            isRunning = true;
            bool useOpera = false;

            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");

            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            media = File.ReadAllText(".\\media.txt");

            List<string> mediaList = media.Split('\n').ToList();

            string exePath = ".\\chromedriver.exe";

            ExtractResource(exePath, useOpera);

            driver = SeleniumWebDriver.SetUpChromeDriver();

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


        public static void FarmSpotify(string media)
        {
            try
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

                if (ClickFarmer.getConfigValue(ObjectRepo.ShowChromeWindow).Equals("false"))
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                }
                else
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                }

                //ObjectRepo.spotify_LogIn.click(driver);

                string username = File.ReadAllText(".\\username.txt");
                string password = File.ReadAllText(".\\password.txt");

                ObjectRepo.spotify_UserNameBox.waitForVisible(driver, 30);
                ObjectRepo.spotify_UserNameBox.SetValue(driver, username);
                ObjectRepo.spotify_PassWordBox.SetValue(driver, password);
                ObjectRepo.spotify_LogInButton.click(driver);

                Thread.Sleep(3000);

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

                            ObjectRepo.spotify_playerControls.scrollTo(driver);

                            if (ObjectRepo.spotify_Play.isVisible(driver))
                            {
                                ObjectRepo.spotify_Play.click(driver);
                                Thread.Sleep(2000);
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
                    Log("Encountered repeat bug.");
                    driver.Close();
                    Thread.Sleep(10000);
                    FarmSpotify("");
                    break;
                }
            }
        }

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
