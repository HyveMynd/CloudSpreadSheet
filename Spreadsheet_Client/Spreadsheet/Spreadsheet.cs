//Implemented by Rory Savage
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;


namespace SS
{
    /// <summary>
    /// Creates a spreadsheet class that extends AbstractSpreadsheet
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Cell> mySpreadsheet;
        private SpreadsheetUtilities.DependencyGraph dependentCells;
        private Cell myCell;
        private bool myChanged;
        //private Formula myFormula;
        /// <summary>
        /// Creates a new Spreadsheet
        /// </summary>
        public Spreadsheet()
            : base(s => true, s => s, "default")
        {

            mySpreadsheet = new Dictionary<string, Cell>();
            dependentCells = new DependencyGraph();

        }
        /// <summary>
        /// Creates a new spread sheet with restrictions and version information
        /// </summary>
        /// <param name="myValid"></param>
        /// <param name="myNormalize"></param>
        /// <param name="myVersion"></param>
        public Spreadsheet(Func<string, bool> myValid, Func<string, string> myNormalize, string myVersion)
            : base(myValid, myNormalize, myVersion)
        {
            mySpreadsheet = new Dictionary<string, Cell>();
            dependentCells = new DependencyGraph();
        }
        /// <summary>
        /// Opens a saved spreadsheet and puts it back together
        /// </summary>
        /// <param name="myPath"></param>
        /// <param name="myValid"></param>
        /// <param name="myNormalize"></param>
        /// <param name="myVersion"></param>
        public Spreadsheet(string myPath, Func<string, bool> myValid, Func<string, string> myNormalize, string myVersion)
            : base(myValid, myNormalize, myVersion)
        {
            mySpreadsheet = new Dictionary<string, Cell>();
            dependentCells = new DependencyGraph();
            //GetSavedVersion(myPath);
            using (XmlReader myReader = XmlReader.Create(myPath))
            {
                string myContent = null;
                while (myReader.Read())
                {
                    
                    if (myReader.IsStartElement())
                    {
                        switch (myReader.Name)
                        {
                            case "Version Information":
                                break;
                            case "cell":
                                break;

                            case "name":

                                myReader.Read();
                                myContent = myReader.Value;
                                break;
                            case "contents":
                                myReader.Read();
                                HashSet<string> mySet = new HashSet<string>();
                                //Foreach look sets the cell contents and evaluates the value of any direct or indirect dependents
                                foreach (string s in SetContentsOfCell(myContent, myReader.Value))
                                {
                                    mySpreadsheet[s].myValue = myEvaluate(s);
                                }
                                break;


                        }
                    }
                }
            }
            Changed = false;

        }

        /// <summary>
        /// Returns the names of all cells. Will not return a name if the cell contains an empty string.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {

