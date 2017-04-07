Spreadsheet, PS6 README.txt

Author: Derek Burns, Madeline MacDonald
Readme last edited: 11/3/2016, 4:13 PM


Project Dependencies
---------------------

-This project utilizes the DLLs built from the PS2 and PS3 assignments that were turned in.
	Builds were made from the PS6 Branch.
		* (SpreadsheetUtilities.dll, build Sep 27, 2016 11:52 PM)
		* (Formula.dll, build November 1, 2016, 9:20PM)


Project Notes
---------------------
- Branched from PS5, [Commit 6f2b3ea9, Oct 26, 2016]

-The underlying class (model) behind our spreadsheet program is the Spreadsheet class, directly stored within the Form class.
-In order to handle data loss, we simply registered a delegate to the Form's "FormClosing" event, and if the user selects no, the
	delegate cancels the close operation. This case was also handled separately if the user tried opening a file while there are
	unsaved changes.
-In order to highlight cells, the SpreadsheetPanel class had to be modified, which is why it is included as its own project within 
	the solution. This modification can be found in DrawingPanel's "onPaint" method, around line 430.
-The Title of a spreadsheet has an asterisk next to the file name if it is an unsaved file.
-The title of the windows form is shown as the name of the saved spreadsheet file, or as "New Spreadsheet.sprd".
-Graphing is enabled in this project under the extra tab on the menu. All graphing is done as a line graph, with invalid points
	being ommited from the data points on the graph. The completed graph is automatically downloaded into the debug folder
	within the project. Data is graphed by entering a column to put X values from and a column to pull Y values from, and
	a range of row values. The values in the X column are matched with the values in the Y column for each row within the range
	and those points are plotted as a line graph.

