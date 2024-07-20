using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThisCard : MonoBehaviour
{
    Context context;
    public List<Card> thisCard;
    public int thisId;
    public static List<Card> CardSummon = new List<Card>();
    //new
    public string Owner;
    //end
    public string CardName;
    public string CardType;
    public int Power;
    public List<EffectActivation> Efect;
    public string[] Range;
    public string Faction;
    //public Text NameText;
    //public Text PowerText;
    //public Text DescriptionText;
    //public Text CostText;


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
    void SelectCard()
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
        FindBattleZones();
        NumberOfCardsIdDeck = PlayerDeck.deck;
        canBeSummon = false;
        summoned = false;
        Zone = true;
        Owner = "Jugador 1";
        PowerTotal = 0;

    }
    void Update()
    {
        Hand = GameObject.Find("Hand");
        if (this.transform.parent == Hand.transform.parent)
        {
            cardBack = false;
        }
        thisCard[0].Owner = Owner;
        CardName = thisCard[0].Name;
        CardType = thisCard[0].Type;
        Power = thisCard[0].Power;
        Range = thisCard[0].Range;
        Faction = thisCard[0].Faction;
        Efect = thisCard[0].OnActivation;

        ThisSprite = Resources.Load<Sprite>(thisCard[0].Name);

        // NameText.text = "" + CardName;
        // PowerText.text = "" + Power;
        // CostText.text = "" + CardType;
        // DescriptionText.text = " " + Efect;

        ThatImage.sprite = ThisSprite;

        cardB = cardBack;

        if (tag == "first")
        {
            //Debug.Log("entro");
            thisCard[0] = context.playerDecks[Owner][context.playerDecks[Owner].Count - 1];
            context.playerDecks[Owner].RemoveAt(context.playerDecks[Owner].Count - 1);
            context.playerHands["Jugador 1"].Add(thisCard[0]);
            NumberOfCardsIdDeck -= 1;
            PlayerDeck.deck -= 1;
            cardBack = false;
            //this.tag = "Untagged";
        }
        // Verificar si tienes suficiente mana para invocar cartas
        Draggable h = GetComponent<Draggable>();
        if (TurnSystem.CurrentMana > 0 && summoned == false && TurnSystem.IsYourTurn == true && TurnSystem.surrenderedPlayer1 == false)
        {
            canBeSummon = true;
            if (h != null)
            {
                //Debug.Log("Activado");
                h.SetDraggable(true); // Activar la función de arrastrar la carta
            }
        }
        else
        {
            canBeSummon = false;
            if (h != null)
            {
                //Debug.Log("Desactivado");
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
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        // Añadir la carta al tablero y a los campos del jugador
        context.TriggerPlayer = SummonedCard.Owner;
        context.board.Add(SummonedCard);
        context.playerFields[SummonedCard.Owner].Add(SummonedCard);
        context.playerHands[Owner].Remove(SummonedCard);

        ActivateEffects(SummonedCard);

        RecalculatePowerTotal(context.TriggerPlayer);
    }

    private void RecalculatePowerTotal(string player)
    {
        PowerTotal = 0;

        List<Card> fieldCards = context.FieldOfPlayer(player);
        foreach (var card in fieldCards)
        {
            PowerTotal += card.Power;
        }

        //Debug.Log("PowerTotal recalculado a: " + PowerTotal);
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
                default:
                    targets = new List<Card>(); // Lista vacía si el source no se reconoce
                    break;
            }

            // Aplicar el efecto a los objetivos seleccionados
            effectActivation.Activate(targets, context);
        }
    }
    // Diccionario para almacenar las zonas de batalla según su tag
    Dictionary<string, GameObject> battleZones = new Dictionary<string, GameObject>();

    // Llenar el diccionario con las zonas de batalla encontradas
    void FindBattleZones()
    {
        string[] zoneTags = { "Melee", "Ranged", "Clima", "Siege", "Leader", "Increase" };
        foreach (string tag in zoneTags)
        {
            GameObject zone = GameObject.FindGameObjectWithTag(tag);
            if (zone != null)
            {
                // Añadir la zona al diccionario con su tag como clave
                battleZones[tag] = zone;
                //Debug.Log("Zona añadida");
            }
        }
    }

    void OperationsForBattleZones()
    {
        string[] battleZoneNames = { "MeleeZone 1", "RangeZone 1", "ClimaZone 1", "ClimaZone 2", "ClimaZone 3", "LeaderZone 1", "SiegeZone 1" };//, "IncrementoZone 1", "IncrementoZone 2", "IncrementoZone 3" };
        foreach (string zoneName in battleZoneNames)
        {
            BattleZone = GameObject.Find(zoneName);
            if (this.transform.parent == BattleZone.transform)
            {
                // Si la carta puede ser invocada en esta zona, la invocamos y la agregamos a CardSummon
                if (canBeSummon)
                {
                    Summon(thisCard[0]);


                    foreach (var card in CardSummon)
                    {

                        Debug.Log("Carta: " + card.Name);
                        Debug.Log(CardSummon.Count);
                        Debug.Log(PowerTotal);
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

}




