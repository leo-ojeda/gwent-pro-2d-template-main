using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ExpressionEvaluator
{
    public string EvaluateExpression(string expression)
    {
        try
        {
            // Reemplazar las potenciaciones dentro de la expresión
            expression = ReplaceExponentiation(expression);

            // Evaluar la expresión aritmética con paréntesis
            double result = EvaluateWithParentheses(expression);

            return result.ToString();
        }
        catch (Exception ex)
        {
            // Manejar excepciones aquí
            Console.WriteLine($"Error al evaluar la expresión: {expression} - {ex.Message}");
            return "0"; // Valor por defecto en caso de error
        }
    }

    private string ReplaceExponentiation(string expression)
    {
        var regex = new Regex(@"(\d+(\.\d+)?)\s*\^\s*(\d+(\.\d+)?)", RegexOptions.IgnoreCase);

        return regex.Replace(expression, match =>
        {
            double baseNumber = double.Parse(match.Groups[1].Value);
            double exponent = double.Parse(match.Groups[3].Value);
            double powerResult = Math.Pow(baseNumber, exponent);
            return powerResult.ToString();
        });
    }

    private double EvaluateWithParentheses(string expression)
    {
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();

        var tokens = Tokenize(expression);
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                outputQueue.Enqueue(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop(); // Eliminar el paréntesis de apertura
                }
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count > 0 && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
        }

        while (operatorStack.Count > 0)
        {
            outputQueue.Enqueue(operatorStack.Pop());
        }

        return EvaluateRPN(outputQueue);
    }

    private IEnumerable<string> Tokenize(string expression)
    {
        var regex = new Regex(@"(\d+(\.\d+)?)|([\+\-\*/\^\(\)])", RegexOptions.IgnoreCase);
        var matches = regex.Matches(expression);

        foreach (Match match in matches)
        {
            yield return match.Value;
        }
    }

    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/" || token == "^";
    }

    private int GetPrecedence(string op)
    {
        if (op == "+" || op == "-") return 1;
        if (op == "*" || op == "/") return 2;
        if (op == "^") return 3;
        return 0;
    }

    private double EvaluateRPN(IEnumerable<string> tokens)
    {
        var stack = new Stack<double>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                double right = stack.Pop();
                double left = stack.Pop();
                double result = token switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    "^" => Math.Pow(left, right),
                    _ => throw new InvalidOperationException("Operador no soportado")
                };
                stack.Push(result);
            }
        }

        return stack.Pop();
    }
}