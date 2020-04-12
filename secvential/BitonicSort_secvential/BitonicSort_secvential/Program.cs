using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace BitonicSort_secvential
{
    public class Program
    {
        static readonly string textFile = "C:\\Users\\Teo Dora\\BitonicSort_secvential\\BitonicSort_secvential\\numbers.txt";
        static string wrtierFile = "C:\\Users\\Teo Dora\\BitonicSort_secvential\\BitonicSort_secvential\\out.txt";
        public static void Main()
        {
            List<int> numbers = new List<int>();
            long totalTicks = 0;

            numbers = ReadFromFile();
            if (numbers == null)
            {
                Console.Write("File is empty");
                return;
            }

            int N = numbers.Count;

            Stopwatch timer = Stopwatch.StartNew();

            Sort(numbers, N);
                
            timer.Stop();
            totalTicks += timer.ElapsedTicks;
            TimeSpan span = new TimeSpan(totalTicks);
            WriteToFile(numbers, span);
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
                        catch(FormatException e)
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
                    writer.WriteLine("Sequential Bitonic Sorter execution time ->  {0} ms", span.TotalMilliseconds);
                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", e.Message);
            }
        }


        /* Wrapper funtion: Calls the bitonicSort for sorting the given sequence of  
       length N in ASCENDING order */
        public static List<int> Sort(List<int> a, int N)
        {   
            BitonicSort(a, 0, N, 1);
            return a;
        }


        /*
         Compares the numbers and swaps them in case is needed
         Input: a - list of integers
                i - integer, position from the first half of the list
                j - integer, position from the second half of the list
                dir - integer, 0 or 1. 0 is descending. 1 is ascending
         */
        public static void CompAndSwap(List<int> a, int i, int j, int dir)
        {
            int k;
            if ((a[i] > a[j]))
                k = 1;
            else
                k = 0;
            if (dir == k)
            {
                var fristEl = a.ElementAt(i);
                var secondEl = a.ElementAt(j);
                a[j] = fristEl;
                a[i] = secondEl;
            }
        }

        /*It recursively sorts a bitonic sequence in ascending order,  
          if dir = 1, and in descending order otherwise (means dir=0).  
          The sequence to be sorted starts at index position low,  
          the parameter cnt is the number of elements to be sorted.*/
        public static void BitonicMerge(List<int> a, int low, int cnt, int dir)
        {
            if (cnt > 1)
            {
                int k = cnt / 2;
                for (int i = low; i < low + k; i++)
                    CompAndSwap(a, i, i + k, dir);
                BitonicMerge(a, low, k, dir);
                BitonicMerge(a, low + k, k, dir);
            }
        }

        /* This function first produces a bitonic sequence by recursively  
            sorting its two halves in opposite sorting orders, and then  
            calls bitonicMerge to make them in the same order */
        public static void BitonicSort(List<int> a, int low, int cnt, int dir)
        {
            if (cnt > 1)
            {
                int k = cnt / 2;

                // sort in ascending order since dir here is 1  
                BitonicSort(a, low, k, 1);

                // sort in descending order since dir here is 0  
                BitonicSort(a, low + k, k, 0);

                // Will merge wole sequence in ascending order  
                // since dir=1.  
                BitonicMerge(a, low, cnt, dir);
            }
        }

    }
}

