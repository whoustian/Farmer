using Farmer;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFarm
{
    class ObjectRepo
    {

        public const string ShowChromeWindow = "ShowChromeWindow";
        public const string PlayFullSongs = "PlayFullSongs";
        public const string PlayInterval = "PlayInterval";

        public static PageObject spotify_LogIn = new PageObject(By.XPath("//*[text()='Log in']"));
        public static PageObject spotify_UserNameBox = new PageObject(By.XPath("//*[@ng-model='form.username']"));
        public static PageObject spotify_PassWordBox = new PageObject(By.XPath("//*[@ng-model='form.password']"));
        public static PageObject spotify_LogInButton = new PageObject(By.XPath("//*[@id='login-button']"));
        public static PageObject spotify_Search = new PageObject(By.XPath("//*[text()='Search']"));
        public static PageObject spotify_SearchBar = new PageObject(By.XPath("//*[@data-testid='search-input']"));
        public static PageObject spotify_nextButton = new PageObject(By.XPath("(//*[@class='control-button spoticon-skip-forward-16'])[1]"));
        public static PageObject spotify_backButton = new PageObject(By.XPath("(//*[@title='Previous'])[1]"));
        public static PageObject spotify_playButton = new PageObject(By.XPath("(//*[@class='control-button spoticon-play-16 control-button--circled'])[1]"));
        public static PageObject spotify_pauseButton = new PageObject(By.XPath("(//*[@class='control-button spoticon-pause-16 control-button--circled])[1]"));
        public static PageObject spotify_shuffleButton = new PageObject(By.XPath("(//*[@class='control-button spoticon-shuffle-16'])[1]"));
        public static PageObject spotify_unShuffleButton = new PageObject(By.XPath("(//*[@class='control-button spoticon-shuffle-16 control-button--active'])[1]"));
        public static PageObject spotify_inCorrectUserNamePWError = new PageObject(By.XPath("//*[text()='Incorrect username or password.']"));
        public static PageObject spotify_Play = new PageObject(By.XPath("(//*[@aria-label='Play'])[2]"));
        public static PageObject spotify_EnableRepeatOne = new PageObject(By.XPath("(//*[@title='Enable repeat one'])[1]"));
        public static PageObject spotify_DisableRepeat = new PageObject(By.XPath("(//*[@title='Disable repeat'])[1]"));
        public static PageObject spotify_EnableRepeat = new PageObject(By.XPath("(//*[@title='Enable repeat'])[1]"));
        public static PageObject spotify_SongPlayTime = new PageObject(By.XPath("(//*[@class='playback-bar__progress-time e80fc2e59729be32410c909c47ef87a3-scss'])[2]"));
        public static PageObject spotify_Advertisement = new PageObject(By.XPath("(//*[text()='Advertisement'])[2]"));
        public static PageObject spotify_SpinnyPause = new PageObject(By.XPath("(//*[@class='control-button spoticon-pause-16 control-button--circled control-button--loading'])[1]"));
        public static PageObject spotify_songTitle = new PageObject(By.XPath("(//*[@data-testid='nowplaying-track-link'])[1]"));
        public static PageObject spotify_offlineIcon = new PageObject(By.XPath("//*[@aria-label='You're offline']"));
        public static PageObject spotify_tabLimit = new PageObject(By.XPath("//*[text()='So you've discovered the tab limit...']"));
        public static PageObject spotify_politeBar = new PageObject(By.XPath("//*[@aria-live='polite']"));
        public static PageObject spotify_likeSong = new PageObject(By.XPath("(//*[@title='Save to your Liked Songs'])[1]"));
        public static PageObject spotfiy_logo = new PageObject(By.XPath("//*[@class='spotify-logo--text']"));
        public static PageObject spotify_playerControls = new PageObject(By.XPath("(//*[@aria-label='Player controls'])[1]"));
        public static PageObject spotify_politeBarbutton = new PageObject(By.XPath("(//*[@aria-label='Connect to a device'])[1]"));

        public static PageObject soundcloud_SearchBar = new PageObject(By.XPath("(//*[@type='search'])[2]"));
        public static PageObject soundcloud_SearchButton = new PageObject(By.XPath("(//*[@type='submit'])[2]"));
        public static PageObject soundcloud_PlayButton = new PageObject(By.XPath("(//*[@title='Play'])[1]"));


        public static PageObject getArtistObject(string artist)
        {
            return new PageObject(By.XPath("(//*[@title='" + artist + "'])[1]"));
        }


    }
}