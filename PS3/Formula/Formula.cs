// Implementation by Derek Burns, u0907203
//
//
// 
//
//Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> tokens;

        //This list is just for easy listing of variables without having to iterate through the tokens List and do tons of checks.
        //Sacrificing small amount of memory space for faster running code.
        private HashSet<string> variables;

        //These are set to true if either division by zero or a valueless variable is hit while performing arithmetic.
        //Makes it easier for the Evaluate method to quickly return a FormulaError struct
        private bool dividedByZero = false;
        private bool valuelessVariable = false;


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// <para>
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.
        /// </para>
        /// </summary>
        /// <param name="formula">String math expression</param>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }




        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression.  
        /// If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// </summary>
        /// <param name="formula">String expression</param>
        /// <param name="normalize">Normalizes all variables to one format</param>
        /// <param name="isValid">Defines a valid variable</param>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            IEnumerable<string> tokenEnumarable = GetTokens(formula);

            this.tokens = new List<string>();
            this.variables = new HashSet<string>();

            int tokenCount = 0;
            int leftParenthCount = 0;
            int rightParenthCount = 0;

            //This is set to true if the previous token was a left parenthesis or an operator
            bool parenthesisOperatorRule = false;
            
            //This is set to true if the previous token was a variable, number, or right parenthesis.
            bool followingRule = false;

            double tempDouble; //Used for Double.TryParse, since it requires a variable to pass in

            

            foreach (var token in tokenEnumarable)
            {
                //Check first token rule / parenthesis following rule, this token should be opening parenthesis, number, or valid variable
                if(tokenCount == 0 || parenthesisOperatorRule)
                {
                    if(Double.TryParse(token, out tempDouble))
                    {
                        //Make next token check for valid token following a number
                        followingRule = true;
                        //Since this token passed the parenthesis following rule, set to false.
                        parenthesisOperatorRule = false;
                    }
                    else if(token == "(")
                    {
                        //Make next token check for valid token following an opening parenthesis
                        parenthesisOperatorRule = true;
                        leftParenthCount++;
                    }
                    //Else make sure is a valid variable by both theFormula class standards,
                    //as well as by the delegate
                    else if (isVariable(token) && isValid(normalize(token)))
                    {
                        //Make next token check for valid token following a variable
                        followingRule = true;
                        //Since this token passed the parenthesis following rule, set to false.
                        parenthesisOperatorRule = false;
                    }
                    else if(parenthesisOperatorRule)
                    {
                        throw new FormulaFormatException("Token immediately following an opening parenthesis/operator should be a number, valid variable, or opening parenthesis.");
                    }
                    else
                    {
                        throw new FormulaFormatException("First token of formula must be a number, valid variable, or opening parenthesis.");
                    }
                }
                //If previous token was a number/variable/closing parenthesis
                else if(followingRule)
                {
                    if(token == ")")
                    {
                        //If the parenthesis counts were balanced before this token was about to be added to the rightParenthCount
                        if(leftParenthCount == rightParenthCount)
                        {
                            throw new FormulaFormatException("No matching opening parenthesis for closing parenthesis.");
                        }
                        followingRule = true;
                        rightParenthCount++;
                    }
                    else if(token == "+" || token == "-" || token == "*" || token == "/")
                    {
                        //This token satisfied the previous token's rules, can set to false
                        followingRule = false;
                        //Make next token be a value, variable, or opening parenthesis
                        parenthesisOperatorRule = true;
                    }
                    else
                    {
                        throw new FormulaFormatException(@"Token immediately following number/variable/closing parenthesis should be either an operator or a closing parenthesis.");
                    }
                }

                //If it is a variable, normalize it before adding to the tokens list
                if (isVariable(token) && isValid(normalize(token)))
                {
                    tokens.Add(normalize(token));
                    variables.Add(normalize(token));
                }
                //If it is a double, normalize it with double.ToString() and add to token list
                else if(Double.TryParse(token, out tempDouble))
                {
                    tokens.Add(tempDouble.ToString());

                }
                else
                {
                    tokens.Add(token);
                }

                tokenCount++;
            }

            if (tokens.Count == 0)
            {
                throw new FormulaFormatException("There needs to be at least one token in the expression.");
            }
            
            //If there is a parenthesis mismatch
            if(leftParenthCount != rightParenthCount)
            {
                throw new FormulaFormatException("Parenthesis mismatch. Not enough closing parenthesis.");          
            }

            string previousToken = tokens[tokenCount - 1];

            //Check to make sure the last token follows the proper format rules.
            if (previousToken == ")")
            {
            }
            else if (Double.TryParse(previousToken, out tempDouble))
            {
            }
            else if(isVariable(previousToken) && isValid(normalize(previousToken)))
            {
            }
            else
            {
                throw new FormulaFormatException("Last token should be a valid variable/value/closing parenthesis.");
            }
        }

        /// <summary>
        /// Returns whether or not a variable conforms to Formula's mandatory format. (I.E. C#'s variable format)
        /// </summary>
        /// <param name="token">String to check</param>
        /// <returns></returns>
        private bool isVariable(string token)
        {
            if(token == "+" || token == "-" || token == "*" || token == "/" || token == "(" || token == ")")
            {
                return false;
            }

            return Regex.IsMatch(token, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables. Variables are normalized (if normalizer was passed in constructor) before being looked up.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        /// </summary>
        /// <param name="lookup">Lookup for variables</param>
        /// <returns>Evaluated double of this formula, if a valid one exists. Otherwise, returns FormulaError object.</returns>
        public object Evaluate(Func<string, double> lookup)
        {
            dividedByZero = false;
            valuelessVariable = false;

            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            foreach (string s in tokens)
            {
                ProcessToken(values, operators, s, lookup);

                //If processing the last token resulted in dividing by zero
                if(dividedByZero)
                {
                    return new FormulaError("Division by zero occurred in the expression.");
                }
                //Elseif the last token involved a variable with no value
                else if (valuelessVariable)
                {
                    return new FormulaError("Expression evaluation encountered variable with no associated value.");
                }
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

            //Next line of code never really runs since validity of expression is checked upon construction.
            //A return is still needed to make the compiler happy, since it doesn't know how we call this function
            return values.Pop();

        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetVariables()
        {
            return variables.ToList();
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// </summary>
        public override string ToString()
        {
            StringBuilder outputString = new StringBuilder();

            foreach (string token in tokens)
            {
                outputString.Append(token);
            }

            return outputString.ToString();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// </summary>
        /// <param name="obj">Other object for comparison</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //Safe type casting. If it is a formula, it will set the reference, otherwise it is null.
            Formula other = obj as Formula;

            if(ReferenceEquals(other, null))
            {
                return false;
            }
            if (this.ToString() != other.ToString())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if(ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
            {
                return true;
            }
            else if(ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                return false;
            }
            else
            {
                return f1.Equals(f2);
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {

            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }


        /// <summary>
        /// Takes a token and performs the appropriate arithmetic / operation.
        /// </summary>
        /// <param name="values">Stack of the current values.</param>
        /// <param name="operators">Stack of the current operators.</param>
        /// <param name="s">The current token being evaluated</param>
        /// <param name="variableEvaluator">The lookup method for string variables (if any)</param>
        private void ProcessToken(Stack<double> values, Stack<string> operators, string s, Func<string,double> variableEvaluator)
        {
            //Used if the token is a double.  
            double tempDouble;

            if (Double.TryParse(s, out tempDouble))
            {
                DoubleToken(values, operators, tempDouble);
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
            //Else pass in the value of the variable to the DoubleToken processor 
            else
            {
                //Try to lookup value of variable. If exception is thrown, the variable didn't have a value.
                try
                {
                    tempDouble = variableEvaluator(s);
                    DoubleToken(values, operators, tempDouble);
                }
                catch(Exception e)
                {
                    //Set flag to have evaluator return FormulaError struct for a valueless variable
                    valuelessVariable = true;
                }
            }
        }

        /// <summary>
        /// Determines whether the input value should be pushed to the stack, or if multiplication/division needs to be performed on the value first.
        /// </summary>
        /// <param name="values">Stack of current values</param>
        /// <param name="operators">Stack of current operators</param>
        /// <param name="value">The value to be operated on.</param>
        private void DoubleToken(Stack<double> values, Stack<string> operators, double value)
        {
            //Ensure that we can check the top of the stack for mult/divide operators, 
            //then check if we need to multiply/divide the value
            if (operators.Count >= 1 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
            {
                MultDivideToken(values, operators, value);
            }
            else
            {
                //If nothing to operate on, just push the double to the stack.
                values.Push(value);
            }

        }

        /// <summary>
        /// Performs addition or subtraction on the top two values in the input stack if the previous operator calls for it. Then pushes the current
        /// operator onto the stack.
        /// </summary>
        /// <param name="values">Stack of values.</param>
        /// <param name="operators">Stack of string arithmetic operators.</param>
        /// <param name="token">The token to be pushed onto the stack.</param>
        /// <param name="pushToken"> Whether or not the input token should be pushed onto the stack after arithmetic is done.
        /// (useful if the input token is a closing parenthesis, which shouldn't be pushed to the stack.)</param>
        private static void AddSubtractToken(Stack<double> values, Stack<string> operators, string token, bool pushToken)
        {
            double tempValueLefthand;
            double tempValueRighthand;

            //Check to see if there is any prior addition/subtraction to do
            if (operators.Count >= 1 && operators.Peek().Equals("+"))
            {
                tempValueRighthand = values.Pop();
                tempValueLefthand = values.Pop();
                // remove "+" from stack
                operators.Pop();

                values.Push(tempValueLefthand + tempValueRighthand);
            }
            else if (operators.Count >= 1 && operators.Peek().Equals("-"))
            {
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
        private void CloseParenthesis(Stack<double> values, Stack<string> operators)
        {
            //Check to see if there is any addition/subtraction to do in the parenthesis, but don't push
            //right parenthesis onto the operator stack
            AddSubtractToken(values, operators, " ", false);

            operators.Pop(); //Remove matching parenthesis from the stack

            //Check to see if there is any multiplication or division to do right before the opening parenthesis
            if (operators.Count >= 1 && (operators.Peek().Equals("*") || operators.Peek().Equals("/")))
            {
                double tempInt = values.Pop();
                MultDivideToken(values, operators, tempInt);
            }

        }

        /// <summary>
        /// Performs a multiplication or division, with the numerator being the topmost double on the stack, and the 
        /// denominator being the input double.
        /// </summary>
        /// <param name="values">Stack of current values</param>
        /// <param name="operators">Stack of current operators</param>
        /// <param name="value">The double (denominator for division)</param>
        private void MultDivideToken(Stack<double> values, Stack<string> operators, double value)
        {
            double poppedValue;

            //If topmost operator is *, perform multiplication
            if (operators.Peek().Equals("*"))
            {
                //Remove the '*' from the operators stack
                operators.Pop();
                poppedValue = values.Pop();
                values.Push(poppedValue * value);
            }
            //Else do division
            else if (operators.Peek().Equals("/"))
            {
                if (value == 0)
                {
                    //If divided by zero, set the appropriate flag so the evaluator can construct the FormulaError
                    this.dividedByZero = true;
                    return;
                }

                //Pop the '/' off the stack
                operators.Pop();
                poppedValue = values.Pop();
                values.Push(poppedValue / value);
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}