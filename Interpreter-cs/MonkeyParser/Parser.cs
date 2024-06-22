
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using static Interpreter_cs.MonkeyParser.Parser;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.MonkeyParser;

public class Parser {
    Lexer lexer;
    Token currentToken;
    Token nextToken;
    ArrayList errors;
    Dictionary<TokenType, infixParse> infixParses;
    Dictionary<TokenType, prefixParse> prefixParses = new Dictionary<TokenType, prefixParse>();

    public Parser(Lexer lex) {
        lexer = lex;
        errors = new ArrayList();
        nextTk();
        nextTk();
        
        registerPrefix(TokenType.IDENT, parseIdentifier);
        registerPrefix(TokenType.INT, parseIntegerLiteral);

    }

    //precedences

    enum Precedences { 
        
        LOWEST = 1, //1
        EQUAL, //2
        LESSGREATER, //3
        SUM, //4
        PRODUCT, //5
        PREFIX, //6
        CALL, //function //7

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

    private Expression parseIdentifier() {
        return new Identifier(currentToken, currentToken.Literal);
    }

    public Statement parseStatement() {

        switch (currentToken.Type) {
            case TokenType.LET:
                return parseLetStatement();
            case TokenType.RETURN:
                return parseReturnStatement();
            default:
                return parseExpressionStatement();
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


    public delegate Expression prefixParse();
   //delegate allows me to pass a reference to this function as a value to other methods;

    public delegate Expression infixParse(Expression expression);
  
    public void registerPrefix(TokenType type, prefixParse prefix) {
        prefixParses.Add(type, prefix);
    }

    public void regiterInfix(TokenType type, infixParse infix) {
        infixParses.Add(type, infix);
    }
    
    private ExpressionStatement parseExpressionStatement() {

        var statement = new ExpressionStatement(currentToken);

        statement.expression = parseExpression(((int)Precedences.LOWEST));

        if (nextTokenIs(TokenType.SEMICOLON)) {
            nextTk();
        }

        return statement;
    
    }

    private Expression parseExpression(int precedence) {

        var prefix = prefixParses[currentToken.Type];

        if (prefix == null) {
            return null;
        }

        //this line is so fucking cool holy shit
        var leftExpression = prefix();

        return leftExpression;
    }

    private Expression parseIntegerLiteral() {

        try {
            var literal = new IntegerLiteral(token: currentToken);
            long value = long.Parse(literal.token.Literal);
            literal.value = value;
            return literal;
        }
        catch (Exception e) {
            Assert.IsTrue(false, "Could not parse token value as a long ");
            errors.Add(e);
            return null;
        }
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