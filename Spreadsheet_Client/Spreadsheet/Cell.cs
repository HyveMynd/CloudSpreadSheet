using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;

namespace SS
{
    class Cell
    {
        private object myContents;
        private object value1;
        public Cell(object input,object valu)
        {
            contents = input;
            myValue = valu;
            
        }
        
        /*  private enum type
          {
              Formula=Formula, number=Double, text=String
                
          }*/
        // object value { get; private set; }
        public object contents
        {
            get { return myContents; }
            set { myContents = value; }
        }
        public object myValue
        {
            get { return value1; }
            set { value1 = value; }
        }
        
    }
}
