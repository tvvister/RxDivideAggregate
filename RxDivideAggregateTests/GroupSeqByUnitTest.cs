using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RxDivideAggregate;

namespace RxSplitUnionUnitTests
{
    [TestFixture]
    public class GroupSeqByUnitTest
    {
        [TestCase(new[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, }, ExpectedResult = new [] { 1,2,3,4,5 })]
        [TestCase(new [] { 1,2,3,4,5 }, ExpectedResult = new[] { 1, 1, 1, 1, 1 })]
        [TestCase(new [] { 0, 0, 1, 0, 0, 1, 4, 0 }, ExpectedResult = new[] { 2, 1, 2, 1, 1, 1})]
        [TestCase(new [] { 0, 0, 1, 0, 0, 1, 4 }, ExpectedResult = new[] { 2, 1, 2, 1, 1})]
        [TestCase(new [] { 0, 0, 1 }, ExpectedResult = new[] { 2, 1 })]
        [TestCase(new [] { 0, 0 }, ExpectedResult = new[] { 2 })]
        [TestCase(new [] { 0 }, ExpectedResult = new[] { 1 })]
        [TestCase(new [] { 1, 1 }, ExpectedResult = new[] { 2 })]
        [TestCase(new [] { 0, 0, 0 }, ExpectedResult = new[] { 3 })]
        [TestCase(new []{1}, ExpectedResult = new []{1})]
        [TestCase(new int[0], ExpectedResult = new int[0])]
        [TestCase(null, ExpectedResult = new int[0])]
        public int[] CountGroupSeqByTest(int[] inputArray)
        {
            return inputArray.GroupSeqBy(x => x).Select(x => x.Count()).ToArray();
        }

        [TestCase(new [] {1,2,2,3,3,3,4,4,4,4,5,5,5,5,5}, ExpectedResult = new [] {1,2,3,4,5})]
        public int[] GroupKeysGroupSeqByTest(int[] inputArray)
        {
            return inputArray.GroupSeqBy(x => x).Select(x => x.Key).ToArray();
        }

        [TestCase(new[] { 0, 1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 0 }, ExpectedResult = "0|1|2,2|3,3,3|4,4,4,4|5,5,5,5,5|0")]
        [TestCase(new[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5 }, ExpectedResult = "1|2,2|3,3,3|4,4,4,4|5,5,5,5,5")]
        [TestCase(new[] { 0,1,0,1,0,1,0,1,0,1,11}, ExpectedResult = "0|1|0|1|0|1|0|1|0|1|11")]
        [TestCase(new[] { 0, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = "0,0,0,0,0,0,0,0")]
        [TestCase(new int[0], ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public String ValuesGroupSeqByTest(int[] inputArray)
        {
            return StringJoin(inputArray.GroupSeqBy(x => x));
        }

        private static string StringJoin(IEnumerable<IGrouping<int, int>> groupSeqBy)
        {
            return String.Join("|",
                groupSeqBy.Select(gr => String.Join(",", gr.Select(x => x.ToString()))));
        }

        [TestCase(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, ExpectedResult = "1|2,3|4,5|6,7|8,9|10")]
        public String OrderGroupSeqByTest(int[] inputArray)
        {
            return StringJoin(inputArray.GroupSeqBy(x => x / 2));
        }


    }
}
