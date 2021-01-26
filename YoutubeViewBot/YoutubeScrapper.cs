using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace YoutubeViewBot
{
    public class YoutubeScrapper
    {
        ChromeOptions options = new ChromeOptions();
        private IWebDriver _webDriver;
        List<string> _videoLinkList = new List<string>();
        IJavaScriptExecutor js;

        public YoutubeScrapper()
        {

            options.AddArgument("--mute-audio");
            options.AddArgument("--autoplay-policy=no-user-gesture-required");
            //options.AddArgument("--disable-gpu");
            //options.AddArgument("--single-process");
            //options.AddArgument("--disable-gpu");
            //options.AddArgument("--headless");
            //options.add_argument('--proxy-server={0}'.format(self.proxy))
            new DriverManager().SetUpDriver(new ChromeConfig());
            //_webDriver = new ChromeDriver(options);
            _webDriver = new ChromeDriver(options);
        }
        private void WatchVedio(string VedioLink)
        {
            _webDriver.Navigate().GoToUrl($"{VedioLink}");

            System.Threading.Thread.Sleep(4000 * 10);


        }
        public void StartBot(string YoutubeChannelVedioURL)
        {
            try
            {
                _webDriver.Navigate().GoToUrl(YoutubeChannelVedioURL);
                System.Threading.Thread.Sleep(2);
                js = (IJavaScriptExecutor)_webDriver;
                var page_len = js.ExecuteScript(
                    "window.scrollTo(0, document.documentElement.scrollHeight);" +
                    "var page_len=document.documentElement.scrollHeight;" +
                    "return page_len;"
                );
                var scroll_complete = false;
                var page_count = 0;
                while (scroll_complete == false)
                {
                    page_count = Convert.ToInt32(page_len);

                    System.Threading.Thread.Sleep(2);
                    page_len = js.ExecuteScript(
                    "window.scrollTo(0, document.documentElement.scrollHeight);" +
                    "var page_len=document.documentElement.scrollHeight;" +
                    "return page_len;");


                    if (page_count == Convert.ToInt32(page_len))
                    {
                        scroll_complete = true;
                        Console.WriteLine("Scrolling Complete!!");
                    }

                }

                try
                {
                    var elements = _webDriver.FindElements(By.Id("video-title"));

                    foreach (var item in elements)
                    {
                        _videoLinkList.Add(item.GetAttribute("href"));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);

                }


                foreach (var item in _videoLinkList)
                {
                    WatchVedio(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TearDown();
            }




        }


        public void TearDown()
        {
            try
            {
                _webDriver.Close();
                _webDriver.Quit();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error:" + ex.Message);
            }

        }
    }
}
