using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Eclipse
    {
        public String Type { get; set; }
        public String Date { get; set; }

        public String Visibility { get; set; }

        public Eclipse(String t, String d, String v)
        {
            this.Type = t;
            this.Date = d;
            this.Visibility = v;
        }

        public override String ToString()
        {
            return this.Date + " " + this.Type + " " + this.Visibility;
        }
    }
}
