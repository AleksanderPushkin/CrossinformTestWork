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
    public class MultiThreadTripletService: ITripletService
    {
        private string Text { get; set; }
        public MultiThreadTripletService()
        {
            
        }

        private void ReadText(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new FileNotFoundException(nameof(path));

            Text = System.IO.File.ReadAllText(path);
        }

        public  List<string> GetTop10Triplets(string path)
        {
            ReadText(path);
            CleanText();
            IDictionary<string, int> triplets = new ConcurrentDictionary<string, int>();
            var words = Text.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var tasks = new Task[words.Length];
            var _l = new object();
            for (int t = 0; t < words.Length; t++)
            {
                var word = words[t];
                tasks[t] = Task.Run(() =>
                {
                // This attempts to increase the concurrency.
                // Make sure that all Tasks start closer to approximately the same time.
                // Otherwise, some tasks might already be done when others just started.
                var letters = word.ToCharArray();
                    if (letters.Length > 2)
                    {
                        for (int i = 0; i < letters.Length - 2; i++)
                        {
                            var triplet = string.Format("{0}{1}{2}", letters[i], letters[i + 1], letters[i + 2]);
                            lock(_l){
                                if (!triplets.ContainsKey(triplet))
                                {
                                    triplets.Add(triplet, 1);
                                }
                                else
                                {
                                    triplets[triplet] = triplets[triplet] + 1;
                                }
                            }

                        }
                    }
                });
            }

            Task.WaitAll(tasks);
            return  GetTop10(triplets);
        }

        private List<string> GetTop10(IDictionary<string, int> triplets)
        {
            return triplets.OrderByDescending(t => t.Value).ThenByDescending(t => t.Key).Take(10).Select(t => t.Key).ToList();
        }

        private void CleanText()
        {
            Text = Regex.Replace(Text, @"\W|[0-9]|\r|\n|\t", " ");
        }
    }
}
