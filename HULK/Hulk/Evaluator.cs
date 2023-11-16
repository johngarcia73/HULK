using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

class Evaluator
{
    public static List<string> errorList = new List<string>();
    static List<Variable> variableList = new List<Variable>();
    static GeneralContext? generalContext;
    public static GeneralContext? functionContext;

    List<string> variableNames = new List<string>();
    public void EvaluateFullExpression(FullExpression fullExpression)
    { 
        errorList = new List<string>();
        if (Syntax.functionDeclare)
        {
            if (!(fullExpression.EntireExpression is Function f))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                errorList.Add("SEMANTIC ERROR: Function declaration expression can not be used.");
                for (int i = 0; i < errorList.Count; i++)
                {
                    System.Console.WriteLine(errorList[i]);
                }
                Console.ResetColor();
            }else
            {
                Expression expression1 = fullExpression.EntireExpression;
                Expression evaluated1 = EvaluateExpression(expression1);
                Syntax.functionDeclare = false;
                if (errorList.Count != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int i = 0; i < errorList.Count; i++)
                    {
                        System.Console.WriteLine(errorList[i]);
                    }
                    Console.ResetColor();
                }else
                {
                    Print(evaluated1);
                }
            }
        }else
        {
            Expression expression = fullExpression.EntireExpression;
            Expression evaluated = EvaluateExpression(expression);
            Syntax.functionDeclare = false;
            if (errorList.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                for (int i = 0; i < errorList.Count; i++)
                {
                    System.Console.WriteLine(errorList[i]);
                }        
                Console.ResetColor();
            }else
            {
                Print(evaluated);
            }
        }      
    }
    public Expression EvaluateExpression(Expression exp)
    {
        if (exp is NumberExp n)
        {
            var x = EvaluateNumber(n);
            return n;
        }
        if (exp is StringExp st)
        {
            var x = EvaluateString(st);
            return st;
        }
        if (exp is Boolean boolean)
        {
            var x = EvaluateBoolean(boolean);
            return x;
        }
        if (exp is UnaryOperation unaryOperation)
        {
            var x = EvaluateUnary(unaryOperation);
            return x;
        }
        if (exp is ReservedFunction rf)
        {
            var evaluated = EvaluateReservedFunction(rf);
            return evaluated;
        }
        if (exp is CompareOperation comparation)
        {
            Boolean evaluated = EvaluateComparation(comparation);
            return evaluated;
        }
        if (exp is DualComparation dual)
        {
            Boolean evaluated = EvaluateMultipleComparations(dual);
            return evaluated;
        }
        if (exp is Conditional conditional)
        {
            Expression corpus = EvaluateConditional(conditional);
            var evaluated = EvaluateExpression(corpus);
            return evaluated;
        }
        if (exp is BinaryOperation b)
        {
            var x = EvaluateBinary(b);
            return x;
        }
        if (exp is Identifier id)
        {
            var x = EvaluateIdentifier(id);
            return x;;
        }
        if (exp is Declaration d)
        {
            var evaluated = EvaluateDeclaration(d);
            return evaluated;
        }
        if (exp is Assignment a)
        {
            var evaluated = EvaluateAssignment(a);
            return evaluated;
        }
        if (exp is StringOperation s)
        {
            var x = EvaluateStringOp(s);
            return x;
        }
        if (exp is Function f)
        {
            EvaluateFunctionDeclaration(f);
            return new StringExp(new Token(TokenType.String, "", 0));
        }
        if (exp is FunctionCall fc)
        {
            var x = EvaluateFunction(fc);
            return x;
        }
        if (exp is CheckPrinting p)
        {
            var x = EvaluateExpression(p.Exp);
            return x;
        }
        errorList.Add("SEMANTIC ERROR: Expression type not recognized.");
        return null;
    }
    public Expression EvaluateReservedFunction(ReservedFunction operation)
    {
        if (operation.Operation == "cos" || operation.Operation == "sin" || operation.Operation == "sqrt")
        {
            var evaluated = EvaluateExpression(operation.Exp);
            if (!(evaluated is NumberExp n))
            {
                errorList.Add($"SEMANTIC ERROR: Expression inside {operation.Operation} function is not a number.");
                return null;
            }else
            {
                double aux = double.Parse(n.Number.Text);
                double result = 0;
                if (operation.Operation == "cos")
                {
                    result = Math.Cos(aux);
                }
                if (operation.Operation == "sin")
                {
                    result = Math.Sin(aux);
                }
                if (operation.Operation == "sqrt")
                {
                    if (aux<0)
                    {
                        errorList.Add($"SEMANTIC ERROR: Can not determinate the squared root of a negative number.");
                        return null;   
                    }
                    result = Math.Sqrt(aux);
                }
                if ((result > 0 && result < 0.000001) || (result < 0 && result > -0.000001))
                {
                    result = 0;
                }
                if (result > 0 && result - Math.Truncate(result) > 0.999999)
                {
                    result = Math.Truncate(result) + 1;
                }
                if (result < 0 && result - Math.Truncate(result) < -0.999999)
                {
                    result = Math.Truncate(result) - 1;
                }

                var final = new NumberExp(new Token(TokenType.Number, result.ToString(), 0));
                return final;
            }

        }
        if (operation.Operation == "pow" || operation.Operation == "log")
        {
            var evaluated = EvaluateExpression(operation.Exp);
            var evaluated2 = EvaluateExpression(operation.Exp2);

            if (!(evaluated is NumberExp n) || !(evaluated2 is NumberExp n2))
            {
                errorList.Add($"SEMANTIC ERROR: Expression inside {operation.Operation} function is not a number.");
                return null;
            }else
            {
                double aux = double.Parse(n.Number.Text);
                double aux2 = double.Parse(n2.Number.Text);

                double result = 0;
                if (operation.Operation == "pow")
                {
                    result = Math.Pow(aux, aux2);
                }
                if (operation.Operation == "log")
                {   
                    if (aux<=0)
                    {
                        errorList.Add($"SEMANTIC ERROR: Can not determinate the logarithm of a negative number.");
                        return null;   
                    }
                    if (aux2<=0)
                    {
                        errorList.Add($"SEMANTIC ERROR: Can not determinate the logarithm in a negative base.");
                        return null;   
                    }
                    if (aux2 == 1)
                    {
                        errorList.Add($"SEMANTIC ERROR: Can not determinate the logarithm in base equlas to 1.");
                        return null;
                    }

                    result = Math.Log(aux, aux2);
                }
                var final = new NumberExp(new Token(TokenType.Number, result.ToString(), 0));
                return final;
            } 
        }
        if (operation.Operation == "rand")
            {
                Random aux3 = new Random();
                double result = aux3.NextDouble();

                var final = new NumberExp(new Token(TokenType.Number, result.ToString(), 0));
                return final;
            }
        errorList.Add($"SEMANTIC ERROR: Invalid token name {operation} as reserved function.");
        return null;
    }
    public Expression EvaluateUnary(UnaryOperation unary)
    {
        var evaluated = EvaluateExpression(unary.Exp);

        switch (unary.Operator.Kind)
        {
            case TokenType.Plus:
                if (evaluated is NumberExp n)
                {
                    return n;
                }else
                {
                    errorList.Add($"SEMANTIC ERROR: Expression is not a number.");
                    return null;
                }
            case TokenType.Minus:
                if (evaluated is NumberExp k)
                {
                    double aux = double.Parse(k.Number.Text);
                    aux = aux * (-1);
                    var evaluated2 = k;
                    evaluated2.Number.Text = aux.ToString();
                    return evaluated2;
                }else
                {
                    errorList.Add($"SEMANTIC ERROR: Expression is not a number.");
                    return null;
                } 
            case TokenType.NotOperator:
                if (evaluated is Boolean b)
                {
                    Boolean aux = b;
                    if (b.BooleanValue.Text == "false")
                    {
                        aux.BooleanValue.Text = "true";
                        aux.Value = true;
                    }else
                    {
                        aux.BooleanValue.Text = "false";
                        aux.Value = false;
                    }
                    return aux;
                }else
                {

                    errorList.Add($"SEMANTIC ERROR: Expression is not a bool value.");
                    return null;
                }    
            default:
                errorList.Add($"SEMANTIC ERROR: Token {unary.Operator.Text} is not a valid operator.");
                return null;

        }
    }
    public Expression EvaluateBoolean(Expression boolean)
    {
        if (boolean is Boolean b1)
        {
            return b1;
        }else if(boolean is Identifier i)
        {
            var x = EvaluateIdentifier(i);
            if (x is Boolean b)
            {
                return b;
            }else
            {
                errorList.Add($"SEMANTIC ERROR: {i.Name} is not a bool value.");
                return null;
            }
        }else if(boolean is DualComparation d)
        {   
            var x = EvaluateMultipleComparations(d);
            return EvaluateBoolean(x);
        }else if(boolean is UnaryOperation u)
        {
            var x = EvaluateUnary(u);
            return EvaluateBoolean(x);
        }else if(boolean is FunctionCall f)
        {
            var x = EvaluateFunction(f);
            return EvaluateBoolean(x);
        }else if(boolean is Conditional c)
        {
            var x =  EvaluateConditional(c);
            return EvaluateBoolean(x);
        }else if(boolean is CompareOperation b3)
        {   
            var x = EvaluateComparation(b3);
            return EvaluateBoolean(x);
        }else
        {
            errorList.Add($"SEMANTIC ERROR: Invalid Expressio for a boolean operation");
            return null;
        }
    }
    public StringExp EvaluateStringOp(StringOperation s)
    {
        var left = s.Left;
        var right = s.Right;

        if (left is NumberExp n1)
        {
            left = new StringExp(new Token(TokenType.String, n1.Number.Text, 0));
        }
        if (right is NumberExp n2)
        {
            right = new StringExp(new Token(TokenType.String, n2.Number.Text, 0));
        }

        Expression term1 = EvaluateString(left);
        Expression term2 = EvaluateString(right);

        if (term1 is StringExp u && term2 is StringExp t)
        {
            if (s.Operator.Kind == TokenType.StringConcatenator)
            {   
                string result = u.Text.Text + t.Text.Text;
                StringExp expression = new StringExp(new Token(TokenType.String, result, 0));
                return expression;
            }else
            {
                errorList.Add($"SEMANTIC ERROR: {s.Operator.Text} is not a valid operator for string operation.");
                return null;  
            }
        }else
        {
            errorList.Add($"SEMANTIC ERROR: Invalid not string type value for a string operation.");
            return null;
        }
    }
    public Expression EvaluateString(Expression exp)
    {   
        if (exp is StringExp s)
        {
            return s;
        }else if(exp is Identifier i)
        {
            var x = EvaluateIdentifier(i);
            if (x is StringExp str)
            {
                return str;
            }else if (x is NumberExp n)
            {
                var aux = new StringExp(new Token(TokenType.String, n.Number.Text, 0));
                return aux;
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Expression is not a string value.");
                return null;
            }
        }else if(exp is FunctionCall f)
        {
            var x =  EvaluateFunction(f);
            return EvaluateString(x);
        }else if(exp is Conditional c)
        {
            var x =  EvaluateConditional(c);
            return EvaluateString(x);
        }else if(exp is Declaration d)
        {
            var x =  EvaluateDeclaration(d);
            return EvaluateString(x);
        }else if(exp is StringOperation o)
        {
            var x = EvaluateStringOp(o);
            return EvaluateString(x);    
        }else if(exp is ReservedFunction r)
        {
            var x =  EvaluateReservedFunction(r);
            return EvaluateString(x);    
        }else
        {
            errorList.Add($"SEMANTIC ERROR: Invalid expression type for a string operation.");
            return null;
        }
    }
    public NumberExp EvaluateBinary(BinaryOperation binary)
    {
        var left = binary.Left;
        var right = binary.Right;

        Expression term1 = EvaluateNumber(left);
        Expression term2 = EvaluateNumber(right);
        float evaluated1 = 0;
        float evaluated2 = 0;

        if (term1 is NumberExp n)
        {
            evaluated1 = float.Parse(n.Number.Text);

            if (term2 is NumberExp m)
            {
                evaluated2 = float.Parse(m.Number.Text);
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Right term is not a number, can not be used for a numeric operation.");
                return null;
            }
        }else
        {
            errorList.Add($"SEMANTIC ERROR: Left term is not a number, can not be used for a numeric operation.");
            return null;
        }

        switch (binary.Operator.Kind)
        {
            case TokenType.Plus:
                float result1 = evaluated1 + evaluated2;
                return new NumberExp(new Token(TokenType.Number, result1.ToString(), 0));

            case TokenType.Minus:
                float result2 = evaluated1 - evaluated2;
                return new NumberExp(new Token(TokenType.Number, result2.ToString(), 0));

            case TokenType.Star:
                float result3 = evaluated1 * evaluated2;
                return new NumberExp(new Token(TokenType.Number, result3.ToString(), 0));

            case TokenType.Slash:
                if (evaluated2 == 0)
                {
                    errorList.Add($"SEMANTIC ERROR: Impossible to divide by Zero.");
                    return null;

                }
                float result4 = evaluated1 / evaluated2;
                return new NumberExp(new Token(TokenType.Number, result4.ToString(), 0));
    
            default:
                errorList.Add($"SEMANTIC ERROR: Token {binary.Operator.Text} is not a valid operator for a niumeric operation");
                return null;  
        }
    }
    public Boolean EvaluateComparation(CompareOperation compareOperation)
    {
        var term1 = compareOperation.Left;
        var term2 = compareOperation.Right;
        var operation = compareOperation.Operator;

        if (term1 is CompareOperation c1)
        {
            term1 = EvaluateComparation(c1);
        }
        if (term2 is CompareOperation c2)
        {
            term2 = EvaluateComparation(c2);
        }

        if (term1 is DualComparation d1)
        {
            term1 = EvaluateMultipleComparations(d1);
        }
        if (term2 is DualComparation d2)
        {
            term2 = EvaluateMultipleComparations(d2);
        }

        if (term1 is BinaryOperation b1)
        {
            term1 = EvaluateBinary(b1);
        }
        if (term2 is BinaryOperation b2)
        {
            term2 = EvaluateBinary(b2);
        }

        if (term1 is UnaryOperation u1)
        {
            term1 = EvaluateUnary(u1);
        }
        if (term2 is UnaryOperation u2)
        {
            term2 = EvaluateUnary(u2);
        }

        if (term1 is Identifier i)
        {
            var value = EvaluateIdentifier(i);
            term1 = value;
        }
        if (term2 is Identifier j)
        {
            var value = EvaluateIdentifier(j);
            term2 = value;
        }
        if (term1 is ReservedFunction rf1)
        {
            var value = EvaluateReservedFunction(rf1);
            term1 = value;
        }
        if (term2 is ReservedFunction rf2)
        {
            var value = EvaluateReservedFunction(rf2);
            term2 = value;
        }

        if (term1 is FunctionCall f1)
        {
            var value = EvaluateFunction(f1);
            term1 = value;
        }
        if (term2 is FunctionCall f2)
        {
            var value = EvaluateFunction(f2);
            term2 = value;
        }
        if(term1 is Conditional con1)
        {
            term1 = EvaluateConditional(con1);
        }
        if (term2 is Conditional con2)
        {
            term2 = EvaluateConditional(con2);
        }

        if (term1 is NumberExp n)
        {
            if (term2 is NumberExp m)
            {
                return CompareNumbers(n, operation, m);
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Impossible to compare different type values.");
                return null;
            }
        }

        if (term1 is StringExp l)
        {
            if (term2 is StringExp m)
            {
                return CompareStrings(l, operation, m);
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Impossible to compare different type values.");
                return null;
            }
        }
        if (term1 is Boolean k)
        {
            if (term2 is Boolean m)
            {
                return CompareBooleans(k, operation, m);
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Impossible to compare different type values.");
                return null;
            }
        }

        errorList.Add($"SEMANTIC ERROR: Invalid type for a comparation");
        return null;
    }
    public Boolean CompareNumbers(NumberExp term1, Token operation, NumberExp term2)
    {
        float left = float.Parse(term1.Number.Text);
        float right = float.Parse(term2.Number.Text);

        switch (operation.Kind)
        {
            case TokenType.CompareEquals:
                if (left == right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }
            case TokenType.NotEquals:
                if (left != right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }   
            case TokenType.Smaller:
                if (left < right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                } 
            case TokenType.SmallerEquals:
                if (left <= right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }
            case TokenType.Bigger:
                if (left > right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                } 
            case TokenType.BiggerEquals:
                if (left >= right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }                 
            default: 
                errorList.Add($"SEMANTIC ERROR: Token {operation.Text} is not a valid comparation operator between numbers");
                return null; 
        }
    }
    public Boolean CompareStrings(StringExp term1, Token operation, StringExp term2)
    {
        string left = term1.Text.Text;
        string right = term2.Text.Text;

        switch (operation.Kind)
        {
            case TokenType.CompareEquals:
                if (left == right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }
            case TokenType.NotEquals:
                if (left != right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }      
            default:
                errorList.Add($"SEMANTIC ERROR: Token {operation.Text} is not a valid comparation operator between strings.");
                return null;    
        }
    }
    public Boolean CompareBooleans(Boolean term1, Token operation, Boolean term2)
    {
        bool left = term1.Value;
        bool right = term2.Value;

        switch (operation.Kind)
        {
            case TokenType.CompareEquals:
                if (left == right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }  
            case TokenType.NotEquals:
                if (left != right)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }
            case TokenType.AndOperator:
                if (left == true && right == true)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }
            case TokenType.OrOperator:
                if (left == true | right == true)
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                    boolean.Value = true;
                    return boolean;
                }else
                {
                    Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));   
                    boolean.Value = false;
                    return boolean;
                }           
            default:
                errorList.Add($"SEMANTIC ERROR: Token {operation.Text} is not a valid comparation operator. between booleans");
                return null;   
        }
    }
    
    public Boolean EvaluateMultipleComparations(DualComparation comparation)
    {
        var left = comparation.Left;
        var _operator = comparation.Operator;
        var right = comparation.Right;

        Boolean boolean1 = new Boolean(new Token(TokenType.Bool, "True", 0));
        boolean1.Value = true;
        Boolean boolean2 = new Boolean(new Token(TokenType.Bool, "False", 0));
        boolean2.Value = false;
        Token andOperator = new Token(TokenType.AndOperator, "&", 0);

        if (left is Identifier id1)
        {
            left = EvaluateIdentifier(id1);
        }
        if (right is Identifier id2)
        {
            right = EvaluateIdentifier(id2);
        }
        if (left is FunctionCall f1)
        {
            left = EvaluateFunction(f1);
        }
        if (right is FunctionCall f2)
        {
            right = EvaluateFunction(f2);
        }

         if (left is ReservedFunction rf1)
        {
            var value = EvaluateReservedFunction(rf1);
            left = value;
        }
        if (right is ReservedFunction rf2)
        {
            var value = EvaluateReservedFunction(rf2);
            right = value;
        }

        if(left is DualComparation dual1)
        {
            left = EvaluateMultipleComparations(dual1);
        }
        if (right is DualComparation dual2)
        {
            right = EvaluateMultipleComparations(dual2);
        }

        if(left is UnaryOperation una1)
        {
            left = EvaluateUnary(una1);
        }
        if (right is UnaryOperation una2)
        {
            right = EvaluateUnary(una2);
        }
        if(left is Conditional con1)
        {
            left = EvaluateConditional(con1);
        }
        if (right is Conditional con2)
        {
            right = EvaluateConditional(con2);
        }

        if (left is Boolean b1)
        {
            if (b1.Value == true)
            {
                left = new CompareOperation(boolean1, andOperator, boolean1);  
            }
            else
            {
                left = new CompareOperation(boolean1, andOperator, boolean2);
            }
        }
        if (right is Boolean b2)
        {
            if (b2.Value == true)
            {
                right = new CompareOperation(boolean1, andOperator, boolean1);  
            }
            else
            {
                right = new CompareOperation(boolean1, andOperator, boolean2);
            }
        }
        
        if (left is CompareOperation c && right is CompareOperation d)
        {
            var term1 = EvaluateComparation(c);
            var term2 = EvaluateComparation(d);

            switch(_operator.Kind)
            {
                case TokenType.AndOperator:
                    if (term1.Value == true && term2.Value == true)
                    {
                        Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0))
                        {
                            Value = true
                        };
                        return boolean;
                    }
                    else
                    {
                        Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));
                        boolean.Value = false;
                        return boolean;
                    }
                case TokenType.OrOperator:
                    if (term1 == null)
                    {
                        errorList.Add("SEMANTIC ERROR: Invalid null token for a comparation operation.");
                        return null;
                    }
                    if (term2 == null)
                    {
                        errorList.Add("SEMANTIC ERROR: Invalid null token for a comparation operation.");
                        return null;
                    }

                    if (term1.Value == true || term2.Value == true)
                    {
                        
                        Boolean boolean = new Boolean(new Token(TokenType.Bool, "true", 0));
                        boolean.Value = true;
                        return boolean;
                    }
                    else
                    {
                        Boolean boolean = new Boolean(new Token(TokenType.Bool, "false", 0));
                        boolean.Value = false;
                        return boolean;
                    }    

                default:
                    errorList.Add($"SEMANTIC ERROR: Token {_operator.Text} is not a valid comparation concatenator.");
                    return null; 

            }
        }
        else
        {
            errorList.Add($"SEMANTIC ERROR: There are not valid comparations to concatenate");
            return null; 
        }
    }
    public Expression EvaluateConditional(Conditional exp)
    {
        var condition = exp.Condition;
        var boolean = EvaluateExpression(condition);
        if (boolean is Boolean b)
        {
            if (b.Value == true)
            {
                condition = exp.Corpus1;
            }
            else
            {
                condition = exp.Corpus2;
            }
        }
        else
        {
            errorList.Add($"SEMANTIC ERROR: Given condition does not return a valid truth value.");
            return null; 
        }
        return condition;
    }
    public Expression EvaluateNumber(Expression number)
    {
        if (number is NumberExp n)
        {
            return n;
        }else if(number is Identifier i)
        {
            var x = EvaluateIdentifier(i);
            if (x is NumberExp n1)
            {
                return n1;
            }else
            {
                errorList.Add($"SEMANTIC ERROR: Token {i.Name} does not return a number");
                return null; 
            }

        }else if(number is BinaryOperation b)
        {
            return EvaluateBinary(b);
        }else if(number is UnaryOperation u)
        {
            var x = EvaluateUnary(u);
            return EvaluateNumber(x);
        }else if(number is FunctionCall f)
        {
            var x = EvaluateFunction(f);
            return EvaluateNumber(x);
        } if (number is ReservedFunction rf)
        {
            var x = EvaluateReservedFunction(rf);
            return EvaluateNumber(x);
        } if (number is Declaration d)
        {
            var x = EvaluateDeclaration(d);
            return EvaluateNumber(x);
        } if (number is Conditional c)
        {
            var x = EvaluateConditional(c);
            return EvaluateNumber(x);
        }else
        {
            errorList.Add($"SEMANTIC ERROR: Given expression does not return a number.");
            return null; 
        }
    }
    public Expression EvaluateDeclaration(Declaration d)
    {
        List<Variable> list = new List<Variable>();
        for (int i = 0; i < d.Assignment.Count; i++)
        { 
            bool flag = false;
            int aux = 0;
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].Name == d.Assignment[i].Term1.Name)
                {
                    aux = j;
                    flag = true;
                    break;
                }
            }
            if (flag)               // Al declarar dos variables de mismo nombre, se queda con el valor de la ultima.
            {
                list.RemoveAt(aux);
            }
            Variable x = new Variable(d.Assignment[i].Term1.Name, d.Type, d.Assignment[i].Term2);
            x.ComingFromFunction = d.Assignment[i].Term1.ComingFromFunction;
            list.Add(x);
        }

        if (generalContext == null)
        {
            generalContext = new GeneralContext(list, null, null);
        }else
        {
            generalContext = new GeneralContext(list, generalContext, null);
        }

        var evaluated = EvaluateExpression(d.Corpus);

        if (generalContext != null)
        {
            if (generalContext.Father != null)
            {
                generalContext = generalContext.Father;
            }else
            {
                generalContext = null;
            }
        }

        return evaluated;
    }
    public Expression EvaluateAssignment(Assignment assign)
    {
        var identifier = assign.Term1;
        var expression = assign.Term2;

        if (generalContext.GetValue(identifier.Name) == null)
        {
            errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier.Name}");
            return null; 
        }else
        {
            for (int i = 0; i < generalContext.Identifiers.Count; i++)
            {
                if (generalContext.Identifiers[i].Name == identifier.Name)
                {   
                    generalContext.Identifiers[i].Exp = expression;
                }
            }

            var x = generalContext.GetValue(identifier.Name);
            return x;
        }
    }
    public Expression EvaluateIdentifier(Identifier identifier)
    {
        if (generalContext != null)
        {
            var value = generalContext.GetValue(identifier.Name);

            if (value == null)
            {
                errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier.Name}");
                return null; 
            }
            var x = EvaluateExpression(value);
            return x;
        }
        if (identifier.ComingFromFunction != null && functionContext != null)
        {
            var value = functionContext.GetValue(identifier.Name, identifier.ComingFromFunction);
            if (value == null)
            {
                errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier.Name}");
                return null; 
            }else
            {
                return EvaluateExpression(value);
            }
        }
        
        errorList.Add($"SEMANTIC ERROR: Reference to an undeclared variable: {identifier.Name}");
        return null;
    }
    public void Print(Expression expression)
    {
        if (expression is NumberExp number)
        {
            System.Console.WriteLine(number.Number.Text);
        }
        else if(expression is StringExp str)
        {
            System.Console.WriteLine(str.Text.Text);
        }
        else if(expression is Boolean boolean)
        {
            System.Console.WriteLine(boolean.BooleanValue.Text);
        }
    }
    List<Function> functions = new List<Function>();
    List<string> functionsNames = new List<string>();
    public void EvaluateFunctionDeclaration(Function function)
    {
        
        if (ListFunction.functionsNames.Contains(function.Name) || Analize.Reserved(function.Name) || Analize.Constants(function.Name) || Analize.ReservedFunction(function.Name))
        {
            errorList.Add($"SEMANTIC ERROR: Function {function.Name} already exists.");
        }else
        {
            ListFunction.functions.Add(function);
            ListFunction.functionsNames.Add(function.Name);
        } 
    }
    public static int stack = 0;
    public Expression EvaluateFunction(FunctionCall function)
    {
        stack++;
        if (stack > 100)
        {
            errorList.Add($"SEMANTIC ERROR: StackOverflow error.");
            return null;
        }
        if(ListFunction.functions.Count == 0)
        {
            errorList.Add($"SEMANTIC ERROR: Reference to an undeclared function: {function.Name}.");
            return null; 
        }
        int j = 0;
        for (int i = 0; i < ListFunction.functions.Count; i++)
        {
            if (ListFunction.functions[i].Name == function.Name)
            {
                j = i;
                break;
            }
            else if(i == ListFunction.functions.Count-1)
            {
                errorList.Add($"SEMANTIC ERROR: Reference to an undeclared function: {function.Name}.");
                return null; 
            }
        }
        
        var auxFunction = ListFunction.functions[j];

        if (function.Arguments.Count != auxFunction.Arguments.Count)
        {
            errorList.Add($"SEMANTIC ERROR: The number of parameters given for {function.Name} is incorrect ({function.Name} gets {function.Arguments.Count} parameters.)");
            return null;
        }
        
        List<Variable> list = new List<Variable>();
        for (int i = 0; i < auxFunction.Arguments.Count; i++)
        {
            list.Add(new Variable(auxFunction.Arguments[i].Name, "", EvaluateExpression(function.Arguments[i])));
            list[i].ComingFromFunction = auxFunction.Arguments[i].ComingFromFunction;
        }
        
        if (functionContext == null)
        {
            functionContext = new GeneralContext(list, null, function.Name);   
        }else
        {
            functionContext = new GeneralContext(list, functionContext, function.Name);
        }

        var evaluated = EvaluateExpression(auxFunction.Output);

        stack--;

        if (functionContext.Father != null)
        {
            functionContext = functionContext.Father;
        }

        return evaluated;
    }
    public static class ListFunction
    {
        public static Dictionary<string, Expression> listaSustituciones = new Dictionary<string, Expression>();
        public static Function executingFunction = new Function("null", null, null, 0);
        public static List<Function> functions = new List<Function>();
        public static List<string> functionsNames = new List<string>();
    } 
}