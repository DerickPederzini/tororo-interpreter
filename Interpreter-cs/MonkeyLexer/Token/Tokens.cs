
namespace Interpreter_cs.MonkeyLexer.Token;

public enum TokenType
{

    //NOT ALLOWED
    ILLEGAL,
    EOF,

    //IDENTIFIERS + LITERAL TYPES
    INT,
    IDENT,

    //OPERATORS
    PLUS,
    MINUS,
    EQUAL,

    //SEPARATORS/DELIMITERS
    COMMA,
    SEMICOLON,

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,

    //KEYWORDS
    FUNCTION,
    LET,

}

public readonly record struct Token(TokenType Type, string Literal)
{

    internal static Token Illegal = new(TokenType.ILLEGAL, "ILLEGAL");
    internal static Token EOF = new(TokenType.EOF, "\0");
    internal static Token INT = new(TokenType.INT, "INT");
    internal static Token IDENT = new(TokenType.IDENT, "IDENT");
    internal static Token PLUS = new(TokenType.PLUS, "+");
    internal static Token MINUS = new(TokenType.MINUS, "-");
    internal static Token EQUAL = new(TokenType.EQUAL, "=");
    internal static Token COMMA = new(TokenType.COMMA, ",");
    internal static Token SEMICOLON = new(TokenType.SEMICOLON, ";");
    internal static Token LPAREN = new(TokenType.LPAREN, "(");
    internal static Token RPAREN = new(TokenType.RPAREN, ")");
    internal static Token LBRACE = new(TokenType.LBRACE, "{");
    internal static Token RBRACE = new(TokenType.RBRACE, "}");
    internal static Token FUNCTION = new(TokenType.FUNCTION, "fn");
    internal static Token LET = new(TokenType.LET, "let");

}

  



