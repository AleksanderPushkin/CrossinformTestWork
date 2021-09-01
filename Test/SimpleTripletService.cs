using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Test
{
    public class SimpleTripletService: ITripletService
    {
        private string Text { get; set; }
        public SimpleTripletService()
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

            CleanText();

            Dictionary<string, int> triplets = new Dictionary<string, int>();
            var words = Text.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(var word in words)
            {
                var letters = word.ToCharArray();
                if (letters.Length <= 2) continue;
                for (int i = 0; i < letters.Length - 2; i++)
                {
                    var triplet = string.Format("{0}{1}{2}",letters[i], letters[i + 1] , letters[i + 2]);
                    if (!triplets.ContainsKey(triplet))
                    {
                        triplets.Add(triplet, 1);
                    }
                    else
                    {
                        triplets[triplet] = triplets[triplet]+ 1;
                    }
                }
            }
           

            return  GetTop10(triplets);
        }

        private void CleanText()
        {
            Text = Regex.Replace(Text, @"\W|[0-9]|\r|\n|\t", " ");
        }

        private List<string> GetTop10(Dictionary<string, int> triplets)
        {
            return triplets.OrderByDescending(t => t.Value).ThenByDescending(t => t.Key).Take(10).Select(t => t.Key).ToList();
        }
    }
}
