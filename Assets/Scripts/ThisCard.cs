using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard;
    public static int thisId;

    public int Id;
    public string CardName;
    public int Cost;
    public int Power;
    public string CardDescription;
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


    void Awake()
    {
        thisCard = new List<Card>();
    }

    void Start()
    {
        thisId=Random.Range(1,15);
        if (thisId >= 0 && thisId < CardDatabase.cardList.Count)
        {
            // Se asigna la carta correspondiente desde la base de datos
            thisCard.Add(CardDatabase.cardList[thisId]);
        }
        else
        {
            Debug.LogError("thisId fuera del rango de CardDatabase.cardList");
        }
        NumberOfCardsIdDeck = PlayerDeck.deck;
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
        Cost = thisCard[0].Cost;
        Power = thisCard[0].Power;
        CardDescription = thisCard[0].CardDescription;

        ThisSprite = thisCard[0].Imagen;



        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        CostText.text = "" + Cost;
        DescriptionText.text = " " + CardDescription;

        ThatImage.sprite = ThisSprite;

        cardB = cardBack;

        if (this.tag == "clone")
        {
            thisCard[0] = PlayerDeck.staticDeck[NumberOfCardsIdDeck - 1];
            NumberOfCardsIdDeck -= 1;
            PlayerDeck.deck -= 1;
            cardBack = false;
            this.tag = "Untagged";
        }
    }
}