using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Ciloci.Flee;

namespace FormulaScore
{
    public class FormulaScore
    {
        private const string valueIDRegexPattern = @"\[(.+?)\]";
        private Dictionary<string, long> formulaIDToIntegerValue = new Dictionary<string, long>();
        private Dictionary<string, double> formulaIDToFloatingPointValue = new Dictionary<string, double>();
        private IGenericExpression<double> compiledExpression;

        public string ScoringFormula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormulaScore()
        {
            ScoringFormula = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoringFormula"></param>
        public FormulaScore(string scoringFormula)
        {
            if (!string.IsNullOrWhiteSpace(scoringFormula))
            {
                ScoringFormula = scoringFormula;
            }
            else
            {
                ScoringFormula = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckFormula()
        {
            // TODO: refactor this. 
            try
            {
                compileExpression();
            }
            catch (ExpressionCompileException)
            {
                // Expression would not compile, so scoring formula is not valid.

                // Ensure no unwanted side-effects from compilation.
                compiledExpression = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the formula is a vaild arithmetic formula without checking 
        /// </summary>
        /// <returns></returns>
        public bool CheckFormulaSyntax()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double CalculateScore()
        {
            // Generate expression and calculate score!
            // Note: This library is amazing and does exactly what we needed.
            if (compiledExpression == null)
            {
                compileExpression();
            }

            double score = compiledExpression.Evaluate();
            return score;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="valueID"></param>
        /// <param name="valueValue"></param>
        public void AddScoringValue(string scoringID, long scoringValue)
        {
            formulaIDToIntegerValue.Add(scoringID, scoringValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="valueID"></param>
        /// <param name="valueValue"></param>
        public void AddScoringValue(string scoringID, double scoringValue)
        {
            formulaIDToFloatingPointValue.Add(scoringID, scoringValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> FetchScoringIDs(string formula)
        {
            return parseScoringIds(formula);
        }

        #region private helper functions

        private void compileExpression()
        {
            ExpressionContext context = new ExpressionContext();
            
            // Allows use of all public static Math functions:
            //context.Imports.AddType(typeof(Math));
            
            // Load values into Flee context.
            foreach (KeyValuePair<string, long> pair in formulaIDToIntegerValue)
            {
                context.Variables[pair.Key] = pair.Value;
            }
            foreach (KeyValuePair<string, double> pair in formulaIDToFloatingPointValue)
            {
                context.Variables[pair.Key] = pair.Value;
            }
            
            compiledExpression = context.CompileGeneric<double>(cleanFormula(ScoringFormula));
        }

        // Need to remove "[]" scoringId demarkation marks before formula can be used.
        private string cleanFormula(string formula)
        {
            // TODO: This all seems sort of hackey. Refactor.
            return formula.Replace("[", "").Replace("]", "");
        }

        // Get the list of scoring values that exist in the text string.
        private static List<string> parseScoringIds(string formula)
        {
            MatchCollection matches = Regex.Matches(formula, valueIDRegexPattern, RegexOptions.IgnoreCase);
            
            List<string> valueIDs = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                // Extract values from the first grouping.
                valueIDs.Add(matches[i].Groups[1].Value);
            }

            return valueIDs;
        }
    }
        #endregion
}
