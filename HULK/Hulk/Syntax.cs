using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

abstract class Term                                                                                               
{
    public abstract TokenType kind{get;}
}
abstract class Expression 
{
    public abstract TokenType kind{get;}
}
sealed class NumberExp : Expression
{
    public NumberExp(Token number)
    {
        Number = number;
    }

    public double Value{get;set;}
    public override TokenType kind => TokenType.Number;
    public Token Number{get;}
}
sealed class Variable
{

    public string Name{get;}
    public string Type{get;}
    public Expression Exp{get; set;}
    public string ComingFromFunction{get;set;}

    public Variable(string name, string type, Expression exp)
    {
        Name = name;
        Type = type;
        Exp = exp;
    }
}
sealed class Assignment : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public Identifier Term1{get;}
    public Expression Term2{get;}
    public Assignment(Identifier term1, Expression term2)
    {
        Term1 = term1;
        Term2 = term2;
    }
}
sealed class StringExp : Expression
{
    public Token Text{get;}
    public override TokenType kind => TokenType.String;
    public StringExp(Token text)
    {
        Text = text;
    }
}
sealed class FullExpression
{
    public Expression EntireExpression{get;}
    public List<string> ErrorsList{get;}
    public Token End{get;}

    public FullExpression(Expression fullExpression, List<string> errorsList, Token end)
    {
        EntireExpression = fullExpression;
        ErrorsList = errorsList;
        End = end;
    }
}
sealed class Declaration : Expression
{
    public string Type{get; set;}
    public List<Assignment> Assignment{get;}
    public Expression Corpus{get;}
    public override TokenType kind => TokenType.ReservedWord;

    public Declaration(string type, List<Assignment> assignment, Expression corpus)
    {
        Type = type;
        Assignment = assignment;
        Corpus = corpus;
    }
}
sealed class Conditional : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public Expression Condition{get;}
    public Expression Corpus1{get;}
    public Expression Corpus2{get;}


    public Conditional(Expression condition, Expression corpus1, Expression corpus2)
    {
        Condition = condition;
        Corpus1 = corpus1;
        Corpus2 = corpus2;
    }

}
sealed class Boolean : Expression
{
    public Boolean(Token boolean)
    {
        BooleanValue = boolean;
    }
    public override TokenType kind => TokenType.Number;
    public Token BooleanValue{get;}
    public bool Value{get;set;}
}
sealed class CheckPrinting : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public Expression Exp;
    public CheckPrinting(Expression exp)
    {
        Exp = exp;
    }
}
sealed class Identifier : Expression
{
    public override TokenType kind => TokenType.Identifier;
    public TokenType Subtype{get;set;}
    public string ComingFromFunction{get;set;}
    public int argumentNumber{get;set;}
    public Expression[] expression{get;set;}
    public string Name{get;}
    public Identifier(string name)
    {
        Name = name;
    }
}
sealed class BinaryOperation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Left{get;}
    public Token Operator{get;}
    public Expression Right{get;}

    public BinaryOperation(Expression left, Token _operator, Expression right)
    {
        Left = left;
        Operator = _operator;
        Right = right;
    }
}
sealed class StringOperation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Left{get;}
    public Token Operator{get;}
    public Expression Right{get;}

    public StringOperation(Expression left, Token _operator, Expression right)
    {
        Left = left;
        Operator = _operator;
        Right = right;
    }
}
sealed class BooleanOperation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Left{get;}
    public Token Operator{get;}
    public Expression Right{get;}

    public BooleanOperation(Expression left, Token _operator, Expression right)
    {
        Left = left;
        Operator = _operator;
        Right = right;
    }
}
sealed class DualComparation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Left{get;}
    public Token Operator{get;}
    public Expression Right{get;}

    public DualComparation(Expression left, Token _operator, Expression right)
    {
        Left = left;
        Operator = _operator;
        Right = right;
    }
}
sealed class CompareOperation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Left{get;}
    public Token Operator{get;}
    public Expression Right{get;}

    public CompareOperation(Expression left, Token _operator, Expression right)
    {
        Left = left;
        Operator = _operator;
        Right = right;
    }
}
sealed class WhileCicle : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public Expression Condition{get;}
    public Expression Sentence{get;}
    public WhileCicle(Expression condition, Expression sentence)
    {
        Condition = condition;
        Sentence = sentence;
    }
}
sealed class ForCicle : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public Expression Declare{get;}
    public Expression Condition{get;}
    public Expression Iteration{get;}
    public Expression Sentence{get;}
    public ForCicle(Expression declare, Expression condition, Expression iteration, Expression sentence)
    {
        Declare = declare;
        Condition = condition;
        Iteration = iteration;
        Sentence = sentence;
    }
}
sealed class Function : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public string Name{get;}
    public List<Identifier> Arguments{get;set;}
    public int ArgumentsNum;
    public Expression Output{get;}

    public Function(string name, List<Identifier> arguments, Expression output, int argumentsNum)
    {
        Name = name;
        Arguments = arguments;
        Output = output;
        ArgumentsNum = argumentsNum;
    }
}
sealed class FunctionCall : Expression
{
    public override TokenType kind => TokenType.ReservedWord;
    public int Stack{get;set;}
    public string Name{get;}
    public List<Expression> Arguments{get;}
    public FunctionCall(string name, List<Expression> arguments)
    {
        Name = name;
        Arguments = arguments;
    }
}
class GeneralContext
    {
        public List<Variable> Identifiers{get;set;}
        public GeneralContext? Father{get;set;}
        public string? FunctionName{get;set;}

        public GeneralContext(List<Variable> identifiers, GeneralContext? father, string? functionName)
        {
            Identifiers = identifiers;
            Father = father;
            FunctionName = functionName;
        }   
        public Expression? GetValue(string identifier)
        {
            for (int i = 0; i < this.Identifiers.Count; i++)
            {
                if (this.Identifiers[i].Name == identifier)
                {
                    return this.Identifiers[i].Exp;
                }
            }
            if (this.Father != null)
            {
                return this.Father.GetValue(identifier);
            }else
            {
                Evaluator.errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier}.");
                return null; 
            }
        }
        public Expression? GetValue(string identifier, string functionName)
        {
            for (int i = 0; i < this.Identifiers?.Count; i++)
            {
                if (this.Identifiers[i].Name == identifier && this.Identifiers[i].ComingFromFunction == functionName)
                {
                    return this.Identifiers[i].Exp;
                }
            }
            if (this.Father != null)
            {
                return this.Father.GetValue(identifier, functionName);
            }else
            {
                Evaluator.errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier}.");
                return null; 
            }
        }
    }

