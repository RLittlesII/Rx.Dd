using ReactiveUI;
using System.Reactive.Disposables;

namespace Rx.Dd.Forms.Filter
{
    public partial class Search
    {
        public Search()
        {
            InitializeComponent();
            this.WhenActivated(
                disposables =>
                {
                    this.Bind(ViewModel, x => x.SearchText, x => x.SearchText.Text)
                       .DisposeWith(disposables);
                }
            );
        }
    }
}