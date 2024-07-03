using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_cs.MonketEvaluator {
    public class Evaluator {
        public ObjectInterface Eval(Node node) {
            Console.WriteLine(node.GetType());
            switch (node) {
                case Prog p:
                    return evalStatements(p.statements);
                case IntegerLiteral integ:
                    return new IntegerObj(integ.value);
                case ExpressionStatement exp:
                    return Eval(exp.expression);
            }

            return null;
        }

        private ObjectInterface evalStatements(List<Statement> statement) {
            ObjectInterface result = null;

            foreach (Statement s in statement) {
                result = Eval(s);
            }
            return result;
        }
    }
}
