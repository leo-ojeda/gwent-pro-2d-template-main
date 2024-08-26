using System;

namespace DSL.Lexer
{
      public class LexerError : Exception
    {
        // Posición en el texto donde ocurrió el error
        public Position Position { get; }

        // Constructor que inicializa el error con un mensaje y una posición
        public LexerError(string message, Position position) : base(message)
        {
            Position = position;
        }

        // Constructor que inicializa el error solo con un mensaje
        public LexerError(string message) : base(message)
        {
        }

        // Sobrescribe el método ToString para dar información detallada sobre el error
        public override string ToString()
        {
            if (Position != null)
            {
                return $"LexerError: {Message} en {Position}";
            }
            else
            {
                return $"LexerError: {Message}";
            }
        }
    }
}
