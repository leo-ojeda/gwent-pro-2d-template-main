using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;



public class ThisCard : MonoBehaviour
{
    Context context;
    public List<Card> thisCard;
    public List<Card> Card;
    public int n;
    public static List<Card> CardSummon = new List<Card>();

    public static int Id;
    //new
    public string Owner;
    //end
    public string CardName;
    public string CardType;
    public int Power;
    public int InitialPower;
    public List<EffectActivation> Efect;
    public string[] Range;
    public string Faction;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PowerText;
    public TextMeshProUGUI EfectText;
    public TextMeshProUGUI RangedText;
    public TextMeshProUGUI TypeText;



    public Sprite ThisSprite;
    public Image ThatImage;
    public bool cardBack;
    public static bool cardB;


    public GameObject Hand;
    public int NumberOfCardsIdDeck;


    public bool canBeSummon;
    public bool summoned;
    public GameObject BattleZone;

    public static int PowerTotal;
    private bool Zone;
    private AudioSource audioSource;
    public GameObject State;
    public GameObject FieldP1;

    void SelectCard()
    {

        if (n < CardDatabase.cardList.Count)
        {
            // Se asigna la carta correspondiente desde la base de datos
            thisCard.Add(CardDatabase.cardList[n]);
        }

    }
    void Awake()
    {
        thisCard = new List<Card>();
    }
    void Start()
    {
        InitialPower = -1;
        FieldP1 = GameObject.Find("Field P1");


        context = FindObjectOfType<Context>();
        if (!context.playerHands.ContainsKey("Jugador 1"))
        {
            context.playerHands["Jugador 1"] = new List<Card>();
        }
        if (!context.playerFields.ContainsKey("Jugador 1"))
        {
            context.playerFields["Jugador 1"] = new List<Card>();
        }
        SelectCard();
        NumberOfCardsIdDeck = PlayerDeck.deck;
        canBeSummon = false;
        summoned = false;
        Zone = true;
        Owner = "Jugador 1";
        PowerTotal = 0;

    }
    void Update()
    {

        CalculatePowerTotal();
        Hand = GameObject.Find("Hand");
        if (this.transform.parent == Hand.transform.parent)
        {
            cardBack = false;
        }
        if (CardToHand.ItName != null)
        {
            CardToHand.ItName.name = thisCard[0].Name;
        }
        thisCard[0].Owner = Owner;
        CardName = thisCard[0].Name;
        CardType = thisCard[0].Type;
        Power = thisCard[0].Power;
        Range = thisCard[0].Range;
        Faction = thisCard[0].Faction;
        Efect = thisCard[0].OnActivation;

        ThisSprite = Resources.Load<Sprite>(thisCard[0].Name);

        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        TypeText.text = "" + CardType;

        RangedText.text = string.Join(", ", Range);

        string efectText = "";
        foreach (var efectActivation in Efect)
        {
            efectText += "Effect: " + efectActivation.effect.name;
            efectText += ":  " + efectActivation.selector.source;
        }
        EfectText.text = efectText;

        ThatImage.sprite = ThisSprite;

        cardB = cardBack;
        if (tag == "Three")
        {
            thisCard[0] = CardToHand.card;
        }

        if (tag == "first")
        {
            // Debug.Log("entro");
            thisCard[0] = context.playerDecks[Owner].Pop();
            context.playerHands[Owner].Add(thisCard[0]);
            NumberOfCardsIdDeck -= 1;
            PlayerDeck.deck -= 1;
            cardBack = false;
        }
        if (State != null && InitialPower != thisCard[0].Power)
        {
            if (InitialPower == -1)
            {
                InitialPower = thisCard[0].Power;
            }
            else
            {
                if (InitialPower > thisCard[0].Power)
                {
                    State.GetComponent<Image>().color = Color.red;
                    PowerText.color = Color.red;
                }
                else if (InitialPower < thisCard[0].Power)
                {
                    State.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    State.GetComponent<Image>().color = Color.white;
                    PowerText.color = Color.black;
                }
            }

            // Actualizar el poder anterior
            InitialPower = thisCard[0].Power;
        }

        // Verificar si tienes suficiente mana para invocar cartas
        Draggable h = GetComponent<Draggable>();
        if (summoned == false && TurnSystem.IsYourTurn == true && TurnSystem.surrenderedPlayer1 == false)
        {
            canBeSummon = true;
            if (h != null)
            {
                h.SetDraggable(true); // Activar la función de arrastrar la carta
            }
        }
        else
        {
            canBeSummon = false;
            if (h != null)
            {
                h.SetDraggable(false); // Desactivar la función de arrastrar la carta
            }
        }

        // Realizar operaciones para cada zona de batalla
        OperationsForBattleZones();
    }
    public void Summon(Card SummonedCard)
    {
        TurnSystem.CurrentMana = 0;
        summoned = true;
        CardSummon.Add(SummonedCard);
        PlayMusic("100");

        context.TriggerPlayer = SummonedCard.Owner;
        context.board.Add(SummonedCard);
        context.playerFields[SummonedCard.Owner].Add(SummonedCard);
        // Field.Add(SummonedCard);
        context.playerHands[Owner].Remove(SummonedCard);

        switch (SummonedCard.Type)
        {
            case "Clima":
                PlayMusic("s2033");
                break;
            case "Increase":
                PlayMusic("s554");
                break;
            default:
                PlayMusic("100");
                break;
        }
        if (SummonedCard.OnActivation != new List<EffectActivation>())
        {

        }

        ActivateEffects(SummonedCard);

    }

