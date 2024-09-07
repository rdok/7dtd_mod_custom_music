using System;
using System.Collections.Generic;
using CustomMusic.Harmony.Adapters;
using CustomMusic.Harmony.Volume;

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

        public static TService Get<TService>(params object[] args)
        {
            if (Database.TryGetValue(typeof(TService), out var provider))
            {
                return ((Func<object[], TService>)provider)(args);
            }

            throw new Exception($"Service {typeof(TService)} not registered");
        }

        public static void Initialise()
        {
            var logger = new Logger();

            Add<IHarmonyAdapter>(args => new HarmonyAdapter());
            Add<IVolumeAnalyzer>(args => new VolumeAnalyzer(logger));
            Add<IVolumeAdjuster>(args => new VolumeAdjuster(logger));
        }
    }
}