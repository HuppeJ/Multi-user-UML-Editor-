using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class History
    {
        public List<HistoryData> history { get; set; }

        public History(HistoryData[] historyData)
        {
            history = new List<HistoryData>();

            foreach(HistoryData data in historyData)
            {
                history.Add(data);
            }
        }
    }
}
