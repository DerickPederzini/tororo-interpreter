
using FluentAssertions;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;
using System.Collections;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.MonkeyParser;

public class Parser {
    Lexer lexer;
    Token currentToken;
    Token nextToken;
    internal ArrayList errors;
    Dictionary<TokenType, infixParse> infixParses = new Dictionary<TokenType, infixParse>();
    Dictionary<TokenType, prefixParse> prefixParses = new Dictionary<TokenType, prefixParse>();

    public Parser(Lexer lex) {
        lexer = lex;
        errors = new ArrayList();
        nextTk();
        nextTk();

        registerPrefix(TokenType.IDENT, parseIdentifier);
        registerPrefix(TokenType.INT, parseIntegerLiteral);
        registerPrefix(TokenType.NOT, parsePrefixExpression);
        registerPrefix(TokenType.MINUS, parsePrefixExpression);
        registerPrefix(TokenType.SEMICOLON, parseIdentifier);

        registerInfix(TokenType.EQUAL, parseInfixExpression);
        registerInfix(TokenType.NOTEQUAL, parseInfixExpression);
        registerInfix(TokenType.PLUS, parseInfixExpression);
        registerInfix(TokenType.MINUS, parseInfixExpression);
        registerInfix(TokenType.MULTIPLY, parseInfixExpression);
        registerInfix(TokenType.SLASH, parseInfixExpression);
        registerInfix(TokenType.LANGLE, parseInfixExpression);
        registerInfix(TokenType.RANGLE, parseInfixExpression);

        registerPrefix(TokenType.TRUE, parseBoolean);
        registerPrefix(TokenType.FALSE, parseBoolean);

        registerPrefix(TokenType.LPAREN, parseGroupedExpression);

        registerPrefix(TokenType.IF, parseIfExpression);
        registerPrefix(TokenType.ELSE, parseIfExpression);

        registerPrefix(TokenType.FUNCTION, parseFunctionExpression);
        registerInfix(TokenType.LPAREN, parseCallExpression);
    }
    //precedences
    enum Precedences {
        LOWEST = 1, //1
        EQUAL = 2, //2
        LESSGREATER = 3, //3
        SUM = 4, //4
        PRODUCT = 5, //5
        PREFIX = 6, //6
        CALL = 7, //function //7
    }

    Dictionary<TokenType, Precedences> precendence = new Dictionary<TokenType, Precedences>() 
    {
        { TokenType.EQUAL, Precedences.EQUAL },
        { TokenType.NOTEQUAL, Precedences.EQUAL },
        { TokenType.PLUS, Precedences.SUM },
        { TokenType.MINUS, Precedences.SUM },
        { TokenType.MULTIPLY, Precedences.PRODUCT },
        { TokenType.SLASH, Precedences.PRODUCT },
        { TokenType.LANGLE, Precedences.LESSGREATER },
        { TokenType.RANGLE, Precedences.LESSGREATER },
        { TokenType.LPAREN, Precedences.CALL }
    };

    public void nextTk() {
        currentToken = nextToken;
        nextToken = lexer.nextToken();
    }

    public int peekPrecendence() {
        if(precendence.TryGetValue(nextToken.Type, out Precedences prec)) {
            return (int)prec;
        }
        return (int)Precedences.LOWEST;
    }

    public int currentPrecedence() {
        if(precendence.TryGetValue(currentToken.Type, out Precedences prec)) {
            return (int)prec;
        }
        return (int)Precedences.LOWEST;
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

        nextTk();
        statement.value = parseExpression((int)Precedences.LOWEST);

        if (nextTokenIs(TokenType.SEMICOLON)) {
            nextTk();
        }

        return statement;
    }

