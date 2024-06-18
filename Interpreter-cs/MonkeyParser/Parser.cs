
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using System.Collections;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.MonkeyParser;

public class Parser {
    Lexer lexer;
    Token currentToken;
    Token nextToken;
    ArrayList errors;

    public Parser(Lexer lex) {
        lexer = lex;
        errors = new ArrayList();
        nextTk();
        nextTk();
    }

    public void nextTk() {
        currentToken = nextToken;
        nextToken = lexer.nextToken();
    }

    public Prog parseProgram(Prog p) {

        while (!currentTokenIs(TokenType.EOF)) {

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
            case TokenType.RETURN:
                return parseReturnStatement();
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

        while (currentTokenIs(TokenType.SEMICOLON)) {
            nextTk();
        }
        return statement;

    }

    public Statement parseReturnStatement() {
    
        ReturnStatement returnStatement = new ReturnStatement(currentToken);

        while (!currentTokenIs(TokenType.SEMICOLON)) {
            nextTk();
        }
        return returnStatement;
    }

    public bool expectedPeek(TokenType type) {
        if (nextToken.Type == type) {
            nextTk();
            return true;
        }
        else {
            peekErrors(type);
            return false;
        }
    }

    public void peekErrors(TokenType type) {
        var msg = $"expected type to be {type}, but got {nextToken.Type}";
        errors.Add(msg);
    }

    public ArrayList Error() {
        return this.errors;
    }

    public bool nextTokenIs(TokenType type) {
        return nextToken.Type == type;
    }

    public bool currentTokenIs(TokenType type) {
        return currentToken.Type == type;
    }

}