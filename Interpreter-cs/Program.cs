// See https://aka.ms/new-console-template for more information

using Interpreter_cs.MonkeyREPL;

Repl repl = new Repl();
Console.WriteLine("This is the Monkey programming language!");

string test = "!5";

Console.WriteLine(test.Length);
Console.WriteLine("Feel free to try and type commands in!");

repl.startReader();
enum e {
    LOWEST = 1,
    HIGH,
}


