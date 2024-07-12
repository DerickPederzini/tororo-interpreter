
using System.Runtime.CompilerServices;

namespace Interpreter_cs.MonkeyLexer.Token;

public enum TokenType {

    //NOT ALLOWED
    ILLEGAL,
    EOF,

    //IDENTIFIERS + LITERAL TYPES
    INT,
    IDENT,

    //OPERATORS
    PLUS,
    MINUS,
    ASSIGN,
    MULTIPLY,
    NOT,
    LANGLE,
    RANGLE,
    EQUAL,
    NOTEQUAL,

    //SEPARATORS/DELIMITERS
    COMMA,
    SEMICOLON,

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,
    SLASH,

    //KEYWORDS
    FUNCTION,
    LET,
    IF,
    ELSE,
    RETURN,
    TRUE,
    FALSE,

    STRING,
}

public record struct Token(TokenType Type, string Literal) {

    internal static Token Illegal = new(TokenType.ILLEGAL, "ILLEGAL");
    internal static Token EOF = new(TokenType.EOF, "\0");
    internal static Token INT = new(TokenType.INT, "INT");
    internal static Token IDENT = new(TokenType.IDENT, "IDENT");
    internal static Token PLUS = new(TokenType.PLUS, "+");
    internal static Token MINUS = new(TokenType.MINUS, "-");
    internal static Token ASSIGN = new(TokenType.ASSIGN, "=");
    internal static Token COMMA = new(TokenType.COMMA, ",");
    internal static Token SEMICOLON = new(TokenType.SEMICOLON, ";");
    internal static Token LPAREN = new(TokenType.LPAREN, "(");
    internal static Token RPAREN = new(TokenType.RPAREN, ")");
    internal static Token LBRACE = new(TokenType.LBRACE, "{");
    internal static Token RBRACE = new(TokenType.RBRACE, "}");
    internal static Token FUNCTION = new(TokenType.FUNCTION, "fn");
    internal static Token LET = new(TokenType.LET, "let");
    internal static Token LANGLE = new(TokenType.LANGLE, "<");
    internal static Token RANGLE = new(TokenType.RANGLE, ">");
    internal static Token MULTIPLY = new(TokenType.MULTIPLY, "*");
    internal static Token NOT = new(TokenType.NOT, "!");
    internal static Token SLASH = new(TokenType.SLASH, "/");

    internal static Token IF = new(TokenType.IF, "if");
    internal static Token ELSE = new(TokenType.ELSE, "else");
    internal static Token RETURN = new(TokenType.RETURN, "return");
    internal static Token TRUE = new(TokenType.TRUE, "true");
    internal static Token FALSE = new(TokenType.FALSE, "false");

    internal static Token EQUAL = new(TokenType.EQUAL, "==");
    internal static Token NOTEQUAL = new(TokenType.NOTEQUAL, "!=");
    
    internal static Token STRING = new(TokenType.STRING, "STRING");

}





