using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;

namespace MadokamiDownloader
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
    public class MadoSeries
    {
        public string Name { get; private set; }
        public string URL { get; private set; }
        public Bitmap Cover { get; private set; } = null;
        private List<MadoFile> Chapters;
        public MadoSeries (string name, string url)
        {
            Name = name;
            URL = url;
        }
    }
    static class Parser
    {
        //finds all links to 2-letter pages beginning with this letter
        static List<string> ParseSingle(char c, string page)
        {
            //grabs the URL and 2-letter name of each subpage
            var reg = new Regex($"https://manga.madokami.al/Manga/{c}/(?<Name>{c}[A-Z_])", RegexOptions.IgnoreCase);
            var matches = reg.Matches(page);
            var res = new List<string>(matches.Count);
            foreach (var match in matches)
            {
                res.Add(match.ToString());
            }
            return res;
        }
    }
    public class MadoPage1
    {
        public string Name { get; private set; }
        public string URL { get; private set; }
        private HttpClientHandler Handler;
        public List<MadoPage2> Children { get; private set; }
        /*
        public string Text
        {
            get
            {
                if (mText != null) return mText;
                var client = new HttpClient(Handler);
                var response = client.GetAsync(URL).Result;
                //throws exception if get failed
                response.EnsureSuccessStatusCode();
                mText = response.Content.ReadAsStringAsync().Result;
                return mText;
            }
        }
        */
        public MadoPage1(string name, string url, HttpClientHandler handler)
        {
            if (name.Length != 1)
                throw new InvalidNameLenthException($"Expected 1 letter name, got: {name}");
            Name = name;
            URL = url;
            Handler = handler;
            //Populate();
        }
        //gets children
        private void Populate()
        {
            string text;
            //is there a better way to do this?
            var client = new HttpClient(Handler);
            var response = client.GetAsync(URL).Result;
            client.Dispose();
            response.EnsureSuccessStatusCode();
            text = response.Content.ToString();
            response.Dispose();
            //grabs the URL and 2-letter name of each subpage
            var reg = new Regex($"https://manga.madokami.al/Manga/{Name}/(?<Name>{Name}[A-Z_])", RegexOptions.IgnoreCase);
            var matches = reg.Matches(text);
            foreach (Match m in matches)
            {
                Children.Add(new MadoPage2(m.Groups["Name"].ToString(), m.ToString(), this, Handler));
            }
        }
    }
    public class MadoPage2
    {
        public string Name { get; private set; }
        public string URL { get; private set; }
        private HttpClientHandler Handler;
        public List<MadoPage4> Children;
        public MadoPage1 Parent { get; private set; }
        public MadoPage2(string name, string url, MadoPage1 parent, HttpClientHandler handler)
        {
            if (name.Length != 2)
                throw new InvalidNameLenthException($"Expected 2 letter name, got: {name}");
            Name = name;
            URL = url;
            Parent = parent;
            Handler = handler;
        }
        private void Populate()
        {
            string text;
            //is there a better way to do this?
            var client = new HttpClient(Handler);
            var response = client.GetAsync(URL).Result;
            client.Dispose();
            response.EnsureSuccessStatusCode();
            text = response.Content.ToString();
            response.Dispose();
            //grabs the URL and 2-letter name of each subpage
            var reg = new Regex($"https://manga.madokami.al/Manga/{Name}/(?<Name>{Name}[A-Z_]{2})", RegexOptions.IgnoreCase);
            var matches = reg.Matches(text);
            foreach (Match m in matches)
            {
                Children.Add(new MadoPage4(m.Groups["Name"].ToString(), m.ToString(), this, Handler));
            }
        }
    }
    public class MadoPage4
    {
        public string Name { get; private set; }
        public string URL { get; private set; }
        MadoPage2 Parent;
        private HttpClientHandler Handler;
        public List<MadoSeries> Children { get; private set; }
        public MadoPage4(string name, string url, MadoPage2 parent, HttpClientHandler handler)
        {
            if (name.Length != 4)
                throw new InvalidNameLenthException($"Expected 4 letter name, got: {name}");
            Name = name;
            URL = url;
            Parent = parent;
            Handler = handler;
        }
    }
    public class MadoTree
    {

    }
    public class MadoFile
    {
        public string Filename;
        public string URL;
    }
    public class InvalidNameLenthException : ArgumentException
    {
        public InvalidNameLenthException()
        {
        }
        public InvalidNameLenthException(string message)
            : base(message)
        {
        }
        public InvalidNameLenthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
