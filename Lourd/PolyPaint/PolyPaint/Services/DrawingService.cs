using PolyPaint.CustomInk;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public static event Action<string> JoinCanvasRoom;
        public static event Action<Stroke> AddStroke;
        public static event Action<CustomStroke> UpdateStroke;

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string canvasName;

        public DrawingService()
        {
            
        }

        public static void Initialize(object o)
        {

            socket.On("joinCanvasTestResponse", (data) =>
            {
                //string joinCanvas = serializer.Deserialize<string>((string)data);

                //JoinCanvasRoom?.Invoke(joinCanvas);
            });

            socket.On("CanvasUpdateTestResponse", (data) =>
            {
                BasicShape updatedStroke = serializer.Deserialize<BasicShape>((string)data);

                UpdateStroke?.Invoke(createStroke(updatedStroke));
            });

            socket.On("createCanvasResponse", (data) =>
            {
                CreateCanvasResponse response = serializer.Deserialize<CreateCanvasResponse>((string)data);
                if (response.isCreated)
                {
                    canvasName = response.canvasName;
                }
            });

            socket.On("joinCanvasRoomResponse", (data) =>
            {
                JoinCanvasRoomResponse response = serializer.Deserialize<JoinCanvasRoomResponse>((string)data);
                if (response.isCanvasRoomJoined)
                {
                    canvasName = response.canvasName;
                }
            });


            socket.On("formCreated", (data) =>
            {
                UpdateFormsData response = serializer.Deserialize<UpdateFormsData>((string)data);
                CustomStroke customStroke = createStroke(response.forms[0]);
                AddStroke?.Invoke(customStroke);
            });
        }

        public static void CreateCanvas(Canvas canvas)
        {
            EditCanevasData editCanevasData = new EditCanevasData(username, canvas);
            socket.Emit("createCanvas", serializer.Serialize(editCanevasData));
        }

        public static void JoinCanvas(string roomName)
        {
            EditGalleryData editGalleryData = new EditGalleryData(username, roomName);
            socket.Emit("joinCanvasRoom", serializer.Serialize(editGalleryData));
        }

        public static void CreateShape(CustomStroke customStroke)
        {
            BasicShape basicShape = customStroke.GetBasicShape();
            List<BasicShape> forms = new List<BasicShape>();
            forms.Add(basicShape);

            UpdateFormsData updateFormsData = new UpdateFormsData(username, canvasName, forms);
            socket.Emit("createForm", serializer.Serialize(updateFormsData));
        }

        public static void UpdateShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> linksTo, List<string> linksFrom)
        {
            BasicShape updatedShape = new BasicShape()
            {
                id = id,
                type = type,
                name = name,
                shapeStyle = shapeStyle,
                linksTo = linksTo,
                linksFrom = linksFrom
            };

            socket.Emit("CanvasUpdateTest", serializer.Serialize(updatedShape));
        }

        private static CustomStroke createStroke(BasicShape basicShape)
        {
            StylusPointCollection points = new StylusPointCollection();

            StylusPoint point = new StylusPoint(basicShape.shapeStyle.coordinates.x, basicShape.shapeStyle.coordinates.y);
            points.Add(point);

            CustomStroke customStroke;
            StrokeTypes type = (StrokeTypes) basicShape.type;

            switch (type)
            {
                case StrokeTypes.CLASS_SHAPE:
                    customStroke = new ClassStroke((ClassShape)basicShape, points);
                    break;
                case StrokeTypes.ARTIFACT:
                    customStroke = new ArtifactStroke(basicShape, points);
                    break;
                case StrokeTypes.ACTIVITY:
                    customStroke = new ActivityStroke(basicShape, points);
                    break;
                case StrokeTypes.ROLE:
                    customStroke = new ActorStroke(basicShape, points);
                    break;
                //case StrokeTypes.COMMENT:
                //    customStroke = new CommentStroke(points);
                //    break;
                //case StrokeTypes.PHASE:
                //    customStroke = new PhaseStroke(points);
                //    break;
                default:
                    customStroke = new ClassStroke((ClassShape)basicShape, points);
                    break;

            }
            customStroke.guid = Guid.Parse(basicShape.id);

            return customStroke;
        }

    }
}
