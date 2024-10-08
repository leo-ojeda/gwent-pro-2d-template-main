using System;

namespace DSL.Lexer
{
    public class Error : Exception
    {
        // Posición en el texto donde ocurrió el error
        public Position Position { get; }

        // Constructor que inicializa el error con un mensaje y una posición
        public Error(string message, Position position) : base(message)
        {
            Position = position;
        }

        // Constructor que inicializa el error solo con un mensaje
        public Error(string message) : base(message)
        {
        }

       
    }
}
