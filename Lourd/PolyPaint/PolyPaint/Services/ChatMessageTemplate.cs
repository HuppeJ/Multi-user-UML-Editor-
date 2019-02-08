using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Modeles
{
    public class ChatMessageTemplate
    {
        public string text { get; set; }
        public string sender { get; set; }
        public long createdAt { get; set; }
    }
}
