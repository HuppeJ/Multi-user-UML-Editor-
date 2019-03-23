using PolyPaint.Templates;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk.Adorners
{
    public class ResizeAdorner2 : Adorner
    {
        private readonly Thumb _topLeft;
        private readonly Thumb _top;
        private readonly Thumb _topRight;
        private readonly Thumb _left;
        private readonly Thumb _right;
        private readonly Thumb _bottomLeft;
        private readonly Thumb _bottom;
        private readonly Thumb _bottomRight;

        // To store and manage the adorner's visual children.
        private readonly VisualCollection _visualChildren;

        private readonly CustomStroke _stroke;

        private RotateTransform rotation;

        // Initialize the ResizeAdorner.
        public ResizeAdorner2(UIElement adornedElement, CustomStroke stroke)
            : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            _stroke = stroke;
            Point center = stroke.GetCenter();
            rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerResize(ref _topLeft, Cursors.SizeNWSE);
            BuildAdornerResize(ref _top, Cursors.SizeNS);
            BuildAdornerResize(ref _topRight, Cursors.SizeNESW);
            BuildAdornerResize(ref _left, Cursors.SizeWE);
            BuildAdornerResize(ref _right, Cursors.SizeWE);
            BuildAdornerResize(ref _bottomLeft, Cursors.SizeNESW);
            BuildAdornerResize(ref _bottom, Cursors.SizeNS);
            BuildAdornerResize(ref _bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            _topLeft.DragDelta += HandleTopLeft;
            _topRight.DragDelta += HandleTopRight;
            _bottomLeft.DragDelta += HandleBottomLeft;
            _bottomRight.DragDelta += HandleBottomRight;
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount => _visualChildren.Count;

        // Handler for resizing from the bottom-right.
        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            var parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left.
        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            var parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-left.
        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            ShapeStyle shapeStyle = (_stroke as ShapeStroke).shapeStyle;
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            var desiredWidth = shapeStyle.width * 30;
            var desiredHeight = shapeStyle.height * 70;
            // adornerWidth & adornerHeight are used for placement as well.
            var adornerWidth = DesiredSize.Width;
            var adornerHeight = DesiredSize.Height;

            _topLeft.Arrange(new Rect(-adornerWidth / 2 + shapeStyle.coordinates.x, -adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _top.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2 + shapeStyle.coordinates.x, -adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2 + shapeStyle.coordinates.x, -adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _left.Arrange(new Rect(-adornerWidth / 2 + shapeStyle.coordinates.x, desiredHeight / 2 - adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _right.Arrange(new Rect(desiredWidth - adornerWidth / 2 + shapeStyle.coordinates.x, desiredHeight / 2 - adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _bottomLeft.Arrange(new Rect(-adornerWidth / 2 + shapeStyle.coordinates.x, desiredHeight - adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _bottom.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2 + shapeStyle.coordinates.x, desiredHeight - adornerHeight / 2 + shapeStyle.coordinates.y, adornerWidth, adornerHeight));
            _bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2 + shapeStyle.coordinates.x, desiredHeight - adornerHeight / 2 + shapeStyle.coordinates.y , adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        private void BuildAdornerResize(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb { Cursor = customizedCursor };

            // Set some arbitrary visual characteristics.
            cornerThumb.Height = cornerThumb.Width = 5;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.Gray);

            _visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        private void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            var parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }

        // GetDesiredTransform

        protected override Visual GetVisualChild(int index) => _visualChildren[index];
    }
}
