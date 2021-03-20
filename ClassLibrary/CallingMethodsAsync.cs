using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class CallingMethodsAsync
    {
        public async Task<List<string>> DownloadAsync(string websiteURL)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(websiteURL).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string sourceCode = await response.Content.ReadAsStringAsync();
            List<string> srcURL = SeparateValues(sourceCode);
            return srcURL;
        }

        public List<string> SeparateValues(string sourceCode)
        {
            List<string> list = new List<string>();
            Regex rx = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches = rx.Matches(sourceCode);

            //foreach (Match match in matches)
            //{
            //    string src = match.Groups[1].Value;
            //    list.Add(src);
            //}

            Parallel.ForEach<Match>(matches, (match) =>
            {
                string src = match.Groups[1].Value;
                list.Add(src);
            });

            return list;
        }
    }
}
