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
        public async Task<List<string>> DownloadAsync(string URL)
        {

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(URL).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                string resourceTask = await response.Content.ReadAsStringAsync();
                List<string> resource = await SeparateValuesAsync(resourceTask);
                return resource;
            }
        }

        public async Task<List<string>> SeparateValuesAsync(string resourceTask)
        {
            List<string> list = new List<string>();
            Regex rx = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches = await Task.Run(() => rx.Matches(resourceTask));

            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;
                list.Add(src);
            }

            return list;
        }
    }
}
