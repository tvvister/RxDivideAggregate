    public static class ObservableExt
    {
        public static LatestObservable<T> GetLatestObservable<T>(this IObservable<T> source)
        {
            return new LatestObservable<T>(source);
        }
    }

    public class LatestObservable<T> : IObservable<T>, IDisposable {
        
        private readonly IObservable<T> _callObservable;
        private readonly Subject<Unit> _callSubject;
        private readonly IDisposable _subscription;

        internal LatestObservable(IObservable<T> source)
        {
            _callSubject = new Subject<Unit>();
            _callObservable = _callSubject
                .WithLatestFrom(source.StartWith(default(T)), (_, x) => x)
                .Publish()
                .RefCount();

            _subscription = _callObservable.Subscribe();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _callObservable.Subscribe(observer);
        }

        public async Task<T> GetLatest()
        {
            T res;
            do
            {
                var task = _callObservable.FirstAsync().ToTask();
                _callSubject.OnNext(Unit.Default);
                res = await task;
            } while (res.Equals(default(T)));
            // _callSubject.OnNext(Unit.Default);

            return res;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
