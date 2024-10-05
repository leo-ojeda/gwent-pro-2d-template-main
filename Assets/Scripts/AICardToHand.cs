using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PowerText;
    public TextMeshProUGUI EfectText;
    public TextMeshProUGUI RangedText;
    public TextMeshProUGUI TypeText;
    public GameObject HandAI;
    public int z;

    public GameObject It;
    public GameObject StateAI;

    public int numberofCardsInDeck;



    public Sprite ThisSprite;
    public Image ThatImage;
    public int InitialPower;

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
        InitialPower = -1;
        HandAI = GameObject.Find("EnemyHand");
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

                It.transform.SetParent(HandAI.transform);
                It.transform.localScale = Vector3.one;
                It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
                It.transform.eulerAngles = new Vector3(25, 0, 0);
                z = 1;
            }



        }
        if (It != null)
        {
            It.name = ThisCard[0].Name;
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
        RangedText.text = string.Join(", ", Range);
        TypeText.text = "" + CardType;


        string efectText = "";
        foreach (var efectActivation in Efect)
        {
            efectText += "Effect: " + efectActivation.effect.name;
            efectText += ":  " + efectActivation.selector.source;
        }
        EfectText.text = efectText;
        ThatImage.sprite = ThisSprite;


        if (this.tag == "AIClone")
        {
            //Debug.Log("Activo");
            ThisCard[0] = context.playerDecks[Owner].Pop();//[numberofCardsInDeck - 1];
            //context.playerDecks[Owner].RemoveAt(numberofCardsInDeck - 1);
            context.board.Add(ThisCard[0]);
            context.playerHands["Jugador 2"].Add(ThisCard[0]);
            numberofCardsInDeck -= 1;
            AI.deckSize -= 1;
            this.tag = "Untagged";
        }

        if (this.transform.parent == HandAI.transform)
        {
            CardBack.SetActive(true);
        }
        else
        {
            CardBack.SetActive(false);
        }
        if (StateAI != null && InitialPower != ThisCard[0].Power)
        {
            if (InitialPower == -1)
            {
                // Si es la primera vez que se asigna, establecer el poder inicial
                InitialPower = ThisCard[0].Power;
            }
            else
            {
                if (InitialPower > ThisCard[0].Power)
                {
                    StateAI.GetComponent<Image>().color = Color.red;
                }
                else if (InitialPower < ThisCard[0].Power)
                {
                    StateAI.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    StateAI.GetComponent<Image>().color = Color.white;
                }
            }
        }


    }


}



