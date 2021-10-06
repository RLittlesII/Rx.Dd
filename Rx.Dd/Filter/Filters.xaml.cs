using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Rx.Dd.Filter
{
    public partial class Filters
    {
        public Filters()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.WhenAnyValue(view => view.ViewModel.Alignments)
                       .BindTo(this, x => x.Alignment.ItemsSource)
                       .DisposeWith(disposables);

                    this.WhenAnyValue(view => view.ViewModel.Heroes)
                       .BindTo(this, x => x.HeroList.ItemsSource)
                       .DisposeWith(disposables);

                    this.Bind(ViewModel, x => x.SelectedAlignment, x => x.Alignment.SelectedItem)
                       .DisposeWith(disposables);

                    this.WhenAnyValue(x => x.ViewModel)
                       .Where(x => x != null)
                       .Select(x => Unit.Default)
                       .ObserveOn(RxApp.TaskpoolScheduler)
                       .InvokeCommand(this, x => x.ViewModel.Initialize)
                       .DisposeWith(disposables);
                });
        }
    }
}