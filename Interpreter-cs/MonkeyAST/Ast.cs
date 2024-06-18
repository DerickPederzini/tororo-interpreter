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
}

public interface Node {
    Token token { get; set; }
}

public interface Expression : Node { }
public interface Statement : Node { }
internal class LetStatement(Token token) : Statement {

    internal Token token = token;
    internal Identifier name;
    internal Expression value;

    Token Node.token { get => token; set => throw new NotImplementedException(); }

    string TokenLiteral() {
        return token.Literal;
    }
}

internal class ReturnStatement(Token token) : Statement {

    internal Token token = token;
    internal Expression value;

    Token Node.token { get => token; set => throw new NotImplementedException(); } 

    string TokenLiteral() {
        return token.Literal;
    }
}

class Identifier(Token token, string ident) {

    internal Token token = token;
    internal string identValue = ident;

    string TokenLiteral() {
        return token.Literal;
    }

}
