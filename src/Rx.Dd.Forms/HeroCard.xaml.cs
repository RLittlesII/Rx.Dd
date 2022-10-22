using ReactiveUI;
using System.Reactive.Disposables;

namespace Rx.Dd.Forms
{
    public partial class HeroCard
    {
        public HeroCard()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.OneWayBind(ViewModel, hero => hero.AvatarUrl, heroCard => heroCard.Avatar.Source, x => x.ToString()).DisposeWith(disposables);
                    this.OneWayBind(ViewModel, hero => hero.Name, heroCard => heroCard.Name.Text, x => x.ToString()).DisposeWith(disposables);
                });
        }
    }
}