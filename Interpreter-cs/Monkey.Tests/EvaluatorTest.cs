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
        MkEnvironment env = new MkEnvironment();

        return ev.Eval(program, env);
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

    public static IEnumerable<object[]> errorTest() {
        yield return new object[] {"5 + true;",
            "type mismatch: INTEGER + BOOLEAN",
            };
        yield return new object[] {
            "5 + true; 5;",
            "type mismatch: INTEGER + BOOLEAN",
        };
        yield return new object[] {"-true",
            "unknown operator: -BOOLEAN",
            };
        yield return new object[] {
           "true + false;",
            "unknown operator: BOOLEAN + BOOLEAN",
            };
        yield return new object[] {
           "5; true + false; 5",
            "unknown operator: BOOLEAN + BOOLEAN",
            };
        yield return new object[] {
           "if (10 > 1) { true + false; }",
           "unknown operator: BOOLEAN + BOOLEAN",
            };
        yield return new object[] {"""
           if (10 > 1) {
            if (10 > 1) {
                return true + false;
            }
            return 1;
        }
        """,
        "unknown operator: BOOLEAN + BOOLEAN",
        };
        yield return new object[] { "foobar", "identifier not found: foobar" };
        yield return new object[] { " \"Hello\" - \"World\"", "unknown operator: STRING - STRING" };
    }

    [Theory]
    [MemberData(nameof(errorTest))]
    public void testErrorMessages(string input, string message) {
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        var errorObj = evaluated as ErrorObj;
        errorObj.Should().NotBeNull();
        errorObj.message.Should().Be(message);
    }

    public static IEnumerable<object[]> letTest() {
        yield return new object[] {"let x = 5; x;", 5 };
        yield return new object[] {"let x = 5 * 5; x;", 25 };
        yield return new object[] {"let a = 5; let b = a; b;", 5 };
        yield return new object[] {"let a = 5; let b = a; let c = a + b * 5; c;", 30 };
    }

    [Theory]
    [MemberData(nameof(letTest))]
    public void testLetEvaluation(string input, object expected) {
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        var integer = Convert.ToInt64(expected);
        testIntegerObject(evaluated, (long)integer);
    }

    [Fact]
    public void testFunctionEvaluation() {
        string input = "fn(x) {x + 2;}";
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        var fn = evaluated as FunctionLiteral;
        fn.parameters.Count.Should().Be(1);
        fn.parameters[0].identValue.Should().Be("x");
        string expectedBody = "(x + 2)";
        fn.body.ToString().Should().Be(expectedBody);
    }

    public static IEnumerable<object[]> testFnApp() {
        yield return new object[] { "let identity = fn(x) { x; }; identity(5);", 5 };
        yield return new object[] { "let identity = fn(x) { return x; }; identity(5);", 5 };
        yield return new object[] { "let double = fn(x) { x * 2; }; double(5);", 10 };
        yield return new object[] { "let add = fn(x, y) { x + y; }; add(5, 5);", 10 };
        yield return new object[] { "let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20 };
        yield return new object[] { "fn(x) { x; };(5);", 5 };
    }

    [Theory]
    [MemberData(nameof(testFnApp))]
    public void testFnApplication(string input, long expected) {
        var evaluated = testEval(input);
        testIntegerObject(evaluated, expected);
    }

    [Fact]
    public void testStringContatenation() {
        string input = "\"Hello\" + \" \" + \"World\"";
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        var str = evaluated as StringObj;
        str.Should().NotBeNull();
        str.value.Should().Be("Hello World");
    }

    public static IEnumerable<object[]> testBuildinLen() {
        yield return new object[] {"len(\"\")", 0};
        yield return new object[] { "len(\"four\")", 4};
        yield return new object[] { "len(\"hello world\")", 11};
        yield return new object[] { "len(1)", "argument to len not supported, got INTEGER"};
        yield return new object[] { "len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1"};
    }

    [Theory]
    [MemberData(nameof(testBuildinLen))]
    public void testBuildInLenApp(string input, object expected) {
        var evaluated = testEval(input);

        switch (expected) {
            case int integer:
                testIntegerObject(evaluated, integer);
                break;
            case string str:
                var errorObj = evaluated as ErrorObj;
                errorObj.Should().NotBeNull();
                Assert.IsInstanceOfType(errorObj, typeof(ErrorObj), "Object is not an error object");
                errorObj.message.Should().Be(str);
                break;
        }
    }

    [Fact]
    public void testArrayLiterals() {
        string input = "[1, 2 * 2, 3 + 3]";
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        var result = evaluated as ArrayObj;
        result.Should().NotBeNull();
        result.list.Count.Should().Be(3);

        testIntegerObject(result.list[0], 1);
        testIntegerObject(result.list[1], 4);
        testIntegerObject(result.list[2], 6);
    }

    public static IEnumerable<object[]> testIndex(){
        yield return new object[] {"[1, 2, 3] [0]", 1 }; 
        yield return new object[] {"[1, 2, 3] [1]", 2 }; 
        yield return new object[] {"[1, 2, 3] [2]", 3 }; 
        yield return new object[] {"let i = 0; [1][i]", 1 }; 
        yield return new object[] {"[1, 2, 3] [1 + 1]; }", 3 }; 
        yield return new object[] {"let myArray = [1, 2, 3]; myArray[2]", 3 }; 
        yield return new object[] {"let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2];", 6 }; 
        yield return new object[] {"let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]", 2 }; 
        yield return new object[] {"[1, 2, 3] [3]", null }; 
        yield return new object[] {"[1, 2, 3] [-1]", null }; 
    }

    [Theory]
    [MemberData(nameof(testIndex))]
    public void testArrayIndexExpression(string input, object expected) {
        var evaluated = testEval(input);
        evaluated.Should().NotBeNull();
        if (evaluated is NullObj) {
            testNullObject(evaluated);
        }
        else {
            var integer = Convert.ToInt64(expected);
            testIntegerObject(evaluated, integer);
        }
    }
}
