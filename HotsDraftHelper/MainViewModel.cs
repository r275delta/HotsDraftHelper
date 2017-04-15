using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HotsDraftHelper.Data;

namespace HotsDraftHelper
{
    internal sealed class MainViewModel : ViewModelBase
    {
        private HeroesStatistics _heroesStatistics;

        private bool _ready = true;
        public bool Ready
        {
            get { return _ready; }
            private set
            {
                if (_ready == value)
                    return;
                _ready = value;
                CallerPropertyChanged();
            }
        }

        private string _loadError;
        public string LoadError
        {
            get { return _loadError; }
            private set
            {
                if (_loadError == value)
                    return;
                _loadError = value;
                CallerPropertyChanged();
            }
        }

        private IReadOnlyList<Map> _availableMaps = Array.Empty<Map>();
        public IReadOnlyList<Map> AvailableMaps
        {
            get { return _availableMaps; }
            private set
            {
                if (Enumerable.SequenceEqual(_availableMaps, value))
                    return;
                _availableMaps = value;
                CallerPropertyChanged();
            }
        }

        private Map _selectedMap;
        public Map SelectedMap
        {
            get { return _selectedMap; }
            set
            {
                if (_selectedMap == value)
                    return;
                _selectedMap = value;
                CallerPropertyChanged();
                Refresh();
            }
        }

        public ObservableCollection<HeroSelectionViewModel> AvailableAllies { get; } = new ObservableCollection<HeroSelectionViewModel>();
        public ObservableCollection<HeroSelectionViewModel> AvailableEnemies { get; } = new ObservableCollection<HeroSelectionViewModel>();
        public ObservableCollection<HeroSelectionViewModel> PickedAllies { get; } = new ObservableCollection<HeroSelectionViewModel>();
        public ObservableCollection<HeroSelectionViewModel> PickedEnemies { get; } = new ObservableCollection<HeroSelectionViewModel>();
        public ObservableCollection<Hero> Bans { get; } = new ObservableCollection<Hero>();

