using System.Net.NetworkInformation;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using YoutubeViewBot.Helpers;
using Proxy = YoutubeViewBot.Helpers.Proxy;

namespace YoutubeViewBot
{

    class Program
    {
        static string id;
        static int threadsCount;
        static string ChannelVedioURL;

        static int pos = 0;

        static ProxyScraper scraper;
        public static UsedProxyType proxyType;

        static int botted = 0;
        static int errors = 0;

        static string viewers = "Parsing...";
        static string title = "Parsing...";

        static object locker = new object();

        static string gitRepo = "Git URL";

        public static string[] Urls = new[] {
            "https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list-raw.txt",
            "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks4.txt",
            "https://api.proxyscrape.com/?request=getproxies&proxytype=socks4&timeout=9000&ssl=yes",
            "https://www.proxy-list.download/api/v1/get?type=socks4"
        };


        

        static string intro = "";


        [STAThread]
        static void Main(string[] args)
        {
            id = dialog("Enter Video ID");
            ChannelVedioURL = $"https://www.youtube.com/channel/{id}/videos";
            threadsCount = Convert.ToInt32(dialog("Enter Threads Count"));

            while (true)
            {
                Logo(ConsoleColor.Cyan);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Select proxy type:\r\n0. Public (Socks4 autoscrape)\r\n1. Http/s\r\n2. Socks4\r\n3. Socks5");

                Console.Write("Your choice: ");
                Console.ForegroundColor = ConsoleColor.Cyan;

                char k = Console.ReadKey().KeyChar;

                try
                {
                    int key = int.Parse(k.ToString());

                    if (key < 0 || key > 3)
                        throw new NotImplementedException();

                    proxyType = (UsedProxyType)key;
                }
                catch
                {
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\r\nSelected {proxyType} proxy");

                break;

            }


            if (proxyType != Helpers.UsedProxyType.Public)
            {
                //Console.Write("Path to proxy list");

                //OpenFileDialog ofd = new OpenFileDialog();
                //ofd.Filter = "Proxy list|*.txt";
                //ofd.ShowDialog();

                //scraper = new ProxyScraper(ofd.FileName);

                //To DO
            }

            else
            {
                //scraper = new ProxyScraper();
            }
                


            Worker();








        }//Closing Main Method

        private static bool CanPing(string address)
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(address, 2000);
                if (reply == null) return false;

                return (reply.Status == IPStatus.Success);
            }
            catch (PingException e)
            {
                return false;
            }
        }

        static void Worker()
        {
            Random random = new Random();
            YoutubeScrapper youtubeScrapper;
            while (true)
            {
                try
                {
                    //Proxy proxy;

                    //proxy = scraper.Next();




                    ////if(CanPing(proxy.Address))
                    ////{

                    //var seleniumproxy = new OpenQA.Selenium.Proxy();
                    //    seleniumproxy.Kind = ProxyKind.Manual;
                    //    seleniumproxy.IsAutoDetect = false;
                    //    seleniumproxy.HttpProxy = proxy.Address;
                    //    seleniumproxy.SslProxy = proxy.Address;

                        //youtubeScrapper = new YoutubeScrapper(seleniumproxy);
                    youtubeScrapper = new YoutubeScrapper();
                    youtubeScrapper.StartBot(ChannelVedioURL);

                        youtubeScrapper.TearDown();
                    //}
                    

                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error:{ex.Message}");
                    
                }
                

            }

        }

        static string dialog(string question)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{question}: ");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string val = Console.ReadLine().Trim();

            Logo(ConsoleColor.Cyan);
            return val;
        }

        static void Logo(ConsoleColor color)
        {
            Console.Clear();

            Console.ForegroundColor = color;
            Console.WriteLine(intro);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("GitHub: ");

            Console.ForegroundColor = color;
            Console.WriteLine(gitRepo);

            pos = Console.CursorTop;
        }
    }
}
