using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class DataAccess
    {
        private static HtmlDocument GetHtmlFromUrl(string url)
        {
            HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Failed req {response.StatusCode}");
            }

            string responseMessage = response.Content.ReadAsStringAsync().Result;
            var doc = new HtmlDocument();
            doc.LoadHtml(responseMessage);
            return doc;
        }
        private static List<Eclipse> GetEclipses(string url)
        {
            Dictionary<string, string> months = new Dictionary<string, string>()
            {
                ["january"] = "01",
                ["february"] = "02",
                ["march"] = "03",
                ["april"] = "04",
                ["may"] = "05",
                ["june"] = "06",
                ["july"] = "07",
                ["august"] = "08",
                ["september"] = "09",
                ["october"] = "10",
                ["november"] = "11",
                ["december"] = "12"
            };
            var doc = GetHtmlFromUrl(url);
            var links = doc.DocumentNode.SelectNodes("//a[@class='ec-link']");
            List<Eclipse> eclipses = new List<Eclipse>();
            foreach (var link in links)
            {
                String href = link.GetAttributeValue("href", "");
                var type = link.SelectSingleNode("//span[@class='ec-type']").InnerText;
                var visibility = link.SelectSingleNode("//span[@class='ec-where']").InnerText;
                string[] splitted = href.Split('/');
                var date = splitted[3];
                var splitted_date = date.Split('-');
                if (Int16.Parse(splitted_date[2]) < 10)
                {
                    splitted_date[2] = '0' + splitted_date[2];
                }
                date = String.Format("{0}/{1}/{2}", splitted_date[2], months[splitted_date[1]], splitted_date[0]);
                Eclipse ecl = new Eclipse(type, date, visibility);
                eclipses.Add(ecl);

            }
            return eclipses;
        }

        public static List<Eclipse> BuildUrlAndGetEclipses(String region, String starty, String type)
        {
            var url = "";
            if (region == "" && type == "" && starty == "")
            {
                url = "http://www.timeanddate.com/eclipse/list.html";
            }
            else if (region == "" && type == "")
            {
                url = "http://www.timeanddate.com/eclipse/list.html?starty=" + starty;
            }
            else if (region == "" && type != "")
            {
                url = "http://www.timeanddate.com/eclipse/list-" + type + ".html?starty=" + starty;
            }
            else if (type == "")
            {
                url = "http://www.timeanddate.com/eclipse/list.html?region=" + region + "7starty=" + starty;
            }
            else
            {
                url = "http://www.timeanddate.com/eclipse/list.html?region=" + region + "&starty=" + starty + "&type=" + type;
            }
            return GetEclipses(url);
        }

        public static void DownloadContent(List<Eclipse> content, String filePath)
        {

            string[] lines = new string[100];
            content.ForEach(ecl => lines.Append(ecl.ToString()));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                content.ForEach(ecl =>
                {
                    writer.WriteLine(ecl.ToString());
                });
            }
        }
    }
}
