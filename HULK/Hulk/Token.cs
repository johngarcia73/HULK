class Token
{
    public TokenType Kind{get; }
    public string Text{get;set; }
    public int Position{get; }
    public Token(TokenType kind, string text, int position)
    {
        Kind=kind;
        Text=text;
        Position=position;
    }
}

//Clasificando los Tokens
public enum TokenType
{
    Number,
    String,
    Bool,
    Identifier,
    Constant,
    Function,
    ReservedFunction,
    Coma,
    WhiteSpace,
    OpenParenthesis,
    CloseParenthesis,
    BinaryOperation,
    StringConcatenator,
    Plus,
    Minus,
    Slash,
    Star,
    Equals,
    NotEquals,
    CompareEquals,
    Bigger,
    BiggerEquals,
    Smaller,
    SmallerEquals,
    OrOperator,
    AndOperator,
    FunctionOperator,
    ReservedWord,
    NotOperator,
    Useless,
    Semicolon,
    EndLine,
}