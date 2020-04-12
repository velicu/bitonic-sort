using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitonicSort_secvential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitonicSort_secvential.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public static void SortTest()
        {
            List<int> expected = new List<int>
            {
                1,
                8,
                10,
                100
            };
            List<int> actual = new List<int>();
            expected.Add(100);
            expected.Add(1);
            expected.Add(10);
            expected.Add(8);
            Program.BitonicSort(actual, 0, 4, 1);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}