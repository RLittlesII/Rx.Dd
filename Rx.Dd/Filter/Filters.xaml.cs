using ReactiveUI;
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
                });
        }
    }
}