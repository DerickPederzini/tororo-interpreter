﻿using FluentAssertions;
using Interpreter_cs.MonkeyEvaluator;
using Interpreter_cs.MonkeyLexer.Token;
using Interpreter_cs.MonkeyObjects;
using Interpreter_cs.MonkeyParser;
using System.Collections;

namespace Interpreter_cs.MonkeyREPL;

public class Repl {

    const string PROMPT = ">>";

    public void startReader() {

        try {

            MkEnvironment env = new MkEnvironment();

            while (true) {
                Console.Write(PROMPT);

                var line = Console.ReadLine();
                if (line == null)
                    break;

                Lexer lex = new Lexer(line);
                Parser p = new Parser(lex);
                MonkeyAST.Prog program = p.parseProgram(new MonkeyAST.Prog());

                if (p.errors.Count != 0) {
                    printParserError(p.errors);
                    continue;
                }

                Evaluator eval = new Evaluator();
                var evalueated = eval.Eval(program, env);
                evalueated.Should().NotBeNull();

                Console.WriteLine(evalueated.Inspect()+"\n");
            }
        }

        catch (IndexOutOfRangeException) {
            Console.WriteLine("Exiting Prompt...");
        }
    }
    public void printParserError(ArrayList errors) {
        Console.WriteLine("Found errors during parsing");
        foreach(var err in errors) {
            Console.WriteLine("\t"+err.ToString()+"\n");
        }
    }



}
