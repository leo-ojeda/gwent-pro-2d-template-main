using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace DSL.Lexer
{
    public class LexerStream : IEnumerable<Token>
{
    private readonly List<Token> _tokens = new();
    private int _position = 0;

    internal Token CurrentToken => Peek(0);

    public LexerStream(string input)
    {
        FillTokenList(new Lexer(input), _tokens);
    }

    private static void FillTokenList(Lexer lexer, List<Token> tokenList)
    {
        lexer.NextToken();
        while (lexer.CurrentToken.Type != TokenType.EOF)
        {
            tokenList.Add(lexer.CurrentToken);
            lexer.NextToken();
        }
        tokenList.Add(new Token(TokenType.EOF, "", lexer.CurrentToken.Pos)); // Añadir token EOF explícitamente
    }

    public Token LookNextToken()
    {
        return Peek(1);
    }

    public Token LookBackToken()
    {
        return Peek(-1);
    }

    public Token Peek(int step)
    {
        int newPosition = _position + step;
        if (newPosition < 0 || newPosition >= _tokens.Count)
        {
            return new Token(TokenType.EOF, "", new Position(0, 0));
        }
        return _tokens[newPosition];
    }

    public void Advance(int steps = 1)
    {
        _position = Math.Clamp(_position + steps, 0, _tokens.Count - 1);
        Debug.Log($"Advanced to position {_position}, Current Token: {CurrentToken}");
    }

    public Token Eat(params TokenType[] types)
    {
        if (Match(types))
        {
            Token current = CurrentToken;
            Debug.Log($"Consuming Token: {current}");
            Advance();
            return current;
        }

        throw new Exception($"Error de sintaxis: se esperaba uno de los siguientes tipos de token: {string.Join(", ", types)} en la posición {CurrentToken.Pos}");
    }

    public bool Match(params TokenType[] types)
    {
        return types.Any(t => t == CurrentToken.Type);
    }

    public override string ToString()
    {
        return string.Join(", ", _tokens);
    }

    public IEnumerator<Token> GetEnumerator()
    {
        return _tokens.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void LogCurrentToken(string methodName)
    {
        Debug.Log($"{methodName} - Current Token: {CurrentToken}, Position: {_position}");
    }
}

}
