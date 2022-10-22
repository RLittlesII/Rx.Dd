using ReactiveUI;
using System.Reactive.Disposables;

namespace Rx.Dd.Forms
{
    public partial class EditableHeroCard
    {
        public EditableHeroCard()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.OneWayBind(ViewModel, x => x.AvatarUrl, x => x.Avatar.Source, x => x.ToString()).DisposeWith(disposables);
                    this.OneWayBind(ViewModel, x => x.Name, x => x.Name.Text).DisposeWith(disposables);
                    this.Bind(ViewModel, x => x.RealName, x => x.RealName.Text).DisposeWith(disposables);
                });
        }
    }
}