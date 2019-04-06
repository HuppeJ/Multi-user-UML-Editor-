using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class HistoryData
    {
        public string username { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
        public Canvas canevas { get; set; }


        public HistoryData()
        {
        }

        public HistoryData(string username, string message, string timestamp, Canvas canevas)
        {
            this.username = username;
            this.message = message;
            this.timestamp = timestamp;
            this.canevas = canevas;
        }
    }
}