        private double? _theoWinRate;
        public double? TheoWinRate
        {
            get { return _theoWinRate; }
            private set
            {
                if (_theoWinRate == value)
                    return;
                _theoWinRate = value;
                CallerPropertyChanged();
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand BanCommand { get; }
        public ICommand UnbanCommand { get; }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(_ => DoLoad());
            ResetCommand = new RelayCommand(_ => ReInit());
            BanCommand = new RelayCommand(h => BanHero(h as Hero));
            UnbanCommand = new RelayCommand(h => UnbanHero(h as Hero));
        }

        private void BanHero(Hero hero)
        {
            if (hero == null)
                return;
            if (PickedAllies.Any(h => h.Hero == hero) ||
                PickedEnemies.Any(h => h.Hero == hero) ||
                Bans.Any(h => h == hero))
            {
                return;
            }
            for (int i = 0; i < AvailableAllies.Count; i++)
            {
                if (AvailableAllies[i].Hero == hero)
                {
                    AvailableAllies.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < AvailableEnemies.Count; i++)
            {
                if (AvailableEnemies[i].Hero == hero)
                {
                    AvailableEnemies.RemoveAt(i);
                    break;
                }
            }
            Bans.Add(hero);
        }

        private void UnbanHero(Hero hero)
        {
            if (hero == null)
                return;
            bool removed = false;
            for (int i = 0; i < Bans.Count; i++)
            {
                if (Bans[i] == hero)
                {
                    Bans.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
                return;
            AvailableAllies.Add(new HeroSelectionViewModel(hero));
            AvailableEnemies.Add(new HeroSelectionViewModel(hero));
            Refresh();
        }

        public void SelectAllyHero(Hero hero)
        {
            if (hero == null)
                return;
            bool removed = false;
            for (int i = 0; i < AvailableAllies.Count; i++)
            {
                if (AvailableAllies[i].Hero == hero)
                {
                    AvailableAllies.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
                return;
            for (int i = 0; i < AvailableEnemies.Count; i++)
            {
                if (AvailableEnemies[i].Hero == hero)
                {
                    AvailableEnemies.RemoveAt(i);
                    break;
                }
            }
            PickedAllies.Add(new HeroSelectionViewModel(hero));
            Refresh();
        }

        public void UnselectAllyHero(Hero hero)
        {
            if (hero == null)
                return;
            bool removed = false;
            for (int i = 0; i < PickedAllies.Count; i++)
            {
                if (PickedAllies[i].Hero == hero)
                {
                    PickedAllies.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
                return;
            AvailableAllies.Add(new HeroSelectionViewModel(hero));
            AvailableEnemies.Add(new HeroSelectionViewModel(hero));
            Refresh();
        }

        public void SelectEnemyHero(Hero hero)
        {
            if (hero == null)
                return;
            bool removed = false;
            for (int i = 0; i < AvailableEnemies.Count; i++)
            {
                if (AvailableEnemies[i].Hero == hero)
                {
                    AvailableEnemies.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
                return;
            for (int i = 0; i < AvailableAllies.Count; i++)
            {
                if (AvailableAllies[i].Hero == hero)
                {
                    AvailableAllies.RemoveAt(i);
                    break;
                }
            }
            PickedEnemies.Add(new HeroSelectionViewModel(hero));
            Refresh();
        }

        public void UnselectEnemyHero(Hero hero)
        {
            if (hero == null)
                return;
            bool removed = false;
            for (int i = 0; i < PickedEnemies.Count; i++)
            {
                if (PickedEnemies[i].Hero == hero)
                {
                    PickedEnemies.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
                return;
            AvailableAllies.Add(new HeroSelectionViewModel(hero));
            AvailableEnemies.Add(new HeroSelectionViewModel(hero));
            Refresh();
        }

        private void DoLoad()
        {
            Ready = false;
            Task.Factory.StartNew(() =>
                {
                    var baseFilter = new HotslogsFilter
                    {
                        Mode = (GameMode)Enum.Parse(typeof(GameMode), Settings.Default.BaseWinrateGameMode),
                        LookbackDays = Settings.Default.BaseWinrateNumberOfDays > 0 ? Settings.Default.BaseWinrateNumberOfDays : (int?)null
                    };
                    var adjFilter = new HotslogsFilter
                    {
                        Mode = (GameMode)Enum.Parse(typeof(GameMode), Settings.Default.AdjustmentsGameMode),
                        LookbackDays = Settings.Default.AdjustmentsNumberOfDays > 0 ? Settings.Default.AdjustmentsNumberOfDays : (int?)null
                    };
                    return HeroesStatistics.FromHotslogsExport(Settings.Default.HotsLogsExportPath, baseFilter, adjFilter);
                })
                .ContinueWith(r =>
                {
                    Ready = true;
                    if (r.IsFaulted)
                        LoadError = "Failed to load: " + r.Exception.Message;
                    else if (r.IsCompleted)
                    {
                        LoadError = null;
                        _heroesStatistics = r.Result;
                        ReInit();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ReInit()
        {
            AvailableAllies.Clear();
            AvailableEnemies.Clear();
            PickedAllies.Clear();
            PickedEnemies.Clear();
            Bans.Clear();
            if (_heroesStatistics == null)
            {
                AvailableMaps = Array.Empty<Map>();
                SelectedMap = null;
                TheoWinRate = null;
                return;
            }
            AvailableMaps = new [] { new Map(-1, "") }.Concat(_heroesStatistics.Maps.Values).ToList();
            foreach (var hero in _heroesStatistics.Heroes.Values)
            {
                AvailableAllies.Add(new HeroSelectionViewModel(hero));
                AvailableEnemies.Add(new HeroSelectionViewModel(hero));
            }
            Refresh();
        }

        private void Refresh()
        {
            double winRate = 0.5;
            foreach (var ally in PickedAllies)
            {
                double adjWinRate = winRate;
                adjWinRate = Utils.ApplyAdjustment(adjWinRate, Utils.CalcAdjustment(0.5, _heroesStatistics.BaseStatistics.WinRates[ally.Hero].Percentage));
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adjWinRate = Utils.ApplyAdjustment(adjWinRate, _heroesStatistics.MapAdjustments.Adjustments[(ally.Hero, SelectedMap)]);
                foreach (var otherAlly in PickedAllies)
                {
                    if (otherAlly == ally)
                        continue;
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        _heroesStatistics.HeroAdjustments.GetSynergyAdjustment(ally.Hero, otherAlly.Hero) / 2);
                }
                foreach (var enemy in PickedEnemies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        _heroesStatistics.HeroAdjustments.GetCounterAdjustment(ally.Hero, enemy.Hero) / 2);
                }
                ally.Diff = Utils.CalcAdjustment(winRate, adjWinRate) * 50;
                winRate = adjWinRate;
            }
            foreach (var enemy in PickedEnemies)
            {
                double adjWinRate = winRate;
                adjWinRate = Utils.ApplyAdjustment(adjWinRate, -Utils.CalcAdjustment(0.5, _heroesStatistics.BaseStatistics.WinRates[enemy.Hero].Percentage));
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adjWinRate = Utils.ApplyAdjustment(adjWinRate, -_heroesStatistics.MapAdjustments.Adjustments[(enemy.Hero, SelectedMap)]);
                foreach (var otherEnemy in PickedEnemies)
                {
                    if (otherEnemy == enemy)
                        continue;
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        -_heroesStatistics.HeroAdjustments.GetSynergyAdjustment(enemy.Hero, otherEnemy.Hero) / 2);
                }
                foreach (var ally in PickedAllies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        -_heroesStatistics.HeroAdjustments.GetCounterAdjustment(enemy.Hero, ally.Hero) / 2);
                }
                enemy.Diff = Utils.CalcAdjustment(winRate, adjWinRate) * 50;
                winRate = adjWinRate;
            }
            TheoWinRate = winRate;
            foreach (var ally in AvailableAllies)
            {
                double adjWinRate = winRate;
                adjWinRate = Utils.ApplyAdjustment(adjWinRate, Utils.CalcAdjustment(0.5, _heroesStatistics.BaseStatistics.WinRates[ally.Hero].Percentage));
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adjWinRate = Utils.ApplyAdjustment(adjWinRate, _heroesStatistics.MapAdjustments.Adjustments[(ally.Hero, SelectedMap)]);
                foreach (var otherAlly in PickedAllies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        _heroesStatistics.HeroAdjustments.GetSynergyAdjustment(ally.Hero, otherAlly.Hero));
                }
                foreach (var enemy in PickedEnemies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        _heroesStatistics.HeroAdjustments.GetCounterAdjustment(ally.Hero, enemy.Hero));
                }
                ally.Diff = adjWinRate - winRate;
            }
            foreach (var enemy in AvailableEnemies)
            {
                double adjWinRate = winRate;
                adjWinRate = Utils.ApplyAdjustment(adjWinRate, -Utils.CalcAdjustment(0.5, _heroesStatistics.BaseStatistics.WinRates[enemy.Hero].Percentage));
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adjWinRate = Utils.ApplyAdjustment(adjWinRate, -_heroesStatistics.MapAdjustments.Adjustments[(enemy.Hero, SelectedMap)]);
                foreach (var otherEnemy in PickedEnemies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        -_heroesStatistics.HeroAdjustments.GetSynergyAdjustment(enemy.Hero, otherEnemy.Hero));
                }
                foreach (var ally in PickedAllies)
                {
                    adjWinRate = Utils.ApplyAdjustment(
                        adjWinRate,
                        -_heroesStatistics.HeroAdjustments.GetCounterAdjustment(enemy.Hero, ally.Hero));
                }
                enemy.Diff = adjWinRate - winRate;
            }
        }
    }
}
