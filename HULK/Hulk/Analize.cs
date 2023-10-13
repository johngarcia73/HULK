using System.Collections;

class Analize
{
    static public List<string> errorList{get;set;} = new List<string>();
    private string _text;
    private int _position;
    public Analize(string text)
    {
       _text=text;
    }

    //Caracter actual
    private char Current
    {
        get
        {
            if(_position >= _text.Length)
            {
                return '\0';
            }

            return _text[_position];
        }    
    }

    //Siguiente Caracter
    private void NextPosition()
    {
        _position++;
    }

    //Compara con palabras reservadas
    static public bool Reserved(string text)
    {
        switch (text)
        {
            case "number": return true;
            case "string": return true;
            case "bool": return true;
            case "print": return true;
            case "if": return true;
            case "else": return true;
            case "while": return true;
            case "for": return true;
            case "let": return true;
            case "in": return true;
            case "function": return true;

            default: return false;
        }
    }
    static public bool Constants(string text)
    {
        switch (text)
        {
            case "E": return true;
            case "PI": return true;
            
            default: return false;
        }
    }

    static public bool ReservedFunction(string text)
    {
        switch (text)
        {
            case "sin":
            case "cos":
            case "sqrt":
            case "pow":
            case "log":
            case "rand":
                return true;

            default: return false;
        }
    }

    //Continua analizando la linea y devuelve el siguiente token
    public Token NextToken()
    {
        if(_position >= _text.Length)
        {
            return new Token(TokenType.EndLine, "\0", _position);
        }

        if (char.IsDigit(Current))
        {
            int startPosition=_position;
            string tokenText="";

            while(char.IsDigit(Current))
            {
                NextPosition();
            }
            if (char.IsLetter(Current))
            {
                while (!char.IsWhiteSpace(Current) && (Current != '\0') && (Current != ';') && (Current != ','))
                {
                    NextPosition();   
                }
                tokenText=_text.Substring(startPosition,_position-startPosition);
                return new Token(TokenType.Useless, tokenText, _position++);
            }
            tokenText=_text.Substring(startPosition,_position-startPosition);
            int.TryParse(tokenText, out var value);

            startPosition = _position;
            string afterComa = "";
            if (Current == '.')
            {
                NextPosition();
                while(char.IsDigit(Current))
                {
                    NextPosition();
                }
                afterComa = "," + _text.Substring(startPosition+1, _position-startPosition-1);
            }
            return new Token(TokenType.Number, tokenText + afterComa, startPosition);
        }
        if (Current == '"')
        {
            int startPosition=_position;
            string tokenText="";
            NextPosition();

            while(Current != '"')
            {
                if (_position == _text.Length-1)
                {
                    break;
                }
                NextPosition();
            }
            tokenText=_text.Substring(startPosition+1,_position-startPosition-1);
            NextPosition();
            int.TryParse(tokenText, out var value);
            return new Token(TokenType.String, tokenText, startPosition);
        }

        if (char.IsWhiteSpace(Current))
        {
            int startPosition=_position;
            string tokenText="";
            
            while(char.IsWhiteSpace(Current))
            {
                NextPosition();
            }
            tokenText=_text.Substring(startPosition,_position-startPosition);
            int.TryParse(tokenText, out var value);
            return new Token(TokenType.WhiteSpace, tokenText, startPosition);
        }

        if (char.IsLetter(Current) || Current == '_')
        {
            int startPosition=_position;
            string tokenText="";
            
            while(char.IsLetterOrDigit(Current) || Current == '_')
            {
                NextPosition();
            }
            tokenText=_text.Substring(startPosition,_position-startPosition);
            if(Reserved(tokenText))
            {
                return new Token(TokenType.ReservedWord, tokenText, startPosition);
            }
            if (Constants(tokenText))
            {
                return new Token(TokenType.Constant, tokenText, startPosition);
            }
            if (ReservedFunction(tokenText))
            {
                return new Token(TokenType.ReservedFunction, tokenText, startPosition);
            }
            if (tokenText == "true" | tokenText == "false")
            {
                return new Token(TokenType.Bool, tokenText, startPosition);
            }
            return new Token(TokenType.Identifier, tokenText, startPosition);  
        }

        if (Current=='+')
        {
            return new Token(TokenType.Plus, "+", _position++);
        } else if (Current=='-')
        {
            return new Token(TokenType.Minus, "-", _position++);
        } else if (Current=='*')
        {
            return new Token(TokenType.Star, "*", _position++);
        } else if (Current=='!')
        {
            NextPosition();
            if (Current=='=')
            {
                return new Token(TokenType.NotEquals, "!=", _position++);
            }
            return new Token(TokenType.NotOperator, "!", _position);
        }else if (Current=='=')
        {
            NextPosition();
            if (Current=='=')
            {
                return new Token(TokenType.CompareEquals, "==", _position++);
            }
            if (Current == '>')
            {
                return new Token(TokenType.FunctionOperator, "=>", _position++);
            }
            return new Token(TokenType.Equals, "=", _position);
        } else if (Current=='<')
        {
            NextPosition();
            if (Current=='=')
            {
                return new Token(TokenType.SmallerEquals, "<=", _position++);
            }
            return new Token(TokenType.Smaller, "<", _position);
        } else if (Current=='>')
        {
            NextPosition();
            if (Current=='=')
            {
                return new Token(TokenType.BiggerEquals, ">=", _position++);
            }
            return new Token(TokenType.Bigger, ">", _position);
        } else if (Current=='&')
        {
            return new Token(TokenType.AndOperator, "&", _position++);
        } else if (Current=='|')
        {
            return new Token(TokenType.OrOperator, "|", _position++);
        } else if (Current=='@')
        {
            return new Token(TokenType.StringConcatenator, "@", _position++);
        } else if (Current==',')
        {
            return new Token(TokenType.Coma, ",", _position++);
        } else if (Current=='(')
        {
            return new Token(TokenType.OpenParenthesis, "(", _position++);
        } else if (Current==')')
        {
            return new Token(TokenType.CloseParenthesis, ")", _position++);
        } else if (Current=='/')
        {
            return new Token(TokenType.Slash, "/", _position++);
        }else if (Current==';')
        {
            return new Token(TokenType.Semicolon, ";", _position++);
        }

        errorList.Add($"LEXICAL ERROR: Invalid token {0}.");
        return new Token(TokenType.Useless, _text.Substring(_position, 1), _position++);
    }
}