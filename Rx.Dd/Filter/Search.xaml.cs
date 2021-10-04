using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rx.Dd.Filter
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