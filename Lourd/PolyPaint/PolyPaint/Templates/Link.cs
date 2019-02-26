using System.Collections.Generic;

namespace PolyPaint.Templates
{
    class Link
    {
        public string id { get; set; }
        public AnchorPoint from { get; set; }
        public AnchorPoint to { get; set; }
        public int type { get; set; }
        public LinkStyle style { get; set; }
        public List<Coordinates> path { get; set; }
    }
}