sealed class UnaryOperation : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Exp{get;}
    public Token Operator{get;}
    public UnaryOperation(Expression exp, Token _operator)
    {
        Exp = exp;
        Operator = _operator;
    }
}
sealed class ReservedFunction : Expression
{
    public override TokenType kind => TokenType.BinaryOperation;
    public Expression Exp{get;}
    public string Operation{get;}
    public Expression Exp2{get;}

    public ReservedFunction(string operation)
    {
        Operation = operation;
    }
    public ReservedFunction(string operation, Expression exp)
    {
        Exp = exp;
        Operation = operation;
    }
    public ReservedFunction(string operation, Expression exp, Expression exp2)
    {
        Exp = exp;
        Exp2 = exp2;
        Operation = operation;
    }

}

class Syntax
{
    public List<Token> tokensList{get;set;} = new List<Token>();
    public List<string> errorList = new List<string>();
    private int _position;
    public Token Current => Find();
    private Token token1;
    public string line{get;}

    public Syntax(string Line)
    {
        line = Line;
    }

    public List<Token> AddToken()
    {

        Analize lex = new Analize(line);

        do
        {
            token1 = lex.NextToken();
            if (token1.Kind == TokenType.Useless)
            {
                errorList.Add($"LEXICAL ERROR: Invalid token name: {token1.Text}.");
            }
            if (token1.Kind != TokenType.WhiteSpace)
            {
                tokensList.Add(token1);    
            }

        }while(token1.Kind != TokenType.EndLine);

        return tokensList;
    }
    public Token Match(TokenType kind)
    {
        if(Current.Kind == kind)
        {
            return Current;
        }
        errorList.Add("SYNTAX ERROR: Unexpected Token " + Current.Kind + " type, expected " + kind);
        return new Token(kind, "", Current.Position);
    }

    public Token Find()
    {
        int index = _position;

        return tokensList[index];
    }

