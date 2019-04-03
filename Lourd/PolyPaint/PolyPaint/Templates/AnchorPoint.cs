using System;

namespace PolyPaint.Templates
{
    public class AnchorPoint
    {
        public string formId { get; set; }
        public int anchor { get; set; }
        public string multiplicity { get; set; }

        public AnchorPoint()
        {
        }

        public AnchorPoint(string formId, int anchor, string multiplicity)
        {
            this.formId = formId;
            this.anchor = anchor;
            this.multiplicity = multiplicity;
        }

        public AnchorPoint(AnchorPoint anchorPoint)
        {
            formId = anchorPoint?.formId;
            anchor = anchorPoint.anchor;
            multiplicity = anchorPoint.multiplicity;
        }

        internal void SetDefaults()
        {
            formId = null;
            anchor = 0;
            multiplicity = "";
        }

        public AnchorPoint GetForServer()
        {
            string newFormId = formId == null? "": formId;

            return new AnchorPoint(newFormId, anchor, multiplicity);
        }

        internal AnchorPoint GetForLourd()
        {
            string newFormId = formId.Equals("") ? null : formId;

            return new AnchorPoint(newFormId, anchor, multiplicity);
        }
    }
}