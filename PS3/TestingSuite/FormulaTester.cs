/* Tests written by Derek Burns, u0907203
 * 
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaTester
{
    /// <summary>
    /// Testing Suite for Formula class.
    /// </summary>
    [TestClass]
    public class FormulaTester
    {
        /// <summary>
        /// Lookup delegate, strictly used for testing a variable with no value (throws exception)
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Throws exception</returns>
        double lookupException(string s)
        {
            throw new Exception();
        }

        /// <summary>
        /// Check to see if variable is like a spreadsheet cell's format
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>True if valid variable</returns>
        bool isValid(string s)
        {
            //Must be of format letters followed by numbers (eg. AAAA1112)
            return Regex.IsMatch(s, "^([a-zA-Z])+([1-9])+$");

        }

        //Testing Constructor-----------------------------------------
        //These tests should pass if the constructor doesn't throw an exception.


        [TestMethod]
        public void public_constructor_OneNumber()
        {
            Formula formula = new Formula("45");
        }

        [TestMethod]
        public void public_constructor_TwoNumber()
        {
            Formula formula = new Formula("15 + 55");
        }

        /// <summary>
        /// Makes sure scientific notation is treated as a double in the construction of the formula.
        /// </summary>
        [TestMethod]
        public void public_constructor_ScientificNotation()
        {
            Formula formula = new Formula("5e0");
        }

        /// <summary>
        /// Ensures that the constructor accepts double parenthesis (or more)
        /// </summary>
        [TestMethod]
        public void public_constructor_DoubleParenthesis()
        {
            Formula formula = new Formula("((5+5))");
        }

        /// <summary>
        /// Ensure that constructor accepts variables as first/only token
        /// </summary>
        [TestMethod]
        public void public_constructor_validVariable1()
        {
            Formula formula = new Formula("Abb1");
        }

        /// <summary>
        /// Ensure that constructor accepts properly formatted variables as first/only token
        /// </summary>
        [TestMethod]
        public void public_constructor_validVariable2()
        {
            Formula formula = new Formula("_A_B_1");
        }

        /// <summary>
        /// Ensure initial token must be value/variable
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_invalidInitialToken()
        {
            Formula formula = new Formula(")");
        }

        /// <summary>
        /// Ensure that parenthesis must have values between them
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_invalidParenthesis()
        {
            Formula formula = new Formula("5 + ()");
        }

        /// <summary>
        /// Ensure that a check for balanced parenthesis is made
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_mismatchedParenthesis()
        {
            Formula formula = new Formula("((84 + 123)");
        }

        /// <summary>
        /// Ensure that a check for balanced parenthesis is made
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_mismatchedParenthesis2()
        {
            Formula formula = new Formula("(55+100))");
        }

        /// <summary>
        /// Ensure that an operator must be followed by variable/value/open parenthesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_repeatedOperators()
        {
            Formula formula = new Formula("5 + + 5");
        }

        /// <summary>
        /// Ensure that closing parenthesis must be followed by an operator or another closing parenthesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_InvalidFollowingParenthesis()
        {
            Formula formula = new Formula("(45)50");
        }

        /// <summary>
        /// Ensure that a variable must be followed by an operator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_repeatedNumbers()
        {
            Formula formula = new Formula("50 85");
        }

        /// <summary>
        /// Ensure that an expression must have at least one token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_emptyString()
        {
            Formula formula = new Formula("");
        }

        /// <summary>
        /// Ensure that last token must be a closing parenthesis/value/variable
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_operatorAtEnd()
        {
            Formula formula = new Formula("5 + 89 +");
        }

        /// <summary>
        /// Ensure constructor checks for valid variable format
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_constructor_invalidVariable()
        {
            Formula formula = new Formula("18 + ?A");
        }

        /// <summary>
        /// Ensure constructor succeeds with valid variable
        /// </summary>
        [TestMethod]
        public void public_constructor_validVariable()
        {
            Formula formula = new Formula("18 + _AAA");
        }

        // Testing Constructor with Validator -------------------------------------

        /// <summary>
        /// Ensure that passed validator successfully validates variable
        /// </summary>
        [TestMethod]
        public void public_validator_validVariable()
        {
            Formula formula = new Formula("5 + AAA1", s => s, isValid);
        }

        /// <summary>
        /// Ensure that passed validator detects invalid variables
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void public_validator_validVariable2()
        {
            Formula formula = new Formula("5 + _A", s => s, isValid);
        }


        // Testing Evaluate() -----------------------------------------------------

        /// <summary>
        /// Ensure basic evaluation works
        /// </summary>
        [TestMethod]
        public void public_evaluate_SingleNumber()
        {
            Formula formula = new Formula("5");

            Assert.AreEqual(5d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Ensure values in parenthesis evaluate correctly
        /// </summary>
        [TestMethod]
        public void public_evaluate_Parenthesis()
        {
            Formula formula = new Formula("(5)");

            Assert.AreEqual(5d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Ensure basic addition works correctly
        /// </summary>
        [TestMethod]
        public void public_evaluate_BasicAddition()
        {
            Formula formula = new Formula("5 + 50");

            Assert.AreEqual(55d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Ensure basic subtraction works correctly
        /// </summary>
        [TestMethod]
        public void public_evaluate_BasicSubtraction()
        {
            Formula formula = new Formula("10 - 2");

            Assert.AreEqual(8d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Make sure negative results can be returned
        /// </summary>
        [TestMethod]
        public void public_evaluate_NegativeResult()
        {
            Formula formula = new Formula("20 - 50");

            Assert.AreEqual(-30d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Make sure vasic division works
        /// </summary>
        [TestMethod]
        public void public_evaluate_Division()
        {
            Formula formula = new Formula("50 / 5");

            Assert.AreEqual(10d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Make sure basic multiplication works
        /// </summary>
        [TestMethod]
        public void public_evaluate_Multiplication()
        {
            Formula formula = new Formula("20 * 3");

            Assert.AreEqual(60d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// Make sure the evaluation of a double/scientific double are the same
        /// </summary>
        [TestMethod]
        public void public_evaluate_ScientificEqualsDouble()
        {
            Formula doubleFormula = new Formula("5");
            Formula scientificFormula = new Formula("5e0");

            Assert.AreEqual(doubleFormula.Evaluate(s => 0), scientificFormula.Evaluate(s => 0));
        }

        /// <summary>
        /// Make sure order of operations is followed
        /// </summary>
        [TestMethod]
        public void public_evaluate_ParenthesisMultDivide()
        {
            Formula formula = new Formula("5*(10) + 55/(55)");


            Assert.AreEqual(51d, formula.Evaluate(s => 0));
        }

        /// <summary>
        /// This test ensures that the floating point evaluation is the same evaluation that C# would return
        /// </summary>
        [TestMethod]
        public void public_evaluate_floatingPointEvaluation()
        {
            Formula formula = new Formula("5.5 / 2.5");

            Assert.AreEqual((5.5/2.5), formula.Evaluate(s => 0));
        }

        /// <summary>
        /// This test ensures that the scientific notation evaluation is the same evaluation that C# would return
        /// </summary>
        [TestMethod]
        public void public_evaluate_scientificEvaluation()
        {
            Formula formula = new Formula("4.15e10+5.14e-2");

            Assert.AreEqual((4.15e10 + 5.14e-2), formula.Evaluate(s => 0));
        }

        // Testing Evaluate with Variables ----------------------------------------

        /// <summary>
        /// Ensures that variable lookup works
        /// </summary>
        [TestMethod]
        public void public_evaluate_variablesToZero()
        {
            Formula formula = new Formula("18 + A1");


            Assert.AreEqual(18d, formula.Evaluate(s => 0));
        }

        // Testing FormulaError ---------------------------------------------------

        /// <summary>
        /// Make sure division by zero returns the appropriate FormulaError object
        /// </summary>
        [TestMethod]
        public void public_FormulaError_divideByZero()
        {
            Formula formula = new Formula("18 / 0");

            
            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError && ((FormulaError)formula.Evaluate(s => 0)).Reason == "Division by zero occurred in the expression.");
        }

        /// <summary>
        /// Ensure that Division by zero is caught with variables as well
        /// </summary>
        [TestMethod]
        public void public_FormulaError_divideByZeroVariable()
        {
            Formula formula = new Formula("100 / ABC");


            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError && ((FormulaError)formula.Evaluate(s => 0)).Reason == "Division by zero occurred in the expression.");
        }


        /// <summary>
        /// Ensure a variable with no value is caught
        /// </summary>
        [TestMethod]
        public void public_FormulaError_VariableNoValue()
        {

            Formula formula = new Formula("50 + ABC");

            Assert.IsTrue(formula.Evaluate(lookupException) is FormulaError && ((FormulaError)formula.Evaluate(lookupException)).Reason == "Expression evaluation encountered variable with no associated value.");
        }

        // Testing ToString() -----------------------------------------------------

        /// <summary>
        /// Basic test for toString functionality
        /// </summary>
        [TestMethod]
        public void public_toString_SingleNumber()
        {
            Formula formula = new Formula("13");

            Assert.AreEqual("13", formula.ToString());
        }

        /// <summary>
        /// Ensure a complex expression string isn't changed
        /// </summary>
        [TestMethod]
        public void public_toString_complexExpression()
        {
            Formula formula = new Formula("(15+5)/(20+1)");

            Assert.AreEqual("(15+5)/(20+1)", formula.ToString());
        }

        /// <summary>
        /// Ensure that spaces are removed from the final string
        /// </summary>
        [TestMethod]
        public void public_toString_spaces()
        {
            Formula formula = new Formula("5 / 4 +51+ 8");

            Assert.AreEqual("5/4+51+8", formula.ToString());
        }

        // Testing GetHashCode() ---------------

        /// <summary>
        /// Ensure the hascodes of two equal Formulas are also equal
        /// </summary>
        [TestMethod]
        public void public_hashCode_SameConstructor()
        {
            Formula f1 = new Formula("25");

            Formula f2 = new Formula("25");

            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        /// <summary>
        /// Ensure the hashcode of two equal Formulas are also equal, regardless of how they were constructed (spaces)
        /// </summary>
        [TestMethod]
        public void public_hashCode_equalObjects()
        {
            Formula f1 = new Formula("(85 + (50))");

            Formula f2 = new Formula("(85+(50))");

            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Test GetVariables -------------------------------------------

        /// <summary>
        /// Ensure that the default normalization is case sensitive and proper variables are returned
        /// </summary>
        [TestMethod]
        public void public_getVariables_caseSensitive()
        {
            Formula f1 = new Formula("a + A");

            //Test assumes that the object returns a List for the IEnumerable
            List<string> variableReturned = f1.GetVariables() as List<string>;
            List<string> variableSet = new List<string> { "a", "A" };

            foreach (var item in variableSet)
            { 
            Assert.IsTrue(variableReturned.Contains(item));
            }

        }

        /// <summary>
        /// Ensure that appropriate variables are returned if they are normalized
        /// </summary>
        [TestMethod]
        public void public_getVariables_caseInSensitive()
        {
            Formula f1 = new Formula("a + A", s => s.ToUpper(), v => true);

            //Test assumes that the object returns a List for the IEnumerable
            List<string> variableReturned = f1.GetVariables() as List<string>;

            Assert.IsTrue(variableReturned.Contains("A"));
            Assert.IsFalse(variableReturned.Contains("a"));

        }

        // Testing Equality

        /// <summary>
        /// Make sure two equally constructed Formulas are also equal
        /// </summary>
        [TestMethod]
        public void public_equals_SameConstructor()
        {
            Formula f1 = new Formula("45 + (8 + 90)");

            Formula f2 = new Formula("45 + (8 + 90)");

            Assert.IsTrue(f1 == f2);
        }

        /// <summary>
        /// Make sure floating points are all normalized
        /// </summary>
        [TestMethod]
        public void public_equals_doublePrecision()
        {
            Formula f1 = new Formula("85");

            Formula f2 = new Formula("85.000");

            Assert.IsTrue(f1 == f2);
        }

        /// <summary>
        /// Assert that, despite spaces, the formulas are equal
        /// </summary>
        [TestMethod]
        public void public_equals_spacing()
        {
            Formula f1 = new Formula("91+55");

            Formula f2 = new Formula("91    +   55");

            Assert.IsTrue(f1 == f2);
        }

        /// <summary>
        /// If two formulas are null references, they are equal
        /// </summary>
        [TestMethod]
        public void public_equals_NullReferences()
        {
            Formula f1 = null;

            Formula f2 = null;

            Assert.IsTrue(f1 == f2);
        }

        /// <summary>
        /// If only one Formula is a null reference, they aren't equal
        /// </summary>
        [TestMethod]
        public void public_equalsMethod_NullReferences2()
        {
            Formula f1 = new Formula("5");

            Formula f2 = null;

            Assert.IsFalse(f1.Equals(f2));
        }


        /// <summary>
        /// Assert that even though they evaluate to the same value, they are different formulas
        /// </summary>
        [TestMethod]
        public void public_notEquals_SameConstructor()
        {
            Formula f1 = new Formula("50");

            Formula f2 = new Formula("55 - 5");

            Assert.IsTrue(f1 != f2);
        }

        /// <summary>
        /// Ensure that comparison works properly both ways.
        /// Also, if only one Formula is a null reference, they arent equal.
        /// </summary>
        [TestMethod]
        public void public_notEquals_OneNullReference()
        {
            Formula f1 = new Formula("50");

            Formula f2 = null;

            //Make sure comparison works both ways
            Assert.IsTrue(f1 != f2 && f2 != f1);
        }

        /// <summary>
        /// Ensure that an object of another type is not equal to the Formula
        /// </summary>
        [TestMethod]
        public void public_notEquals_differentObject()
        {
            Formula f1 = new Formula("50");

            Stack<string> otherObject = new Stack<string>();

            //Make sure comparison works both ways
            Assert.IsFalse(f1.Equals(otherObject));
        }
    }
}