    public Statement parseReturnStatement() {
        ReturnStatement returnStatement = new ReturnStatement(currentToken);
        nextTk();
        returnStatement.value = parseExpression((int)Precedences.LOWEST);
    
        if (!expectedPeek(TokenType.SEMICOLON)) {
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

    public void registerInfix(TokenType type, infixParse infix) {
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
        try {

            var prefix = prefixParses[currentToken.Type];

            if (prefix == null) {
                noPrefixParseFnError(currentToken.Type);
                return null;
            }
            var leftExpression = prefix();

            while (!nextTokenIs(TokenType.SEMICOLON) && peekPrecendence() > precedence) {
                var infix = infixParses[nextToken.Type];

                if (infix == null) {
                    return leftExpression;
                }
                nextTk();
                leftExpression = infix(leftExpression);
            }
            return leftExpression;
        }
        catch (KeyNotFoundException e) {
            Console.WriteLine($"The value {lexer.input[lexer.position - 1]} was not present in the list of possible characters for that expression");
            errors.Add(e);
            return null;
        }
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

    public void noPrefixParseFnError(TokenType token) {
        Assert.IsTrue(false, "No prefix parse function");
        errors.Add(token);
    }

    private Expression parsePrefixExpression() {
        var exp = new PrefixExpression(currentToken);
        exp.operators = currentToken.Literal;
        nextTk();   
        exp.right = parseExpression(precedence: (((int)Precedences.PREFIX)));

        return exp;
    }

    private Expression parseInfixExpression(Expression left) {
        var exp = new InfixExpression(currentToken) {
            operators = currentToken.Literal,
            leftValue = left
        };
        int prec = currentPrecedence();
        nextTk();   
        exp.rightValue = parseExpression(precedence: prec);

        return exp;
    }

    private Expression parseBoolean() {
        return new Bool(currentToken, currentTokenIs(TokenType.TRUE));
    }

    private Expression parseGroupedExpression() {
        nextTk();
        var exp = parseExpression((int)Precedences.LOWEST);

        if (!expectedPeek(Token.RPAREN.Type)) {
            return null;
        }

        return exp;
    }

    private Expression parseIfExpression() {
        var exp = new IfExpression(currentToken);

        if (!expectedPeek(TokenType.LPAREN)) {
            return null;
        }
        nextTk();
        exp.condition = parseExpression((int)Precedences.LOWEST);

        if (!expectedPeek(TokenType.RPAREN)) {
            return null;
        }

        if (!expectedPeek(TokenType.LBRACE)) {
            return null;
        }
        exp.consequence = parseBlockStatement();

        if (nextTokenIs(TokenType.ELSE)) {
            nextTk();

            if (!expectedPeek(TokenType.LBRACE)) {
                return null;
            }
            exp.alternative = parseBlockStatement();
        }

        return exp;
    }

    private Expression parseFunctionExpression() {
        var exp = new FunctionExpression(currentToken);

        if (!expectedPeek(TokenType.LPAREN)) {
            return null;
        }

        exp.parameters = parseFunctionParemeters(exp);

        if (!expectedPeek(TokenType.LBRACE)) {
            return null;
        }    
        exp.functionBody = parseBlockStatement();

        return exp;
    }

    private List<Identifier> parseFunctionParemeters(FunctionExpression exp) {
        if (nextTokenIs(TokenType.RPAREN)) {
            nextTk();
            return exp.parameters;
        }

        nextTk();

        if (currentTokenIs(TokenType.IDENT) && nextTokenIs(TokenType.RPAREN)) {
            exp.parameters.Add((Identifier)parseIdentifier());
            nextTk();
            return exp.parameters;
        }
        while (!currentTokenIs(TokenType.RPAREN)) {

            if (currentTokenIs(TokenType.IDENT)) {
                exp.parameters.Add((Identifier)parseIdentifier());
            }
            nextTk();
        }

        return exp.parameters;
    }

    private BlockStatement parseBlockStatement() {
        var blockStmt = new BlockStatement(currentToken);
        nextTk();

        while(!currentTokenIs(TokenType.RBRACE) && !currentTokenIs(TokenType.EOF)) {
            var statement = parseStatement();
            statement.Should().NotBe(null);
            blockStmt.statements.Add(statement);
            nextTk();
        }

        return blockStmt;
    }

    private Expression parseCallExpression(Expression fn) {
        var exp = new CallExpression(currentToken) { identifierExpression = fn };
        exp.arguments = parseArguments();
        return exp;
    }

    private List<Expression> parseArguments() {
        var args = new List<Expression>();

        if (nextTokenIs(TokenType.RPAREN)) {
            return args;
        }

        nextTk();
        args.Add(parseExpression((int)Precedences.LOWEST));

        while (nextTokenIs(TokenType.COMMA)) {
            nextTk();
            nextTk(); 
            args.Add(parseExpression((int)Precedences.LOWEST));
        }
        if (!expectedPeek(TokenType.RPAREN)) {
            return null;
        }
        return args;
    }  

    public bool expectedPeek(TokenType type) {
        if (nextTokenIs(type)) {
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
        return errors;
    }

    public bool nextTokenIs(TokenType type) {
        return nextToken.Type == type;
    }

    public bool currentTokenIs(TokenType type) {
        return currentToken.Type == type;
    }
}