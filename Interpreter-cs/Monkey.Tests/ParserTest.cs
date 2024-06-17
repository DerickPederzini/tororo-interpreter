using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Tracing;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyParser;
using System.Collections;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace Interpreter_cs.Monkey.Tests;

public class ParserTest {
    string input = """
        let x = 5;
        let y = 2;
        let z = 3;
        """;

    [Fact]
    public void testLetStatements() {
        Lexer lex = new Lexer(input);
        Parser parser = new Parser(lex);

        var p = parser.parseProgram(new Prog());

        if (p.statements == null) {
            throw new Exception("Parse program returned null");
        }
        if (p.statements.Count != 3) {
            throw new Exception("Program does not have 3 statements");
        }

        var tests = new string[]
        {
            "x",
            "y",
            "z",
        };

        for (int i = 0; i < tests.Length; i++) {
            Statement s = p.statements[i];
            testLetStatement(s, tests[i]).Should().BeTrue();
        }

    }

    public bool testLetStatement(Statement statementDone, string expected) {
        statementDone.token.Literal.Should().Be("let", "statement.TokenLiteral should be 'let' ");

        if (statementDone is not LetStatement letStatement) {
            Assert.IsTrue(false, $"Statement should be of type Letstatement, but it is {statementDone.GetType}");
            return false;
        }

        letStatement.name.identValue.Should().Be(expected);
        letStatement.name.token.Literal.Should().Be(expected);

        return true;
    }


}
