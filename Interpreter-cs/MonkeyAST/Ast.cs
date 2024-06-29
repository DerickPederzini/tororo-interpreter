using Interpreter_cs.MonkeyLexer.Token;
using System.Data.Common;
using static Interpreter_cs.MonkeyAST.Expression;

namespace Interpreter_cs.MonkeyAST;

public class Prog {

    internal List<Statement> statements = new List<Statement>();
    public string TokenLiteral() {
        if (statements.Count > 0) {
            return statements[0].token.Literal;
        }
        else {
            return "";
        }
    }

    public override string ToString() {

        string outString = "";

        for (int i = 0; i < statements.Count; i++){
            Console.Write(statements[i].ToString());
            outString += statements[i].ToString();
        }
        
        return outString.ToString();
    }
}   

public interface Node {
    Token token { get; set; }
    string TokenLiteral();
    string ToString();
}

public interface Expression : Node { }
public interface Statement : Node { }
internal class LetStatement(Token token) : Statement {

    internal Token token = token;
    internal Identifier name;
    internal Identifier value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public override string ToString() {
        string outString = token.Literal + " " + name.identValue + " = ";

        if(value != null) {
            outString += (value.identValue);
        }

        outString += ";";

        return outString;
    }

    public string TokenLiteral() {
        return token.Literal;
    }
}

internal class ReturnStatement(Token token) : Statement {

    internal Token token = token;
    internal Expression value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public override string ToString() {
        string outString = token.Literal + " ";

        if(value != null) {
            outString += (value.ToString());
        }

        outString += ";";

        return outString;
    }

    public string TokenLiteral() {
        return token.Literal;
    }
}

internal class ExpressionStatement(Token token) : Expression, Statement {
    internal Token token = token;
    internal Expression expression;

    Token Node.token { get => token ; set => throw new NotImplementedException(); }

    public override string ToString() {
        
        if(expression != null) {
            return expression.ToString();
        }
        return "";

    }

    public string TokenLiteral() {
        return token.Literal;
    }
}

class Identifier(Token token, string ident) : Expression {

    internal Token token = token;
    internal string identValue = ident;

    Token Node.token { get => token ; set => throw new NotImplementedException(); }

    string Node.TokenLiteral() {
        return identValue;
    }

    string Node.ToString() {
        return $"{identValue}"; 
    }

}

class IntegerLiteral(Token token) : Expression{
    internal Token token = token;
    internal long value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public override string ToString() {
        return token.Literal;
    }
}

class PrefixExpression(Token token) : Expression {

    internal Token token = token;
    internal string operators;
    internal Expression right;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public override string ToString() {
        return $"({operators}{right.ToString()})";
    }
}

class InfixExpression(Token token) : Expression {

    internal Token token = token;
    internal Expression rightValue;
    internal string operators;
    internal Expression leftValue;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public override string ToString() {
        return $"({leftValue.ToString()} {operators} {rightValue.ToString()})";
    }
}

class Bool(Token token, bool value) : Expression {

    internal Token token = token;
    internal bool value = value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public override string ToString() {
        return value.ToString().ToLower();
    }
}

class IfExpression(Token tok) : Expression {

    internal Token token = tok;
    internal Expression condition;
    internal BlockStatement consequence;
    internal BlockStatement alternative;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public string toString() {
        var outStr =  $"if{condition.ToString()} {consequence.ToString()}";
        if (alternative != null) {
            outStr += $"else {alternative.ToString()}";
        }

        return outStr;
    }
}

class FunctionExpression(Token tok) : Expression {
    internal Token token = tok;
    internal List<Identifier> parameters = new List<Identifier>();
    internal BlockStatement functionBody;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }
    public override string ToString() {
        var param = new List<string>();

        foreach(var parameter in parameters) {
            param.Add(parameter.ToString());
        }
        return $"{TokenLiteral()} ({string.Join(param.ToString(), ",")}) {functionBody.ToString()}";
    }
}

public class BlockStatement(Token tok) : Statement {

    internal Token token = tok;
    internal List<Statement> statements = new List<Statement>(); 

    Token Node.token { get => tok; set => throw new NotImplementedException(); }

    public string TokenLiteral() {
        return token.Literal;
    }

    public override string ToString() {
        var outStr = "";
        foreach (var statement in statements) {
            outStr += statement.ToString();
        }
        return outStr;
    }
}
