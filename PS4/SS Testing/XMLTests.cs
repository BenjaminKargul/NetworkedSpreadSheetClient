/* Author: Derek Burns, u0907203
 * 
 * Tests regarding reading/saving of spreadsheets.
 * 
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;

namespace SS_Testing
{
    [TestClass]
    public class XMLTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Spreadsheet savedSheet = new Spreadsheet();

            savedSheet.SetContentsOfCell("a1", "Haha");
            savedSheet.SetContentsOfCell("b1", "=AB12");

            savedSheet.Save("testXML.xml");

            Spreadsheet loadedSheet = new Spreadsheet("testXML.xml", a => true, b => b, "default");

            Assert.AreEqual(((string)loadedSheet.GetCellContents("a1")), "Haha");

            Assert.IsInstanceOfType(loadedSheet.GetCellValue("b1"), typeof(FormulaError));
        }

        [TestMethod]
        public void testVersionGet()
        {
            Spreadsheet sheet = new Spreadsheet(a => true, b => b, "125");
            sheet.Save("testVersion.xml");

            Assert.AreEqual(sheet.GetSavedVersion("testVersion.xml"), "125");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testMismatchVersion()
        {
            Spreadsheet sheet = new Spreadsheet(a => true, b => b, "125");
            sheet.Save("testVersion.xml");

            Spreadsheet secondsheet = new Spreadsheet("testVersion.xml", a => true, b => b, "300");

        }

        /// <summary>
        /// Tests for double declaration of cell name (two sets of <name></name> tags)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testRepeatCellName()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/repeatedCellName.xml", a => true, b => b, "default");

        }

        /// <summary>
        /// Tests for double declaration of cell contents (two sets of <contents></contents> tags)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testRepeatCellContents()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/repeatedCellContents.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for an invalid element directly nested within the Spreadsheet tags
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testInvalidElementSpreadsheet()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/invalidElementSpreadsheet.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for the lack of text between <name></name>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testNoTextBetweenNameTags()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/noTextBetweenNameTags.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for the lack of text between <contents></contents>
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testNoTextBetweenContentsTags()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/noTextBetweenContentsTags.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for the lack of a specified version
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testNoVersion()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/noVersion.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for the lack of a specified version when using GetSavedVersion
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testNoVersion2()
        {
            Spreadsheet sheet = new Spreadsheet(a => true, b => b, "default");

            sheet.GetSavedVersion("../../XML Files/noVersion.xml");


        }

        /// <summary>
        /// Tests for an incorrect element nested within a Cell element
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testIncorrectNestedCellElement()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/incorrectNestedCellElement.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for double declaration of a spreadsheet
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testDoubleSpreadsheetdeclaration()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/spreadsheetNotRootNode.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for random text inside of the <cell></cell> tags (and outside of name or contents tags)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testRandomTextCellDeclaration()
        {
            Spreadsheet sheet = new Spreadsheet("../../XML Files/randomTextInCellDeclaration.xml", a => true, b => b, "default");


        }

        /// <summary>
        /// Tests for double declaration of a cell element
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void testInvalidFileName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("ab:cd.xml");


        }
    }
}
