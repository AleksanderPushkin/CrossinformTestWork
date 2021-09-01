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
            if (!File.Exists(path)) throw new ArgumentNullException(nameof(path));

            Text = System.IO.File.ReadAllText(path);
        }

        public List<string> GetTop10Triplets(string path)
        {
            ReadText(path);

            CleanText();
           

            ConcurrentDictionary<string, int> triplets = new ConcurrentDictionary<string, int>();
            var words = Text.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            //var _l = new object();
            Parallel.ForEach(words, word =>
            {
                var letters = word.ToCharArray();
                if (letters.Length > 2)
                {
                    for (int i = 0; i < letters.Length - 2; i++)
                    {
                        var triplet = string.Format("{0}{1}{2}", letters[i], letters[i + 1], letters[i + 2]);
                        //lock (_l)
                        //{
                            triplets.AddOrUpdate(triplet, 1, (_, value) =>  value + 1);
                       // }

                    }
                }
            });
          
            return  GetTop10(triplets);
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
