using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{

    /// <summary>
    /// Used to convert cell names to integer addresses.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public delegate void NameToAddressConverter(string s, out int row, out int col);

    public partial class FindWindow : Form
    {
        SpreadsheetPanel sPanel;
        Spreadsheet sheet;
        NameToAddressConverter nameConverter;

        public FindWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Used to open a find tool.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="panel"></param>
        /// <param name="converter"></param>
        public FindWindow(Spreadsheet ss, SpreadsheetPanel panel, NameToAddressConverter converter) : this()
        {
            sPanel = panel;
            sheet = ss;
            nameConverter = converter;
        }

        /// <summary>
        /// Called when the find button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findButton_Click(object sender, EventArgs e)
        {
            //Clear previously highlighted cells.
            sPanel.clearHighlightedCells();

            //If there is nothing to search for, just stop here
            if(searchBox.Text == "")
            {
                resultText.Text = "No results.";
                return;
            }

            bool containsValue;
            bool containsContent;
            string cellContents;
            string cellValue;

            int row = 0;
            int col = 0;

            int numResults = 0;

            

            IEnumerable<String> activeCells = sheet.GetNamesOfAllNonemptyCells();

            //Go through each cell in the spreadsheet
            foreach(string cell in activeCells)
            {
                cellContents = sheet.GetCellContentsAsString(cell);
                cellValue = sheet.GetCellValue(cell).ToString();

                //Check whether or not the cells contain the values (depending on whether or not case is desired)
                if (ignoreCase.Checked)
                {
                    containsValue = cellValue.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    containsContent = cellContents.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                else
                {
                    containsValue = cellValue.IndexOf(searchBox.Text) >= 0;
                    containsContent = cellContents.IndexOf(searchBox.Text) >= 0;
                }
                
                //If contains the value, or if "check formulas" and the cells contain that formula, add that cell to be highlighted
                if((containsContent && formulaSearch.Checked) || (containsValue))
                {
                    nameConverter(cell, out row, out col);

                    sPanel.highlightCell(col, row);

                    //Add one to the results
                    numResults++;
                }
                
            }

            //Display the correct result text.
            switch(numResults)
            {
                case 0:
                    resultText.Text = "No results.";
                    break;
                case 1:
                    resultText.Text = "1 result.";
                    break;
                default:
                    resultText.Text = numResults + " results.";
                    break;
            }

        }

        /// <summary>
        /// Called when tool is closed. Clears highlighting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopHighlighting(object sender, FormClosingEventArgs e)
        {
            sPanel.clearHighlightedCells();
        }
    }
}
