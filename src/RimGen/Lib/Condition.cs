namespace RimGen.Lib
{
    public class Condition
    {
        public Condition(AttributeEnum attr, int minValue)
        {
            this.Attr = attr;
            this.MinValue = minValue;
        }

        public AttributeEnum Attr { get; set; }
        public int MinValue { get; set; }
    }
}
