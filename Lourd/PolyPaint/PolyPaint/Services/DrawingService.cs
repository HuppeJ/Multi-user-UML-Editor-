using PolyPaint.CustomInk;
using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public event Action<string> JoinCanvasRoom;
        public event Action<CustomStroke> NewStroke;
        public event Action<CustomStroke> UpdateStroke;

        public DrawingService()
        {
            int i = 0;
        }

        public void Initialize(object o)
        {
            socket.On("joinCanvasTestResponse", (data) =>
            {
                string joinCanvas = serializer.Deserialize<string>((string)data);

                JoinCanvasRoom?.Invoke(joinCanvas);
            });

            socket.On("canvasUpdateTestResponse", (data) =>
            {
                BasicShape updatedStroke = serializer.Deserialize<BasicShape>((string)data);

                UpdateStroke?.Invoke(createStroke(updatedStroke));
            });
        }

        // TODO: Ajouter le nom de la room quand ça va être mis sur le serveur
        public void JoinCanvas()
        {
            socket.Emit("joinCanvasTest");
        }

        public void UpdateShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> links)
        {
            BasicShape updatedShape = new BasicShape()
            {
                id = id,
                type = type,
                name = name,
                shapeStyle = shapeStyle,
                links = links
            };

            socket.Emit("canvasUpdateTest", serializer.Serialize(updatedShape));
        }

        private CustomStroke createStroke(BasicShape basicShape)
        {
            StylusPointCollection points = new StylusPointCollection();
            points.Add(new StylusPoint(basicShape.shapeStyle.coordinates.x, basicShape.shapeStyle.coordinates.y));

            CustomStroke customStroke;

            switch (basicShape.type)
            {
                case 0:
                    customStroke = new ClassStroke(points);
                    break;
                case 1:
                    customStroke = new ActivityStroke(points);
                    break;
                case 2:
                    customStroke = new ArtifactStroke(points);
                    break;
                case 3:
                    customStroke = new ActorStroke(points);
                    break;
                //case 4:
                //    customStroke = new CommentStroke(points);
                //    break;
                //case 5:
                //    customStroke = new PhaseStroke(points);
                //    break;
                default:
                    customStroke = new ClassStroke(points);
                    break;

            }

            return customStroke;
        }

    }
}
