using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace RxDivideAggregate
{
    public static class RxDivideAggregateExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregatedInput"></typeparam>
        /// <typeparam name="TAggregatedOutput"></typeparam>
        /// <typeparam name="TSimpleInput"></typeparam>
        /// <typeparam name="TSimpleOutput"></typeparam>
        /// <typeparam name="TGroup"></typeparam>
        /// <param name="source"></param>
        /// <param name="splitFunc"></param>
        /// <param name="concatKeySelector"></param>
        /// <param name="predicateFunc"></param>
        /// <param name="convertFunc"></param>
        /// <param name="aggregateFunc"></param>
        /// <returns></returns>
        internal static IObservable<TAggregatedOutput> DivideAggregate<TAggregatedInput, TAggregatedOutput, TSimpleInput,
            TSimpleOutput, TGroup>(
            this IObservable<TAggregatedInput> source,
            Func<TAggregatedInput, IReadOnlyList<TSimpleInput>> splitFunc,
            Func<TSimpleInput, TGroup> concatKeySelector,
            Func<IObservable<TSimpleInput>, IObservable<bool>> predicateFunc,
            Func<IObservable<TSimpleInput>, IObservable<TSimpleOutput>> convertFunc,
            Func<IReadOnlyList<TSimpleOutput>, TAggregatedOutput> aggregateFunc)
        {
            var splitStream = source.SelectMany((ci, number) =>
                {
                    var inputs = splitFunc(ci);
                    return inputs
                        .Select(si => new
                        {
                            SimpleInput = si,
                            CompositeInputNumber = number,
                            inputs.Count
                        });
                })
                .GroupBy(si => concatKeySelector(si.SimpleInput));

            var simpleOutputStream = splitStream.Select(concatGroup =>
            {
                var sharedConcatGroup = concatGroup.Publish().RefCount();
                var predicateStream = predicateFunc(sharedConcatGroup.Select(si => si.SimpleInput));
                var convertStream = convertFunc(sharedConcatGroup.Select(si => si.SimpleInput));

                return sharedConcatGroup.Zip(predicateStream, convertStream, (si, pred, conv) =>
                    new
                    {
                        SimpleOutput = conv,
                        si.Count,
                        si.CompositeInputNumber,
                        Predicate = pred
                    }
                );
            }).Merge();

            return simpleOutputStream.GroupBy(so => new {so.CompositeInputNumber, so.Count})
                .Select(outputGroup =>
                    outputGroup
                        .Take(outputGroup.Key.Count)
                        .Where(so => so.Predicate)
                        .Select(so => so.SimpleOutput)
                        .Buffer(outputGroup.Key.Count)
                        .Select(simpleOutputs => aggregateFunc(simpleOutputs.ToArray())))
                .Merge();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregated"></typeparam>
        /// <typeparam name="TSimple"></typeparam>
        /// <typeparam name="TGroup"></typeparam>
        /// <param name="source"></param>
        /// <param name="splitFunc"></param>
        /// <param name="groupSelector"></param>
        /// <param name="predicateFunc"></param>
        /// <param name="unionFunc"></param>
        /// <returns></returns>
        public static IObservable<TAggregated> DivideAggregate<TAggregated, TSimple, TGroup>(
            this IObservable<TAggregated> source,
            Func<TAggregated, IReadOnlyList<TSimple>> splitFunc,
            Func<TSimple, TGroup> groupSelector,
            Func<IObservable<TSimple>, IObservable<bool>> predicateFunc,
            Func<IReadOnlyList<TSimple>, TAggregated> unionFunc)
        {
            return source.DivideAggregate(
                splitFunc,
                groupSelector,
                predicateFunc,
                convertFunc: x => x,
                aggregateFunc: unionFunc
            );
        }
    }
}
