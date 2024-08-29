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
            new Parameter(boostAmount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                if (target.Range.Contains("M") && target.Type != "Golden" && target.Type != "Leader")
                {

                    target.Power += boostAmount;
                    Debug.Log("Boosted power of " + target.Name + " by " + boostAmount);
                }
            }
        });
    }
    public static Effect BoostPowerR(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter(boostAmount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                if (target.Range.Contains("R") && target.Type != "Golden" && target.Type != "Leader")
                {

                    target.Power += boostAmount;
                    Debug.Log("Boosted power of " + target.Name + " by " + boostAmount);
                }
            }
        });
    }
    public static Effect BoostPowerS(int boostAmount)
    {
        return new Effect("Boost Power", new List<Parameter>
        {
            new Parameter(boostAmount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                if (target.Range.Contains("S") && target.Type != "Golden" && target.Type != "Leader")
                {

                    target.Power += boostAmount;
                    Debug.Log("Boosted power of " + target.Name + " by " + boostAmount);
                }
            }
        });
    }

    public static Effect Damage(int amount)
    {
        return new Effect("Damage", new List<Parameter>
        {
            new Parameter( amount)
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
            CardToHand.InstantiateCard(context.CardPrefab, context.Hands, topCard);
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
    public static Effect Increase(int amount)
    {
        return new Effect("Increase", new List<Parameter>
        {
            new Parameter( amount)
        },
        (targets, context) =>
        {
            foreach (var target in targets)
            {
                for (int i = 0; i < amount; i++)
                {
                    if (target.Type != "Golden" && target.Type != "Leader")
                    {

                        target.Power += 1;
                    }
                }
                Debug.Log("Increase power of " + target.Name + " by " + amount);
            }
        });
    }
}
