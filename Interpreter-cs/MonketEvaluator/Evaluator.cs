using FluentAssertions;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyObjects;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Interpreter_cs.MonketEvaluator {
    public class Evaluator {

        public struct References() {
            internal static NullObj NULL = new NullObj();
            internal static BooleanObj TRUE = new BooleanObj(true);
            internal static BooleanObj FALSE = new BooleanObj(false);
        }

        public ObjectInterface Eval(Node node) {
            switch (node) {
                case Prog p:
                    return evalStatements(p.statements);
                case IntegerLiteral integ:
                    return new IntegerObj(integ.value);
                case ExpressionStatement exp:
                    return Eval(exp.expression);
                case Bool boole:
                    return deciderOnBooleanObj(boole.value);
                case PrefixExpression exp:
                    var right = Eval(exp.right);
                    return evalPrefixExpression(exp.operators, right);
            }
            return References.NULL;
        }

        private ObjectInterface evalStatements(List<Statement> statement) {
            ObjectInterface result = null;

            foreach (Statement s in statement) {
                result = Eval(s);
            }
            return result;
        }

        private BooleanObj deciderOnBooleanObj(bool input) {
            if (input) {
                return References.TRUE;
            }
            return References.FALSE;
        }

        private ObjectInterface evalPrefixExpression(string operators, ObjectInterface right) {
            if (operators == "!") {
                return evalBangOperator(right);
            }
            else if (operators == "-") {
                return evalPrefixOperator(right);
            }
            else {
                return References.NULL;
            }
        }

        private ObjectInterface evalBangOperator(ObjectInterface right) {
            right.Should().NotBeNull();
            if (right.Equals(References.TRUE)) {
                return References.FALSE;
            }
            else if (right.Equals(References.FALSE)) {
                return References.TRUE;
            }
            else if (right.Equals(References.NULL)) {
                return References.TRUE;
            }
            else {
                return References.FALSE;
            }
        }

        private ObjectInterface evalPrefixOperator(ObjectInterface right) {
            if(right.GetType() != typeof(IntegerObj)) {
                return References.NULL;
            }
            var val = right as IntegerObj;
            return new IntegerObj(-val.value);
        }
    }
}
