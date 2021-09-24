using System;
using ReactiveUI;
using Rx.Dd.Filter;
using Sextant;
using Sextant.XamForms;
using Splat;
using Xamarin.Forms;

namespace Rx.Dd
{
    public class Startup
    {
        public Startup(IDependencyResolver dependencyResolver = null)
        {
            if (dependencyResolver == null)
            {
                dependencyResolver = new ModernDependencyResolver();
            }

            RxApp.DefaultExceptionHandler = new ExceptionHandler();
            RegisterServices(dependencyResolver);
            RegisterViewModels(dependencyResolver);
            RegisterViews(dependencyResolver);
            Build(dependencyResolver);
        }

        private void Build(IDependencyResolver dependencyResolver) =>
            Locator.SetLocator(dependencyResolver);

        private void RegisterViews(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterView<MainPage, MainViewModel>();
            dependencyResolver.RegisterView<Filters, FiltersViewModel>();
        }

        private void RegisterViewModels(IDependencyResolver dependencyResolver) => dependencyResolver.RegisterViewModel<MainViewModel>();

        private void RegisterServices(IDependencyResolver dependencyResolver)
        {
            var navigationView = new NavigationView();
            dependencyResolver.RegisterNavigationView(() => navigationView);
            dependencyResolver.RegisterLazySingleton<IParameterViewStackService>(() => new ParameterViewStackService(navigationView));
            dependencyResolver.RegisterLazySingleton<IViewModelFactory>(() => new DefaultViewModelFactory());
            dependencyResolver.InitializeReactiveUI();
        }

        public Page NavigateToStart<T>()
            where T : IViewModel
        {
            Locator.Current.GetService<IParameterViewStackService>().PushPage<T>().Subscribe();
            return Locator.Current.GetNavigationView(nameof(NavigationView));
        }
    }
}