using System;
using System.Linq;
using System.Reactive.Linq;
using RxDivideAggregate;

namespace MainApplication
{
    class Program
    {
        private const int FiresCount = 1;
        private const int ReportSize = 1;

        private static readonly FireId[] FireIds = Enumerable.Range(0, FiresCount)
            .Select(x => new FireId(x + 1))
            .ToArray();

        private static readonly TimeSpan EventRaisingInterval = TimeSpan.FromSeconds(1);

        static void Main(string[] args)
        {
            var rand = new Random();
            var inputRawDataSource = Observable.Interval(EventRaisingInterval)
                .Select(x =>
                {
                    var fireStates = Enumerable.Range(0, ReportSize)
                        .Select(i => rand.Next(1, FiresCount))
                        .Distinct()
                        .Select(fireId => new FireId(fireId))
                        .ToDictionary(id => id, id => new FireState(DateTime.UtcNow, (State)rand.Next(1, 3)));
                    return new FireStates(fireStates);
                });

            inputRawDataSource = inputRawDataSource.Take(30).Publish().RefCount();

            var validFireStream = inputRawDataSource
                .DivideAggregate(
                    splitFunc: fireStates =>
                        fireStates.States.Select(fs => new { FireId = fs.Key, FireState = fs.Value }).ToArray(),
                    groupSelector: simpleInput => simpleInput.FireId,
                    predicateFunc: fireStream =>
                        fireStream.Buffer(2, 1).Where(arr => arr.Count == 2).Select(fs => fs.First().FireState.State == fs.Last().FireState.State),
                    unionFunc: states => new FireStates(states.ToDictionary(x => x.FireId, x => x.FireState))
                );

            inputRawDataSource.Subscribe(Console.WriteLine);
            validFireStream.Subscribe(@event => Console.WriteLine("\t" + @event));
            Console.ReadLine();
        }
    }
}
