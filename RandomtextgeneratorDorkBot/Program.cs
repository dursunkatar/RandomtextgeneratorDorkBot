using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RandomtextgeneratorDorkBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int languageIndex = 0;
            List<string> dorks = new();
            string[] languages = { "fr", "de", "en" };

            Console.WriteLine("Dork Başı Giriniz");
            string dorkTitle = Console.ReadLine();

            Console.WriteLine("Kaç Adet Dork Oluşturmak İstiyorsunuz ?");
            int dorkCount = int.Parse(Console.ReadLine());

            Console.WriteLine("\nDorklar üretiliyor...");

            do
            {
                string html = getRandomText(languages[languageIndex++]);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode nodes = doc.QuerySelector("div#randomtext_box");
                string text = nodes.InnerText.Trim();
                string[] keywords = Regex.Split(text, " ");
                dorks.AddRange(keywords.Where(keyword => keyword.Length >= 4).Select(keywork => dorkTitle + keywork.Trim()));

                if (dorks.Count >= dorkCount)
                    break;

                if (languageIndex == 3)
                    languageIndex = 0;

            } while (true);

            File.WriteAllLines("dorks.txt", dorks);

            Console.WriteLine("Dorklar aynı dizinde dorks.txt dosyasına kaydedildi");
            Console.ReadLine();
        }

        private static string getRandomText(string language)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://randomtextgenerator.com/");
            string body = $"text_mode=plain&language={language}&Go=Go";
            var bytesBody = Encoding.ASCII.GetBytes(body);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytesBody.Length;
            request.Host = "randomtextgenerator.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0";
            using var stream = request.GetRequestStream();
            stream.Write(bytesBody, 0, bytesBody.Length);
            using var response = (HttpWebResponse)request.GetResponse();
            using var streamReader = new StreamReader(response.GetResponseStream());
            return streamReader.ReadToEnd();
        }
    }
}
