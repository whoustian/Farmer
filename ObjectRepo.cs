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

        public static PageObject spotify_LogIn = new PageObject(By.XPath("//*[text()='Log in']"));
        public static PageObject spotify_UserNameBox = new PageObject(By.XPath("//*[@ng-model='form.username']"));
        public static PageObject spotify_PassWordBox = new PageObject(By.XPath("//*[@ng-model='form.password']"));
        public static PageObject spotify_LogInButton = new PageObject(By.XPath("//*[@id='login-button']"));
        public static PageObject spotify_Search = new PageObject(By.XPath("//*[text()='Search']"));
        public static PageObject spotify_SearchBar = new PageObject(By.XPath("//*[@data-testid='search-input']"));

        public static PageObject getArtistObject(string artist)
        {
            return new PageObject(By.XPath("(//*[@title='" + artist + "'])[1]"));
        }


    }
}