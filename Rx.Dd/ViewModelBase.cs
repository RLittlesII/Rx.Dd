using Sextant;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rx.Dd
{
    public class ViewModelBase : IViewModel, INavigable
    {
        private ISubject<INavigationParameter> _whenNavigatedTo = new Subject<INavigationParameter>();
        private ISubject<INavigationParameter> _whenNavigatedFrom = new Subject<INavigationParameter>();
        private ISubject<INavigationParameter> _whenNavigatingTo = new Subject<INavigationParameter>();

        public ViewModelBase() { }
        public string Id { get; }

        public IObservable<INavigationParameter> NavigatedTo => _whenNavigatedTo.AsObservable();
        public IObservable<INavigationParameter> NavigatedFrom => _whenNavigatedFrom.AsObservable();
        public IObservable<INavigationParameter> NavigatingTo => _whenNavigatingTo.AsObservable();

        IObservable<Unit> INavigated.WhenNavigatedTo(INavigationParameter parameter)
        {
            _whenNavigatedTo.OnNext(parameter);
            return Observable.Return(Unit.Default);
        }

        IObservable<Unit> INavigated.WhenNavigatedFrom(INavigationParameter parameter)
        {

            _whenNavigatedFrom.OnNext(parameter);
            return Observable.Return(Unit.Default);
        }

        IObservable<Unit> INavigating.WhenNavigatingTo(INavigationParameter parameter)
        {
            _whenNavigatingTo.OnNext(parameter);
            return Observable.Return(Unit.Default);
        }
    }
}