    private void CalculatePowerTotal()
    {
        PowerTotal = 0;

        List<Card> fieldCards = context.playerFields[Owner];
        foreach (var card in fieldCards)
        {
            if (card.Type == "Leader" || card.Type == "Clima" || card.Type == "Increase")
            {
                card.Power = 0;
                continue;
            }

            PowerTotal += card.Power;
        }

        //Debug.Log("PowerTotal Recalculado a: " + PowerTotal);
    }


    void ActivateEffects(Card card)
    {

        // Asegurarse de que TriggerPlayer esté configurado
        context.TriggerPlayer = card.Owner;

        foreach (var effectActivation in card.OnActivation)
        {
            List<Card> targets;

            switch (effectActivation.selector.source)
            {
                case "Field":
                    targets = context.Field;
                    break;
                case "Deck":
                    targets = context.Deck;
                    break;
                case "Hand":
                    targets = context.Hand;
                    break;
                case "Graveyard":
                    targets = context.Graveyard;
                    break;
                case "otherField":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Field;
                    break;
                case "otherDeck":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Deck;
                    break;
                case "otherHand":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Hand;
                    break;
                case "board":
                    targets = context.board;
                    break;
                default:
                    targets = new List<Card>(); // Lista vacía si el source no se reconoce
                    break;
            }

            // Aplicar el efecto a los objetivos seleccionados
            effectActivation.Activate(targets, context);
        }
    }


    void OperationsForBattleZones()
    {
        string[] battleZoneNames = { "MeleeZone 1", "RangeZone 1", "ClimaZone", "SiegeZone 1", "IncreaseZone" };
        foreach (string zoneName in battleZoneNames)
        {
            BattleZone = GameObject.Find(zoneName);
            if (transform.parent == BattleZone.transform)
            {
                // Si la carta puede ser invocada en esta zona, la invocamos y la agregamos a CardSummon
                if (canBeSummon)
                {
                    Summon(thisCard[0]);


                    foreach (var card in CardSummon)
                    {

                        //   Debug.Log("Carta: " + card.Name);
                        //   Debug.Log(CardSummon.Count);
                        //   Debug.Log(PowerTotal);
                    }
                }
            }
        }
        if (TurnSystem.Round == 2 && Zone == true)
        {
            //Debug.Log("Ronda 2 a comenzado");
            CardSummon.Clear();
            Zone = false;
        }
        if (TurnSystem.Round == 3 && Zone == false)
        {
            //Debug.Log("Ronda 3 a comenzado");
            CardSummon.Clear();
            Zone = true;
        }
    }


    string PlayMusic(string x)
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip musicClip = Resources.Load<AudioClip>(x);
        audioSource.clip = musicClip;
        audioSource.Play();
        return x;
    }
    // public Card SetCardData(Card card)
    // {
    //     thisCard[0] = card;
    //     Debug.Log(card);
    //     return card;
    // }




}




