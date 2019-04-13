using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using RxDivideAggregate;
using Assert = NUnit.Framework.Assert;

namespace RxSplitUnionUnitTests
{
    [TestFixture]
    public class SimpleOverloadSplitUnionMethodUnitTest
    {
        [TestCase(new[] { 1 }, ExpectedResult = new int[0])]
        public int[] SingleEventTest(int[] inputs)
        {
            var rawDataSource = inputs.ToObservable();
            return ApplySplitUnionForFilterInGroup(rawDataSource);
        }

        [TestCase(new[] { 1, 1, 2, 3, 4, 5, 6, 7, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 17, 18, 19, 20, 20, 20 })]
        public void OnlyTwoConsecutiveReportsTest(int[] inputs)
        {
            var rawDataSource = inputs.ToObservable();
            var results = ApplySplitUnionForFilterInGroup(rawDataSource);
            Assert.AreEqual(results, inputs.Zip(inputs.Skip(1), (c, p) => new {c, skip = c != p}).Where(x => !x.skip).Select(x =>x.c).ToArray());
        }

        private static int[] ApplySplitUnionForFilterInGroup(IObservable<int> rawDataSource)
        {
            return rawDataSource.DivideAggregate(
                    value => new[] {value},
                    item => 1, //in simple test we need only group
                    dataSource => dataSource
                        .Buffer(2, 1)
                        .Select(consecutivePair =>
                        {
                            return consecutivePair.Count == 2 && consecutivePair.First() == consecutivePair.Last();
                        }), //analysis for separated group,
                    data => data.First()) //trivial in this simple test
                .ToEnumerable()
                .ToArray();
        }
    }
}
