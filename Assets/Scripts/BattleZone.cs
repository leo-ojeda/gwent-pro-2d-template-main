using UnityEngine;
using System.Collections.Generic;

public class BattleZone : MonoBehaviour
{
    public List<Card> Cards { get; private set; }

    void Awake()
    {
        Cards = new List<Card>();
    }

    // Método para agregar una carta a esta zona de batalla
    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    // Método para eliminar una carta de esta zona de batalla
    public void RemoveCard(Card card)
    {
        Cards.Remove(card);
    }
}