    public void NextPosition()
    {
        if (_position < tokensList.Count-1)
        {
            _position++;
        }
    }
    public bool identifierIsInsideFunction = false;
    public static bool functionDeclare = false;
    public FullExpression CheckFullExpression()
    {
        var expression = CheckExpression();
        var token = Match(TokenType.Semicolon);
        NextPosition();
        var finalToken = Match(TokenType.EndLine);
        return new FullExpression(expression, errorList, token);
    }
    public Expression CheckExpression()
    {
        switch (Current.Kind)
        {   
            case TokenType.OpenParenthesis:
                return CheckBinary();

            case TokenType.Number:
                return CheckBinary();

            case TokenType.String:
                return CheckStringOperation();

            case TokenType.Function:
                return CheckBinary();
            
            case TokenType.Plus:
            case TokenType.Minus:
            case TokenType.NotOperator:
                return CheckBinary();

            case TokenType.Identifier:    
                return CheckBinary();

            case TokenType.Bool:
                return CheckOrConditional();

            case TokenType.ReservedWord:
                return CheckReserved();

            case TokenType.Constant:
                return CheckBinary();

            case TokenType.ReservedFunction:
                return CheckBinary();    
            default:
                errorList.Add("Unexpected token " + Current.Text + ".");
            break;    
        }
        return new NumberExp(Current);
    }
    public Expression CheckStringOperation()
    {
        var term1 = CheckTerm();
        while (Current.Kind == TokenType.StringConcatenator)
        {
            var _operator = Current;
            NextPosition();
            var term2 = CheckTerm();
            term1 = new StringOperation(term1, _operator, term2);
        }
        
        return term1;
    }
    public Expression CheckUnary()
    {
        var operation = Current;
        NextPosition();
        if (Current.Kind == TokenType.Number)
        {
            NumberExp n = new NumberExp(Current);
            NextPosition();
            return new UnaryOperation(n, operation);
        }
        var exp = CheckExpression();
        return new UnaryOperation(exp, operation);
    }
    public Expression CheckParenthesis()
    {
        NextPosition();
        var exp = CheckExpression();
        if (Current.Kind != TokenType.CloseParenthesis)
        {
            errorList.Add("SYNTAX ERROR: Close Parenthesis missing.");
        }
        NextPosition();
        return exp;
    }
    public Expression CheckBinary()
    {   
        Token token1 = tokensList[_position+1];

        if (Current.Kind == TokenType.OpenParenthesis)
        {
            token1 = tokensList[_position+2];
        }
        if (tokensList[_position+1].Kind == TokenType.Identifier && token1.Kind == TokenType.OpenParenthesis)
        {
            int aux = 0;
            for (int i = _position+2; i < tokensList.Count; i++)
            {
                if (tokensList[i].Kind == TokenType.CloseParenthesis)
                {
                    aux = i;
                    break;
                }
            }
            token1 = tokensList[aux+1];
        }

        if (token1.Kind == TokenType.StringConcatenator)
        {
            return CheckStringOperation();
        }else if (token1.Kind == TokenType.Bigger | token1.Kind == TokenType.BiggerEquals | token1.Kind == TokenType.Smaller | token1.Kind == TokenType.SmallerEquals |
                token1.Kind == TokenType.CompareEquals | token1.Kind == TokenType.NotEquals | token1.Kind == TokenType.OrOperator | token1.Kind == TokenType.AndOperator)
        {
            return CheckOrConditional();
            
        }else if(tokensList[_position+1].Kind == TokenType.OpenParenthesis)
        {
            int aux = 0;
            for (int i = _position+1; i < tokensList.Count; i++)
            {
                if (tokensList[i].Kind == TokenType.CloseParenthesis)
                {
                    aux = i+1;
                    break;
                }
                if (tokensList[i].Kind == TokenType.EndLine)
                {
                    errorList.Add("SYNTAX ERROR: Expected ')' token.");
                }
            }
            TokenType aux2 = tokensList[aux].Kind;
            
            if (aux2 == TokenType.StringConcatenator)
            {
                return CheckStringOperation();
            }else if(aux2 == TokenType.CompareEquals || aux2 == TokenType.NotEquals || aux2 == TokenType.Bigger || 
                aux2 == TokenType.Smaller || aux2 == TokenType.BiggerEquals || aux2 == TokenType.SmallerEquals || aux2 == TokenType.OrOperator || aux2 == TokenType.AndOperator)
            {
                return CheckOrConditional();
            }else
            {
                return CheckSuma();
            }
        }else
        {
            return CheckSuma();
        }
    }
    public Expression CheckSuma()
    {
        var term1 = CheckProduct();

        while (Current.Kind == TokenType.Plus | Current.Kind == TokenType.Minus)
        {
            Token operation = Current;
            NextPosition();
            var term2 = CheckProduct();
            term1 = new BinaryOperation(term1, operation, term2);
        }
        return term1;
    }
    public Expression CheckProduct()
    {
        var term1 = CheckTerm();
        while (Current.Kind == TokenType.Star | Current.Kind == TokenType.Slash)
        {
            Token operation = Current;
            NextPosition();
            var term2 = CheckTerm();
            term1 = new BinaryOperation(term1, operation, term2);
        }
        return term1;
    }
    public Expression CheckCompare()
    {
        var term1 = CheckTerm();
        var _operator = Current;
        
        while (_operator.Kind == TokenType.CompareEquals || _operator.Kind == TokenType.NotEquals || _operator.Kind == TokenType.Bigger || 
        _operator.Kind == TokenType.Smaller || _operator.Kind == TokenType.BiggerEquals || _operator.Kind == TokenType.SmallerEquals)
        {
            NextPosition();

            var term2 = CheckTerm();
            term1 = new CompareOperation(term1, _operator, term2);
            _operator = Current;
        }
        return term1;
    }
    public Expression CheckTerm()
    {
        if (Current.Kind == TokenType.OpenParenthesis)
        {
            return CheckParenthesis();
        }else if(Current.Kind == TokenType.Number)
        {
           return CheckNumber();
        }else if(Current.Kind == TokenType.Plus | Current.Kind == TokenType.Minus | Current.Kind == TokenType.NotOperator)
        {
           return CheckUnary();
        }else if (Current.Kind == TokenType.Function)
        {
            return CheckFunction();
        }else if (Current.Kind == TokenType.Constant)
        {
            return CheckConstant();
        }else if (Current.Kind == TokenType.ReservedFunction)
        {
            return CheckReservedFunction();
        }else if (Current.Kind == TokenType.String)
        {
           return CheckString();
        }else if (Current.Kind == TokenType.Identifier)
        {
            if (tokensList[_position+1].Kind == TokenType.OpenParenthesis)
            {
                return CheckFunction();
            }
            
           return CheckIdentifier();
        }else if (Current.Kind == TokenType.Bool)
        {
            return CheckBoolean();
        }else if (Current.Kind == TokenType.Constant)
        {
            return CheckConstant();
        }else if (Current.Kind == TokenType.ReservedWord)
        {
            return CheckReserved();
        }else
        {
            errorList.Add($"LEXICAL ERROR: Invalid Token name: {Current.Text}.");
            return CheckIdentifier();
        }
    }
    public Expression CheckString()
    {
        var text = Match(TokenType.String);
        NextPosition();
        StringExp expression = new StringExp(text);
        return expression;
    }
    public Identifier CheckIdentifier()
    {
        if (tokensList[_position+1].Kind == TokenType.OpenParenthesis)
        {
            var id1 = CheckFunction();
        }
        var id = Match(TokenType.Identifier);
        Identifier identifier = new Identifier(Current.Text);
        if (identifierIsInsideFunction)
        {
            identifier.ComingFromFunction = ExecutingFunction.executingFunction.Name;
            identifier.argumentNumber = ExecutingFunction.countingArgument;

            ExecutingFunction.countingArgument++;
        }
        NextPosition();
        return identifier;
    }
    public Expression CheckNumber()
    {
        bool aux = false;

        
        if (Current.Kind == TokenType.Plus)
        {
            NextPosition();
        }else if(Current.Kind == TokenType.Minus)
        {
            aux = true;
            NextPosition();
        }

        var num = Match(TokenType.Number);
        NextPosition();
        NumberExp number = new NumberExp(num);

        
        if (aux)
        {
            number.Number.Text = "-" + number.Number.Text;
            number.Value = double.Parse(number.Number.Text);
        }else
        {
            number.Value = double.Parse(number.Number.Text);   
        }
        

        return number;
    }
    public Expression CheckReserved()
    {
        switch (Current.Text)
        {
            case "function":
                return CheckFunctionDeclaration();
            case "let":
                return CheckDeclaration();
            case "while":
                return CheckWhileCicle();
            case "for":
                return CheckForCicle();     
            case "print":
                return CheckPrint();
            case "if":
                return CheckConditional();    

            default:
                errorList.Add("SYNTAX ERROR: Unexpected Reserved Word.");
                return new NumberExp(Current);    
        }
    }
    public Expression CheckConstant()
    {
        switch (Current.Text)
        {   
            case "PI":
                double pi = Math.PI;
                var aux = new NumberExp(new Token(TokenType.Number, pi.ToString(), 0));
                NextPosition();
                return aux;
            case "E":
                double e = Math.E;
                var aux1 = new NumberExp(new Token(TokenType.Number, e.ToString(), 0));
                NextPosition();
                return aux1;

            default:
                errorList.Add("SYNTAX ERROR: Token is not a constant");
                return null;
        }
    }
    public Expression CheckReservedFunction()
    {
        string operation = Current.Text;
        NextPosition();
        var open = Match(TokenType.OpenParenthesis);
        NextPosition();

        List<Expression> args = new List<Expression>();
        while (Current.Kind != TokenType.Coma)
        {
            if (Current.Kind == TokenType.CloseParenthesis)
            {
                break;
            }
            var exp = CheckExpression();
            args.Add(exp);
            if (Current.Kind == TokenType.CloseParenthesis)
            {
                break;
            }
            NextPosition();
        }
        var close = Match(TokenType.CloseParenthesis);
        NextPosition();

        if (operation == "cos" || operation == "sin" || operation == "sqrt")
        {
            if (args.Count != 1)
            {
                errorList.Add($"SYNTAX ERROR: Incorrect Number of parameters for {operation} function.");
                return null;
            }

            var parameter = args[0];

            return new ReservedFunction(operation, parameter);
        }
        if (operation == "pow" || operation == "log")
        {
            if (args.Count != 2)
            {
                errorList.Add($"SYNTAX ERROR: Incorrect Number of parameters for {operation} function.");
                return null;
            }

            var parameter1 = args[0];
            var parameter2 = args[1];

            return new ReservedFunction(operation, parameter1, parameter2);
        }
        if (operation == "rand")
        {
            if (args.Count != 0)
            {
                errorList.Add($"SYNTAX ERROR: Incorrect Number of parameters for {operation} function.");
                return null;
            }

            return new ReservedFunction(operation);
        }
        errorList.Add($"SYNTAX ERROR: Invalid token name {operation} as reserved function.");
        return null;
    }
    public Expression CheckDeclaration()
    {
        NextPosition();
        string type = "";
        List<Assignment> declarations = new List<Assignment>();
        Assignment assignment = CheckAssign();
        declarations.Add(assignment);
        while(Current.Kind == TokenType.Coma)
        {
            NextPosition();
            assignment = CheckAssign();
            declarations.Add(assignment);
        }

        if (Current.Text == "in")
        {
            NextPosition();
        }else
        {
            errorList.Add("SYNTAX ERROR: Unexpected token " + Current.Text + ", expected 'in'.");
            return null;
        }
        if (Current.Text == "function")
        {
            errorList.Add("SYNTAX ERROR: Can not declare a function in a let-in scope.");
            return null;
        }
        var expression = CheckExpression();
        return new Declaration(type, declarations, expression);
    }
    public Expression CheckPrint()
    {
        NextPosition();
        var openphar = Match(TokenType.OpenParenthesis);
        NextPosition();
        var exp = CheckExpression();
        var closephar = Match(TokenType.CloseParenthesis);
        NextPosition();

        return new CheckPrinting(exp);
    }
    public Expression CheckBoolean()
    {
        var x = Match(TokenType.Bool);
        Boolean boolean = new Boolean(x);
        boolean.Value = bool.Parse(boolean.BooleanValue.Text);
        NextPosition();

        return boolean;
    }
    public Expression CheckConditional()
    {
        NextPosition();
        var condition = CheckOrConditional();
        var corpus1 = CheckExpression();
        string reservedElse = Current.Text;
        if (reservedElse != "else")
        {
            errorList.Add("SYNTAX ERROR: Expected reserved word 'else'.");
        }
        NextPosition();
        var corpus2 = CheckExpression();
        return new Conditional(condition, corpus1, corpus2);
    }
    public Expression CheckOrConditional()
    {
        var term1 = CheckAndConditional();
        while (Current.Kind == TokenType.OrOperator)
        {
            Token operation = Current;
            NextPosition();
            var term2 = CheckAndConditional();
            term1 = new DualComparation(term1, operation, term2);
        }
        return term1;
    }
    public Expression CheckAndConditional()
    {
        var term1 = CheckCompare();
        while (Current.Kind == TokenType.AndOperator)
        {
            Token operation = Current;
            NextPosition();
            var term2 = CheckCompare();
            term1 = new DualComparation(term1, operation, term2);
        }
        return term1;
    }
    public Assignment CheckAssign()
    {
        Identifier term1 = CheckIdentifier();
        var equal = Match(TokenType.Equals);
        NextPosition();
        var term2 = CheckExpression();
        
        return new Assignment(term1, term2);
    }
    public Expression CheckWhileCicle()
    {
        NextPosition();
        var openParenthesis = Match(TokenType.OpenParenthesis);
        NextPosition();
        var condition = CheckCompare();
        var closeParenthesis = Match(TokenType.CloseParenthesis);
        NextPosition();
        var exp = CheckExpression();

        return new WhileCicle(condition, exp);
    }
    public Expression CheckForCicle()
    {
        NextPosition();
        var openParenthesis = Match(TokenType.OpenParenthesis);
        NextPosition();
        var declaration = CheckAssign();
        var coma1 = Match(TokenType.Coma);
        NextPosition();
        var condition = CheckCompare();
        var coma2 = Match(TokenType.Coma);
        NextPosition();
        var iteration = CheckAssign();
        var closeParenthesis = Match(TokenType.CloseParenthesis);
        NextPosition();
        var exp = CheckExpression();

        return new ForCicle(declaration, condition, iteration, exp);
    }
    public Expression CheckFunctionDeclaration()
    {
        identifierIsInsideFunction = true;
        NextPosition();
        if (Analize.Reserved(Current.Text) || Analize.Constants(Current.Text) || Analize.ReservedFunction(Current.Text))
        {
            errorList.Add($"SEMANTIC ERROR: Function name {Current.Text} already exists and it is a reserved command of HULK.");
        }
        var text = Match(TokenType.Identifier);
        NextPosition();
        string name = text.Text;
        ExecutingFunction.executingFunction = new Function(name, null, null, 0);
        var open = Match(TokenType.OpenParenthesis);
        List<Identifier> arguments = new List<Identifier>();
        int count = 0;
        do
        {
            NextPosition();
            if (Current.Kind == TokenType.CloseParenthesis)
            {
                break;
            }
            Identifier identifier = CheckIdentifier();
            arguments.Add(identifier);

        } while (Current.Kind == TokenType.Coma);
        var close = Match(TokenType.CloseParenthesis);
        ExecutingFunction.countingArgument = 0;
        NextPosition();

        var functionOperator = Match(TokenType.FunctionOperator);
        NextPosition();
        var corpus = CheckExpression();
        identifierIsInsideFunction = false;
        ExecutingFunction.executingFunction = new Function(null, null, null, 0);

        functionDeclare = true;
        return new Function(name, arguments, corpus, count);
    }
    public Expression CheckFunction()
    {
        string name = Current.Text;
        string aux = "";
        NextPosition();
        var open = Match(TokenType.OpenParenthesis);

        List<Expression> arguments = new List<Expression>();
        do
        {
            NextPosition();
            if (Current.Kind == TokenType.CloseParenthesis)
            {
                break;
            }
            var x = CheckExpression();
            arguments.Add(x);

        } while (Current.Kind == TokenType.Coma);

        var close = Match(TokenType.CloseParenthesis);
        NextPosition();

        return new FunctionCall(name, arguments);
    }
    
    public static class ExecutingFunction
    {
        public static Function executingFunction = new Function(null, null, null, 0);
        public static int countingArgument = 0;
    }
}