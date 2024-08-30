using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DSL.Lexer;

namespace DSL.Parser
{

    public class Parser
    {
        private readonly LexerStream _lexerStream;

        public Parser(LexerStream lexerStream)
        {
            _lexerStream = lexerStream;
        }

        public List<Card> ParseCards()
        {
            List<Card> cards = new List<Card>();
            while (!IsEndOfInput())
            {
                try
                {
                    var card = ParseCard();
                    cards.Add(card);
                }
                catch (LexerError)
                {
                    SkipToNextStatement();
                }
                catch (Exception)
                {
                    SkipToNextStatement();
                }
            }
            return cards;
        }
        public bool IsEndOfInput()
        {
            return _lexerStream.CurrentToken.Type == TokenType.EOF;
        }

        public void SkipToNextStatement()
        {
            while (!IsEndOfInput() && _lexerStream.CurrentToken.Type != TokenType.Card && _lexerStream.CurrentToken.Type != TokenType.Effect)
            {
                _lexerStream.NextToken();
            }
        }


        public Card ParseCard()
        {
            try
            {
                if (_lexerStream.CurrentToken.Type != TokenType.Card)
                {
                    ThrowSyntaxError($"Expected 'card' but found '{_lexerStream.CurrentToken.Value}'");
                }

                Consume(TokenType.Card);
                Consume(TokenType.OpenBrace);

                string type = null;
                string name = null;
                string faction = null;
                int power = 0;
                List<string> range = new List<string>();
                List<EffectActivation> onActivation = new List<EffectActivation>();

                while (!Match(TokenType.CloseBrace))
                {
                    if (Match(TokenType.Type))
                    {
                        type = ParseProperty(TokenType.Type);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Name))
                    {
                        name = ParseProperty(TokenType.Name);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Faction))
                    {
                        faction = ParseProperty(TokenType.Faction);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Power))
                    {
                        power = ParseIntegerProperty(TokenType.Power);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.Range))
                    {
                        range = ParseListProperty(TokenType.Range);
                        Consume(TokenType.Comma);
                    }
                    else if (Match(TokenType.OnActivation))
                    {
                        onActivation = ParseEffectActivationList();
                    }
                    else
                    {
                        ThrowSyntaxError($"Unexpected token '{LookAhead().Value}'");
                    }
                }

                Consume(TokenType.CloseBrace);

                return new Card(name, power, type, range.ToArray(), faction, onActivation, "");
            }
            catch (LexerError ex)
            {
                Debug.LogError(ex.ToString());
                throw;
            }
        }


        private string ParseProperty(TokenType propertyType)
        {
            Consume(propertyType);
            Consume(TokenType.Colon);

            // Usar ConsumeAny para manejar diferentes tipos de valores
            Token valueToken = ConsumeAny(TokenType.String, TokenType.Identifier, TokenType.Number, TokenType.False, TokenType.True);
            if (valueToken.Type == TokenType.String || valueToken.Type == TokenType.Identifier || valueToken.Type == TokenType.Number || valueToken.Type == TokenType.False || valueToken.Type == TokenType.True)
            {
                return valueToken.Value;
            }
            else
            {
                ThrowSyntaxError($"Expected 'String', 'Identifier', or 'Number' for property '{propertyType}', but found '{valueToken.Type}'");
                return null; 
            }
        }


        private List<string> ParseListProperty(TokenType propertyType)
        {
            List<string> items = new List<string>();

            Consume(propertyType);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBracket);

            while (!Match(TokenType.CloseBracket))
            {
                Token token = ConsumeAny(TokenType.String, TokenType.Number);
                items.Add(token.Value);
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(TokenType.CloseBracket);
            return items;
        }


        private List<EffectActivation> ParseEffectActivationList()
        {
            List<EffectActivation> activations = new List<EffectActivation>();

            // Consume 'OnActivation' y ':'
            Consume(TokenType.OnActivation);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBracket);

            // Procesar el contenido dentro del corchete
            while (!Match(TokenType.CloseBracket))
            {
                if (Match(TokenType.OpenBrace))
                {
                    activations.Add(ParseEffectActivation());
                }
                else
                {
                    ThrowSyntaxError($"Se esperaba '{{' pero se encontró '{_lexerStream.CurrentToken.Value}'");
                }
            }

            Consume(TokenType.CloseBracket);
            return activations;
        }

        private EffectActivation ParseEffectActivation()
        {
            
            Consume(TokenType.OpenBrace);

            Effect effect = null;
            Selector selector = null;
            PostAction postAction = null;

            while (!Match(TokenType.CloseBrace))
            {
                // Verificar si se encuentra un CloseBracket prematuramente y terminar el bucle
              //  if (Match(TokenType.CloseBracket))
              //  {
              //      // Terminar el bucle y retornar el estado actual
              //      return new EffectActivation(effect, selector, postAction);
              //  }

                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.effect:
                        effect = ParseCardEffect(); // Usa el nuevo método para analizar efectos dentro de una carta
                        break;
                    case TokenType.Selector:
                        selector = ParseSelector();
                        break;
                    case TokenType.PostAction:
                        postAction = ParsePostAction();
                        break;
                    case TokenType.OpenBrace:
                        effect = ParseSimpleEffect();
                        break;
                    default:
                        ThrowSyntaxError($"Token inesperado el pepe '{_lexerStream.CurrentToken.Value}' en EffectActivation");
                        break;
                }

                //_lexerStream.Advance(); // Avanza al siguiente token después de procesar uno.
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }        
                Consume(TokenType.CloseBrace);
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }

            // Retorna el objeto EffectActivation con los valores finales
            return new EffectActivation(effect, selector, postAction);
        }


        public List<Effect> ParseEffects()
        {
            List<Effect> effects = new List<Effect>();

            Consume(TokenType.Effect);
            Consume(TokenType.OpenBrace);

            while (!Match(TokenType.CloseBrace))
            {
                try
                {
                    if (Match(TokenType.Name))
                    {
                        var effect = ParseEffect();
                        effects.Add(effect);
                    }
                    else
                    {
                        ThrowSyntaxError("Expected 'Effect' keyword", TokenType.Effect);
                    }
                }
                catch (Exception ex)
                {
                    
                    ThrowSyntaxError($"Error parsing effect: {ex.Message}");
                }
            }

            Consume(TokenType.CloseBrace);
            return effects;
        }
        public Effect ParseEffect()
        {          

            string effectName = null;
            List<Parameter> parameters = new List<Parameter>();
            Action<List<Card>, Context> action = null;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Name:
                        effectName = ParseProperty(TokenType.Name);
                        Consume(TokenType.Comma);
                        break;
                    case TokenType.Params:
                        parameters = ParseParameters();
                        break;
                    case TokenType.Action:
                        action = ParseAction();
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect");
                        break;
                }
            }

            if (effectName == null)
            {
                ThrowSyntaxError("Effect name is required", TokenType.Name);
            }

            return new Effect(effectName, parameters, action);
        }


        private List<Parameter> ParseParameters()
        {
            List<Parameter> parameters = new List<Parameter>();

            Consume(TokenType.Params);
            Consume(TokenType.Colon);  
            Consume(TokenType.OpenBrace); 

            while (!Match(TokenType.CloseBrace))
            {


                string paramName = Consume(TokenType.Amount).Value;
                Consume(TokenType.Colon);

                ParamType paramType;
                if (Match(TokenType.Number))
                {
                    paramType = ParamType.Number;
                    Consume(TokenType.Number);
                }
                else if (Match(TokenType.String))
                {
                    paramType = ParamType.String;
                    Consume(TokenType.String);
                }
                else if (Match(TokenType.Bool))
                {
                    paramType = ParamType.Bool;
                    Consume(TokenType.Bool);
                }
                else
                {
                    ThrowSyntaxError($"Tipo de parámetro inesperado después de '{paramName}'");
                    return null; // Para evitar más errores en tiempo de compilación, aunque normalmente ThrowSyntaxError lanzará una excepción
                }

                // Crear el parámetro y añadirlo a la lista
                parameters.Add(new Parameter(paramType));

            
            }          
                Consume(TokenType.CloseBrace);
                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            
            return parameters;
        }

       
        private Action<List<Card>, Context> ParseAction()
        {
            Consume(TokenType.Action);
            Consume(TokenType.Colon); 
            Consume(TokenType.OpenParen); 

            // Leer y verificar los parámetros 'targets' y 'context'
            var targetsParam = Consume(TokenType.Identifier).Value; 
            Consume(TokenType.Comma);
            var contextParam = Consume(TokenType.Identifier).Value; 
            Consume(TokenType.CloseParen); 
            Consume(TokenType.Arrow); 
            Consume(TokenType.OpenBrace);

            // Inicializar listas y diccionarios para almacenar acciones y variables locales
            List<Action<List<Card>, Context>> actions = new List<Action<List<Card>, Context>>();
            Dictionary<string, object> localVariables = new Dictionary<string, object>();

            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.For))
                {
                    // Manejar el bucle 'for'
                    ParseForLoop(actions, localVariables);
                }
                else if (Match(TokenType.Identifier))
                {
                    // Procesar identificadores fuera de bucles
                    ParseIdentifier(actions, localVariables);
                }
                else
                {
                    ThrowSyntaxError($"Token inesperado '{_lexerStream.CurrentToken.Value}' en el bloque de acción.");
                }
            }

            Consume(TokenType.CloseBrace);

            // Retornar la acción final que ejecutará todas las acciones parseadas
            return (cards, ctx) =>
            {
                foreach (var action in actions)
                {
                    action(cards, ctx); // Ejecutar cada acción con las cartas y el contexto proporcionados
                }
            };
        }

        private void ParseForLoop(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables)
        {
            Consume(TokenType.For);

            var targetVarToken = Consume(TokenType.Identifier);
            var targetVar = targetVarToken.Value; 

            if (!Match(TokenType.In))
            {
                ThrowSyntaxError("Expected 'in' after for-loop variable", TokenType.In);
            }
            Consume(TokenType.In);

            var listNameToken = Consume(TokenType.Identifier);
            var listName = listNameToken.Value; // Nombre de la lista, ej., "targets"

            if (!Match(TokenType.OpenBrace))
            {
                ThrowSyntaxError("Expected '{' to open for-loop body", TokenType.OpenBrace);
            }
            Consume(TokenType.OpenBrace); // Consumir el token '{' que inicia el cuerpo del bucle

            List<Action<Card, Context>> loopActions = new List<Action<Card, Context>>();

            // Procesar el cuerpo del bucle 'for'
            while (!Match(TokenType.CloseBrace))
            {
                if (Match(TokenType.Identifier))
                {
                    ParseForLoopIdentifier(loopActions, localVariables, actions);
                }
                else
                {
                    ThrowSyntaxError("Unexpected token in for-loop body");
                }
            }
            Consume(TokenType.CloseBrace); 

            if (Match(TokenType.SemiColon))
            {
                Consume(TokenType.SemiColon); 
            }
            else
            {
                ThrowSyntaxError("Expected ';' after for-loop body", TokenType.SemiColon);
            }

            // Agregar las acciones del bucle 'for' a la lista principal de acciones
            actions.Add((cards, ctx) =>
            {
                foreach (var card in cards)
                {
                    foreach (var loopAction in loopActions)
                    {
                        loopAction(card, ctx);
                    }
                }
            });
        }

        private void ParseForLoopIdentifier(List<Action<Card, Context>> loopActions, Dictionary<string, object> localVariables, List<Action<List<Card>, Context>> actions)
        {
            var variableToken = LookAhead(); 
            var variable = variableToken.Value;

            if (Match(TokenType.Identifier))
            {

                ParseAssignment(variable, localVariables,actions);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token in for-loop identifier handling: '{variableToken.Value}'");
            }
        }
   
        private void ParseWhileLoop(List<Action<List<Card>, Context>> actions)
        {
            Consume(TokenType.While);
            Consume(TokenType.OpenParen);

            var loopVarToken = Consume(TokenType.Identifier);
            var loopVar = loopVarToken.Value; // Variable de control del bucle, ej., "i"

            if (!Match(TokenType.Increment))
            {
                ThrowSyntaxError("Expected '++' for while-loop increment", TokenType.Increment);
            }
            Consume(TokenType.Increment);

            if (!Match(TokenType.Less))
            {
                ThrowSyntaxError("Expected '<' after increment in while-loop condition", TokenType.Less);
            }
            Consume(TokenType.Less);

            var loopConditionToken = Consume(TokenType.Amount);
            var loopConditionValue = loopConditionToken.Value; // Valor de condición del bucle, ej., "Amount"

            Consume(TokenType.CloseParen); 

            List<Action<Card, Context>> whileActions = new List<Action<Card, Context>>();

            // Procesar el cuerpo del bucle 'while'
            if (Match(TokenType.Identifier))
            {
                var targetToken = Consume(TokenType.Identifier);
                var target = targetToken.Value; // Variable objetivo, ej., "target"

                if (Match(TokenType.Dot))
                {
                    Consume(TokenType.Dot);
                    var propertyToken = Consume(TokenType.Power);
                    var property = propertyToken.Value; // Propiedad, ej., "Power"

                    if (Match(TokenType.Minus))
                    {
                        Consume(TokenType.Minus);
                        Consume(TokenType.Equals);
                        var valueToken = Consume(TokenType.Number);
                        int value;

                        if (!int.TryParse(valueToken.Value, out value))
                        {
                            ThrowSyntaxError("Invalid number format for decrement operation in while-loop", TokenType.Number);
                        }
                        
                        whileActions.Add((targetCard, ctx) => targetCard.Power -= value);
                    }
                    else
                    {
                        ThrowSyntaxError($"Unexpected token after property '{propertyToken.Value}' in while-loop");
                    }
                }
                else
                {
                    ThrowSyntaxError($"Expected '.' after identifier '{targetToken.Value}' in while-loop");
                }
            }

            // Agregar las acciones del bucle 'while' a la lista principal
            actions.Add((cards, ctx) =>
            {
                foreach (var target in cards)
                {
                    int currentValue = 0; // Inicializar la variable de control
                    while (currentValue < 1) // Usar el valor de 'loopConditionValue' directamente
                    {
                        currentValue++; // Incrementar después de la comparación
                        foreach (var action in whileActions)
                        {
                            action(target, ctx);
                        }
                    }
                }
            });
        }

        private void ParseIdentifier(List<Action<List<Card>, Context>> actions, Dictionary<string, object> localVariables)
        {
            var contextObjectToken = LookAhead();
            string contextObject = contextObjectToken.Value;

           
            if (Match(TokenType.Identifier))
            {
                ParseAssignment(contextObject, localVariables,actions);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token el after identifier '{contextObject}'");
            }
        }

        private void ParseAssignment(string variableName, Dictionary<string, object> localVariables,List<Action<List<Card>, Context>> actions)
        {

            while (LookAhead().Type != TokenType.CloseBrace)
            {
                while (!Match(TokenType.SemiColon))
                {

                    var identifier = LookAhead();
                    variableName = identifier.Value;

                    if (identifier.Value == "context" || identifier.Value == "target")
                    {
                        ConsumePropertyOrMethod();

                        Consume(TokenType.Dot);


                        if (identifier.Value == "context")
                        {
                            // Consumir propiedades y métodos específicos del contexto

                            var propertyOrMethod = ConsumePropertyOrMethod();

                            switch (propertyOrMethod.Type)
                            {
                                // Propiedades de context que requieren un parámetro
                                case TokenType.DeckOfPlayer:
                                case TokenType.HandOfPlayer:
                                case TokenType.FieldOfPlayer:
                                case TokenType.GraveyardOfPlayer:
                                    Consume(TokenType.OpenParen);
                                    Consume(TokenType.Identifier);
                                    Consume(TokenType.CloseParen);
                                    break;

                                // Propiedades de context que pueden ser seguidas por un método
                                case TokenType.Deck:
                                case TokenType.Hand:
                                case TokenType.Field:
                                case TokenType.Graveyard:
                                case TokenType.Board:
                                    if (Match(TokenType.Dot))
                                    {
                                        Consume(TokenType.Dot);
                                        var method = ConsumeMethod();

                                        switch (method.Type)
                                        {
                                            case TokenType.Pop:
                                            case TokenType.Shuffle:
                                                Consume(TokenType.OpenParen);
                                                Consume(TokenType.CloseParen);
                                                break;

                                            case TokenType.Push:
                                            case TokenType.SendBottom:
                                            case TokenType.Remove:
                                            case TokenType.Find:
                                            case TokenType.Add:
                                                Consume(TokenType.OpenParen);
                                                Consume(TokenType.Identifier); 
                                                Consume(TokenType.CloseParen);
                                                break;

                                            default:
                                                ThrowSyntaxError($"Unexpected context method '{method.Value}'");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        ThrowSyntaxError($"Expected method after context property '{propertyOrMethod.Value}'");
                                    }
                                    break;

                                default:
                                    ThrowSyntaxError($"Unexpected context property '{propertyOrMethod.Value}'");
                                    break;
                            }
                        }
                        else if (identifier.Value == "target")
                        {
                            var targetProperty = ConsumeTargetProperty();

                            switch (targetProperty.Type)
                            {
                                case TokenType.Name:
                                case TokenType.Power:
                                //case TokenType.Type:
                                //case TokenType.Range:
                                case TokenType.Faction:
                               // case TokenType.OnActivation:
                                case TokenType.Owner:
                                    // Propiedad de target
                                    break;

                                default:
                                    ThrowSyntaxError($"Unexpected target property '{targetProperty.Value}'");
                                    break;
                            }
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected '.' after identifier '{identifier.Value}'");
                        }
                    }
                    else if (identifier.Type == TokenType.Identifier)
                    {
                        string variable = Consume(TokenType.Identifier).Value;
                        if (Match(TokenType.Equals))
                        {
                            Consume(TokenType.Equals);


                            if (Match(TokenType.Number))
                            {
                                var valueToken = Consume(TokenType.Number);
                                int value;

                                if (!int.TryParse(valueToken.Value, out value))
                                {
                                    ThrowSyntaxError("Invalid number format for assignment", TokenType.Number);
                                }
                                Consume(TokenType.SemiColon);
                                localVariables[variable] = value; // Almacenar la variable localmente

                            }

                        }
                        if (Match(TokenType.Dot))
                        {
                            Consume(TokenType.Dot);
                            var A = LookAhead();
                            ConsumePropertyOrMethod();
                            if (IsMethodToken(A.Type))
                            {
                                switch (A.Type)
                                {
                                    case TokenType.Pop:
                                    case TokenType.Shuffle:
                                        Consume(TokenType.OpenParen);
                                        Consume(TokenType.CloseParen);
                                        break;

                                    case TokenType.Push:
                                    case TokenType.SendBottom:
                                    case TokenType.Remove:
                                    case TokenType.Find:
                                    case TokenType.Add:
                                        Consume(TokenType.OpenParen);
                                        Consume(TokenType.Identifier);
                                        Consume(TokenType.CloseParen);
                                        break;

                                    default:
                                        ThrowSyntaxError($"Unexpected context method '{A.Value}'");
                                        break;
                                }
                            }
                        }

                    }
                    else if (identifier.Type == TokenType.While)
                    {                       
                        {
                            ParseWhileLoop(actions);
                        }
                    }


                    else
                    {
                        ThrowSyntaxError($"Expected 'context' or 'target', but found '{identifier.Value}'");
                    }

                }
              
                Consume(TokenType.SemiColon);
            }
        }


        private Token ConsumePropertyOrMethod()
        {
            // Consumo un token que es una propiedad o método específico de context
            var nextToken = LookAhead();
            if (IsPropertyOrMethodToken(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else if (Match(TokenType.Identifier))
            {
                return Consume(TokenType.Identifier);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a context property or method.");
                return null; // Nunca se alcanza, solo para cumplir con el flujo de control
            }
        }

        private Token ConsumeMethod()
        {
            
            var nextToken = LookAhead();
            if (IsMethodToken(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a method after context property.");
                return null; // Nunca se alcanza, solo para cumplir con el flujo de control
            }
        }

        private Token ConsumeTargetProperty()
        {
            
            var nextToken = LookAhead();
            if (IsTargetPropertyToken(nextToken.Type))
            {
                return Consume(nextToken.Type);
            }
            else
            {
                ThrowSyntaxError($"Unexpected token '{nextToken.Value}'. Expected a target property.");
                return null; 
            }
        }

        private bool IsPropertyOrMethodToken(TokenType type)
        {
            // Verifica si el tipo de token es una propiedad o método de context
            return type == TokenType.DeckOfPlayer ||
                   type == TokenType.HandOfPlayer ||
                   type == TokenType.FieldOfPlayer ||
                   type == TokenType.GraveyardOfPlayer ||
                   type == TokenType.Deck ||
                   type == TokenType.Hand ||
                   type == TokenType.Field ||
                   type == TokenType.Graveyard ||
                   type == TokenType.Board ||
                   type == TokenType.Push;


        }

        private bool IsMethodToken(TokenType type)
        {
            // Verifica si el tipo de token es un método de context
            return type == TokenType.Pop ||
                   type == TokenType.Shuffle ||
                   type == TokenType.Push ||
                   type == TokenType.SendBottom ||
                   type == TokenType.Remove ||
                   type == TokenType.Find ||
                   type == TokenType.Add;
        }

        private bool IsTargetPropertyToken(TokenType type)
        {
            // Verifica si el tipo de token es una propiedad de target
            return type == TokenType.Name ||
                   type == TokenType.Power ||
                   type == TokenType.Type ||
                   type == TokenType.Range ||
                   type == TokenType.Faction ||
                   type == TokenType.Owner;
        }



        private Selector ParseSelector()
        {
            Consume(TokenType.Selector);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string source = null;
            bool single = false;
            string predicate = null;

            bool hasSource = false;
            bool hasSingle = false;
            bool hasPredicate = false;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Source:
                        if (hasSource) ThrowSyntaxError("Duplicate 'Source' property in Selector");
                        source = ParseProperty(TokenType.Source);
                        hasSource = true;
                        break;
                    case TokenType.Single:
                        if (hasSingle) ThrowSyntaxError("Duplicate 'Single' property in Selector");
                        string singleValue = ParseProperty(TokenType.Single);

                        if (bool.TryParse(singleValue, out bool parsedSingle))
                        {
                            single = parsedSingle;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Bool' for property 'Single', but found '{singleValue}'");
                        }
                        hasSingle = true;
                        break;
                    case TokenType.Predicate:
                        if (hasPredicate) ThrowSyntaxError("Duplicate 'Predicate' property in Selector");
                        predicate = ParsePredicate();
                        hasPredicate = true;
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Selector");
                        break;
                }
               
                if (!Match(TokenType.CloseBrace) && !Match(TokenType.Comma))
                {
                    ThrowSyntaxError($"Expected a comma or closing brace in Selector, but found '{_lexerStream.CurrentToken.Value}'");
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }
            if (Match(TokenType.CloseBrace))
            {
                Consume(TokenType.CloseBrace);
            }


            return new Selector(source, single, predicate);
        }




        private PostAction ParsePostAction()
        {
            Consume(TokenType.PostAction);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenBrace);

            string type = null;
            Selector selector = null;
            // por arreglar
            int? amount = null;

            bool hasType = false;
            bool hasSelector = false;

            while (!Match(TokenType.CloseBrace))
            {
                switch (_lexerStream.CurrentToken.Type)
                {
                    case TokenType.Type:
                        if (hasType) ThrowSyntaxError("Duplicate 'Type' property in PostAction");
                        type = ParseProperty(TokenType.Type);
                        hasType = true;
                        break;
                    case TokenType.Amount:
                        string amountValue = ParseProperty(TokenType.Amount);
                        if (int.TryParse(amountValue, out int parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                        }
                        break;
                    case TokenType.Selector:
                        if (hasSelector) ThrowSyntaxError("Duplicate 'Selector' property in PostAction");
                        selector = ParseSelector();
                        hasSelector = true;
                        break;
                    default:
                        ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in PostAction");
                        break;
                }

                
                if (!Match(TokenType.CloseBrace) && !Match(TokenType.Comma))
                {
                    ThrowSyntaxError($"Expected a comma or closing brace in PostAction, but found '{_lexerStream.CurrentToken.Value}'");
                }

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }
            }          
                Consume(TokenType.CloseBrace);

                if (Match(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                }           

            return new PostAction(type, selector);
        }


        private string ParsePredicate() //Debe mejorarse
        {
            
            Consume(TokenType.Predicate);
            Consume(TokenType.Colon);
            Consume(TokenType.OpenParen); 

            var predicate = new StringBuilder();

            // Consumimos el token Unit (ej. 'unit')
            var paramToken = Consume(TokenType.Unit); // Esto debería ser 'unit' o cualquier otro parámetro específico
            predicate.Append(paramToken.Value);

            Consume(TokenType.CloseParen); 
          
            if (Match(TokenType.Arrow)) 
            {
                Consume(TokenType.Arrow);
                predicate.Append(" => ");

                // Consumimos el cuerpo de la expresión lambda
                while (!Match(TokenType.CloseBrace))
                {
                    var token = Consume(LookAhead().Type);

                    // Maneja identificadores específicos y operadores
                    if (token.Type == TokenType.Unit || token.Type == TokenType.Identifier ||
                        token.Type == TokenType.String || token.Type == TokenType.Number ||
                        token.Type == TokenType.Faction || token.Type == TokenType.Power || token.Type == TokenType.False || token.Type == TokenType.True) // <-- Aseguramos que Faction es válido
                    {
                        predicate.Append(token.Value);
                    }
                    else if (IsOperator(token.Type))
                    {
                        predicate.Append($" {token.Value} ");
                    }
                    else if (token.Type == TokenType.EqualsEquals)
                    {
                        predicate.Append(" == ");
                    }
                    else if (token.Type == TokenType.And)
                    {
                        predicate.Append(" && ");
                    }
                    else if (token.Type == TokenType.Or)
                    {
                        predicate.Append(" || ");
                    }
                    else
                    {
                        throw new LexerError($"Unexpected token '{token.Value}' at position {token.Pos}.");
                    }
                }
            }
            else
            {
                throw new LexerError("Expected '=>' after the parameter list in the predicate.");
            }

            return predicate.ToString().Trim(); // Retorna el predicado como una cadena de texto
        }


        private bool IsOperator(TokenType type)
        {
            return type == TokenType.EqualsEquals || type == TokenType.NotEqual ||
                   type == TokenType.Less || type == TokenType.Greater ||
                   type == TokenType.LessOrEqual || type == TokenType.GreaterOrEqual ||
                   type == TokenType.And || type == TokenType.Or ||
                   type == TokenType.Plus || type == TokenType.Minus ||
                   type == TokenType.Multiply || type == TokenType.Slash ||
                   type == TokenType.Modulus || type == TokenType.Dot;
        }

        private Token Consume(TokenType type)
        {
            if (LookAhead().Type != type)
            {
                ThrowSyntaxError($"Expected token of type '{type}', but found '{LookAhead().Type}'", type);
            }
            Token token = _lexerStream.CurrentToken;
            _lexerStream.Advance();
            //LogTokenConsumed(token);
            return token;
        }

        public bool Match(TokenType type)
        {
            return LookAhead().Type == type;
        }

        private Token LookAhead()
        {
            return _lexerStream.CurrentToken;
        }

        private void LogCurrentToken(string context)
        {
            Debug.Log($"Current Token in {context}: {_lexerStream.CurrentToken}");
        }

      //  private void LogTokenConsumed(Token token)
      //  {
      //      Debug.Log($"Consumed Token: {token}");
      //  }

        private void ThrowSyntaxError(string message, TokenType expectedType = TokenType.None)
        {
            string errorMessage = $"Syntax Error: {message}. Found '{_lexerStream.CurrentToken.Value}' at position {_lexerStream.CurrentToken.Pos}.";
            Debug.LogError(errorMessage);
            throw new LexerError(errorMessage);
        }

        private int ParseIntegerProperty(TokenType propertyType)
        {
            Consume(propertyType);
            Consume(TokenType.Colon);

            // Usar ConsumeAny para manejar números
            Token valueToken = ConsumeAny(TokenType.Number);
            if (valueToken.Type == TokenType.Number)
            {
                if (!int.TryParse(valueToken.Value, out int result))
                {
                    ThrowSyntaxError($"Invalid integer value '{valueToken.Value}' for property '{propertyType}'.");
                }
                return result;
            }
            else
            {
                ThrowSyntaxError($"Expected 'Number' for property '{propertyType}', but found '{valueToken.Type}'");
                return 0; // Este retorno nunca se alcanzará, es solo para evitar errores de compilación
            }
        }

        private Token ConsumeAny(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Match(type))
                {
                    Token token = _lexerStream.CurrentToken;
                    _lexerStream.Advance();
                   // LogTokenConsumed(token);
                    return token;
                }
            }

            ThrowSyntaxError($"Expected one of the following types: {string.Join(", ", types)} but found '{LookAhead().Type}'");
            return null; 
        }
        private Effect ParseCardEffect()
        {
            Consume(TokenType.effect);
            Consume(TokenType.Colon);
            if (Match(TokenType.OpenBrace))
            {

                Consume(TokenType.OpenBrace);

                string effectName = null;
                int? amount = null;
                List<Parameter> parameters = new List<Parameter>();
                Action<List<Card>, Context> action = null;

                while (!Match(TokenType.CloseBrace))
                {
                    switch (_lexerStream.CurrentToken.Type)
                    {
                        case TokenType.Name:
                            effectName = ParseProperty(TokenType.Name);
                            Consume(TokenType.Comma);
                            break;
                        case TokenType.String:
                            Token valueToken = ConsumeAny(TokenType.String);
                            effectName = valueToken.Value;
                            Consume(TokenType.Comma);
                            break;
                        case TokenType.Amount:
                            string amountValue = ParseProperty(TokenType.Amount);
                            Consume(TokenType.Comma);
                            if (int.TryParse(amountValue, out int parsedAmount))
                            {
                                amount = parsedAmount;
                            }
                            else
                            {
                                ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                            }
                            break;
                        default:
                            ThrowSyntaxError($"Unexpected token '{_lexerStream.CurrentToken.Value}' in Effect within a card");
                            break;
                    }

                   
                }
                if (Match(TokenType.CloseBrace))
                {
                    Consume(TokenType.CloseBrace);
                }

                
                return new Effect(effectName, parameters, action); 
            }
            else
            {
                return ParseSimpleEffect();
            }

        }
        private Effect ParseSimpleEffect()
        {

            if (Match(TokenType.OpenBrace))
            {
                Consume(TokenType.OpenBrace); 
            }

            if (Match(TokenType.Effect))
            {
                string effectName = ParseProperty(TokenType.Effect);
                Consume(TokenType.Comma);
                
                if (Match(TokenType.CloseBrace))
                {
                    return new Effect(effectName, new List<Parameter>(), null); // action es null porque no hay selector ni postAction
                }
                else
                {
                    ThrowSyntaxError("Expected '}' after simple effect name.");
                }
            }
            else if (Match(TokenType.String))
            {
                Token valueToken = ConsumeAny(TokenType.String);
                string effectName = "";
                if (valueToken.Type == TokenType.String)
                {
                    effectName = valueToken.Value;
                    if (Match(TokenType.Comma))
                    {
                        Consume(TokenType.Comma);
                    }
                    if (!Match(TokenType.Amount))
                    {
                        return new Effect(effectName, new List<Parameter>(), null);
                    }
                    else if (Match(TokenType.Amount))
                    {

                        int? amount = null;
                        List<Parameter> parameters = new List<Parameter>();
                        Action<List<Card>, Context> action = null;
                        string amountValue = ParseProperty(TokenType.Amount);
                        Consume(TokenType.Comma);
                        if (int.TryParse(amountValue, out int parsedAmount))
                        {
                            amount = parsedAmount;
                        }
                        else
                        {
                            ThrowSyntaxError($"Expected 'Number' for property 'Amount', but found '{amountValue}'");
                        }
                        return new Effect(effectName, parameters, action);

                    }
                }
                else
                {
                    ThrowSyntaxError($"Expected 'Value' for property '{TokenType.String}', but found '{valueToken.Type}'");
                    return null; 
                }
            }

            return null; // Retorno por defecto si hay un error
        }

    }
}
