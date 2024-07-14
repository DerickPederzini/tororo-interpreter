using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_cs.MonkeyLexer.Token;

public class Lexer
{
    internal string input;//as a whole
    internal int position;// points to current character
    int readPosition; //points to after the current character
    internal char currentCharacter;

    public Lexer(string input) {
        this.input = input;
        readCharacter();
    }

    public void readCharacter()
    {
        if (readPosition >= input.Length) {
            currentCharacter = '\0';//ASCII code for Null

        }
        else {
            currentCharacter = input.ElementAt(readPosition);
        }
        position = readPosition;
        readPosition++;

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
        SkipWhiteSpaces();

        Token tokens;

        if (currentCharacter == '+')
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

        else if (currentCharacter == '/')
            tokens = Token.SLASH;

        else if (currentCharacter == '*')
            tokens = Token.MULTIPLY;

        else if (currentCharacter == '>')
            tokens = Token.RANGLE;

        else if (currentCharacter == '<')
            tokens = Token.LANGLE;

        else if (currentCharacter == '=')
        {
            char charAfterCurrent = peakChar();

            if (charAfterCurrent == '=')
            {
                tokens = Token.EQUAL;
                readCharacter();
            }
            else 
                tokens = Token.ASSIGN;
            

        }
        else if (currentCharacter == '!')
        {
            char charAfterCurrent = peakChar();

            if (charAfterCurrent == '=')
            {
                tokens = Token.NOTEQUAL;
                readCharacter();
            }

            else 
                tokens = Token.NOT;
            
        }
        else if (currentCharacter == '"') {
            tokens = Token.STRING;
            tokens.Literal = readString();
        }
        else if (Char.IsLetter(currentCharacter))
        {
            //this is a very cool way of doing switches
            return readIdentifier() switch
            {
                "fn" => Token.FUNCTION,
                "let" => Token.LET,
                "if" => Token.IF,
                "else" => Token.ELSE,
                "true" => Token.TRUE,
                "false" => Token.FALSE,
                "return" => Token.RETURN,
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
            if(currentCharacter == '\0')
                return Token.EOF;

            tokens = Token.Illegal;
        }

        readCharacter();

        return tokens;

    }

    public char peakChar()
    {
        if(readPosition >= input.Length)    
        {
            return '\0';
        }
        return input[readPosition];
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

    private string readString() {
        int cPosition = position + 1;
        while (true) {
            readCharacter();
            if (currentCharacter == '"' || currentCharacter == '\0') {
                break;
            }
        }
        return input[cPosition..position];
    }

}


