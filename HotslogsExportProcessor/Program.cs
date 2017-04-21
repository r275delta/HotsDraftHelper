using HotsDraftLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotslogsExportProcessor
{
    internal static class Program
    {
        public static void Main(string[] args)
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
            var stats = StatisticsCollection.FromHotslogsExport(Settings.Default.HotsLogsExportPath, baseFilter, adjFilter);
            stats.SerializeToFile(Settings.Default.ExtractDestinationPath);
        }
    }
}