            return mySpreadsheet.Keys;
        }
        /// <summary>
        /// Takes in a cell name returns the contents of the cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            if (Object.ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            else
            {
                if (!mySpreadsheet.ContainsKey(name))
                {
                    return "";
                }
                else
                {

                    return mySpreadsheet[name].contents;
                }
            }



        
        }
        /// <summary>
        /// Takes a cell name and a double sets them to a cell and returs dependents also sets value of cell
        /// Then caculates any direct or indirect dependents
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            name = Normalize(name);
            HashSet<string> mySet = new HashSet<string>();
            HashSet<string> replaceSet = new HashSet<string>();
            myCell = new Cell(number,number);

            if (Object.ReferenceEquals(name,null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            else
            {
                if (mySpreadsheet.ContainsKey(name))
                {
                    mySpreadsheet[name] = myCell;
                }
                else
                {
                    mySpreadsheet.Add(name, myCell);
                }
                mySet.Add(name);
                dependentCells.ReplaceDependees(name, replaceSet);
                foreach (string s in GetCellsToRecalculate(name))
                {
                    mySet.Add(s);
                }
                foreach (string s in mySet)
                {
                    myCell=new Cell(GetCellContents(s),myEvaluate(s));
                    mySpreadsheet[s] = myCell;
                }
                Changed = true;
                return mySet;
            }

        }
        /// <summary>
        /// Takes in a cell name and the text of a cell and returns dependents and sets cell value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            name = Normalize(name);
            HashSet<string> mySet = new HashSet<string>();
            HashSet<string> replaceSet = new HashSet<string>();
            myCell = new Cell(text,text);

            if (Object.ReferenceEquals(text,null))
            {
                throw new ArgumentNullException();
            }
            if (text == "")
            {
                if (mySpreadsheet.ContainsKey(name))
                {
                    mySpreadsheet[name] = myCell;
                }
                return mySet;
            }
            if (Object.ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            if (mySpreadsheet.ContainsKey(name))
            {
                mySpreadsheet[name] = myCell;
               
            }
            else
            {
                mySpreadsheet.Add(name, myCell);
            }
            mySet.Add(name);
            dependentCells.ReplaceDependees(name, replaceSet);
            foreach (string s in GetCellsToRecalculate(name))
            {
                mySet.Add(s);
            }
            foreach (string s in mySet)
            {
                if (s == name)
                {
                    mySpreadsheet[s] = myCell;
                }
                else
                {
                myCell=new Cell(GetCellContents(s),myEvaluate(s));
                mySpreadsheet[s]=myCell;
                }
                
            }
            Changed = true;
            return mySet;

        }
        /// <summary>
        /// Takes in a cell name and a formula and returns the dependents
        /// Calculates the value of the formula then recalculates the value of any cell that depends directly or inderectly on it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            name = Normalize(name);
            HashSet<string> mySet = new HashSet<string>();
            HashSet<string> replaceSet = new HashSet<string>();


            if (Object.ReferenceEquals(formula, null))
            {
                throw new ArgumentNullException();
            }
            if (Object.ReferenceEquals(name,null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            try
            {

                foreach (string s in formula.GetVariables())
                {

                    replaceSet.Add(s);
                }

                // dependentCells.ReplaceDependees(name, mySet);
                dependentCells.ReplaceDependees(name, replaceSet);
                GetCellsToRecalculate(name);
            }
            catch (CircularException)
            {
                throw new CircularException();
            }
            Formula myEvl = new Formula(formula.ToString(), IsValid, Normalize);
            myCell = new Cell(formula,myEvl.Evaluate(myLookup));
            if (mySpreadsheet.ContainsKey(name))
            {
                mySpreadsheet[name] = myCell;
            }
            else
            {
                mySpreadsheet.Add(name, myCell);
            }
            mySet.Add(name);
            foreach (string s in GetCellsToRecalculate(name))
            {
                mySet.Add(s);
            }
            foreach (string s in mySet)
            {
                myCell = new Cell(GetCellContents(s), myEvaluate(s));
                mySpreadsheet[s]=myCell;
            }
            Changed = true;
            return mySet;
        }
        /// <summary>
        /// Takes in a cell name and returns the depentendents for that cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = Normalize(name);
            if (Object.ReferenceEquals(name, null))
            {
                throw new ArgumentNullException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            return dependentCells.GetDependents(name);
        }
        /// <summary>
        /// Tells if a cell has been changed since it was created or saved.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return myChanged;
            }
            protected set
            {
                myChanged = value;
            }
        }
        /// <summary>
        /// Gets version information from file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader myReader = XmlReader.Create(filename))
                {
                    while (myReader.Read())
                    {
                        if (myReader.IsStartElement())
                        {
                            switch (myReader.Name)
                            {
                                case "spreadsheet":

                                    return /*myReader["version"]*/"";
                            }

                        }

                    }
                    return /*myReader["version"]*/"";
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Directory not found");
            }
            catch (XmlException)
            {
                throw new SpreadsheetReadWriteException("File is not readable");
            }
            catch (ArgumentException)
            {
                throw new SpreadsheetReadWriteException("Invalid file name");
            }
        }
        /// <summary>
        /// Saves spreadsheet to a file
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            double d = 0;
            string p = null;
            Formula f;
            try
            {
              /*  if(mySpreadsheet.Count()==0)
                {
                    throw new SpreadsheetReadWriteException("Spreadsheet is empty");
                }*/
                using (XmlWriter myWriter = XmlWriter.Create(filename))
                {
                    myWriter.WriteStartDocument();
                    myWriter.WriteStartElement("spreadsheet");
                    //myWriter.WriteAttributeString("version", Version);
                    foreach (string s in GetNamesOfAllNonemptyCells())
                    {
                        if (GetCellContents(s) is string)
                        {
                            p = (string)GetCellContents(s);
                            myWriter.WriteStartElement("cell");
                            myWriter.WriteElementString("name", s);
                            myWriter.WriteElementString("contents", p.ToString());
                            myWriter.WriteEndElement();
                        }
                        if (GetCellContents(s) is double)
                        {
                            d = (double)GetCellContents(s);
                            myWriter.WriteStartElement("cell");
                            myWriter.WriteElementString("name", s);
                            myWriter.WriteElementString("contents", d.ToString());
                            myWriter.WriteEndElement();
                        }

                        if (GetCellContents(s) is Formula)
                        {
                            string k = "=";
                            f = (Formula)GetCellContents(s);
                            k += f.ToString();
                            myWriter.WriteStartElement("cell");
                            myWriter.WriteElementString("name", s);
                            myWriter.WriteElementString("contents", k);
                            myWriter.WriteEndElement();
                        }
                    }


                    myWriter.WriteEndElement();
                    myWriter.WriteEndDocument();
                }
                Changed = false;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Directory does not exist");
            }
            catch (ArgumentException)
            {
                throw new SpreadsheetReadWriteException("Invalid file name");
            }

        }
        /// <summary>
        /// Gets the value of the cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            name = Normalize(name);
            if (Object.ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            if (mySpreadsheet.ContainsKey(name))
            {
                return mySpreadsheet[name].myValue;
            }
            else
            {
                return "";
            }

        }
        /// <summary>
        /// Sets the contents of the cell
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            name = Normalize(name);
            double myDouble = 0;
            if (Object.ReferenceEquals(content, null))
            {
                throw new ArgumentNullException();
            }
            if (Object.ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }
            if (!Regex.IsMatch(name, "^([a-z]|[A-Z])+\\d+$") | !IsValid(name))
            {
                throw new InvalidNameException();
            }
            if (Double.TryParse(content, out myDouble))
            {
                return SetCellContents(name, myDouble);
            }
            else
                if (content.StartsWith("="))
                {
                    content = content.TrimStart('=');
                    Formula myFormula = new Formula(content, IsValid, Normalize);
                    return SetCellContents(name, myFormula);
                }
                else
                {
                    return SetCellContents(name, content);
                }

        }
        /// <summary>
        /// Used to get values of direct or indirect dependents
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private object myEvaluate(string name)
        {
            Formula myEval;
           // GetCellContents(name);
            if (GetCellContents(name) is double)
            {
                return (double)mySpreadsheet[name].myValue;
            }
            if (GetCellContents(name) is string)
            {
                return (string)mySpreadsheet[name].myValue;
            }
            if (GetCellContents(name) is Formula)
            {
                Formula myFormula = (Formula)GetCellContents(name);
                string form = myFormula.ToString();
                myEval = new Formula(form, IsValid, Normalize);
                return myEval.Evaluate(myLookup);
            }

            return 0;
        }
        /// <summary>
        /// Lookup method for variables
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double myLookup(string name)
        {
            if(!mySpreadsheet.ContainsKey(name))
            {
                throw new ArgumentException("Uknown Variable");
            }
            if (mySpreadsheet[name].myValue is double)
            {
                
                return (double)mySpreadsheet[name].myValue;
            }
            else
            {
                throw new ArgumentException("Unknown variable");
            }
        }
        

    }
  

}
