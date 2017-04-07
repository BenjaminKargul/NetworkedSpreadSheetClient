using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        Dictionary<string, Cell> cellSet;
        DependencyGraph dependencies;

        /// <summary>
        /// Creates a new, empty Spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            cellSet = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (ReferenceEquals(name, null))
            {
                throw new ArgumentNullException();
            }
            else if (!isValidName(name))
            {
                throw new InvalidNameException();
            }

            Cell cell;

            //If the cell is in the Dictionary, it has a value. Return the contents of the cell.
            if(cellSet.TryGetValue(name, out cell))
            {
                return cell.contents;
                //TODO: If a formula, return a copy of the formula
            }
            else
            {
                //Else if cell doesn't contain a value, return empty string
                return "";
            }

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
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
        public override ISet<string> SetCellContents(string name, Formula formula)
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
        public override ISet<string> SetCellContents(string name, string text)
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
        public override ISet<string> SetCellContents(string name, double number)
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
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Use the dependency graph to return the dependents of a cell
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// An abstracted helper method that all SetCellContents methods can call.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <param name="contents">Object to put into the cell (double, String, Formula)</param>
        /// <returns>Set of the cells that need to be updated.</returns>
        private ISet<string> SetCellContentsHelper(string name, object contents)
        {
            if(ReferenceEquals(name, null))
            {
                throw new ArgumentNullException();
            }
            else if (!isValidName(name))
            {
                throw new InvalidNameException();
            }

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
            bool contentsIsFormula = contents is Formula;

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
            else if (contentsIsFormula)
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
        /// Checks to make sure the input string is a valid cell name
        /// </summary>
        /// <param name="name">Cell Name</param>
        /// <returns></returns>
        private bool isValidName(string name)
        {
            //Must begin with an underscore or letters, followed by more underscores/letters/numbers
            bool isValid = Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.IgnorePatternWhitespace);

            return isValid;
        }

        /// <summary>
        /// Represents a spreadsheet cell.
        /// </summary>
        private class Cell
        {
            public string name;
            public object contents;

            public Cell(string name, object contents)
            {
                this.name = name;
                this.contents = contents;                
            }
        }
    }


}
