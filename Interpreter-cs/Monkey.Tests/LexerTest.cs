using FluentAssertions;
using Interpreter_cs.MonkeyLexer.Token;

using Xunit;

namespace Interpreter_cs.Monkey.Tests;

public class LexerTest {
    [Fact]
    public void Test_NextToken() {
        const string testInput =
            """
            let five = 5;
            let ten = 10;
            let add = fn(x, y) {
                x + y;
            };
            let result = add(five, ten);
            !-/*5;
            5 < 10 > 5;
            if (5 < 10){
                return true;
            }else{
                return false;
            }
            10 == 10;
            9 != 10;    
            """;

        var lexer = new Lexer(testInput);

        var tokens = new List<Token>
        {
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "five"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.INT, "5"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "ten"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.INT, "10"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "add"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.FUNCTION, "fn"),
            new(TokenType.LPAREN, "("),
            new(TokenType.IDENT, "x"),
            new(TokenType.COMMA, ","),
            new(TokenType.IDENT, "y"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.IDENT, "x"),
            new(TokenType.PLUS, "+"),
            new(TokenType.IDENT, "y"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "result"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.IDENT, "add"),
            new(TokenType.LPAREN, "("),
            new(TokenType.IDENT, "five"),
            new(TokenType.COMMA, ","),
            new(TokenType.IDENT, "ten"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.NOT, "!"),
            new(TokenType.MINUS, "-"),
            new(TokenType.SLASH, "/"),
            new(TokenType.MULTIPLY, "*"),
            new(TokenType.INT, "5"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.INT, "5"),
            new(TokenType.LANGLE, "<"),
            new(TokenType.INT, "10"),
            new(TokenType.RANGLE, ">"),
            new(TokenType.INT, "5"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.IF, "if"),
            new(TokenType.LPAREN, "("),
            new(TokenType.INT, "5"),
            new(TokenType.LANGLE, "<"),
            new(TokenType.INT, "10"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.RETURN, "return"),
            new(TokenType.TRUE, "true"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
            new(TokenType.ELSE, "else"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.RETURN, "return"),
            new(TokenType.FALSE, "false"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
            new(TokenType.INT, "10"),
            new(TokenType.EQUAL, "=="),
            new(TokenType.INT, "10"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.INT, "9"),
            new(TokenType.NOTEQUAL, "!="),
            new(TokenType.INT, "10"),
            new(TokenType.SEMICOLON, ";"),


            new(TokenType.EOF, "\0"),
        };

        foreach (var token in tokens) {
            var nextToken = lexer.nextToken();
            token.Should().Be(nextToken);
        }
    }
}
