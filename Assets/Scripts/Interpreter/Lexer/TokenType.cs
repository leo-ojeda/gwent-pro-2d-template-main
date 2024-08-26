namespace DSL.Lexer
{
    public enum TokenType
    {
        // Tokens especiales
        SOF, // Start of File
        EOF, // End of File

        // Delimitadores
        OpenBrace,     // {
        CloseBrace,   // }
        OpenBracket,    // [
        CloseBracket,  // ]
        OpenParen,      // (
        CloseParen,    // )
        Colon,                // :
        Comma,                // ,
        Equals,               // =
        Plus,                 // +
        Minus,                // -
        Multiply,             // *
        Slash,                // /
        Modulus,              // %
        Less,                 // <
        Greater,              // >
        LessOrEqual,          // <=
        GreaterOrEqual,       // >=
        Exclamation,                  // !
        NotEqual,             // !=

        // Tipos de datos
        String,
        Number,
        Bool,
        False,
        True,

        // Identificadores
        Identifier,
        EndOfInput,

        // Palabras clave del DSL
        Card,
        Effect,
        Type,
        Name,
        Faction,
        Power,
        Range,
        OnActivation,
        Selector,
        Params,
        Action,
        Amount,
        Source,
        Single,
        Predicate,
        NumberType,
        For,
        In,
        While,
        Return,
        Owner,
        PostAction,
        // Contexto del juego
        TriggerPlayer,
        HandOfPlayer,
        GraveyardOfPlayer,
        FieldOfPlayer,
        DeckOfPlayer,
        Hand,
        Field,
        Graveyard,
        Deck,
        Board,
        Push,
        SendBottom,
        Shuffle,
        Pop,
        Remove,
        Find,


       
    }
}
