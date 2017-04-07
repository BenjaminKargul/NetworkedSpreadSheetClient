using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace SS
{
    /// <summary>
    /// Example of using a SpreadsheetPanel object
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheetData;

        //Used to keep track of file name and path, so clicking save will work after initial save as
        string filePath;
        string fileName;

        //Used to keep track of a new spreadsheet form being initially saved (clicking save brings
        //up the "save as" dialog instead
        bool initiallySaved;

        /// <summary>
        /// Constructor for the Spreadsheet window.
        /// </summary>
        public Form1()
        {
            spreadsheetData = new Spreadsheet(isValidCellName, s => s.ToUpper(), "ps6");
            fileName = "New Spreadsheet.sprd";

            InitializeComponent();

            //Every time the selection on the panel changes, update the contents of the text boxes on top of GUI.
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(0, 0); //Set initial selection to A1
        }

        /// <summary>
        /// Converts a row and column number into a valid cell name.
        /// </summary>
        /// <param name="row">Row of cell</param>
        /// <param name="col">Column of cell</param>
        /// <returns>Cell name</returns>
        private string cellNameNumToString(int row, int col)
        {
            //Take row number, and add 1 to convert to proper variable name
            row += 1;

            //Convert number to ascii alphabet
            char columnChar = (char)(col + 65);

            return columnChar.ToString() + row.ToString();
        }

        /// <summary>
        /// Takes a cell name and converts it into row and column numbers.
        /// Returns -1 if string isn't a cell name.
        /// </summary>
        /// <param name="cellName">Name of cell</param>
        /// <param name="row">Integer representing row number</param>
        /// <param name="col">Integer representing column number</param>
        private void cellNameStringToNum(string cellName, out int row, out int col)
        {
            row = -1;
            col = -1;

            if (cellName.Length < 2)
            {
                //If the string isn't long enough to work with, return the negative values.
                return;
            }

            //Convert the first character from alphabetic ascii to a valid number
            col = cellName[0] - 65;

            //Try to get the number from the second part of the cell name
            int.TryParse(cellName.Substring(1), out row);
            //Subtract 1, since column 1 actually has a coordinate of 0 in the spreadsheet panel
            row -= 1;
        }

        /// <summary>
        /// Validator for spreadsheet cell names.
        /// </summary>
        /// <param name="s">Name of cell</param>
        /// <returns>Returns true if cell is a valid name.</returns>
        private bool isValidCellName(string s)
        {
            int row;
            int col;

            cellNameStringToNum(s, out row, out col);

            //If row and column are between the boundaries, return true
            bool rowValid = (row >= 0) && (row < 99);
            bool colValid = (col >= 0) && (col < 26);

            return rowValid && colValid;
        }

        /// <summary>
        /// Updates the title according to the save status of the file. 
        /// </summary>
        private void updateFormTitle()
        {
            if(!initiallySaved || spreadsheetData.Changed)
            {
                this.Text = fileName + "*";
            }
            else
            {
                this.Text = fileName;
            }
        }

        /// <summary>
        /// Changes textboxes to display info for selected cell
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;

            ss.GetSelection(out col, out row);
            string cellName = cellNameNumToString(row, col);
            boxCellContents.Text = spreadsheetData.GetCellContentsAsString(cellName);
            boxCellValue.Text = spreadsheetData.getCellValueAsString(cellName);
            boxCurrentCell.Text = cellName;

            updateFormTitle();
        }

        /// <summary>
        /// Called when the "New" menu button is clicked. Opens a new spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNew_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            DemoApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        /// Called when the "Close" menu button is clicked. Closes the current spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Called when "Save As" menu button is clicked. Opens save as dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            SaveAsDialog.FileName = fileName;
            SaveAsDialog.ShowDialog();
        }

        /// <summary>
        /// Called when "Save" menu item is clicked. Saves the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSave_Click(object sender, EventArgs e)
        {
            if (initiallySaved)
            {
                //If the file had already had "save as" called, just save the file.

                spreadsheetData.Save(filePath);


                updateFormTitle();
            }
            else
            {
                //Else, perform a "save as" instead.
                menuSaveAs.PerformClick();
            }

        }

        /// <summary>
        /// The event to run when "Save" is clicked in the save as dialog. Saves the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsDialog_FileOk(object sender, CancelEventArgs e)
        {
            //Get the file path, and the name, of the file.
            filePath = SaveAsDialog.FileName;
            //Filepath actually contains the file name as well, just extract it.
            fileName = Path.GetFileName(filePath);

            //Save the file
            spreadsheetData.Save(filePath);


            initiallySaved = true;

            updateFormTitle();
        }

        /// <summary>
        /// Called when "Help" menu button clicked. Opens help menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuHelp_Click(object sender, EventArgs e)
        {
            HelpWindow helpForm = new HelpWindow();
            helpForm.Show();
            //TODO: Better form of dialog box to use? Center it on the window when created?
            //MessageBox.Show("Temp Help Box", "Help");
        }

        /// <summary>
        /// Called when "Open" menu button clicked. Attempts to open a file over the new spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOpen_Click(object sender, EventArgs e)
        {
            OpenFile.ShowDialog();
        }

        /// <summary>
        /// Sets the contents of the selected cell to the value entered in the cell contents textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetContents_Click(object sender, EventArgs e)
        {
            try
            {
                int row, col;

                //get cell position, name anad new contents from text box
                spreadsheetPanel1.GetSelection(out col, out row);
                string cellName = cellNameNumToString(row, col);
                string newCellContents = boxCellContents.Text;

                //Set the cell to it's new value, update the displayed value in the textbox and spreadsheet
                ISet<string> cellsToUpdate = spreadsheetData.SetContentsOfCell(cellName, newCellContents);
                String value = spreadsheetData.getCellValueAsString(cellName);

                spreadsheetPanel1.SetValue(col, row, value);

                //Update the text boxes with current values
                displaySelection(spreadsheetPanel1);
                                
                updateCellValues(cellsToUpdate);

            } //Only catch if the exception is a CircularException or a FormulaFormatException
            catch (Exception ex) when (ex is CircularException || ex is FormulaFormatException)
            {
                if(ex is CircularException)
                {
                    MessageBox.Show("This change would cause a formula to rely on itself, and cannot be completed.");
                }
                else
                {
                    //Print formula format exception instead
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Used to redraw the desired cells.
        /// </summary>
        /// <param name="cellsToUpdate">The cells to redraw</param>
        private void updateCellValues(IEnumerable<string> cellsToUpdate)
        {
            int row, col;
            String value;

            foreach (string cell in cellsToUpdate)
            {
                //change the displayed cells to reflect updates
                value = spreadsheetData.getCellValueAsString(cell);
                cellNameStringToNum(cell, out row, out col);
                spreadsheetPanel1.SetValue(col, row, value);
            }

            updateFormTitle();
        }

        /// <summary>
        /// Called when the "Open" button on the OpenFile dialog is clicked. Attempts to open the desired file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_FileOk(object sender, CancelEventArgs e)
        {
            if (spreadsheetData.Changed)
            {
                var closeMessage = MessageBox.Show("You have unsaved changes, do you still want to open a new file?",
                     "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (closeMessage == DialogResult.No)
                {
                    return;
                }

            }

            //Store previous data incase an exception happens with opening the file
            string oldFilePath = filePath;
            string oldFileName = fileName;
            Spreadsheet oldSpreadsheet = spreadsheetData;

            //Get file name and path from the dialog
            filePath = OpenFile.FileName;
            fileName = Path.GetFileName(filePath);


            try
            {
                //Create a new spreadsheet with the desired filepath
                spreadsheetData = new Spreadsheet(filePath, isValidCellName, s => s.ToUpper(), "ps6");
            }
            catch(Exception ex)
            {
                //Catch any IO exceptions, restore old data
                filePath = oldFilePath;
                fileName = oldFileName;
                spreadsheetData = oldSpreadsheet;

                MessageBox.Show("Error opening file. \n\n" + ex.Message);

                return;
            }

            //Spreadsheet is technically saved
            initiallySaved = true;

            //Redraw all cells
            updateCellValues(spreadsheetData.GetNamesOfAllNonemptyCells());

            updateFormTitle();
        }

        /// <summary>
        /// Called when the form attempts to close. Prompts the user if data would be lost.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If there is unsaved data, open prompt
            if (spreadsheetData.Changed)
            {
                var closeMessage = MessageBox.Show("You have unsaved changes, do you want to exit without saving?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                //If decided to keep program open, cancel the close
                if (closeMessage == DialogResult.No)
                {
                    e.Cancel = true;
                }
                //Else continue closing program.
                else
                {
                    e.Cancel = false;
                }
            }
        }

        /// <summary>
        /// Called when the "find" menu button is clicked. Opens Find tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindWindow find = new FindWindow(spreadsheetData, spreadsheetPanel1, cellNameStringToNum);

            find.Show();
        }

        /// <summary>
        /// Called when Clear button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSpreadSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var clearMessage = MessageBox.Show("Are you sure you want to clear the spreadsheet?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //If decided not to clear
            if (clearMessage == DialogResult.No)
            {
                return;
            }
            //Else continue clearing

            int row, col;
            boxCellContents.Text = "";
            boxCellValue.Text = "";

            //Go through each cell in the spreadsheet and clear it
            foreach (string s in spreadsheetData.GetNamesOfAllNonemptyCells())
            {
                spreadsheetData.SetContentsOfCell(s,"");
                cellNameStringToNum(s, out row, out col);
                spreadsheetPanel1.SetValue(col, row, "");
            }
            updateFormTitle();
        }

        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphForm graphWindow = new GraphForm(spreadsheetData);
            graphWindow.Show();
        }
    }
}
