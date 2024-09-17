using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DSL.Lexer
{
    public class LexerStream
    {
        private readonly List<Token> _tokens = new();
        private int _position = 0;

        public Token CurrentToken => Peek(0);

        public LexerStream(string input)
        {
            FillTokenList(new Lexer(input), _tokens);
        }

        public void NextToken()
        {
            if (_position < _tokens.Count - 1)
            {
                _position++;
            }
        }

        private static void FillTokenList(Lexer lexer, List<Token> tokenList)
        {
            lexer.NextToken();
            while (lexer.CurrentToken.Type != TokenType.EOF)
            {
                tokenList.Add(lexer.CurrentToken);
               // Debug.Log($"Token añadido: {lexer.CurrentToken.Type} - {lexer.CurrentToken.Value}");
                lexer.NextToken();
            }
            tokenList.Add(new Token(TokenType.EOF, "", lexer.CurrentToken.Pos));
            Debug.Log("Token EOF añadido.");
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
        }

        // Nuevo método LookAhead
        public Token LookAhead(int offset)
        {
            int lookaheadPosition = _position + offset;
            if (lookaheadPosition < 0 || lookaheadPosition >= _tokens.Count)
            {
                return new Token(TokenType.EOF, "", new Position(0, 0));
            }
            return _tokens[lookaheadPosition];
        }
    }

}
