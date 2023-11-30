using System.Data;
public class Program
{
    static void Main(string[] args)
    {
        while(true)
        {
            Evaluator.stack = 0;
            Console.Write("> ");
            var line = Console.ReadLine();
            var syntax = new Syntax(line);
            var evaluator = new Evaluator();
            syntax.AddToken();
            var expression = syntax.CheckFullExpression();
            
            if (expression.ErrorsList.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                /*for (int i = 0; i < expression.ErrorsList.Count; i++)
                {
                    System.Console.WriteLine(expression.ErrorsList[i]);
                }*/
                System.Console.WriteLine(expression.ErrorsList[0]);
                Console.ResetColor();
            }else
            {
                evaluator.EvaluateFullExpression(expression);
            }
        }
    }
}