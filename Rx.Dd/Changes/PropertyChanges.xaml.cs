using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rx.Dd.Changes
{
    public partial class PropertyChanges
    {
        public PropertyChanges()
        {
            InitializeComponent();

            this.WhenActivated(
                disposables =>
                {
                    this.WhenAnyValue(view => view.ViewModel.Heroes)
                       .BindTo(this, x => x.HeroList.ItemsSource)
                       .DisposeWith(disposables);

                    this.WhenAnyValue(x => x.ViewModel)
                       .Where(x => x != null)
                       .Select(x => Unit.Default)
                       .ObserveOn(RxApp.TaskpoolScheduler)
                       .InvokeCommand(this, x => x.ViewModel.Initialize)
                       .DisposeWith(disposables);
                }
            );
        }
    }
}