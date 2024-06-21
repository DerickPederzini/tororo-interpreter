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

    public string toString() {

        string outString = "";

        for (int i = 0; i < statements.Count; i++){
            Console.Write(statements[i].token.ToString());
            outString += statements[i].toString();
        }
        
        return outString;
    }
}   

public interface Node {
    Token token { get; set; }
    string TokenLiteral();

    string toString();
}

public interface Expression : Node { }
public interface Statement : Node { }
internal class LetStatement(Token token) : Statement {

    internal Token token = token;
    internal Identifier name;
    internal Identifier value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    public string toString() {

        string outString = "";

        outString = token.Literal + " " + name.identValue + " = ";

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

    public string toString() {
        string outString = "";

        outString = token.Literal + " ";

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

    public string toString() {
        
        if(expression != null) {
            return expression.toString();
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

    string Node.toString() {
        return "Token: "+token+" Ident: "+identValue; 
    }

}
