using FluentAssertions;
using Interpreter_cs.MonketEvaluator;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyObjects;
using Interpreter_cs.MonkeyParser;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.Monkey.Tests;
public class EvaluatorTest {

    public static IEnumerable<object[]> EvaluatorTestData() {

        yield return new object[] { "5", (long)5 };
        yield return new object[] { "10", (long)10};
        yield return new object[] { "-5", (long)-5};
        yield return new object[] { "-10", (long)-10};
        yield return new object[] { "5 + 5 + 5 + 5 - 10", (long)10};
        yield return new object[] { "5 * 2 + 10", (long)20};
        yield return new object[] { "5 + 2 * 10", (long)25};
        yield return new object[] { "5 * 2 - 10", (long)0};
        yield return new object[] { "50 / 2 * 2 + 10", (long)60};
        yield return new object[] { "2 * (5 + 10)", (long)30};
        yield return new object[] { "(5 + 10 * 2 + 15 / 3) * 2 + -10", (long)50};
    }

    [Theory]
    [MemberData(nameof(EvaluatorTestData))]
    public void testEvalLiteralExpression(string input, long expectedVal) {
        var evalueated = testEval(input);
        testIntegerObject(evalueated, expectedVal);
    }

    private ObjectInterface testEval(string input) {
        Lexer lex = new Lexer(input);
        Parser p = new Parser(lex);

        Prog program = p.parseProgram(new Prog());
        Evaluator ev = new Evaluator();

        return ev.Eval(program);
    }

    private bool testIntegerObject(ObjectInterface eval, long expectedVal) {
        var result = eval as IntegerObj;
        result.Should().NotBeNull();
        Assert.IsInstanceOfType(result, typeof(IntegerObj));
        result.value.Should().Be(expectedVal);
        return true;
    }

   
     public static IEnumerable<object[]> EvaluatorBooleanData() {

        yield return new object[] { "true", true };
        yield return new object[] { "false", false};
        yield return new object[] { "5 > 5", false};
        yield return new object[] { "10 == 10", true};
        yield return new object[] { "5 + 5 != 1 + 1", true};
        yield return new object[] { "5 * 5 == 10 * 2 + 5", true};
        yield return new object[] { "true == false", false};
        yield return new object[] { "true == true", true};
        yield return new object[] { "false == false", true};
        yield return new object[] { "5 > 1", true};
        yield return new object[] { "(5 > 1) == true", true};
        yield return new object[] { "(5 > 1) == false", false};
        yield return new object[] { "(5 < 1) == false", true};
        yield return new object[] { "(5 < 1) == true", false};
        yield return new object[] { "5 < 10", true};
    }

    [Theory]
    [MemberData(nameof(EvaluatorBooleanData))]
     public void testBooleanLiteralExpression(string input, bool expected){
            var evaluated = testEval(input);
            testBooleanObject(evaluated, expected);
    }

    private bool testBooleanObject(ObjectInterface eval, bool expected) {
        var result = eval as BooleanObj;
        result.Should().NotBeNull();
        Assert.IsInstanceOfType(result, typeof(BooleanObj));
        result.value.Should().Be(expected);
        return true;
    }

    public static IEnumerable<object[]> bangTest() {
        yield return new object[] { "!true", false};
        yield return new object[] { "!false", true};
        yield return new object[] { "!5", false};
        yield return new object[] { "!!5", true};
        yield return new object[] { "!!true", true};
        yield return new object[] { "!!false", false};
        yield return new object[] { "!!!true", false};
        yield return new object[] { "!!!false", true};
    }

    [Theory]
    [MemberData(nameof(bangTest))]
    public void testBangOperator(string input, bool val) {
        var evaluated = testEval(input);
        testBooleanObject(evaluated, val);
    }


}
