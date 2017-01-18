using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
