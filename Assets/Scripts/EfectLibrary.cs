using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class EffectLibrary
{
    public static Effect BoostPowerM(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter("Amount",ParamType.Number,boostAmount)
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
    public static Effect BoostPowerR(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter("Amount",ParamType.Number,boostAmount)
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
    public static Effect BoostPowerS(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter("Amount",ParamType.Number,boostAmount)
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
            new Parameter("Amount", ParamType.Number,amount)
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
            Debug.Log(context.TriggerPlayer);

            Card topCard = context.Deck.Pop();
            context.Hand.Add(topCard);
            context.Hand.Shuffle();
            Debug.Log("Draw card: " + topCard.Name);
        });
    }
    public static Effect Drawlvl(int numberOfCards)
    {
        return new Effect("Draw", new List<Parameter>(),
        (targets, context) =>
        {
            Debug.Log(context.TriggerPlayer); 

            int cardsToDraw = Mathf.Min(numberOfCards, context.Deck.Count);
            for (int i = 0; i < cardsToDraw; i++)
            {
                Card topCard = context.Deck.Pop();
                context.Hand.Add(topCard);
                Debug.Log("Draw card: " + topCard.Name);
            }

            // Mezclar la mano después de robar las cartas
            context.Hand.Shuffle();
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
    public static Effect Increase(int amount)
    {
        return new Effect("Increase", new List<Parameter>
        {
            new Parameter("Amount", ParamType.Number,amount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                for (int i = 0; i < amount; i++)
                {
                    target.Power += 1;
                }
                Debug.Log("Increase power of " + target.Name + " by " + amount);
            }
        });
    }
}
