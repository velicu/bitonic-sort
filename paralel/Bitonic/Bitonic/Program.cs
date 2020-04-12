using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Bitonic
{
    class Program
    {
        static readonly string textFile = "C:\\Users\\Teo Dora\\BitonicSort_secvential\\BitonicSort_secvential\\numbers.txt";
        static string wrtierFile = "C:\\Users\\Teo Dora\\Bitonic\\Bitonic\\out.txt";
        static void Main(string[] args)
        {
            int iterations = 5;
            List<int>  numbers = new List<int>();

            numbers = ReadFromFile();
            if (numbers == null)
            {
                Console.Write("File is empty");
                return;
            }
            TestSpeed("Multi Threaded ", new MultiThreadedSorter(), numbers, iterations);
            Console.ReadLine();

        }

        /*
       Read from file:
       Output: numbers - list of integers
      */

        public static List<int> ReadFromFile()
        {
            List<int> a = new List<int>();
            try
            {
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);
                foreach (string line in lines)
                {
                    string sep = "\t";
                    string[] words = line.Split(' ');
                    foreach (var no in words)
                    {
                        try
                        {
                            a.Add(int.Parse(no));
                        }

                        catch (FormatException e)
                        {
                            Console.WriteLine("Error casting data from file to int {0}", e.Message);
                        }
                    }

                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", e.Message);
            }

            if (a.Count > 0)
            {
                return a;
            }
            return null;
        }

        /*
         Write to file
         Input: numbers - list of integers
                span - var of type TimeSpan. Gives the time in ms
        */

        public static void WriteToFile(List<int> numbers, TimeSpan span)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(wrtierFile))
                {
                    writer.Flush();
                    foreach (var number in numbers)
                    {
                        writer.Write(number);
                        writer.Write(' ');
                    }
                    writer.WriteLine();
                    writer.WriteLine("Sequential Bitonic Sorter execution time ->  {0} ms" , span.TotalMilliseconds);
                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", e.Message);
            }
        }

        static void TestSpeed(string testName, BaseBitonicSorter sorter, List<int> originalArray, int iterations)
        {
            long totalTicks = 0;
            List<int> data = originalArray;
            for (int i = 0; i < iterations; i++)
            {
                Stopwatch timer = Stopwatch.StartNew();
                sorter.Sort(ref data);
                timer.Stop();
                totalTicks += timer.ElapsedTicks;
            }
            foreach (var number in data)
            {
                Console.WriteLine(number.ToString());
            }
            TimeSpan span = new TimeSpan(totalTicks);
            Console.WriteLine(span.TotalMilliseconds);
            WriteToFile(data, span);
        }
    }

    public class BaseBitonicSorter
    {
        protected List<int> a;
        protected const bool Ascending = true, Descending = false;

        public virtual void Sort(ref List<int> array)
        {
        }

        protected void Compare(int src, int dst, bool direction)
        {
            if (direction == (a[src] > a[dst]))
            {
                Exchange(src, dst);
            }
        }

        protected void Exchange(int i, int j)
        {
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }
    }

    public class SingleThreadedBitonicSorter : BaseBitonicSorter
    {

        public override void Sort(ref List<int> array)
        {
            a = array;
            BitonicSort(0, a.Count, Ascending);
        }

        private void BitonicSort(int index, int length, bool direction)
        {
            if (length > 1)
            {
                int median = (length / 2);
                if (median > 1)
                {
                    BitonicSort(index, median, Ascending);
                    BitonicSort(index + median, median, Descending);
                }
                BitonicMerge(index, length, direction);
            }
        }

        private void BitonicMerge(int index, int length, bool direction)
        {
            if (length > 1)
            {
                int median = (length / 2);

                for (int i = index; i < (index + median); i++)
                {
                    Compare(i, (i + median), direction);
                }
                if (median > 1)
                {
                    BitonicMerge(index, median, direction);
                    BitonicMerge((index + median), median, direction);
                }
            }
        }
    }

    public class MultiThreadedSorter : BaseBitonicSorter
    {
        private int minimumLength = 1;

        public override void Sort(ref List<int> array)
        {
            a = array;
            minimumLength = a.Count / Environment.ProcessorCount;
            BitonicSort(new BitonicParameters(0, a.Count, Ascending));
        }

        private void BitonicSort(object AsyncState)
        {
            BitonicParameters parameters = (BitonicParameters)AsyncState;
            if (parameters.Length > 1)
            {
                if (parameters.Length > minimumLength)
                {
                    int median = parameters.Length / 2;
                    Thread left = new Thread(BitonicSort);
                    Thread right = new Thread(BitonicSort);
                    left.Start(new BitonicParameters(parameters.Index, median, Ascending));
                    right.Start(new BitonicParameters(parameters.Index + median, median, Descending));
                    left.Join();
                    right.Join();
                }
                else
                {
                    int median = (parameters.Length / 2);
                    if (median > 1)
                    {
                        BitonicSort(new BitonicParameters(parameters.Index, median, Ascending));
                        BitonicSort(new BitonicParameters(parameters.Index, median, Ascending));
                    }
                }
                BitonicMerge(new BitonicParameters(parameters.Index, parameters.Length, parameters.Direction));
            }
        }

        private void BitonicMerge(object AsyncState)
        {
            BitonicParameters parameters = (BitonicParameters)AsyncState;
            if (parameters.Length > 1)
            {
                if (parameters.Length > minimumLength)
                {
                    int median = (parameters.Length / 2);

                    for (int i = parameters.Index; i < (parameters.Index + median); i++)
                    {
                        Compare(i, (i + median), parameters.Direction);
                    }
                    Thread left = new Thread(BitonicMerge);
                    Thread right = new Thread(BitonicMerge);
                    left.Start(new BitonicParameters(parameters.Index, median, parameters.Direction));
                    right.Start(new BitonicParameters(parameters.Index + median, median, parameters.Direction));
                    left.Join();
                    right.Join();
                }
                else
                {
                    int median = (parameters.Length / 2);

                    for (int i = parameters.Index; i < (parameters.Index + median); i++)
                    {
                        Compare(i, (i + median), parameters.Direction);
                    }
                    if (median > 1)
                    {
                        BitonicMerge(new BitonicParameters(parameters.Index, median, parameters.Direction));
                        BitonicMerge(new BitonicParameters(parameters.Index + median, median, parameters.Direction));
                    }
                }
            }
        }

        private struct BitonicParameters
        {
            public BitonicParameters(int index, int length, bool direction)
            {
                Index = index;
                Length = length;
                Direction = direction;
            }
            public int Index;
            public int Length;
            public bool Direction;
        }
    }
}
