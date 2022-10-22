using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd.Forms.Sort
{
    public partial class Sorting
    {
        public Sorting()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.WhenAnyValue(view => view.ViewModel.Teams)
                       .BindTo(this, x => x.Teams.ItemsSource)
                       .DisposeWith(disposables);

                    this.WhenAnyValue(view => view.ViewModel.Heroes)
                       .BindTo(this, x => x.HeroList.ItemsSource)
                       .DisposeWith(disposables);

                    this.Bind(ViewModel, x => x.SelectedTeam, x => x.Teams.SelectedItem)
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