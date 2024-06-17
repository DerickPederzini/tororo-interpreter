
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;

namespace Interpreter_cs.MonkeyParser;

public class Parser {
    Lexer lexer;
    Token currentToken;
    Token nextToken;

    public Parser(Lexer lex) {
        lexer = lex;
        nextTk();
        nextTk();
    }

    public void nextTk() {
        currentToken = nextToken;
        nextToken = lexer.nextToken();
    }

    public Prog parseProgram(Prog p) {

        while (currentToken != Token.EOF) {

            var statement = parseStatement();
            if (statement != null) {
                p.statements.Add(statement);
            }
            nextTk();
        }

        return p;

    }

    public Statement parseStatement() {

        switch (currentToken.Type) {
            case TokenType.LET:
                return parseLetStatement();
            default:
                return null;
        }

    }

    public Statement parseLetStatement() {
        LetStatement statement = new LetStatement(currentToken);

        if (!expectedPeek(TokenType.IDENT)) {
            return null;
        }

        statement.name = new Identifier(currentToken, currentToken.Literal);

        if (!expectedPeek(TokenType.ASSIGN)) {
            return null;
        }

        while (currentToken != Token.SEMICOLON) {
            nextTk();
        }
        return statement;

    }

    public bool expectedPeek(TokenType type) {
        if (nextToken.Type == type) {
            nextTk();
            return true;
        }
        else {
            return false;
        }
    }

}
