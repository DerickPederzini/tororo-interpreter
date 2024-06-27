using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Tracing;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyParser;
using System.CodeDom.Compiler;
using System.Collections;
using System.Net.WebSockets;
using System.Security.Principal;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace Interpreter_cs.Monkey.Tests;

public class ParserTest {

    [Fact]
    public void testLetStatements() {

    string input = """
        let x = 5;
        let y = 2;
        let z = 2;
        """;

        Lexer lex = new Lexer(input);
        Parser parser = new Parser(lex);

        var p = parser.parseProgram(new Prog());
        checkParserErrors(parser);

        if (p.statements == null) {
            throw new Exception("Parse program returned null");
        }
        if (p.statements.Count != 6) {
            throw new Exception("Program does not have 3 statements");
        }

        var tests = new string[]
        {
            "x",
            "y",
            "z",
        };
        var j = 0;

        for (int i = 0; i < tests.Length; i++) {
            Statement s = p.statements[i];
            if(s.TokenLiteral() == "let") {
                testLetStatement(s, tests[j]).Should().BeTrue();
                j++;
            }
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
    [Fact]
    public void TestBoolenExpression() {
        string input = "true";

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

        var ident = statement.expression as Bool;

        ident.Should().NotBeNull("Ident is null");

        if(ident is not Expression) {
            Assert.IsTrue(false, $"ident is not a statement, got {ident.GetType}");
        }

        ident.value.Should().Be(true);

        ident.token.Literal.Should().Be("true");

    }  


    [Fact]
    public void testIntegerLiteralExpression() {
        string input = "5";

        Lexer lex = new Lexer(input);
        Parser p = new Parser(lex);
        Prog program = p.parseProgram(new Prog());

        checkParserErrors(p);

        if (program.statements.Count != 1) {
            Assert.IsTrue(false, $"Program does have correct amount of expression statement, got {program.statements.Count}");
        }

        var expression = program.statements[0] as ExpressionStatement;

        expression.Should().NotBeNull("Expression is null");

        if (expression is not ExpressionStatement) {
            Assert.IsTrue(false, "Expression is not a expression statement");
        }

        var literal = expression.expression as IntegerLiteral;

        literal.Should().NotBeNull("Literal is null");

        if(literal.value != 5) {
            Assert.IsTrue(false, $"Literal value does not equal 5, got {literal.value}");
        }

        if(literal.TokenLiteral() != "5") {
            Assert.IsTrue(false, $"Literal token literal value does not equal '5', got {literal.TokenLiteral()}");
        }
    }

    struct testPrefix(string input, string operation, long value) 
    {
        internal string input = input;
        internal string operation = operation;
        internal long value = value;
    };

    [Fact]
    public void testParsingPrefixExpressions() {
        testPrefix[] inputs = {
            new testPrefix("!5", "!", 5),
            new testPrefix("-15", "-", 15)
        };

        foreach (testPrefix test in inputs) {

        Lexer lex = new Lexer(test.input);
        Parser p = new Parser(lex);
        Prog program = p.parseProgram(new Prog());

        checkParserErrors(p);

        if (program.statements.Count != 1) {
            Assert.IsTrue(false, $"Program does not have one statement, instead got {program.statements.Count}");
        }

        var statement = program.statements[0] as ExpressionStatement;

        if (statement is not ExpressionStatement) {
            Assert.IsTrue(false, "statement is not a expression statement, it is a "+statement.GetType());
        }

        var expression = statement.expression as PrefixExpression;
        
        if(expression is not PrefixExpression) {
            Assert.IsTrue(false, $"expression is not a Expression Statement, it is a {expression.GetType()}");
        }

        if (expression.operators != test.operation) {
            Assert.IsTrue(false, "Expression operator is not expected");
        }

        if (!testIntegerLiteral(expression.right, test.value)) {
            return;
        }
        }
    }

    struct testInfix(string input, long lValue, string operators, long rValue) {
        internal string input = input;
        internal long leftValue = lValue;
        internal string operators = operators;
        internal long rightValue = rValue;
    }

    [Fact]
    public void testParsingInfixExpressions() {
        testInfix[] infixTest = {
            new testInfix("5 + 5;", 5, "+", 5),
            new testInfix("5 - 5;", 5, "-", 5),
            new testInfix("5 * 5;", 5, "*", 5),
            new testInfix("5 / 5;", 5, "/", 5),
            new testInfix("5 > 5;", 5, ">", 5),
            new testInfix("5 < 5;", 5, "<", 5),
            new testInfix("5 == 5;", 5, "==", 5),
            new testInfix("5 != 5;", 5, "!=", 5),
        };

        foreach (var test in infixTest) {

            Lexer lex = new Lexer(test.input);
            Parser p = new Parser(lex);
            Prog program = p.parseProgram(new Prog());

            checkParserErrors(p);

            program.Should().NotBeNull();

            if(program.statements.Count != 1) {
                Assert.IsTrue(false, $"Program statement count is not equal to 3, it is equal to {program.statements.Count}");
            }

            var statements = program.statements[0] as ExpressionStatement;

            statements.Should().NotBeNull();

            if (statements is not ExpressionStatement) {
                Assert.IsTrue(false, "Statements types was expected to be an Expression statement, but instead got "+statements.GetType());
            }

            var expression = statements.expression as InfixExpression;

            expression.Should().NotBeNull();

            if (!testIntegerLiteral(expression.leftValue, test.leftValue)) {
                Assert.IsTrue(false, $"Test Literal did not pass the test, expected {test.leftValue} and got {expression.leftValue}");
            }

            if (expression.operators != test.operators) {
                Assert.IsTrue(false, $"Expression operators is not {test.operators}, it is {expression.operators} ");
            }

            if (!testIntegerLiteral(expression.rightValue, test.rightValue)) {
                Assert.IsTrue(false, $"Test Literal did not pass the test, expected {test.rightValue} and got {expression.rightValue}");
            }
        }
    }

    struct testOperatorPrecedense(string input, string expected) {

        internal string input = input;
        internal string expected = expected;

    }

    [Fact]
    public void testOperatorPrecedenseParsing() {

        testOperatorPrecedense[] tests = {
            new testOperatorPrecedense("-a * b", "((-a) * b)"),
            new testOperatorPrecedense("!-a", "(!(-a))"),
            new testOperatorPrecedense("a + b + c", "((a + b) + c)"),
            new testOperatorPrecedense("3 + 4 * 5 == 3 * 1 + 4 * 5","((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new testOperatorPrecedense("false", "false"),
            new testOperatorPrecedense("3 > 5 == false", "((3 > 5) == false)"),
            new testOperatorPrecedense("3 < 5 == true", "((3 < 5) == true)")

        };

        foreach (var test in tests) {

            Lexer lex = new Lexer(test.input);
            Parser p = new Parser(lex);
            Prog program = p.parseProgram(new Prog());

            checkParserErrors(p);

            var actual = program.toString();

            actual.Should().NotBe(null);

            //This pass does not test, i have to make some corrections in the toString() overrides!
            if(actual != test.expected){
                Assert.IsTrue(false, "Actual is not expected, expected "+test.expected+"  got "+actual);
            }
        }
    }
    public bool testIntegerLiteral(Expression exp, long value) {
        var integ = exp as IntegerLiteral;

        if (integ.value != value) {
            Assert.IsTrue(false, $"Token Literal does not correspond {value}, instead got {integ.value}");
            return false;
        }

        if (integ.token.Literal != value.ToString()) {
            Assert.IsTrue(false, $"Token Literal does not correspond {value.ToString()}, instead got {integ.token.Literal}");
            return false;
        }
        return true;
    }
    public bool testIdentifier(Expression exp, string value) {

        var ident = exp as Identifier;

        ident.Should().NotBe(null, "Assertion should be an Identifier, but it is a "+ident.GetType());

        ident.identValue.Should().Be(value, "Ident value should be "+value);

        ident.token.Literal.Should().Be(value, "Ident Token Literal value should be "+value);
            
        return true;

    }

    public bool testBooleanExpression(Expression exp, bool value) {

        var ident = exp as Bool;

        ident.Should().NotBe(null);

        ident.value.Should().Be(value);

        ident.TokenLiteral().Should().Be(value.ToString());

        return true;
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
