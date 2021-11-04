using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchesCounter
{
    internal interface IMatchable
    {
        bool Match(int value);
    }

    internal class SingleNumberPattern : IMatchable
    {
        public SingleNumberPattern(int value) => Value = value;

        public int Value { get; }

        public bool Match(int value) => Value == value;
    }

    internal class RangePattern : IMatchable
    {
        public RangePattern(int from, int to) => (From, To) = (from, to); // don't add validation that (from < to) intentionally so input data should be correct here

        public int From { get; }
        public int To { get; }

        public bool Match(int value) => value >= From && value <= To;
    }

    public class Program
    {
        static void Main()
        {
            var list = new[] { 5, 4, 3, 1, 9, 5, 9, 6, 3 };

            Console.WriteLine(countMatches(list, "5")); // will return 2
            Console.WriteLine(countMatches(list, "3 - 8")); // will return 6
            Console.WriteLine(countMatches(list, "3, 5, 8")); // will return 4
            Console.WriteLine(countMatches(list, "3-5, 7-9, 12")); // will NOT! return 8, will return 7 instead
        }

        private static int countMatches(int [] values, string searchString) // I would name it from capital C for consistency
        {
            /*
             * Extra question: how would you change your solution if you knew that the values list is very long (100000 values) and that multiple searches will be applied on the same list?
             * What other considerations will need to be taken to decide on the most efficient solution?
             * Answer:
             * It depends on the input data.
             * We can start by applying "relaxations" to our input patterns: "1-3,2-5,1,2,12,3,4,5" => "1-5,12". This will definitely cut big number of useless checks.
             * After such relaxations we'll be able to go further and use BTree like DBs do. It would be nice to build BTree just from searchString so these "relaxations" will be applied just in time.
             * 
             * Another way to improve the algorithm, which is simplier that the first one to implement, is to make HashSet from searchString.
             * For example, pattern "1-5,2-7,12,4" will be HashSet<int> with values [1,2,3,4,5,6,7,12], after we do so we'll be able to any single number match by O(1). I place this solution as second because of extra memory usage.
             * There're probably better solutions, need more time.
             */

            // TODO: add validation. Btw, is such pattern "-10--3" possible? From -10 to -3, if yes - rework.
            var searchPatterns = searchString.Split(',');
            List<IMatchable> patterns = new List<IMatchable>();

            foreach(var searchPattern in searchPatterns)
            {
                if (searchPattern.Contains("-"))
                {
                    var boundaries = searchPattern.Split('-');
                    var range = new RangePattern(int.Parse(boundaries[0]), int.Parse(boundaries[1]));
                    patterns.Add(range);
                }
                else if (int.TryParse(searchPattern, out var value))
                {
                    var number = new SingleNumberPattern(value);
                    patterns.Add(number);
                }
                else
                {
                    throw new ArgumentException($"Uknown pattern type '{searchPattern}'");
                }
            }

            return values.Count(v => patterns.Any(p => p.Match(v)));
        }
    }
}
