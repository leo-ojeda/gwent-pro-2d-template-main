using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSL.Lexer
{
    public class LexerStream : IEnumerable<Token>
    {
        private readonly List<Token> _tokens = new();
        private int _position = 0;

        // Devuelve el token actual basado en la posición.
        internal Token CurrentToken => Peek(0);

        // Constructor que inicializa la lista de tokens a partir del input.
        public LexerStream(string input)
        {
            FillTokenList(new Lexer(input), _tokens);
        }

        // Método para llenar la lista de tokens utilizando el lexer.
        private static void FillTokenList(Lexer lexer, List<Token> tokenList)
        {
            lexer.NextToken();
            while (lexer.CurrentToken.Type != TokenType.EOF)
            {
                tokenList.Add(lexer.CurrentToken);
                lexer.NextToken();
            }
        }

        // Devuelve el siguiente token sin avanzar la posición.
        public Token LookNextToken() => Peek(1);

        // Devuelve el token anterior sin cambiar la posición.
        public Token LookBackToken() => Peek(-1);

        // Permite ver un token a una cantidad específica de pasos desde la posición actual.
        public Token Peek(int step)
        {
            int newPosition = _position + step;
            if (newPosition >= 0 && newPosition < _tokens.Count)
            {
                return _tokens[newPosition];
            }

            // Usa la posición del último token o crea una nueva posición por defecto (si aplicable)
            Position lastTokenPosition = _tokens.Count > 0 ? _tokens.Last().Pos : new Position(0, 0);

            return new Token(TokenType.EOF, "", lastTokenPosition);
        }

        // Avanza la posición actual en una cantidad especificada de pasos.
        public void Advance(int steps = 1) => _position += steps;

        // Consume el token actual si coincide con uno de los tipos especificados.
        public Token Eat(params TokenType[] types)
        {
            if (Match(types))
            {
                Token current = CurrentToken;
                Advance();
                return current;
            }

            throw new Exception($"Error de sintaxis: se esperaba uno de los siguientes tipos de token: {string.Join(", ", types)} en la posición {CurrentToken.Pos}");
        }

        // Verifica si el token actual coincide con alguno de los tipos especificados.
        public bool Match(params TokenType[] types) => types.Any(t => t == CurrentToken.Type);


        // Convierte la secuencia de tokens en una cadena de texto.
        public override string ToString()
        {
            return string.Join(", ", _tokens);
        }

        // Implementación del método GetEnumerator para permitir la iteración.
        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
