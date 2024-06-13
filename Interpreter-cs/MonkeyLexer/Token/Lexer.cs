using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_cs.MonkeyLexer.Token;

public class Lexer(string input)
{
    string input = input;//as a whole
    int position;// points to current character
    int readPosition; //points to after the current character
    char currentCharacter = input[0];

    //creating the lexer
    public Lexer makeLexer(string input)
    {
        var lex = new Lexer(input);
        lex.readCharacter();
        return lex;
    }

    public void readCharacter()
    {
        if (readPosition >= input.Length)
        {
            currentCharacter = '\0';//ASCII code for Null
        }
        else
        {
            currentCharacter = input.ElementAt(readPosition);

            position++;
            readPosition = position + 1;

        }

    }

    private string readIdentifier()
    {
        int currentPosition = position;

        while (Char.IsLetter(this.currentCharacter))
        {
            readCharacter();
        }
        return input[currentPosition..position];

    }

    public Token nextToken()
    {
        Console.WriteLine("Entering");
        SkipWhiteSpaces();
        Console.WriteLine("Exiting");

        Token tokens;
    
        if (currentCharacter == '=')
           tokens = Token.EQUAL;

        else if (currentCharacter == '+')
           tokens = Token.PLUS;

        else if (currentCharacter == '-')
           tokens = Token.MINUS;

        else if (currentCharacter == ',')
           tokens = Token.COMMA;
        
        else if (currentCharacter == '}')
           tokens = Token.RBRACE;

        else if (currentCharacter == '{')
           tokens = Token.LBRACE;

        else if (currentCharacter == ')')
           tokens = Token.RPAREN;

        else if (currentCharacter == '(')
           tokens = Token.LPAREN;

        else if (currentCharacter == ';')
           tokens = Token.SEMICOLON;

        else if (Char.IsLetter(currentCharacter))
        {
                //this is a very cool way of doing switches
           return readIdentifier() switch
           {
               "fn" => Token.FUNCTION,
               "let" => Token.LET,
               var ident => new(TokenType.IDENT, ident.ToString()),
           };
        }
        else if (Char.IsDigit(currentCharacter))
        {  
           tokens = new Token(TokenType.INT, readNumber());
           return tokens;
        }
        else
        {
           tokens = new Token(TokenType.EOF, currentCharacter.ToString());
        }

        readCharacter();

        return tokens;

    }

    public void SkipWhiteSpaces()
    {
        while (currentCharacter == ' ' || currentCharacter == '\r' || currentCharacter == '\n' || currentCharacter == '\t')
        {
           readCharacter();
        }
    }

    private string readNumber()
    {
        int currentPosition = position;
        while (Char.IsDigit(currentCharacter))
        {
            readCharacter();
        }
        return input[currentPosition..position];
    }

}


