/* Author: Derek Burns, u0907203
 * 
 * Assignment: PS5 
 * 
 */
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> cellSet = new Dictionary<string, Cell>();
        DependencyGraph dependencies = new DependencyGraph();

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary> 
        public override bool Changed
        {
            get;

            protected set;
        }
        
        /// <summary>
        /// Constructs a new, empty spreadsheet. 
        /// </summary>
        public Spreadsheet() : base(s => true, x => x, "default")
        {

        }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        /// <param name="isValid">Variable name validity check</param>
        /// <param name="normalize">Variable normalizer</param>
        /// <param name="version">Version number</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Just calls the base constructor with the same parameters
        }

        /// <summary>
        /// Constructs an abstract spreadsheet from the input file by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        /// <param name="filePath">Path to the spreadsheet XML file to be loaded.</param>
        /// <param name="isValid">Variable name validity check.</param>
        /// <param name="normalize">Variable normalizer</param>
        /// <param name="version">Version number</param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            loadSpreadsheet(filePath);
        }
        public void setChanged(bool isItChanged)
        {
            Changed = isItChanged;
        }
        /// <summary>
        /// Helper method to load the spreadsheet from a file.
        /// </summary>
        /// <param name="filePath">Path to the spreadsheet file.</param>
        private void loadSpreadsheet(string filePath)
        {   
            //Try block used to catch any exceptions, and rethrow them as a SpreadsheetReadWriteException
            try
            {   
                
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    //Create a new XMLDocument from the reader
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);

                    //Go through each root "node" in the document.
                    foreach (XmlNode rootNode in document.ChildNodes)
                    {
                        if (rootNode.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            //Just here to make sure that reading the XMLDeclaration does not throw an exception
                        }
                        //If we hit the spreadsheet node
                        else if (rootNode.NodeType == XmlNodeType.Element && rootNode.Name == "spreadsheet")
                        {
                                                        
                            //Ensure the version of the file and this spreadsheet match
                            //Try catch used incase "version" is not an index in the attributes list
                            try
                            {
                                if (rootNode.Attributes["version"].Value != this.Version.ToString())
                                {
                                    //TODO: Trim white space in version before checking? (When saving as well?)
                                    throw new Exception("File version does not match the version of current spreadsheet.");
                                }
                            }
                            catch(Exception)
                            {
                                throw new Exception("Spreadsheet version not specified.");
                            }
                            //Go through each nested node ("cell") between <spreadsheet> </spreadsheet>
                            foreach (XmlNode node in rootNode.ChildNodes)
                            {
                                readCellXML(node);
                            }

                        }
                        else
                        {
                            throw new Exception("Invalid XML format. Root node can only be a spreadsheet.");
                        }
                    }

                    //A newly opened file hasn't technically been changed, (or "unsaved")
                    //TODO: Keep this part?
                    this.Changed = false;
                }
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Called when reading the elements inside the spreadsheet element.
        /// Attempts to read and create cells.
        /// </summary>
        /// <param name="cell">The current element being read.</param>
        private void readCellXML(XmlNode cell)
        {   
            //If the node isn't an element, or isn't defined as a cell, throw an exception
            if(cell.NodeType != XmlNodeType.Element || cell.Name != "cell")
            {
                throw new Exception("Spreadsheet can only have nested Cell elements.");
            }

            //Used to keep track of whether or not cell name was already declared
            bool readName = false;
            //Used to keep track of whether or not cell contents was already declared
            bool readContents = false;

            //Future name/contents of cell
            string name = "";
            string contents = "";

            //Go through each nested element in the current element
            foreach(XmlNode node in cell.ChildNodes)
            {   
                if(node.NodeType == XmlNodeType.Element)
                {
                    //If element is <name>
                    if (node.Name == "name")
                    {
                        //Attempt to read the cell name, update readName flag
                        name = readCellNameXML(ref readName, node);
                    }
                    else if (node.Name == "contents")
                    {
                        //attempt to read cell contents, update readContents flag
                        contents = readCellContentsXML(ref readContents, node);
                    }
                    else
                    {
                        //If not <cell> or <contents> nodes, throw exception
                        throw new Exception("Cells can only contain contents or name elements.");
                    }

                    //Go to next nested node
                    continue;
                    
                }

                //If not an element (I.E. just text), throw exception
                throw new Exception("Cells can only contain contents or name elements.");

            }

            //If succeeded in reading, attempt to construct the cell inside the spreadsheet object
            this.SetContentsOfCell(name, contents);

        }


        /// <summary>
        /// Helper method for reading the contents of a cell (between <contents></contents> tags)
        /// </summary>
        /// <param name="readContents">Have contents already been read</param>
        /// <param name="node">The current node element</param>
        /// <returns>The cell's string contents</returns>
        private string readCellContentsXML(ref bool readContents, XmlNode node)
        {
            if (readContents == true)
            {
                //If we already read the contents of the cell, throw exception
                throw new Exception("Cell contents can only be declared once within element.");
            }

            //Make sure there is only text between the "contents" tags
            if (node.ChildNodes.Count != 1 || node.ChildNodes[0].NodeType != XmlNodeType.Text)
            {
                throw new Exception("Cell contents element must contain valid text.");
            }
            else
            {
                //We have successfully read the contents
                readContents = true;
                //Return the text between the "contents" tags
                return node.ChildNodes[0].Value;
            }
        }

        /// <summary>
        /// Helper method for reading the name of a cell (between <name></name> tags)
        /// </summary>
        /// <param name="readName">Has the name already been read</param>
        /// <param name="node">The current node element</param>
        /// <returns>The cell's string name</returns>
        private string readCellNameXML(ref bool readName, XmlNode node)
        {
            if (readName == true)
            {
                //If we already read the cell's name, throw exception
                throw new Exception("Cell name can only be declared once within element.");
            }

            //Make sure there is only text between the "name" tags
            if(node.ChildNodes.Count != 1 || node.ChildNodes[0].NodeType != XmlNodeType.Text)
            {
                throw new Exception("Cell name element must contain valid text.");
            }
            else
            {
                //We have successfully read the name of the cell
                readName = true;
                //Return the name of the cell
                return node.ChildNodes[0].Value;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <returns>Cell contents</returns>
        public override object GetCellContents(string name)
        {
            //Make sure input string isn't input
            if (ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }

            //If name isn't null, normalize it before using it
            name = Normalize(name);

            if (!isValidNameAllCriteria(name))
            {
                throw new InvalidNameException();
            }

            Cell cell;

            //If the cell is in the Dictionary, it has a value. Return the contents of the cell.
            if(cellSet.TryGetValue(name, out cell))
            {
                return cell.contents;
            }
            else
            {
                //Else if cell doesn't contain a value, return empty string
                return "";
            }

        }

        public string GetCellContentsAsString(string name)
        {
            object contents = GetCellContents(name);
            string contentString;
            if (contents.GetType() == typeof(Formula))
            {
                contentString = "=" + contents.ToString();
            }
            else
            {
                contentString = contents.ToString();
            }
            return contentString;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        /// <returns>Enumerable list of nonempty cells</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {

            //The dictionary only contains nonempty cells to begin with.
            //Construct a new list with the names of these cells (The "keys" of the dictionary that stores the cells).
            return new List<string>(cellSet.Keys);
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="formula">The formula to input</param>
        /// <returns>Set of changed cells</returns>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            return SetCellContentsHelper(name, formula);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="formula">The string to input</param>
        /// <returns>Set of changed cells</returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            return SetCellContentsHelper(name, text);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="formula">The double to input</param>
        /// <returns>Set of changed cells</returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            return SetCellContentsHelper(name, number);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(ReferenceEquals(name, null))
            {
                throw new ArgumentNullException();
            }

            //If name isn't null, normalize it before we use it
            name = Normalize(name);

            if (!isValidNameAllCriteria(name))
            {
                throw new InvalidNameException();
            }

            //Use the dependency graph to return the dependents of a cell
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// An abstracted helper method that all SetCellContents methods can call.
        /// 
        /// If contents is null, throw ArgumentNullException.
        /// 
        /// Else, if name is null or invalid, throw InvalidNameException
        /// 
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="contents">Object to put into the cell (double, String, Formula)</param>
        /// <returns>Set of the cells that need to be updated.</returns>
        private ISet<string> SetCellContentsHelper(string name, object contents)
        {

            //Return the value that the InsertCell helper method returns.
            return InsertCell(name, contents);
        }

        /// <summary>
        /// This method attempts to set the contents of a cell depending on the type of object it is.
        /// If it is a Formula, checks for a circular dependency, and throws the appropriate exception.
        /// 
        /// If an empty string is passed, the cell is removed from the cellSet.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="contents">Object being put into the cell (double, String, Formula)</param>
        /// <returns>Set of the cells that need to be updated.</returns>
        private ISet<string> InsertCell(string name, object contents)
        {
            //Construct a new cell with the contents that were passed to this method.
            Cell newCell = new Cell(name, contents);

            //Used to keep track of the old cell contents, if any
            Cell oldCell;

            bool setContainedOldCell = cellSet.TryGetValue(name, out oldCell);

            //Used to keep track of the old dependees of the cell, if any.
            IEnumerable<string> oldDependees = new List<string>();

            if (contents is string)
            {
                //Remove the cell (if one previously existed) to allow updated cell to replace it
                cellSet.Remove(name);

                //Check to make sure that the string being added isn't empty.
                //If it is empty, then we don't need to add a new cell to the dictionary.
                //since any cell not in the dictionary is assumed to be an empty string.
                if ((contents as string) != "")
                {
                    cellSet.Add(name, newCell);
                }

                //Remove all the dependees of the cell (a string doesn't depend on another cell)
                dependencies.ReplaceDependees(name, new List<string>());
            }
            else if(contents is double)
            {
                //Remove the cell (if one previously existed) to allow updated cell to replace it
                cellSet.Remove(name);
                //Replace cell with new cell
                cellSet.Add(name, newCell);
                //A single double doesn't depend on any other cells, so remove dependees
                dependencies.ReplaceDependees(name, new List<string>());
            }
            else if (contents is Formula)
            {
                //Remove the cell (if one previously existed) to allow updated cell to replace it
                cellSet.Remove(name);
                //Add the new cell in its place
                cellSet.Add(name, newCell);
                //Get the old dependees from the dependency graph
                oldDependees = dependencies.GetDependees(name);
                //Safely cast the input object into a Formula
                Formula newFormula = contents as Formula;
                //replace the dependees of the cell with the variables that the Formula uses
                dependencies.ReplaceDependees(name, newFormula.GetVariables());
            }
            try
            {
                IEnumerable<string> cellsToRecalculate = GetCellsToRecalculate(name);

                recalculateCells(cellsToRecalculate);

                this.Changed = true;

                return new HashSet<string>(cellsToRecalculate);
            }
            catch (CircularException e)
            {
                //Remove the recently added cell
                cellSet.Remove(name);

                //If there was an old cell, put it back
                if (setContainedOldCell)
                {
                    cellSet.Add(name, oldCell);
                }

                //Replace the dependees of the cell with the old ones, if any
                dependencies.ReplaceDependees(name, oldDependees);

                //Continue throwing the exception
                throw e;
            }
        }

        /// <summary>
        /// Helper method, used to recalculate a list of cells (in order, from first to last)
        /// </summary>
        /// <param name="cellsToRecalculate">List of cells to recalculate</param>
        private void recalculateCells(IEnumerable<string> cellsToRecalculate)
        {
            foreach(string s in cellsToRecalculate)
            {
                Cell currentCell;

                //If the cell in question has no contents, no calculation is needed
                //(really only used if a cell is set back to an empty string, prevents exceptions)
                if(!cellSet.TryGetValue(s, out currentCell))
                {
                    
                }
                //A cell with string has a value of the string
                else if(currentCell.contents is string)
                {
                    currentCell.value = currentCell.contents;
                }
                //A cell with a double has the value of the double
                else if(currentCell.contents is double)
                {
                    currentCell.value = currentCell.contents;
                }
                //Else the cell is a formula, evaluate the formula
                else
                {
                    currentCell.value = (currentCell.contents as Formula).Evaluate(cellLookup);
                }
            }
        }

        /// <summary>
        /// Helper lookup method for evaluating cells in a formula
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <returns>The value of that cell</returns>
        private double cellLookup(string name)
        {
            Cell lookupCell;

            //If the cellset didn't contain the desired cell, it doesn't have a value, so throw an exception for the formula evaluator
            if(!cellSet.TryGetValue(name, out lookupCell)) {
                throw new ArgumentException();
            }

            //Check if the value of the cell isn't a double
            bool isntDouble = !(lookupCell.value is double);

            if (isntDouble)
            {
                throw new ArgumentException();
            }
            else
            {
                //Else, return the value stored in the cell
                return (double)lookupCell.value;
            }
        }

        /// <summary>
        /// Checks to make sure the input string is a valid cell name
        /// </summary>
        /// <param name="name">Cell Name</param>
        /// <returns></returns>
        private bool isValidNameAllCriteria(string name)
        {
            //Must begin with an underscore or letters, followed by more underscores/letters/numbers
            bool SpreadsheetCriteria = Regex.IsMatch(name, @"^([a-zA-Z])+([0-9])+$", RegexOptions.IgnorePatternWhitespace);

            //Check with the validator used in the constructor
            bool UserDefinedCriteria = IsValid(name);

            //Make sure both criteria are valid
            return SpreadsheetCriteria && UserDefinedCriteria;
        }

        /// <summary>
        /// Get the version of the saved spreadsheet file
        /// </summary>
        /// <param name="filename">Path to the spreadsheet file</param>
        /// <returns>Version of file</returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    //Create XMLDocument from the reader
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);

                    //Go through the root nodes of the file (i.e. "spreadsheet" tag)
                    foreach (XmlNode rootNode in document.ChildNodes)
                    {
                        
                        //If we hit the spreadsheet tag,
                        if (rootNode.NodeType == XmlNodeType.Element && rootNode.Name == "spreadsheet")
                        {
                            //Try catch used incase version is not specified
                            try
                            {
                                //Return the version in the tag
                                return rootNode.Attributes["version"].Value;
                            }
                            catch(Exception e)
                            {
                                throw new Exception("Spreadsheet version not specified.");
                            }

                        }

                    }

                    //If spreadsheet tag was never found
                    throw new Exception("File is invalid or has no version.");


                }
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Saves the spreadsheet to the file.
        /// 
        /// Directories must be created before calling this method, if they don't exist yet.
        /// </summary>
        /// <param name="filename">Path to the file</param>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //Make sure the written file is indented
            settings.Indent = true;
            settings.IndentChars = ("\t");

            try
            {
                //Create a new xml writer
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    //Write XML Declaration
                    writer.WriteStartDocument();
                    //Write <spreadsheet> tag
                    writer.WriteStartElement("spreadsheet");
                    //Write version attribute
                    writer.WriteAttributeString("version", this.Version);

                    //Write all the cells
                    foreach (var cell in cellSet.Values)
                    {
                        writeCellXML(cell, writer);
                    }

                    //Close with </spreadsheet>
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                //File now has not changed since saving
                this.Changed = false;
            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Helper method to write a cell to the xml
        /// </summary>
        /// <param name="cell">Current cell</param>
        /// <param name="writer">The current XMLwriter</param>
        private void writeCellXML(Cell cell, XmlWriter writer)
        {
            //Write <cell>
            writer.WriteStartElement("cell");
            //Write <name>
            writer.WriteStartElement("name");
            //Write name contents
            writer.WriteString(cell.name);
            //Write </name>
            writer.WriteEndElement();
            //Write <contents>
            writer.WriteStartElement("contents");

            string contentString;

            //If contents is a string, just write that string
            if(cell.contents is string)
            {
                contentString = cell.contents as string;
            }
            //If contents is a double, write the string version of that double
            else if(cell.contents is double)
            {
                contentString = ((double)(cell.contents)).ToString();
            }
            else
            {
                //Else contents is a formula, cast it to a formula and get it's string
                contentString = ((Formula)(cell.contents)).ToString();
                //Preappend an "=" to denote it as a formula for future reading
                contentString = String.Concat("=", contentString);
            }

            //Write cell contents
            writer.WriteString(contentString);

            //Write </contents>
            writer.WriteEndElement();
            //Write </cell>
            writer.WriteEndElement();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        /// <param name="name">Name of cell</param>
        /// <returns>Value of cell</returns>
        public override object GetCellValue(string name)
        {
            if (ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }

            //If name isn't null, normalize it before we use it
            name = Normalize(name);

            if (!isValidNameAllCriteria(name))
            {
                throw new InvalidNameException();
            }

            Cell currentCell;

            if(cellSet.TryGetValue(name, out currentCell))
            {
                return currentCell.value;
            }

            //If cell didn't exist in cellset, return an empty string
            return "";
        }

        public string getCellValueAsString(string name)
        {
            object cellValueObj = GetCellValue(name);
            if (cellValueObj.GetType() == typeof(string))
            {
                return (string)cellValueObj;
            }
            else if (cellValueObj.GetType() == typeof(double))
            {
                return cellValueObj.ToString();
            }
            else if (cellValueObj.GetType() == typeof(FormulaError))
            {
                return "FORMULA ERROR";
            }
            return "ERROR";

        }

        /// <summary>
        /// Sets the contents of the desired cell.
        /// Returns a set of cells that have been changed by this operation.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="content">Contents of the cell</param>
        /// <returns>Set of changed cells</returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentNullException();
            }
            else if (ReferenceEquals(name, null))
            {
                throw new InvalidNameException();
            }

            //If name isn't null, normalize it before using it
            name = Normalize(name);

            if (!isValidNameAllCriteria(name))
            {
                //Name isn't valid
                throw new InvalidNameException();
            }



            double temp;

            //If the contents is a double, set it to a double and set the cell contents
            if(Double.TryParse(content, out temp))
            {
                return SetCellContents(name, temp);
            }
            //If it is a formula (starts with =)
            else if(content.Length > 0 && content[0] == '=')
            {
                //Get a new string without the leading equal sign
                string tempString = content.Remove(0, 1);

                //Construct a formula from this string
                Formula tempFormula = new Formula(tempString, Normalize, isValidNameAllCriteria);

                //Set the contents to the formula
                return SetCellContents(name, tempFormula);
            }
            else
            {
                //Else contents is a string, call overloaded method
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// Represents a spreadsheet cell.
        /// </summary>
        private class Cell
        {
            public string name;
            public object contents;
            public object value;

            //Constructs a cell with a name and content
            public Cell(string name, object contents)
            {
                this.name = name;
                this.contents = contents;                
            }
        }
    }


}
