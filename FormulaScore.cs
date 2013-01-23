using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ciloci.Flee;

namespace FormulaScore
{
    public class FormulaScore
    {
        private Dictionary<string, long> formulaIDToIntegerValue = new Dictionary<string, long>();
        private Dictionary<string, double> formulaIDToFloatingPointValue = new Dictionary<string, double>();
        IGenericExpression<double> compiledExpression;

        public string ScoringFormula { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoringFormula"></param>
        public FormulaScore(string scoringFormula)
        {
            ScoringFormula = scoringFormula;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckScoringFormula()
        {
            try
            {
                compileExpression();
            }
            catch (ExpressionCompileException ex)
            {
                // Expression would not compile, so scoring formula is not valid.

                // Ensure no unwanted side-effects from compilation.
                compiledExpression = null;
                return false;
            }

            return true;
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
        public void AddScoringValue(string valueName, string valueID, long valueValue)
        {
            //formulaIDToValueName.Add(valueID, valueName);
            formulaIDToIntegerValue.Add(valueID, valueValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="valueID"></param>
        /// <param name="valueValue"></param>
        public void AddScoringValue(string valueName, string valueID, double valueValue)
        {
            //formulaIDToValueName.Add(valueID, valueName);
            formulaIDToFloatingPointValue.Add(valueID, valueValue);
        }

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
            
            compiledExpression = context.CompileGeneric<double>(ScoringFormula);
        }
    }
}
