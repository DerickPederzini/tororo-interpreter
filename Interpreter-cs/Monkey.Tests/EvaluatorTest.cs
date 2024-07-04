using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Tracing;
using Interpreter_cs.MonketEvaluator;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyObjects;
using Interpreter_cs.MonkeyParser;
using System.CodeDom.Compiler;
using System.Collections;
using System.Data;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;



namespace Interpreter_cs.Monkey.Tests;
public class EvaluatorTest {

    public static IEnumerable<object[]> EvaluatorTestData() {

        yield return new object[] { "5", (long)5 };
        yield return new object[] { "10", (long)10};
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

}
