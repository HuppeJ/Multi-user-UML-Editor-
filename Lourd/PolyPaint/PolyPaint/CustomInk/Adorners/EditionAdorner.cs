using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolyPaint.CustomInk
{
    class EditionAdorner : Adorner
    {
        private CustomStroke stroke;
        private EditionButton editButton;
        private DeleteButton deleteButton;
        private CustomInkCanvas canvas;
        private Rect rectangleEdit;
        private Rect rectangleDelete;

        VisualCollection visualChildren;

        // Be sure to call the base class constructor.
        public EditionAdorner(UIElement adornedElement, CustomStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.stroke = stroke;
            this.canvas = canvas;

            rectangleEdit = new Rect(stroke.GetBounds().TopRight.X, stroke.GetBounds().TopRight.Y - 20, 20, 20);
            rectangleDelete = new Rect(stroke.GetBounds().TopRight.X + 20, stroke.GetBounds().TopRight.Y - 20, 20, 20);

            AddButtons(stroke, canvas);

        }

        private void AddButtons(CustomStroke stroke, CustomInkCanvas canvas)
        {
            visualChildren = new VisualCollection(this);

            editButton = new EditionButton(stroke, canvas);
            editButton.Cursor = Cursors.Hand;
            editButton.Width = 20;
            editButton.Height = 20;
            editButton.Background = Brushes.White;

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("../../Resources/pencil.png", UriKind.Relative);
            img.EndInit();

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = img;

            editButton.Content = image;

            visualChildren.Add(editButton);

            deleteButton = new DeleteButton(stroke, canvas);
            deleteButton.Cursor = Cursors.Hand;
            deleteButton.Width = 20;
            deleteButton.Height = 20;
            deleteButton.Background = Brushes.White;

            BitmapImage img2 = new BitmapImage();
            img2.BeginInit();
            img2.UriSource = new Uri("../../Resources/trash.png", UriKind.Relative);
            img2.EndInit();

            System.Windows.Controls.Image image2 = new System.Windows.Controls.Image();
            image2.Source = img2;
            deleteButton.Content = image2;

            visualChildren.Add(deleteButton);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var result = base.MeasureOverride(constraint);
            // ... add custom measure code here if desired ...
            InvalidateVisual();
            return result;
        }

        /// <summary>
        /// Draw the rotation handle and the outline of
        /// the element.
        /// </summary>
        /// <param name="finalSize">The final area within the 
        /// parent that this element should use to arrange 
        /// itself and its children.</param>
        /// <returns>The actual size used. </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (rectangleEdit.IsEmpty || rectangleDelete.IsEmpty)
            {
                return finalSize;
            }

            // Draws the rectangle
            editButton.Arrange(rectangleEdit);
            deleteButton.Arrange(rectangleDelete);

            return finalSize;
        }

        // Override the VisualChildrenCount and 
        // GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }
    }
}