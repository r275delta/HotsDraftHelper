using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesDrafter
{
    public sealed class HeroAdjustments
    {
        private readonly Context _context;
        private readonly Dictionary<Hero, double> _synergies = new Dictionary<Hero, double>();
        public IReadOnlyDictionary<Hero, double> Synergies => _synergies;
        private readonly Dictionary<Hero, double> _counters = new Dictionary<Hero, double>();
        public IReadOnlyDictionary<Hero, double> Counters => _counters;
        private readonly Dictionary<Map, double> _maps = new Dictionary<Map, double>();
        public IReadOnlyDictionary<Map, double> Maps => _maps;

        public HeroAdjustments(Context context)
        {
            _context = context;
        }

        public HeroAdjustments Add(
            double baseWinrate,
            IEnumerable<Tuple<Hero, double>> synergyWinrates = null,
            IEnumerable<Tuple<Hero, double>> counterWinrates = null,
            IEnumerable<Tuple<Map, double>> mapWinrates = null)
        {
            foreach (var synergyWinrate in synergyWinrates ?? Enumerable.Empty<Tuple<Hero, double>>())
            {
                if (!_synergies.ContainsKey(synergyWinrate.Item1))
                    _synergies[synergyWinrate.Item1] = _context.WinrateAdjuster.GetAdjustment(baseWinrate, synergyWinrate.Item2);
            }

            foreach (var counterWinrate in counterWinrates ?? Enumerable.Empty<Tuple<Hero, double>>())
            {
                if (!_counters.ContainsKey(counterWinrate.Item1))
                    _counters[counterWinrate.Item1] = _context.WinrateAdjuster.GetAdjustment(baseWinrate, counterWinrate.Item2);
            }

            foreach (var mapWinrate in mapWinrates ?? Enumerable.Empty<Tuple<Map, double>>())
            {
                if (_maps.ContainsKey(mapWinrate.Item1))
                    _maps[mapWinrate.Item1] = _context.WinrateAdjuster.GetAdjustment(baseWinrate, mapWinrate.Item2);
            }

            return this;
        }

        public HeroAdjustments Add(HeroAdjustments other)
        {
            if (_context != other._context)
                throw new Exception("Context mismatch");

            foreach (var kvp in other.Synergies)
            {
                if (!_synergies.ContainsKey(kvp.Key))
                    _synergies[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in other.Counters)
            {
                if (!_counters.ContainsKey(kvp.Key))
                    _counters[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in other.Maps)
            {
                if (!_maps.ContainsKey(kvp.Key))
                    _maps[kvp.Key] = kvp.Value;
            }

            return this;
        }
    }
}
