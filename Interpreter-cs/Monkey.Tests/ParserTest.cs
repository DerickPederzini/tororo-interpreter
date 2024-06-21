using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Tracing;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyParser;
using System.CodeDom.Compiler;
using System.Collections;
using System.Security.Principal;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace Interpreter_cs.Monkey.Tests;

public class ParserTest {
    string input = """
        let x = 5;
        let y = 2;
        let z = 2;
        """;

    [Fact]
    public void testLetStatements() {
        Lexer lex = new Lexer(input);
        Parser parser = new Parser(lex);

        var p = parser.parseProgram(new Prog());
        checkParserErrors(parser);

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

    [Fact]
    public void testReturnStatements() {

        string input = @"
                return 5;
                return 10;
                return 993322;
            ";

        Lexer lex = new Lexer(input);
        Parser p = new Parser(lex);

        var program = p.parseProgram(new Prog());

        checkParserErrors(p);

        if (program.statements.Count != 3) {
            throw new Exception();
        }

        for(int i = 0; i < program.statements.Count; i++) {

            if(program.statements[i] is not ReturnStatement returns) {
                Assert.IsTrue(false, "not return statement");
            }
            var returnStatement = program.statements[i] as ReturnStatement;
            returnStatement.token.Literal.Should().Be("return");
                
        }


    }

    [Fact]
    public void TestIdentifierExpression() {
        string input = "foobar ";

        Lexer lex = new Lexer(input);
        Parser p = new Parser(lex);
        Prog program = p.parseProgram(new Prog());

        checkParserErrors(p); 

        if (program.statements.Count != 1) {
            Assert.IsTrue(false, $"Program does not have enough statements, found {program.statements.Count}");
        }

        var statement = program.statements[0] as ExpressionStatement;

        if (statement is not ExpressionStatement express) {
            Assert.IsTrue(false, $"program.statements[0] is not a expressionStatement, got {program.statements.GetType}");
        }

        var ident = statement.expression as Identifier;

        ident.Should().NotBeNull("Ident is null");

        if(ident is not Expression) {
            Assert.IsTrue(false, $"ident is not a statement, got {ident.GetType}");
        }

        ident.identValue.Should().Be("foobar");

        ident.token.Literal.Should().Be("foobar");

    }


    public void checkParserErrors(Parser parser) {
        var errors = parser.Error();

        if(errors.Count == 0) {
            return;
        }
        int i = 0;
        foreach (var error in errors) {
            Assert.IsTrue(false, "parser error"+error);
            i++;
        }

    }
}
