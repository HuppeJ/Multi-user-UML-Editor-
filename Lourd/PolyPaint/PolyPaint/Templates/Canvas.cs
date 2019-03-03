using System.Collections.Generic;

namespace PolyPaint.Templates
{
    class Canvas
    {
        public string id { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string owner { get; set; }
        public int accessibility { get; set; }
        public string password { get; set; }
        public List<BasicShape> shapes { get; set; }
        public List<Link> links { get; set; }
    }
}