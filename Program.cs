using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using qubeyond_test.Domain;
using System.Diagnostics;

namespace qubeyond_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Set Matrix File Path(E.g.: C:\qubeyond_test\matrix.txt):");
            string matrixPath = Console.ReadLine();

            Console.WriteLine(@"Set Words to find File Path(E.g.: C:\qubeyond_test\words.txt):");
            string wordsPath = Console.ReadLine();

            IEnumerable<string> words = LoadMatrixFile(matrixPath);
            IEnumerable<string> wordsToFind = LoadMatrixFile(wordsPath);

            string firstWord = wordsToFind.FirstOrDefault();

            var sameSize = wordsToFind.All(
                    (item) =>
                        item.Length == firstWord.Length ? true : false);

            if (sameSize)
            {
                WordFinder wordFinder = new WordFinder(words);
                var timer = System.Diagnostics.Stopwatch.StartNew();
                IEnumerable<string> total = wordFinder.Find(wordsToFind);
                timer.Stop();
                Console.WriteLine("Total time: " + timer.ElapsedMilliseconds.ToString());

                List<WordOcurrence> result = total.GroupBy(x => x).Select(x => new WordOcurrence
                {
                    Word = x.Key,
                    Ocurrence = x.Count()
                }).OrderByDescending(x => x.Ocurrence).ToList();

                result.ForEach(x =>
                {
                    Console.WriteLine(string.Format("Word: '{0}' - Total: '{1}'", x.Word, x.Ocurrence));
                });
            }
            else
            {
                Console.WriteLine("Words must be the same length.");
            }
        }

        public static IEnumerable<string> LoadMatrixFile(string path)
        {
            List<string> words = new List<string>();
            if (File.Exists(path))
            {
                IEnumerable<string> lines = File.ReadLines(path);
                if (lines != null && lines.Any())
                {
                    lines = lines.Select(s => s.ToLowerInvariant());
                    words.AddRange(lines);
                }
                else
                {
                    Console.WriteLine("File is empty.");    
                }
            }
            else
            {
                Console.WriteLine("Cannot find file.");
            }

            return words;
        }

        public static IEnumerable<string> LoadWordsFile(string path)
        {
            List<string> words = new List<string>();
            if (File.Exists(path))
            {
                IEnumerable<string> lines = File.ReadLines(path);
                if (lines != null && lines.Any())
                {
                    lines = lines.Select(s => s.ToLowerInvariant());
                    words.AddRange(lines);
                }
                else
                {
                    Console.WriteLine("File is empty.");    
                }
            }
            else
            {
                Console.WriteLine("Cannot find file.");
            }

            return words;
        }
    }
}
