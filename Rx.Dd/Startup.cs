using System;
using ReactiveUI;
using Refit;
using Rx.Dd.Data;
using Rx.Dd.Filter;
using Rx.Dd.Sort;
using Sextant;
using Sextant.XamForms;
using Splat;
using System.Net.Http;
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

        private static void Build(IDependencyResolver dependencyResolver) =>
            Locator.SetLocator(dependencyResolver);

        private static void RegisterViews(IDependencyResolver dependencyResolver) => dependencyResolver.RegisterView<MainPage, MainViewModel>()
           .RegisterView<Filters, FiltersViewModel>()
           .RegisterView<Search, SearchViewModel>()
           .RegisterView<Sorting, SortingViewModel>()
           .RegisterView<Search, SearchViewModel>();

        private static void RegisterViewModels(IDependencyResolver dependencyResolver) => dependencyResolver
           .RegisterViewModel(new MainViewModel(dependencyResolver.GetService<IParameterViewStackService>()))
           .RegisterViewModel(new FiltersViewModel(dependencyResolver.GetService<IHeroCache>()))
           .RegisterViewModel(new SearchViewModel(dependencyResolver.GetService<IHeroApiClient>()))
           .RegisterViewModel(new SortingViewModel(dependencyResolver.GetService<IHeroCache>()));

        private static void RegisterServices(IDependencyResolver dependencyResolver)
        {
            var navigationView = new NavigationView();
            dependencyResolver.RegisterNavigationView(() => navigationView);
            dependencyResolver.RegisterLazySingleton<IParameterViewStackService>(() => new ParameterViewStackService(navigationView));
            dependencyResolver.RegisterLazySingleton<IViewModelFactory>(() => new DefaultViewModelFactory());

            dependencyResolver.Register<ISuperheroApiContract>(() => RestService.For<ISuperheroApiContract>("https://www.superheroapi.com/api/{accesstoken}/", new RefitSettings()));
            dependencyResolver.Register<IHeroApiClient>(() => new HeroApiClient(dependencyResolver.GetService<ISuperheroApiContract>()));
            dependencyResolver.RegisterLazySingleton<IHeroCache>(() => new HeroCache(dependencyResolver.GetService<IHeroApiClient>()));
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