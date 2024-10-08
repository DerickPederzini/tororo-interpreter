﻿using Interpreter_cs.MonkeyAST;
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
            case ArrayObj array:
                return new IntegerObj(val: array.list.Count);
            case StringObj str:
                return new IntegerObj(str.value.Length);
            default:
                return new ErrorObj() { message = "argument to len not supported, got " + obj[0].ObjectType() };
        }
   }

    public static ObjectInterface first(params ObjectInterface[] obj) {
        if (obj.Length != 1) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to rest must be ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        if (arr.list.Count > 0) {
            return arr.list[0];
        }
        return null;
    }

        public static ObjectInterface last(params ObjectInterface[] obj) {
        if (obj.Length != 1) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to rest must be ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var len = arr.list.Count;
        if (len > 0) {
            return arr.list[len - 1];
        }
        return null;
    }


    public static ObjectInterface rest(params ObjectInterface[] obj) {
        if (obj.Length != 1) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to rest must be ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var len = arr.list.Count;
        if (len > 0) {
            var newElem = new List<ObjectInterface>();
            newElem = arr.list;
            return new ArrayObj() { list = newElem };
        }
        return null;
    }

    public static ObjectInterface push(params ObjectInterface[] obj) {
        if (obj.Length != 2) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to rest must be ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var len = arr.list.Count;
        var newElem = new List<ObjectInterface>();
        newElem = arr.list;
        newElem.Add(obj[1]);
        return new ArrayObj() { list = newElem };
    }

    public static ObjectInterface remove(params ObjectInterface[] obj) {
        if (obj.Length != 2) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1"};
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to remove must be an ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var newElem = new List<ObjectInterface>();
        newElem = arr.list;
        bool found = false;
        var value = obj[1] as IntegerObj;
        foreach (IntegerObj i in newElem) {
            if(i.value == value.value) {
                newElem.Remove(i);
                found = true;
                break;
            }
        }
        if (!found) {
            return new ErrorObj() { message = $"could not find the element to remove in the array" };
        }
        return new ArrayObj() { list = newElem };
    }

    public static ObjectInterface removeAt(params ObjectInterface[] obj) {
        if (obj.Length != 2) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        if (obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to remove must be an ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var newElem = new List<ObjectInterface>();
        newElem = arr.list;
        var value = obj[1] as IntegerObj;

        if (value.value > newElem.Count) {
            return new ErrorObj() { message = $"Index is out of bound for the array" };
        }

        for (int i = 0; i <= value.value; i++) {
            if(i == value.value){
                newElem.RemoveAt(i);
            }
        }

        return new ArrayObj() { list = newElem };
    }

    public static ObjectInterface sOrT(params ObjectInterface[] obj) {
        if (obj.Length != 1) {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }
        
        if(obj[0].ObjectType() != "ARRAY") {
            return new ErrorObj() { message = $"argument to sort must be an ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var newElem = new List<ObjectInterface>();
        newElem = arr.list;
        newElem = Sorts.bogoSort(newElem);

        return new ArrayObj() { list = newElem };
    }
    public static ObjectInterface sort
        (params ObjectInterface[] obj)
    {
        if (obj.Length != 1)
        {
            return new ErrorObj() { message = $"wrong number of arguments. got={obj.Length}, want=1" };
        }

        if (obj[0].ObjectType() != "ARRAY")
        {
            return new ErrorObj() { message = $"argument to sort must be an ARRAY, got {obj[0].ObjectType()}" };
        }
        var arr = obj[0] as ArrayObj;
        var newElem = new List<ObjectInterface>();
        newElem = arr.list;
        newElem = Sorts.sort(newElem);

        return new ArrayObj() { list = newElem };
    }
}

public interface Sorts{


    public static List<ObjectInterface> sort(List<ObjectInterface> list)
    {
        //implement normal sort later
        return null;
    }


    public static List<ObjectInterface> bogoSort(List<ObjectInterface> list)
    {
        double attempts = 0;
        //implement this as a meme :)
        while(!isSorted(list))
        {
            attempts++;
            shuffle(list);
        }
        double fact = 1;
        for(int i = list.Count; i >= 1; i--) {
            fact *= i;
        }
        Console.WriteLine($"Congrats, you did it exaclty {attempts} attenpts");
        Console.WriteLine($"With a probability of {(double)(attempts/(double)list.Count * fact)}");
        return list;
    }
    public static bool isSorted(List<ObjectInterface> list)
    {
        int n = list.Count - 1;
        for (int i = 0; i < n; i++)
        {
        List<IntegerObj> li = new List<IntegerObj>();
            foreach(IntegerObj j in list)
            {
                li.Add((IntegerObj)j);
                Console.Write(j.value+",");
            }
            Console.WriteLine(" ");
            if (li[i].value > li[i+1].value)
            {
                return false;
            }
        }
        return true;
    }


    public static void shuffle(List<ObjectInterface> list)
    {
        ObjectInterface i;
        int t;
        ObjectInterface r;
        Random rand = new Random();

        for (int j = 0; j < list.Count; j++)
        {
            i = list.ElementAt(j);
            t = rand.Next(0, list.Count);
            list[j] = list[t];
            list[t] = i;
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


