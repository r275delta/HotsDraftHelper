using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HotsDraftLib;
using System.Text;

namespace HotsDraftHelper
{
    internal sealed class MainViewModel : ViewModelBase
    {
        private StatisticsCollection _stats;

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
            Task<StatisticsCollection> loadTask;
            if (Settings.Default.UseRawHotslogsExport)
            {
                loadTask = Task.Factory.StartNew(() =>
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
                    return StatisticsCollection.FromHotslogsExport(Settings.Default.HotsLogsExportPath, baseFilter, adjFilter);
                });
            }
            else
            {
                loadTask = Task.Factory.StartNew(() => StatisticsCollection.DeserializeFromFile(Settings.Default.DraftDataPath));
            }

            loadTask.ContinueWith(r =>
                {
                    Ready = true;
                    if (r.IsFaulted)
                        LoadError = "Failed to load: " + r.Exception.Message;
                    else if (r.IsCompleted)
                    {
                        LoadError = null;
                        _stats = r.Result;
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
            if (_stats == null)
            {
                AvailableMaps = Array.Empty<Map>();
                SelectedMap = null;
                TheoWinRate = null;
                return;
            }
            AvailableMaps = new [] { new Map(-1, "") }.Concat(_stats.Maps.Values).ToList();
            foreach (var hero in _stats.Heroes.Values)
            {
                AvailableAllies.Add(new HeroSelectionViewModel(hero));
                AvailableEnemies.Add(new HeroSelectionViewModel(hero));
            }
            Refresh();
        }

        private void Refresh()
        {
            double totalAdj = 0;
            foreach (var ally in PickedAllies)
            {
                double adj = 0;
                var breakdown = new List<(string source, double amount)>();
                adj = AddAdjustment(adj, _stats.GetHeroAdjustment(ally.Hero), "Base", breakdown);
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adj = AddAdjustment(adj, _stats.MapAdjustments[(ally.Hero, SelectedMap)], "Map", breakdown);
                foreach (var otherAlly in PickedAllies)
                {
                    if (otherAlly == ally)
                        continue;
                    adj = AddAdjustment(adj, _stats.GetSynergyAdjustment(ally.Hero, otherAlly.Hero), $"w/{otherAlly.HeroName}", breakdown, 0.5);
                }
                foreach (var enemy in PickedEnemies)
                    adj = AddAdjustment(adj, _stats.GetCounterAdjustment(ally.Hero, enemy.Hero), $"vs. {enemy.HeroName}", breakdown, 0.5);

                ally.Diff = Utils.ApplyAdjustment(0.5, adj) - 0.5;
                ally.SetBreakdown(breakdown);
                totalAdj += adj;
            }
            foreach (var enemy in PickedEnemies)
            {
                double adj = 0;
                var breakdown = new List<(string source, double amount)>();
                adj = AddAdjustment(adj, _stats.GetHeroAdjustment(enemy.Hero), "Base", breakdown, -1);
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adj = AddAdjustment(adj, _stats.MapAdjustments[(enemy.Hero, SelectedMap)], "Map", breakdown, -1);
                foreach (var otherEnemy in PickedEnemies)
                {
                    if (otherEnemy == enemy)
                        continue;
                    adj = AddAdjustment(adj, _stats.GetSynergyAdjustment(enemy.Hero, otherEnemy.Hero), $"w/{otherEnemy.HeroName}", breakdown, -0.5);
                }
                foreach (var ally in PickedAllies)
                    adj = AddAdjustment(adj, _stats.GetCounterAdjustment(enemy.Hero, ally.Hero), $"vs. {ally.HeroName}", breakdown, - 0.5);
                enemy.Diff = Utils.ApplyAdjustment(0.5, adj) - 0.5;
                enemy.SetBreakdown(breakdown);
                totalAdj += adj;
            }
            TheoWinRate = Utils.ApplyAdjustment(0.5, totalAdj);
            foreach (var ally in AvailableAllies)
            {
                double adj = 0;
                var breakdown = new List<(string source, double amount)>();
                adj = AddAdjustment(adj, _stats.GetHeroAdjustment(ally.Hero), "Base", breakdown);
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adj = AddAdjustment(adj, _stats.MapAdjustments[(ally.Hero, SelectedMap)], "Map", breakdown);
                foreach (var otherAlly in PickedAllies)
                    adj = AddAdjustment(adj, _stats.GetSynergyAdjustment(ally.Hero, otherAlly.Hero), $"w/{otherAlly.HeroName}", breakdown);
                foreach (var enemy in PickedEnemies)
                    adj = AddAdjustment(adj, _stats.GetCounterAdjustment(ally.Hero, enemy.Hero), $"vs. {enemy.HeroName}", breakdown);
                ally.Diff = Utils.ApplyAdjustment(TheoWinRate.Value, adj) - TheoWinRate.Value;
                ally.SetBreakdown(breakdown);
            }
            foreach (var enemy in AvailableEnemies)
            {
                double adj = 0;
                var breakdown = new List<(string source, double amount)>();
                adj = AddAdjustment(adj, _stats.GetHeroAdjustment(enemy.Hero), "Base", breakdown, -1);
                if (SelectedMap != null && SelectedMap.Id >= 0)
                    adj = AddAdjustment(adj, _stats.MapAdjustments[(enemy.Hero, SelectedMap)], "Map", breakdown, -1);
                foreach (var otherEnemy in PickedEnemies)
                    adj = AddAdjustment(adj, _stats.GetSynergyAdjustment(enemy.Hero, otherEnemy.Hero), $"w/{otherEnemy.HeroName}", breakdown, -1);
                foreach (var ally in PickedAllies)
                    adj = AddAdjustment(adj, _stats.GetCounterAdjustment(enemy.Hero, ally.Hero), $"vs. {ally.HeroName}", breakdown, -1);
                enemy.Diff = Utils.ApplyAdjustment(TheoWinRate.Value, adj) - TheoWinRate.Value;
                enemy.SetBreakdown(breakdown);
            }
        }
        
        private double AddAdjustment(double current, Statistic adjustment, string context, List<(string context, double adj)> breakdown, double scaleFactor = 1)
        {
            if (adjustment.SampleSize >= Settings.Default.MinSampleSize)
            {
                var adj = adjustment.Value * scaleFactor;
                breakdown.Add((context, adj));
                return current + adj;
            }
            return current;
        }
    }
}
