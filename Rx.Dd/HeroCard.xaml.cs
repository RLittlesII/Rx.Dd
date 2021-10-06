using ReactiveUI;
using System.Reactive.Disposables;

namespace Rx.Dd
{
    public partial class HeroCard
    {
        public HeroCard()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.OneWayBind(ViewModel, x => x.AvatarUrl, x => x.Avatar.Source, x => x.ToString()).DisposeWith(disposables);
                    this.OneWayBind(ViewModel, x => x.Name, x => x.Name.Text, x => x.ToString()).DisposeWith(disposables);
                });
        }
    }
}