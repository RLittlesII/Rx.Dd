using DynamicData;
using ReactiveUI;
using Rx.Dd.Data;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Rx.Dd.ViewModel
{
    public interface IHeroCache : IObservable<IChangeSet<Hero, string>>
    {
        IObservable<Unit> Load();
    }

    public class HeroCache : IHeroCache
    {
        public HeroCache(IHeroApiClient heroApiClient)
        {
            _apiClient = heroApiClient;

            _recordChangeSet = _heroCache.Connect().RefCount();
        }

        public IDisposable Subscribe(IObserver<IChangeSet<Hero, string>> observer) => _recordChangeSet.Subscribe(observer);

        public IObservable<Unit> Load() => Observable.Create<Unit>(
            observer => _apiClient.GetHeroes()
               .Subscribe(heroes =>
                    {
                        _heroCache.AddOrUpdate(heroes.Where(x => x.Response == "success").Select(Convert));
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    },
                    RxApp.DefaultExceptionHandler.OnNext));

        public static Hero Convert(SuperHeroRecord hero)
        {
                var groupAffiliation = hero.Connections.GroupAffiliation;
                var items = groupAffiliation.Split(new char[] { ';', ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
                var teams = items.Select(
                        z => z.IndexOf('(', StringComparison.OrdinalIgnoreCase) > -1 ? z.Substring(0, z.IndexOf('(', StringComparison.OrdinalIgnoreCase)) : z
                    )
                   .Where(z => !z.Contains("formerly", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("former", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("was once", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("of the", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("ally", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("and", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("Captain", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("None", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("Luke", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("Misty", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("Inc", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("leader", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("United States", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("the", StringComparison.OrdinalIgnoreCase))
                   .Where(z => !z.Contains("of America", StringComparison.OrdinalIgnoreCase))
                   .Select(z => z.Trim())
                   .Where(z => z.Length > 3)
                   .ToList();

                return new Hero
                {
                    Id = hero.Id,
                    Alignment = hero.Biography.Alignment,
                    AvatarUrl = hero.Image?.Url,
                    Created = DateTimeOffset.Now.ToString("u"),
                    Gender = hero.Appearance.Gender == "null" ? "Unknown" : hero.Appearance.Gender,
                    Race = hero.Appearance.Race == "null" ? "Unknown" : hero.Appearance.Race,
                    Powerstats = hero.Powerstats,
                    Name = hero.Name,
                    RealName = hero.Biography.FullName,
                    Publisher = hero.Biography.Publisher,
                    Teams = teams
                };
        }

        private readonly IHeroApiClient _apiClient;
        private readonly IObservable<IChangeSet<Hero, string>> _recordChangeSet;
        private readonly SourceCache<Hero, string> _heroCache = new SourceCache<Hero, string>(x => x.Id);
    }
}