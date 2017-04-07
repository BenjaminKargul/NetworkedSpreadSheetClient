Spreadsheet, PS4 README.txt

Author: Derek Burns
Readme last edited: 9/29/2016, 8:29 PM


Project Dependencies
---------------------

This project utilizes the DLLs built from the PS2 and PS3 assignments that were turned in.
		* (Formula.dll and SpreadsheetUtilities.dll, build Sep 27, 2016 11:52 PM)



Initial Thoughts
---------------------

First looking through the project, I saw that a method for detecting circular dependencies was already
implemented for us. I also saw that the same provided methods returned a list of variables that needs to be 
recalculated, and noted that I could use these to simultaneously get the Set of dependents while checking for
circular dependencies, without the need for a lot of ugly looking code.


Project Notes
---------------------

All of the SetCellContents methods were abstracted to the same method, since they all share similar code. In the case that a
Formula is passed as the contents of a cell, It will swap out the cell contents with the new Formula, keeping the old contents and
dependees incase a CircularDependency is found. In that case, the old contents/dependees are restored. (9/29/2016)


Notes for TA
---------------------

For some reason, the Code Coverage also tests for the testing class itself, which brings down the average percentage.
		* My Spreadsheet project itself has 98.36% coverage. (9/29/2016)