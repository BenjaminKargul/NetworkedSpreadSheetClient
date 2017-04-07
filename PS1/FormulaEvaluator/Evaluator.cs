/* Author: Derek Burns
 * 
 * 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//TODO: More in depth commenting at the code level.

namespace FormulaEvaluator
{
    /// <summary>
    /// This class contains tools for evaluating math expressions.
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// This is for the use of looking up the number assosciated with a string variable.
        /// </summary>
        /// <param name="v">The string of the variable to lookup.</param>
        /// <returns>The int associated with the input variable.</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// This function evaluates an input string math expression.
        /// </summary>
        /// <param name="exp">Math expression</param>
        /// <param name="variableEvaluator">Delegate to evaluate variables.</param>
        /// <returns>The solution to the expression.</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Remove whitespace before tokenizing
            string removedWhitespace = Regex.Replace(exp, @"\s*", string.Empty);

            //Split input string insto usable tokens
            string[] substrings = Regex.Split(removedWhitespace, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            foreach (string s in substrings)
            {
                ProcessToken(values, operators, s, variableEvaluator);
            }

            //Gone through all tokens. Do final check of operators.

            //If no operators, ensure there is only one value left
            if (operators.Count == 0 && values.Count == 1)
            {
                return values.Pop();
            }
            //Else make sure only one operator remains, that it is a '+' or '-', and that there are exactly two values for it to work on.
            else if (operators.Count == 1 && values.Count == 2 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
            {
                AddSubtractToken(values, operators, "no token to pass", false);

                return values.Pop();
            }
            //Else there was a mismatch between the number of integers and operators.
            else
            {
                throw new ArgumentException("Mismatch between amount of values and operators.");
            }
        }

        /// <summary>
        /// Takes a token and performs the appropriate arithmetic / operation.
        /// </summary>
        /// <param name="values">Stack of the current values.</param>
        /// <param name="operators">Stack of the current operators.</param>
        /// <param name="s">The current token being evaluated</param>
        /// <param name="variableEvaluator">The lookup method for string variables (if any)</param>
        private static void ProcessToken(Stack<int> values, Stack<string> operators, string s, Lookup variableEvaluator)
        {
            //Used if the token is an integer.  
            int tempInt;

            if (s.Equals(" ") || s.Equals("")) //If there is white space, ignore the string
            {
                return;
            }
            else if (int.TryParse(s, out tempInt))
            {
                IntToken(values, operators, tempInt);
            }

            else if (s.Equals("("))
            {
                operators.Push(s);
                return;
            }
            else if (s.Equals(")"))
            {
                CloseParenthesis(values, operators);
            }
            else if (s.Equals("*") || s.Equals("/"))
            {
                operators.Push(s);
                return;
            }
            else if (s.Equals("+") || s.Equals("-"))
            {
                AddSubtractToken(values, operators, s, true);
            }
            //Make sure it is a valid variable, otherwise throw Argument Exception
            else if (Regex.IsMatch(s, "^([a-zA-Z])+([1-9])+$")) {
                IntToken(values, operators, variableEvaluator(s));
            }
            else
            {
                throw new ArgumentException(s + " is not a valid token.");
            }
        }

        /// <summary>
        /// Determines whether the input integer should be pushed to the stack, or if multiplication/division needs to be performed on the integer first.
        /// </summary>
        /// <param name="values">Stack of current values</param>
        /// <param name="operators">Stack of current operators</param>
        /// <param name="integer">The integer to be operated on.</param>
        private static void IntToken(Stack<int> values, Stack<string> operators, int integer)
        {
            //Ensure that we can check the top of the stack for mult/divide operators, 
            //then check if we need to multiply/divide the integer
            if (operators.Count >= 1 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
            {
                MultDivideToken(values, operators, integer);
            }
            else
            {
                //If nothing to operate on, just push the integer to the stack.
                values.Push(integer);
            }

        }

        /// <summary>
        /// Performs addition or subtraction on the top two integers in the input stack if the previous operator calls for it. Then pushes the current
        /// operator onto the stack.
        /// </summary>
        /// <param name="values">Stack of integer values.</param>
        /// <param name="operators">Stack of string arithmetic operators.</param>
        /// <param name="token">The token to be pushed onto the stack.</param>
        /// <param name="pushToken"> Whether or not the input token should be pushed onto the stack after arithmetic is done.
        /// (useful if the input token is a closing parenthesis, which shouldn't be pushed to the stack.)</param>
        private static void AddSubtractToken(Stack<int> values, Stack<string> operators, string token, bool pushToken)
        {
            int tempValueLefthand;
            int tempValueRighthand;

            //Check to see if there is any prior addition/subtraction to do
            if (operators.Count >= 1 && operators.Peek().Equals("+"))
            {
                if (values.Count < 2)
                {
                    throw new ArgumentException("Not enough integers to perform addition.");
                }
                tempValueRighthand = values.Pop();
                tempValueLefthand = values.Pop();
                // remove "+" from stack
                operators.Pop();

                values.Push(tempValueLefthand + tempValueRighthand);
            }
            else if (operators.Count >= 1 && operators.Peek().Equals("-"))
            {
                if (values.Count < 2)
                {
                    throw new ArgumentException("Not enough integers to perform subtraction.");
                }

                tempValueRighthand = values.Pop();
                tempValueLefthand = values.Pop();
                // remove "-" from stack
                operators.Pop();

                values.Push(tempValueLefthand - tempValueRighthand);
            }

            if (pushToken)
            {
                //Push the current token
                operators.Push(token);
            }
        }

        /// <summary>
        /// Determines what to do if a closed parenthesis is encountered. (Performs order of operations inside and immediately outside of the parenthesis in question)
        /// </summary>
        /// <param name="values">Stack of the current values.</param>
        /// <param name="operators">Stack of the current operators.</param>
        private static void CloseParenthesis(Stack<int> values, Stack<string> operators)
        {
            //Check to see if there is any addition/subtraction to do in the parenthesis, but don't push
            //right parenthesis onto the operator stack
            AddSubtractToken(values, operators, " ", false);

            //Check to see if there isn't a complimenting opening parenthesis
            if (operators.Count < 1 || !operators.Peek().Equals("("))
            {
                throw new ArgumentException("Parenthesis mismatch. Expected '('.");
            }

            operators.Pop(); //Remove matching parenthesis

            //Check to see if there is any multiplication or division to do right before the opening parenthesis
            if (operators.Count >= 1 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
            {
                if (values.Count < 2)
                {
                    throw new ArgumentException("Not enough values for multiplication/division operators to use.");
                }

                int tempInt = values.Pop();
                MultDivideToken(values, operators, tempInt);
            }

        }

        /// <summary>
        /// Performs a multiplication or division, with the numerator being the topmost integer on the stack, and the 
        /// denominator being the input integer.
        /// </summary>
        /// <param name="values">Stack of current values</param>
        /// <param name="operators">Stack of current operators</param>
        /// <param name="integer">The integer (denominator for division)</param>
        private static void MultDivideToken(Stack<int> values, Stack<string> operators, int integer)
        {
            int poppedValue;

            //If topmost operator is *, perform multiplication
            if (operators.Peek().Equals("*"))
            {
                if (values.Count < 1)
                {
                    throw new ArgumentException("Operator '*' has no lefthand integer to multiply.");
                }
                operators.Pop();
                poppedValue = values.Pop();
                values.Push(poppedValue * integer);
            }
            //Else do division
            else if (operators.Peek().Equals("/"))
            {
                if (values.Count < 1)
                {
                    throw new ArgumentException("Operator '/' has no numerator.");
                }
                if (integer == 0)
                {
                    throw new ArgumentException("Can't divide by zero.");
                }

                operators.Pop();
                poppedValue = values.Pop();
                values.Push(poppedValue / integer);
            }
        }
    }
}
