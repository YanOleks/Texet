using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Texet.Utils
{
    internal class Expression : ExpressionBaseVisitor<string>
    {

        private double ParseDouble(string expression)
        {
            return double.Parse(expression, System.Globalization.CultureInfo.InvariantCulture);
        }
        public override string VisitPower(ExpressionParser.PowerContext context)
        {
            double baseValue = ParseDouble(Visit(context.expression(0)));
            double exponent = ParseDouble(Visit(context.expression(1)));
            return Math.Pow(baseValue, exponent).ToString();
        }

        public override string VisitCompare([NotNull] ExpressionParser.CompareContext context)
        {
            double left = ParseDouble(Visit(context.expression(0)));
            double right = ParseDouble(Visit(context.expression(1)));
            var res = context.GetChild(1).GetText() switch
            {
                "<" => left < right,
                ">" => left > right,
                "=" => left == right,
                ">=" => left >= right,
                "<=" => left <= right,
                _ => throw new InvalidOperationException("Unsupported operator")
            };
            return res.ToString();
        }

        public override string VisitMultDiv(ExpressionParser.MultDivContext context)
        {
            double left = ParseDouble(Visit(context.expression(0)));
            double right = ParseDouble(Visit(context.expression(1)));
            var res = context.GetChild(1).GetText() switch
            {
                "*" => left * right,
                "/" => left / right,
                _ => throw new InvalidOperationException("Unsupported operator")
            };
            return res.ToString();
        }

        public override string VisitAddSub(ExpressionParser.AddSubContext context)
        {
            double left = ParseDouble(Visit(context.expression(0)));
            double right = ParseDouble(Visit(context.expression(1)));
            var res = context.GetChild(1).GetText() switch
            {
                "+" => left + right,
                "-" => left - right,
                _ => throw new InvalidOperationException("Unsupported operator")
            };
            return res.ToString();
        }

        public override string VisitModulus(ExpressionParser.ModulusContext context)
        {
            double left = ParseDouble(Visit(context.expression(0)));
            double right = ParseDouble(Visit(context.expression(1)));
            return (left % right).ToString();
        }

        public override string VisitNumber(ExpressionParser.NumberContext context)
        {
            return context.GetText();
        }

        public override string VisitParentheses(ExpressionParser.ParenthesesContext context)
        {
            return Visit(context.expression());
        }

        public override string VisitFunctionCall(ExpressionParser.FunctionCallContext context)
        {
            string funcName = context.func().GetText();
            List<double> args = [];

            foreach (var arg in context.args().expression())
            {
                args.Add(ParseDouble(Visit(arg)));
            }

            return ApplyFunction(funcName, args).ToString();
        }

        private double ApplyFunction(string funcName, List<double> args)
        {
            return funcName switch
            {
                "max" => args.Max(),
                "min" => args.Min(),
                "abs" => Math.Abs(args[0]),
                _ => throw new Exception($"Unsupported function: {funcName}")
            };
        }
    }
}
