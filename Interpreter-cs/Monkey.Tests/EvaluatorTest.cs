using FluentAssertions;
using Interpreter_cs.MonkeyEvaluator;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyObjects;
using Interpreter_cs.MonkeyParser;
using Microsoft.Testing.Platform.Extensions.Messages;
using Newtonsoft.Json;
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

    public static IEnumerable<object[]> ifElseTest() {
        yield return new object[] {"if (true) { 10 }", 10}; 
        yield return new object[] {"if (false) { 10 }", null};
        yield return new object[] {"if (1) {10}", 10};
        yield return new object[] {"if (1 > 2) { 10 }", null}; 
        yield return new object[] {"if (1 < 2) { 10 }", 10}; 
        yield return new object[] {"if (1 > 2) { 10 } else { 20 }", 20 }; 
        yield return new object[] {"if (1 < 2) { 10 } else { 20 }", 10 }; 
    }

    [Theory]
    [MemberData(nameof(ifElseTest))]
    public void testIfElseExpressions(string input, object value) {
        var evaluated = testEval(input);

        //TO-DO: MAKE THIS A LITTLE BIT BETTER BECAUSE THERE IS NO WAY
        var integer = Convert.ToInt32(value);
        if(integer != 0 && value != null) {
            testIntegerObject(evaluated, (long)integer);
        }else {
            testNullObject(evaluated).Should().Be(true);
        }
    }

    private bool testNullObject(object value) {
        if (value is not NullObj) {
            return false;
        }
        return true;
    }

    public static IEnumerable<object[]> returnTest() {
        yield return new object[] {"return 10;", 10}; 
        yield return new object[] {"return 10; 9;", 10};
        yield return new object[] {"return 2 * 5; 9;", 10};
        yield return new object[] {"9; return 2 * 5; 9;", 10};
        yield return new object[] {"if (5 < 10) { if (5 < 10) { return 10; } return 1; } ", 10}; 
    }

    [Theory]
    [MemberData(nameof(returnTest))]
    public void testReturnStatement(string input, object value) {
        var evaluated = testEval(input);

        //TO-DO: MAKE THIS A LITTLE BIT BETTER BECAUSE THERE IS NO WAY
        var integer = Convert.ToInt32(value);
        if(integer != 0 && value != null) {
            testIntegerObject(evaluated, integer);
        }else {
            testNullObject(evaluated).Should().Be(true);
        }
    }

}
