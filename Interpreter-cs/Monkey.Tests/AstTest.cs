using FluentAssertions;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyLexer.Token;

using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.Monkey.Tests {
    public class AstTest {

        [Fact]
        public void TestString() {

            Prog program = new Prog() {
                statements = new List<Statement> {
                    new LetStatement(token: new Token(Type: TokenType.LET, Literal: "let")) {
                        name = new Identifier(token: new Token(Type: TokenType.IDENT, Literal: "myVar"), ident: "myVar") ,
                        value = new Identifier(token: new Token(Type: TokenType.IDENT, Literal: "anotherVar"), ident:"anotherVar"),
                        
                    }
                }
            };
            
            if(program.toString() != "let myVar = anotherVar;") {
                Assert.IsTrue(false, "error, program.toString() is wrong! "+ program.toString());
            }

        }

    }
}
