namespace DSL.Lexer
{
    public enum TokenType
    {
        // Tokens especiales
        SOF, // Start of File
        EOF, // End of File

        // Delimitadores
        OpenBrace,      // {
        CloseBrace,     // }
        OpenBracket,    // [
        CloseBracket,   // ]
        OpenParen,      // (
        CloseParen,     // )
        Colon,          // :
        Comma,          // ,
        Dot,            // .
        SemiColon,
        // Operadores aritméticos
        Plus,           // +
        Minus,          // -
        Multiply,       // *
        Slash,          // /
        Modulus,        // %
        Increment,      // ++

        // Operadores de comparación
        Equals,             // =
        EqualsEquals,       // ==
        NotEqual,           // !=
        Less,               // <
        Greater,            // >
        LessOrEqual,        // <=
        GreaterOrEqual,     // >=

        // Operadores lógicos
        And,            // &&
        Or,             // ||
        Exclamation,    // !

        // Operadores de asignación y otros
        Arrow,          // =>
        At,             // @
        DoubleAt,       // @@

        // Tipos de datos
        String,
        Number,
        Bool,
        False,
        True,
        None,

        // Identificadores
        Identifier,
        EndOfInput,
        Variable,

        // Palabras clave del DSL
        Card,
        Effect,
        effect,
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
        Unit,

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
        Add,
        
    }
}
