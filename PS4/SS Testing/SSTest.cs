using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace SS_Testing
{
    [TestClass]
    public class SSTest
    {
        /// <summary>
        /// Ensures no exception is thrown when passing a valid string (starting with an underscore)
        /// </summary>
        [TestMethod]
        public void public_validName1()
        {
            Spreadsheet ss = new Spreadsheet();

            //Test should pass if method doesn't throw exception
            ss.SetCellContents("_abc123", 5);
        }

        /// <summary>
        /// Ensures that no exception is thrown when passing a valid string (Starting with a letter)
        /// </summary>
        [TestMethod]
        public void public_validName2()
        {
            Spreadsheet ss = new Spreadsheet();

            //Test should pass if method doesn't throw exception
            ss.SetCellContents("abc123", 5);
        }

        /// <summary>
        /// Ensures passing a null string into SetCellContents throws the proper exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void public_nullName()
        {
            Spreadsheet ss = new Spreadsheet();

            //Null name should be an invalid name
            ss.SetCellContents(null, 5);
        }

        /// <summary>
        /// Ensures passing a null string into GetCellContents throws the proper exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void public_nullName2()
        {
            Spreadsheet ss = new Spreadsheet();

            //Null name should be an invalid name
            ss.GetCellContents(null);
        }

        /// <summary>
        /// Ensure that an InvalidNameException is properly thrown by SETCellContents if an invalid string is passed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void public_invalidName()
        {
            Spreadsheet ss = new Spreadsheet();

            //Name not starting with _ or letter should be an invalid name
            ss.SetCellContents("1AB", 5);
        }

        /// <summary>
        /// Ensure that an InvalidNameException is properly thrown by GETCellContents if an invalid string is passed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void public_invalidName2()
        {
            Spreadsheet ss = new Spreadsheet();

            //Name not starting with _ or letter should be an invalid name
            ss.GetCellContents("1AB");
        }

        /// <summary>
        /// Ensure that a circular exception is properly thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void public_CircularException1()
        {
            Formula b1 = new Formula("A1 + 5");
            Formula a1 = new Formula("B1 + 5");

            Spreadsheet ss = new Spreadsheet();

            ss.SetCellContents("A1", a1);
            ss.SetCellContents("B1", b1);

        }

        /// <summary>
        /// Ensure that a cell that contains itself as a dependee throws a circular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void public_CircularException2()
        {
            Formula a1 = new Formula("A1");

            Spreadsheet ss = new Spreadsheet();

            ss.SetCellContents("A1", a1);

        }

        /// <summary>
        /// This method tests to make sure that, if a circular exception is thrown, the cell contents
        /// is left unchanged.
        /// </summary>
        [TestMethod]
        public void public_CircularExceptionUnchangedCell()
        {
            //Cell's original formula
            Formula one = new Formula("B1 + 5");

            //Formula that causes exception
            Formula two = new Formula("A1");

            Spreadsheet ss = new Spreadsheet();

            ss.SetCellContents("A1", one);

            try
            {
                ss.SetCellContents("A1", two);
            }
            catch(CircularException e)
            {
                
            }

            //Ensure that the formula in cell A1 is the same as the original formula, unchanged.
            Formula contents = ss.GetCellContents("A1") as Formula;
            Assert.AreEqual(one, contents);

        }

        /// <summary>
        /// This test ensures that the SetCellContents properly updates an existing cell's contents.
        /// </summary>
        [TestMethod]
        public void public_updateContents()
        {
            Spreadsheet ss = new Spreadsheet();

            String s = "Hello";

            ss.SetCellContents("a1", s);

            Assert.AreEqual(ss.GetCellContents("a1"), s);

            ss.SetCellContents("a1", "");

            Assert.AreEqual(ss.GetCellContents("a1"), "");
        }

        /// <summary>
        /// Ensures that GetNamesOfAllNonemptyCells properly returns a set of nonempty cells.
        /// </summary>
        [TestMethod]
        public void public_getNonEmptyCells()
        {
            Spreadsheet ss = new Spreadsheet();

            HashSet <string> variableSet= new HashSet<string>() { "a1", "b1", "c1" };

            foreach(var s in variableSet)
            {
                ss.SetCellContents(s, "Test String");
            }

            foreach (var s in ss.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(variableSet.Contains(s));
            }

        }

        /// <summary>
        /// Ensures that GetNamesOfAllNonemptyCells properly returns a set of nonempty cells.
        /// This time, a cell is emptied, and ensures that the now-empty cell isn't returned.
        /// </summary>
        [TestMethod]
        public void public_getNonEmptyCellsDeletion()
        {
            Spreadsheet ss = new Spreadsheet();

            HashSet<string> variableSet = new HashSet<string>() { "a1", "b1", "c1" };

            foreach (var s in variableSet)
            {
                ss.SetCellContents(s, "Test String");
            }

            ss.SetCellContents("c1", "");

            variableSet.Remove("c1");

            foreach (var s in ss.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(variableSet.Contains(s));
            }

        }
    }
}
