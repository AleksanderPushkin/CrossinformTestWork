using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    public class ParallelTaskTripletService :ITripletService
    {
        private string Text { get; set; }
        public ParallelTaskTripletService()
        {
           
        }

        private void ReadText(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException(nameof(path));

            Text = System.IO.File.ReadAllText(path);
        }

        public List<string> GetTop10Triplets(string path)
        {
            ReadText(path);
            
            char[] c = Text.ToCharArray();
            ConcurrentDictionary<string, int> cd = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Parallel.For(0, c.Length - 2, (i) => {
                if (char.IsLetter(c[i]) && char.IsLetter(c[i + 1]) && char.IsLetter(c[i + 2]))
                {
                    var triplet = c[i].ToString() + c[i + 1].ToString() + c[i + 2].ToString();
                    cd.AddOrUpdate(triplet, 1, (_, n) => n + 1);
                }
            });

            return  GetTop10(cd);
        }

        private List<string> GetTop10(ConcurrentDictionary<string, int> triplets)
        {
            return triplets.OrderByDescending(t => t.Value).ThenByDescending(t=>t.Key).Take(10).Select(t => t.Key).ToList();
        }

        private void CleanText()
        {
            Text = Regex.Replace(Text, @"\W|[0-9]|\r|\n|\t", " ");
        }
    }
}
