using System;
using System.Collections.Generic;
using CustomMusic.Harmony.Adapters;

namespace CustomMusic.Harmony
{
    public static class Services
    {
        private static readonly Dictionary<Type, Delegate> Database =
            new Dictionary<Type, Delegate>();

        public static void Add<TService>(Func<object[], TService> provider)
        {
            Database[typeof(TService)] = provider;
        }

        public static TService Resolve<TService>(params object[] args)
        {
            if (Database.TryGetValue(typeof(TService), out var provider))
            {
                return ((Func<object[], TService>)provider)(args);
            }

            throw new Exception($"Service {typeof(TService)} not registered");
        }

        public static void Initialise()
        {
            Add<IHarmony>((args) => new Adapters.Harmony());
        }
    }
}