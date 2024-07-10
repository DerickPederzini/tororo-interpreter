using FluentAssertions;
using Interpreter_cs.MonkeyAST;
using Interpreter_cs.MonkeyObjects;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Type = Interpreter_cs.MonkeyObjects.Type;

namespace Interpreter_cs.MonkeyEvaluator {
    public class Evaluator {

        record struct References() {
            internal static readonly NullObj NULL = new NullObj();
            internal static readonly BooleanObj TRUE = new BooleanObj(true);
            internal static readonly BooleanObj FALSE = new BooleanObj(false);
        }

        public ObjectInterface Eval(Node node, MkEnvironment env) {
            switch (node) {
                case Prog p:
                    return evalProgram(p.statements, env);
                case IntegerLiteral integ:
                    return new IntegerObj(integ.value);
                case ExpressionStatement exp:
                    return Eval(exp.expression, env);
                case Bool boole:
                    return deciderOnBooleanObj(boole.value);
                case PrefixExpression exp:
                    var right = Eval(exp.right, env);
                    if (isError(right)) {
                        return right;
                    }
                    return evalPrefixExpression(exp.operators, right);
                case InfixExpression infix:
                    var l = Eval(infix.leftValue, env);
                    if (isError(l)) {
                        return l;
                    }
                    var r = Eval(infix.rightValue, env);
                    if (isError(r)) {
                        return r;
                    }
                    return evalInfixExpression(infix.operators, l, r);
                case BlockStatement block:
                    return evalBlockStatements(block, env);
                case IfExpression ifElse:
                    return evalIfElseExpression(ifElse, env);
                case ReturnStatement returnStatement:
                    var ret = Eval(returnStatement.value, env);
                    if (isError(ret)) {
                        return ret;
                    }
                    return new ReturnObj(ret);
                case LetStatement letstmt:
                    var val = Eval(letstmt.value, env);
                    if (isError(val)) {
                        return val;
                    }
                    return env.environment[letstmt.name.identValue] = val;
                case Identifier ident:
                    return evalIdentifier(ident, env);
            }
            return References.NULL;
        }

        private ObjectInterface evalIdentifier(Identifier ident, MkEnvironment env) {
            try {
                var val = env.environment[ident.identValue];
                val.Should().NotBeNull();
                return val;
            }
            catch (Exception e) {
                return ErrorFound("identifier not found: "+ident.identValue);
            }
        }

        private bool isError(ObjectInterface obj) {
            if(obj != null) {
                return obj.ObjectType() == Type.ERROR_OBJ;
            }
            return false;
        }

        private ObjectInterface evalBlockStatements(BlockStatement block, MkEnvironment env) {
            ObjectInterface result = null;
            
            foreach (Statement s in block.statements) {
                result = Eval(s, env);
                if (result.GetType() == typeof(ReturnObj)) {
                    return result;
                }
                else if (result.GetType() == typeof(ErrorObj)) {
                    return result;
                }
            }
            return result;

        }

        private ObjectInterface evalProgram(List<Statement> statement, MkEnvironment env) {
            ObjectInterface result = null;

            foreach (Statement s in statement) {
                result = Eval(s, env);
                if (result.GetType() == typeof(ReturnObj)) {
                    ReturnObj returnObj = result as ReturnObj;
                    return returnObj.value;
                } else if (result.GetType() == typeof(ErrorObj)) {
                    ErrorObj errorObj = result as ErrorObj;
                    return errorObj;
                }
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
                return ErrorFound($"unknown operator: {operators} {right.ObjectType()}");
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
                return ErrorFound("unknown operator: -"+right.ObjectType());
            }
            var val = right as IntegerObj;
            return new IntegerObj(-val.value);
        }

        private ObjectInterface evalInfixExpression(string operators, ObjectInterface l, ObjectInterface r) {

            if (l is IntegerObj && r is IntegerObj) {
                return evalInfixInteger(operators, (IntegerObj)l, (IntegerObj)r);
            }
            else if (l.GetType() != r.GetType()){
                return ErrorFound($"type mismatch: {l.ObjectType()} {operators} {r.ObjectType()}");
            }
            else if (operators == "==") {
                return deciderOnBooleanObj(l == r);
            }
            else if (operators == "!=") {
                return deciderOnBooleanObj(l != r);
            }
            else {
                return ErrorFound($"unknown operator: {l.ObjectType()} {operators} {r.ObjectType()}");
            }
        }

        private ObjectInterface evalInfixInteger(string op, IntegerObj l, IntegerObj r) {

            if (op == "+") {
                return new IntegerObj(l.value + r.value);
            }
            else if (op == "-") {
                return new IntegerObj(l.value - r.value);
            }
            else if (op == "*") {
                return new IntegerObj(l.value * r.value);
            }
            else if (op == ">") {
                return deciderOnBooleanObj(l.value > r.value);
            }
            else if (op == "<") {
                return deciderOnBooleanObj(l.value < r.value);
            }
            else if (op == "==") {
                return deciderOnBooleanObj(l.value == r.value);
            }
            else if (op == "!=") {
                return deciderOnBooleanObj(l.value != r.value);
            }
            else if (op == "/"){
                return new IntegerObj(l.value / r.value);
            }else {
                return ErrorFound($"unknown operator: {l.ObjectType()} {op} {r.ObjectType()}");
            }
        }

        private ObjectInterface evalIfElseExpression(IfExpression ifExp, MkEnvironment env) {

            var condition = Eval(ifExp.condition, env);
            if (isError(condition)) {
                return condition;
            }
            if (isTruthy(condition)) {
                return Eval(ifExp.consequence, env);
            }
            else if (ifExp.alternative != null) {
                return Eval(ifExp.alternative, env);
            }
            else {
                return References.NULL;
            }
        }

        private bool isTruthy(ObjectInterface condition) {

            if (condition.ObjectType().Equals(References.NULL)) {
                return false;
            }else if (condition.Equals(References.TRUE)) {
                return true;
            }else if (condition.Equals(References.FALSE)) {
                return false;
            }else {
                return true;
            }
        }

        private ErrorObj ErrorFound(string format) {
            Console.WriteLine(format);
            return new ErrorObj() { message = format }; 
        }
    }
}
