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
        static public string ProxyAddress { get; set; }

        public YoutubeScrapper()
        {

            var proxy = new Proxy();
            proxy.Kind = ProxyKind.Manual;
            proxy.IsAutoDetect = false;
            proxy.HttpProxy = ProxyAddress;
            
            proxy.SslProxy = ProxyAddress;
            //options.Proxy = proxy;
            options.AddArgument("ignore-certificate-errors");
            options.AddArgument("--mute-audio");
            options.AddArgument("--autoplay-policy=no-user-gesture-required");
            options.AddArgument("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            //options.AddArgument("--disable-gpu");
            //options.AddArgument("--single-process");
            options.AddArgument("--disable-gpu");
            //options.AddArgument("--headless");
            options.AddArgument($"--proxy-server={ProxyAddress}");
            new DriverManager().SetUpDriver(new ChromeConfig());
            _webDriver = new ChromeDriver(options);
            _webDriver.Manage().Window.Size = new System.Drawing.Size(1920, 1050);
            //_webDriver = new ChromeDriver(options);
        }
        private void WatchVedio(string VedioLink)
        {
            _webDriver.Navigate().GoToUrl($"{VedioLink}");

            System.Threading.Thread.Sleep(4000 * 10);


        }
        private By GetObj(string locatorType, string selector)
        {
            Dictionary<string, By> map = new Dictionary<string, By>();
            map.Add("ID", By.Id(selector));

            map.Add("NAME", By.Name(selector));
            map.Add("XPATH", By.XPath(selector));
            map.Add("TAG", By.TagName(selector));
            map.Add("CLASS", By.ClassName(selector));
            map.Add("CSS", By.CssSelector(selector));
            map.Add("LINKTEXT", By.LinkText(selector));

            return map[locatorType];


        }

        public IWebElement GetElement(string elementTag, string locator)
        {
            By _by = GetObj(locator, elementTag);


            if (IsElementPresent(_by))
            {
                return _webDriver.FindElement(_by);
            }

            return null;
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                _webDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }


        public void StartBot(string YoutubeChannelVedioURL)
        {
            try
            {
                _webDriver.Navigate().GoToUrl(YoutubeChannelVedioURL);
                
                System.Threading.Thread.Sleep(2);

                String currentURL = _webDriver.Url;


                if (!currentURL.Contains("m.youtube.com"))
                {
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
                }
                

                try
                {
                    IReadOnlyCollection<IWebElement> elements = null;
                    if (IsElementPresent(By.Id("video-title")))
                    {
                        elements = _webDriver.FindElements(By.Id("video-title"));
                    }

                    if (IsElementPresent(By.ClassName("item")))
                    {
                        elements = _webDriver.FindElements(By.ClassName("compact-media-item-image"));
                    }


                    if ( elements==null)
                    {

                    }



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
