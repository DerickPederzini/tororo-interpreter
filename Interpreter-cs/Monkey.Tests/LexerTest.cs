using FluentAssertions;
using Interpreter_cs.MonkeyLexer.Token;

using Xunit;

namespace Interpreter_cs.Monkey.Tests
{
    public class LexerTest
    {
        [Fact]

        public void Test_NextToken()
        {
            const string testInput =
                """
                let five = 5;
                let ten = 10;
                let add = fn(x, y) {
                    x + y;
                };
                let result = add(five, ten);
                """;

            var lexer = new Lexer(testInput);

            var tokens = new List<Token>
            {
                new(TokenType.LET, "let"),
                new(TokenType.IDENT, "five"),
                new(TokenType.EQUAL, "="),
                new(TokenType.INT, "5"),
                new(TokenType.SEMICOLON, ";"),
                new(TokenType.LET, "let"),
                new(TokenType.IDENT, "ten"),
                new(TokenType.EQUAL, "="),
                new(TokenType.INT, "10"),
                new(TokenType.SEMICOLON, ";"),
                new(TokenType.LET, "let"),
                new(TokenType.IDENT, "add"),
                new(TokenType.EQUAL, "="),
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
                new(TokenType.EQUAL, "="),
                new(TokenType.IDENT, "add"),
                new(TokenType.LPAREN, "("),
                new(TokenType.IDENT, "five"),
                new(TokenType.COMMA, ","),
                new(TokenType.IDENT, "ten"),
                new(TokenType.RPAREN, ")"),
                new(TokenType.SEMICOLON, ";"),


                new(TokenType.EOF, "\0"),
            };

            foreach (var token in tokens)
            {
                var nextToken = lexer.nextToken();
                token.Should().Be(nextToken);
            }
        }
    }
}
