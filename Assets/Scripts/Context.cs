using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Context : MonoBehaviour
{

    public GameObject CardPrefab; // Asigna este prefab desde el Inspector
    public GameObject Hands; // Asigna el objeto de la mano desde el Inspector
    public string TriggerPlayer { get; set; }
    public List<Card> board { get; set; }
    public Dictionary<string, List<Card>> playerHands { get; set; }
    public Dictionary<string, List<Card>> playerFields { get; set; }
    public Dictionary<string, List<Card>> playerGraveyards { get; set; }
    public Dictionary<string, List<Card>> playerDecks { get; set; }

    public Context()
    {

    }

    void Awake()
    {
        board = new List<Card>();
        playerHands = new Dictionary<string, List<Card>>();
        playerFields = new Dictionary<string, List<Card>>();
        playerGraveyards = new Dictionary<string, List<Card>>();
        playerDecks = new Dictionary<string, List<Card>>();

        // Inicializa las listas para todos los jugadores relevantes
        InitializePlayer("Jugador 1");
        InitializePlayer("Jugador 2");
    }

    void InitializePlayer(string player)
    {
        playerHands[player] = new List<Card>();
        playerFields[player] = new List<Card>();
        playerGraveyards[player] = new List<Card>();
        playerDecks[player] = new List<Card>();
    }



    public List<Card> HandOfPlayer(string player) => playerHands.ContainsKey(player) ? playerHands[player] : new List<Card>();
    public List<Card> FieldOfPlayer(string player) => playerFields.ContainsKey(player) ? playerFields[player] : new List<Card>();
    public List<Card> GraveyardOfPlayer(string player) => playerGraveyards.ContainsKey(player) ? playerGraveyards[player] : new List<Card>();
    public List<Card> DeckOfPlayer(string player) => playerDecks.ContainsKey(player) ? playerDecks[player] : new List<Card>();

    public List<Card> Hand => HandOfPlayer(TriggerPlayer);
    public List<Card> Field => FieldOfPlayer(TriggerPlayer);
    public List<Card> Graveyard => GraveyardOfPlayer(TriggerPlayer);
    public List<Card> Deck => DeckOfPlayer(TriggerPlayer);
}

public static class CardListExtensions
{
    public static void Shuffle(this List<Card> cards)
    {
        var rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public static void Push(this List<Card> cards, Card card)
    {
        cards.Insert(0, card);
    }

    public static void SendBottom(this List<Card> cards, Card card)
    {
        cards.Add(card);
    }

    public static Card Pop(this List<Card> cards)
    {
        if (cards.Count == 0) return null;
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public static void Remove(this List<Card> cards, Card card)
    {
        cards.Remove(card);
    }

    public static List<Card> Find(this List<Card> cards, Func<Card, bool> predicate)
    {
        return cards.Where(predicate).ToList();
    }

}

