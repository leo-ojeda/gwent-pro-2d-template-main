using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AICardToHand : MonoBehaviour
{
    Context context;
    public List<Card> ThisCard = new List<Card>();

    public int thisId;
    public string Owner;
    public string CardName;
    public string CardType;
    public int Power;
    public List<EffectActivation> Efect;
    public string[] Range;
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
        Owner = "Jugador 2";
        context = FindObjectOfType<Context>();
         if (!context.playerHands.ContainsKey("Jugador 2"))
        {
            context.playerHands["Jugador 2"] = new List<Card>();
        }
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

        ThisCard[0].Owner = Owner;
        CardName = ThisCard[0].Name;
        CardType = ThisCard[0].Type;
        Power = ThisCard[0].Power;
        Efect = ThisCard[0].OnActivation;
        Range = ThisCard[0].Range;

        ThisSprite = Resources.Load<Sprite>(ThisCard[0].Name);

        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        CostText.text = "" + CardType;
        DescriptionText.text = " " + Efect;
        ThatImage.sprite = ThisSprite;


        if (this.tag == "AIClone")
        {
            //Debug.Log("Activo");
            ThisCard[0] = context.playerDecks[Owner][numberofCardsInDeck - 1];
            context.playerDecks[Owner].RemoveAt(numberofCardsInDeck - 1);
            context.playerHands["Jugador 2"].Add(ThisCard[0]);
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
