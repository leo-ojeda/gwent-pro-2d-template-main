using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AICardToHand : MonoBehaviour
{
    public List<Card> ThisCard = new List<Card>();

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

    public GameObject Hand;
    public int z;

    public GameObject It;
    public int numberofCardsInDeck;



    public Sprite ThisSprite;
    public Image ThatImage;

    public GameObject CardBack;
    // Start is called before the first frame update
    void Awake()
    {
        ThisCard = new List<Card>();
    }
    void SelectRandomCard()
    {

        if (thisId >= 0 && thisId < CardDatabase.cardList.Count)
        {
            // Se asigna la carta correspondiente desde la base de datos
            ThisCard.Add(CardDatabase.cardList[thisId]);
        }

    }

    void Start()
    {
        Hand = GameObject.Find("EnemyHand");
        numberofCardsInDeck = AI.deckSize;
        z = 0;
        SelectRandomCard();
    }

    // Update is called once per frame
    void Update()
    {
        if (z == 0)
        {
            if (It.tag == "AIClone")
            {

                //Debug.Log("Activo CardtoMand");

                It.transform.SetParent(Hand.transform);
                It.transform.localScale = Vector3.one;
                It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
                It.transform.eulerAngles = new Vector3(25, 0, 0);
                z = 1;
            }



        }
        Id = ThisCard[0].Id;
        CardName = ThisCard[0].CardName;
        CardType = ThisCard[0].CardType;
        Power = ThisCard[0].Power;
        Efect = ThisCard[0].Efect;
        Attack = ThisCard[0].Attack;
        Cost = ThisCard[0].Cost;

        ThisSprite = ThisCard[0].Imagen;

        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        CostText.text = "" + CardType;
        DescriptionText.text = " " + Efect;
        ThatImage.sprite = ThisSprite;


        if (this.tag == "AIClone")
        {
            //Debug.Log("Activo");
            ThisCard[0] = AI.StaticEnemyDeck[numberofCardsInDeck - 1];
            numberofCardsInDeck -= 1;
            AI.deckSize -= 1;
            this.tag = "Untagged";
        }

        if (this.transform.parent == Hand.transform)
        {
            CardBack.SetActive(true);
        }
        else
        {
            CardBack.SetActive(false);
        }
    }
}
