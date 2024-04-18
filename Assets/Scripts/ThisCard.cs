using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using JetBrains.Annotations;

public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard;
    public int thisId;

    public int Id;
    public string CardName;
    public string CardType;
    public int Power;
    public string Efect;
    public string Attack;
    public int Cost;
    public Text NameText;
    public Text PowerText;
    public Text DescriptionText;
    public Text CostText;


    public Sprite ThisSprite;
    public Image ThatImage;
    public bool cardBack;
    public static bool cardB;


    public GameObject Hand;
    public int NumberOfCardsIdDeck;

    public bool canBeSummon;
    public bool summoned;
    public GameObject battleZone;


    void SelectRandomCard()
    {

        if (thisId >= 0 && thisId < CardDatabase.cardList.Count)
        {
            // Se asigna la carta correspondiente desde la base de datos
            thisCard.Add(CardDatabase.cardList[thisId]);
        }

    }

    void Awake()
    {
        thisCard = new List<Card>();
    }


    void Start()
    {
        SelectRandomCard();
        NumberOfCardsIdDeck = PlayerDeck.deck;

        canBeSummon = false;
        summoned = false;
    }

    void Update()
    {
        Hand = GameObject.Find("Hand");
        if (this.transform.parent == Hand.transform.parent)
        {
            cardBack = false;
        }


        Id = thisCard[0].Id;
        CardName = thisCard[0].CardName;
        CardType = thisCard[0].CardType;
        Power = thisCard[0].Power;
        Efect = thisCard[0].Efect;
        Attack = thisCard[0].Attack;
        Cost = thisCard[0].Cost;

        ThisSprite = thisCard[0].Imagen;



        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        CostText.text = "" + CardType;
        DescriptionText.text = " " + Efect;


        ThatImage.sprite = ThisSprite;

        cardB = cardBack;

        if (this.tag == "first")
        {
            //Debug.Log("entro");
            thisCard[0] = PlayerDeck.staticDeck[NumberOfCardsIdDeck - 1];
            NumberOfCardsIdDeck -= 1;
            PlayerDeck.deck -= 1;
            cardBack = false;
            //this.tag = "Untagged";
        }
       // if (TurnSystem.CurrentMana >= Cost && summoned == false)
       // {
       //     canBeSummon = true;
       // }
       // else canBeSummon = false;
//
       // if (canBeSummon == true)
       // {
       //     gameObject.GetComponent<Draggable>().enabled = true;
       // }
       // else gameObject.GetComponent<Draggable>().enabled = false;
//
       // battleZone = GameObject.Find("Zone");
//
       // if (summoned == false && this.transform.parent == battleZone.transform)
       // {
       //     Summon();
       // }
//
    }
      //  public void Summon()
      //  {
      //      TurnSystem.CurrentMana -= Cost;
      //      summoned = true;
      //  }

}
