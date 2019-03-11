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
    }
}