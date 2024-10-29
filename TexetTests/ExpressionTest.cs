using Antlr4.Runtime;
using Texet.Utils;
namespace TexetTests
{
    public class ExpressionTest
    {
        [TestCase("3+3", ExpectedResult = "6")]
        [TestCase("max(10,-1)", ExpectedResult = "10")]
        [TestCase("2^3", ExpectedResult = "8")]
        [TestCase("3= 3", ExpectedResult = "True")]
        [TestCase("3^(min(10,2))", ExpectedResult = "9")]
        public string Test1(string input)
        {
            var lexer = new ExpressionLexer(new AntlrInputStream(input));
            var tokens = new CommonTokenStream(lexer);
            var parser = new ExpressionParser(tokens);

            var tree = parser.expression();
            var calculator = new Expression();
            var result = calculator.Visit(tree);

            return result;
        }
    }
}