using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;

namespace PS1_Testing
{
    class VariableStuff
    {
        public static int VariableConstant(string s)
        {
            return 2;
        }

    }

    class Test
    {
        //TODO: More tests (Variable lookup?)
        static void Main(string[] args)
        {
            //--------Testing class example------------

            testExpression("5 + 3 * 7 -8 / (4 + 3)-2 / 2", 24, VariableStuff.VariableConstant);

            //--------Testing Lab Example--------------

            testExpression("(10/(6/3)/2)", 2, VariableStuff.VariableConstant);

            testExpression("7(*8)", 56, VariableStuff.VariableConstant);

            testExpression("(7)(8)(9)++", 24, VariableStuff.VariableConstant);

            //--------Testing base arithmetic----------

            testExpression("5+5", 10, VariableStuff.VariableConstant);

            testExpression("5-5", 0, VariableStuff.VariableConstant);

            testExpression("5*5", 25, VariableStuff.VariableConstant);

            testExpression("5/5", 1, VariableStuff.VariableConstant);

            testExpression("(5)", 5, VariableStuff.VariableConstant);

            //-------Testing mixed arithmetic---------

            testExpression("5+5-5", 5, VariableStuff.VariableConstant);

            testExpression("5+5*5", 30, VariableStuff.VariableConstant);

            testExpression("5+4/2", 7, VariableStuff.VariableConstant);

            testExpression("5-5/5", 4, VariableStuff.VariableConstant);

            testExpression("5*5-5", 20, VariableStuff.VariableConstant);

            testExpression("4*(5*4 + 3)", 92, VariableStuff.VariableConstant);

            //-------Test int division---------

            testExpression("1/7", 0, VariableStuff.VariableConstant);

            testExpression("5/2", 2, VariableStuff.VariableConstant);

            //-------Test whitespace/newlines

            testExpression("5 4+ 6", 60, VariableStuff.VariableConstant);

            testExpression("5\n4+6", 60, VariableStuff.VariableConstant);

            //-------Test case insensitive Variables (All vars = 2)----

            testExpression("AAAA1111", 2, VariableStuff.VariableConstant);

            testExpression("aaAA1111", 2, VariableStuff.VariableConstant);

            testExpression("abaa12123", 2, VariableStuff.VariableConstant);

            //-----Test invalid variable----

            try
            {
                testExpression("ab123aaa", 0, VariableStuff.VariableConstant);
                Console.WriteLine("Failed \t Test of invalid variable format: \"ab123aaa\"");
                Console.WriteLine("\t Proper ArgumentException wasn't thrown.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Passed \t Test of invalid variable format: \"ab123aaa\"");
                Console.WriteLine("\t Proper ArgumentException was thrown.");
                Console.WriteLine("\t \"" + e.Message + "\""); 
            }


            Console.Read();
        }

        private static void testExpression(string expression, int expected, Evaluator.Lookup VariableLookup)
        {
            int result = Evaluator.Evaluate(expression, VariableLookup);

            bool success = false;

            if (expected == result)
            {
                success = true;
            }

            if(success)
            {
                Console.WriteLine("Passed \t Test of \"" + @expression + "\".");
            }
            else
            {
                Console.WriteLine("Failed \t Test of \"" + @expression + "\".");
            }
            Console.WriteLine("\t Expected: " + expected);
            Console.WriteLine("\t Returned: " + result);
        }

    }
}
