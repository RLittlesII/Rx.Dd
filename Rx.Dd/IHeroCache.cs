using DynamicData;
using Rx.Dd.Data;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd
{
    public interface IHeroCache : IObservable<IChangeSet<Hero, string>> { }

    public class HeroCache : IHeroCache
    {
        public HeroCache(IHeroApiClient heroApiClient) =>
            _heroApiClient = heroApiClient;

        public IDisposable Subscribe(IObserver<IChangeSet<Hero, string>> observer)
        {
            var clientDisposable = _heroApiClient.GetHeroes().Select(x => x.Select(Convert)).Subscribe(heroes => _heroCache.EditDiff(heroes, (first, second) => first.Id == second.Id));
            var cacheDisposable = _heroCache.Connect().RefCount().Subscribe(observer);

            return Disposable.Create(
                () =>
                {
                    clientDisposable.Dispose();
                    cacheDisposable.Dispose();
                }
            );
        }

        private static Hero Convert(SuperHeroRecord hero)
        {
            var groupAffiliation = hero.Connections.GroupAffiliation;
            var items = groupAffiliation.Split(new char[] { ';', ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
            var teams = items.Select(z => z.IndexOf('(', StringComparison.OrdinalIgnoreCase) > -1 ? z.Substring(0, z.IndexOf('(', StringComparison.OrdinalIgnoreCase)) : z)
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
                AvatarUrl = hero.Image?.Url.ToString(),
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

        private readonly SourceCache<Hero, string> _heroCache = new SourceCache<Hero, string>(x => x.Id);

        private readonly IHeroApiClient _heroApiClient;
    }
}