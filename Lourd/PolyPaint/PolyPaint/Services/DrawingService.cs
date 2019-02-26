using PolyPaint.Templates;
using System;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public event Action<BasicShape> NewStroke;
        public event Action<BasicShape> UpdateStroke;

        public DrawingService()
        {

        }

    }
}
