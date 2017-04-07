Spreadsheet, PS5 README.txt

Author: Derek Burns
Readme last edited: 10/6/2016, 10:30 PM


Project Dependencies
---------------------

-This project utilizes the DLLs built from the PS2 and PS3 assignments that were turned in.
		* (Formula.dll and SpreadsheetUtilities.dll, build Sep 27, 2016 11:52 PM)



Initial Thoughts
---------------------

-First looking through the project, I saw that a method for detecting circular dependencies was already
implemented for us. I also saw that the same provided methods returned a list of variables that needs to be 
recalculated, and noted that I could use these to simultaneously get the Set of dependents while checking for
circular dependencies, without the need for a lot of ugly looking code.

Notes for TA
---------------------

-For some reason, the Code Coverage also tests for the testing class itself, which brings down the average percentage.
		* My Spreadsheet project itself has 98.36% coverage. (9/29/2016)

-I can't, for the life of me, figure out how to include files into the bin folder on the repository, so I have no idea
how to test manually created files that are meant to be invalid and throw SpreadsheetReadWriteExceptions. (10/6/2016)

-Fixed the previous problem by just having the files stored in a resource folder, and having the spreadsheet constructor
go up two directories to the project folder, and then into the directory of the XML files (10/6/2016)

Project Notes
---------------------

-All of the SetCellContents methods were abstracted to the same method, since they all share similar code. In the case that a
Formula is passed as the contents of a cell, It will swap out the cell contents with the new Formula, keeping the old contents and
dependees incase a CircularDependency is found. In that case, the old contents/dependees are restored. (9/29/2016)

-When loading in a spreadsheet file, I opted to use an XmlDocument instead of an XmlReader simply because it has the ability
to go through elements, rather than manually reading each line of the document. Makes reading the hierarchy of the elements
much easier, and I didn't have to deal with a bunch of confusing booleans to keep track of whether  i'm in a nested element, etc.... (10/6/2016)


- Branched from PS4, [Commit f87dec6, Sep 29, 2016] (10/4/2016, 1:45 PM)
