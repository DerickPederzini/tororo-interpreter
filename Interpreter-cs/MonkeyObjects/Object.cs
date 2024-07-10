﻿using Interpreter_cs.MonkeyAST;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Security.Cryptography.X509Certificates;

namespace Interpreter_cs.MonkeyObjects;

public readonly record struct Type(string type) {
    internal static string Integer_OBJ = "INTEGER";
    internal static string BOOL_OBJ = "BOOLEAN";
    internal static string NULL_OBJ = "NULL";
    internal static string RETURN_OBJ = "RETURN";
    internal static string ERROR_OBJ = "ERROR";
    internal static string FUNCTION_OBJ = "FUNCTION";
}
public interface ObjectInterface {
    string ObjectType();
    public string Inspect();
}

public class IntegerObj(long val) : ObjectInterface {
    internal long value = val;
    public string Inspect() {
        return value.ToString();
    }
    public string ObjectType() {
        return Type.Integer_OBJ;
    }
}

public class BooleanObj(bool val) : ObjectInterface {
    internal bool value = val;
    public string Inspect() {
        return value.ToString().ToLower();
    }
    public string ObjectType() {
        return Type.BOOL_OBJ;
    }
}

public class NullObj() : ObjectInterface {
    public string Inspect() {
        return "null";
    }
    public string ObjectType() {
        return Type.NULL_OBJ;
    }
}

public class ReturnObj(ObjectInterface val) : ObjectInterface {
    internal ObjectInterface value = val;
    public string Inspect() {
        return value.ToString().ToLower();
    }
    public string ObjectType() {
        return Type.RETURN_OBJ;
    }
}

public class ErrorObj() : ObjectInterface {
    internal string message;
    public string Inspect() {
        return "ERROR: "+ message;
    }
    public string ObjectType() {
        return Type.ERROR_OBJ;
    }
}

public class FunctionLiteral() : ObjectInterface {
    internal List<Identifier> parameters = new List<Identifier>();
    internal BlockStatement body;
    internal MkEnvironment env;
    public string Inspect() {
        List<string> param = new List<string>();

        foreach(var s in parameters) {
            param.Add(s.ToString());
        }
        return $"fn({string.Join(param.ToString(), ",")})"+"{\n"+body.ToString()+"\n}";
    }

    public string ObjectType() {
        return Type.FUNCTION_OBJ;
    }
}

