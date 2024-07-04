namespace Interpreter_cs.MonkeyObjects;

public readonly record struct Type(string type) {
    internal static Type Integer_OBJ = new("Integer");
    internal static Type BOOL_OBJ = new("Boolean");
    internal static Type NULL_OBJ = new("null");
}
public interface ObjectInterface {
    Type ObjectType();
    public string Inspect();
}

public class IntegerObj(long val) : ObjectInterface {
    internal long value = val;
    public string Inspect() {
        return value.ToString();
    }
    public Type ObjectType() {
        return Type.Integer_OBJ;
    }
}

public class BooleanObj(bool val) : ObjectInterface {
    internal bool value = val;
    public string Inspect() {
        return "";
    }
    public Type ObjectType() {
        return Type.BOOL_OBJ;
    }
}

public class NullObj() : ObjectInterface {
    public string Inspect() {
        return "";
    }
    public Type ObjectType() {
        return Type.NULL_OBJ;
    }
}
