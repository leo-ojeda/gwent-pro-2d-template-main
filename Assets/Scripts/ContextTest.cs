using Unity.VisualScripting;
using UnityEngine;

public class ContextTest : MonoBehaviour
{
    private Context context;

    void Start()
    {
        context = FindObjectOfType<Context>();

        if (context != null)
        {
           // Debug.Log("Context se ha inicializado correctamente.");

            // Prueba la inicialización de los diccionarios
            if (context.board != null && context.playerHands != null && context.playerFields != null && context.playerGraveyards != null && context.playerDecks != null)
            {
                //Debug.Log("Todos los diccionarios se han inicializado correctamente.");

                // Verifica si las zonas del tablero están inicializadas

            }
            else
            {
                Debug.LogError("Al menos uno de los diccionarios no se ha inicializado.");
            }
        }
        else
        {
            Debug.LogError("Context no se ha inicializado.");
        }
        //context.TriggerPlayer = "Jugador 1";
       
    }
    void Update()
    {
        //  string playerId = "Jugador 1";
        //  if (context.playerDecks.ContainsKey(playerId))
        //  {
        //      int cardCount = context.playerDecks[playerId].Count;
        //      Debug.Log($"El jugador {playerId} tiene {cardCount} cartas en su mazo.");
        //  }
        //  else
        //  {
        //      Debug.Log($"No se encontró un mazo para el jugador {playerId}.");
        //  }
//
        // Debug.Log(context.Board.Count);
        //Debug.Log(context.Deck.Count);
        //Debug.Log(context.Hand.Count);
        //Debug.Log(" vamo a calmarno" + context.DeckOfPlayer("Jugador 1").Count);




      //   string playerId = "Jugador 1";
      //   if (context.playerHands.ContainsKey(playerId))
      //   {
      //       int cardCount = context.playerHands[playerId].Count;
      //       Debug.Log($"El jugador {playerId} tiene {cardCount} cartas en su mano.");
      //     //  Debug.Log(AI.Control);
      //   }
      //   else
      //   {
      //       Debug.Log($"No se encontró un mazo para el jugador {playerId}.");
      //   }
        
        //Debug.Log("cartas en el campo" + context.board.Count);
       
       



        //  string playerId = "Jugador 2";
        //  if (context.playerGraveyards.ContainsKey(playerId))
        //  {
        //      int cardCount = context.playerGraveyards[playerId].Count;
        //      Debug.Log($"El jugador {playerId} tiene {cardCount} cartas en su cementerio.");
        //  }
        //  else
        //  {
        //      Debug.Log($"No se encontró un mazo para el jugador {playerId}.");
        //  }




      //  string playerId = "Jugador 2";
      //  if (context.playerFields.ContainsKey(playerId))
      //  {
      //      int cardCount = context.playerFields[playerId].Count;
      //      Debug.Log($"El jugador {playerId} tiene {cardCount} cartas en su campo.");
      //  }
      //  else
      //  {
      //      Debug.Log($"No se encontró un mazo para el jugador {playerId}.");
      //  }





    }

}
