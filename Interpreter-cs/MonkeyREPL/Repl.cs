using Interpreter_cs.MonkeyLexer.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_cs.MonkeyREPL;

public class Repl {

    const string PROMPT = ">>";

    public void startReader() {
        try {

            while (true) {
                Console.Write(PROMPT);

                var line = Console.ReadLine();
                if (line == null)
                    break;

                var lex = new Lexer(line);

                for (Token tok = lex.nextToken(); tok != Token.EOF; tok = lex.nextToken()) {
                    Console.WriteLine($"{tok}");
                }

            }

        }
        catch (IndexOutOfRangeException e) {
            Console.WriteLine("Exiting Prompt...");
        }

    }



}
