using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace HotsDraftHelper.Data
{
    internal sealed class MatchSummaryCollection
    {
        private class MatchSummaryTimestampComparer : IComparer<MatchSummary>
        {
            private static readonly IComparer<DateTime> _dateTimeComparer = Comparer<DateTime>.Default;
            public int Compare(MatchSummary x, MatchSummary y)
            {
                return _dateTimeComparer.Compare(x.Timestamp, y.Timestamp);
            }
        }

        public sealed class Builder
        {
            private readonly HotslogsFilter _filter;
            private readonly OrderedBag<MatchSummary> _summaries = new OrderedBag<MatchSummary>(new MatchSummaryTimestampComparer());

            private DateTime? _cutOff;

            public Builder(HotslogsFilter filter)
            {
                _filter = filter;
            }

            public void TryAddSummary(MatchSummary summary)
            {
                if (_filter.Mode != summary.GameMode)
                    return;
                if (_filter.LookbackDays.HasValue)
                {
                    var summaryCutOff = summary.Timestamp.AddDays(-_filter.LookbackDays.Value);
                    if (_cutOff == null || summaryCutOff > _cutOff)
                        SetCutOff(summaryCutOff);
                    else if (summary.Timestamp < _cutOff)
                        return;
                }
                _summaries.Add(summary);
            }

            private void SetCutOff(DateTime value)
            {
                _cutOff = value;
                while (_summaries.Count > 0 && _summaries.GetFirst().Timestamp < _cutOff)
                    _summaries.RemoveFirst();
            }

            public MatchSummaryCollection Build()
            {
                return new MatchSummaryCollection(_summaries);
            }
        }

        private readonly Dictionary<long, MatchSummary> _summaries;
        public IReadOnlyDictionary<long, MatchSummary> Summaries => _summaries;
        public MatchSummaryCollection(IEnumerable<MatchSummary> summaries)
        {
            _summaries = summaries.ToDictionary(s => s.MatchId);
        }
    }

}
