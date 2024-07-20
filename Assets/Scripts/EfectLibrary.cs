using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class EffectLibrary
{
    public static Effect BoostPower(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter("BoostAmount", ParamType.Number, boostAmount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                target.Power += boostAmount;
                Debug.Log("Boosted power of " + target.Name + " by " + boostAmount);
            }
        });
    }

    public static Effect Damage(int amount)
    {
        return new Effect("Damage", new List<Parameter>
        {
            new Parameter("Amount", ParamType.Number, amount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                for (int i = 0; i < amount; i++)
                {
                    target.Power -= 1;
                }
                Debug.Log("Decreased power of " + target.Name + " by " + amount);
            }
        });
    }

    public static Effect Draw()
    {
        return new Effect("Draw", new List<Parameter>(),
        (targets, context) =>
        {
            Card topCard = context.Deck.Pop();
            CardToHand.InstantiateCard(context.CardPrefab, context.Hands,topCard);
            context.Hand.Add(topCard);
            context.Hand.Shuffle();
            Debug.Log("Draw card: " + topCard.Name);
        });
    }

    public static Effect ReturnToDeck()
    {
        return new Effect("ReturnToDeck", new List<Parameter>(),
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                var owner = target.Owner;
                var deck = context.DeckOfPlayer(owner);
                deck.Push(target);
                context.board.Remove(target);
                Debug.Log("Returned " + target.Name + " to the deck of " + owner);
            }
        });
    }
}
