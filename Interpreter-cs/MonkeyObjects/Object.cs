using Interpreter_cs.MonkeyAST;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Interpreter_cs.MonkeyObjects;

public readonly record struct Type(string type) {
    internal static string Integer_OBJ = "INTEGER";
    internal static string BOOL_OBJ = "BOOLEAN";
    internal static string NULL_OBJ = "NULL";
    internal static string RETURN_OBJ = "RETURN";
    internal static string ERROR_OBJ = "ERROR";
    internal static string FUNCTION_OBJ = "FUNCTION";
    internal static string STRING_OBJ = "STRING";
    internal static string BUILDIN_OBJ = "BUILDIN";
    internal static string ARRAY_OBJ = "ARRAY";
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
            param.Add(s.identValue.ToString());
        }
        return $"fn({string.Join(", ", param)})"+"{\n"+body.ToString()+"\n}";
    }

    public string ObjectType() {
        return Type.FUNCTION_OBJ;
    }
}

public class StringObj(string val) : ObjectInterface {
    internal string value = val;

    public string Inspect() {
        return value;
    }

    public string ObjectType() {
        return Type.STRING_OBJ;
    }
}

public delegate ObjectInterface BuildInFunction(params ObjectInterface[] obj);

public class Builds() {
    public static ObjectInterface len(ObjectInterface [] obj) {

        if(obj.Length != 1) {
            return new ErrorObj() { message = "wrong number of arguments. got=" + obj.Length +", want=1" };
        }

        switch (obj[0]) {
            case StringObj str:
                return new IntegerObj(str.value.Length);
            default:
                return new ErrorObj() { message = "argument to len not supported, got " + obj[0].ObjectType() };
        }
   }
}
public class BuildIn : ObjectInterface {

    internal BuildInFunction fn;

    public string Inspect() {
        return "buildin function";
    }

    public string ObjectType() {
        return Type.BUILDIN_OBJ;
    }
}

public class ArrayObj() : ObjectInterface {
    internal List<ObjectInterface> list = new List<ObjectInterface>();

    public string Inspect() {
        var listStr = new List<string>();
        foreach (var obj in list) {
            listStr.Add(obj.Inspect());
        }
        string val = $"[{string.Join(", ", listStr.ToArray())}]";
        return val;
    }

    public string ObjectType() {
        return Type.ARRAY_OBJ;
    }
}


