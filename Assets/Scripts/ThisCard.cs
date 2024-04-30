using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;





public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard;
    public int thisId;
    public static List<Card> CardSummon = new List<Card>();

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
    public GameObject BattleZone;

    public static int PowerTotal;
    private bool Zone;
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
        SelectCard();
        FindBattleZones();
        NumberOfCardsIdDeck = PlayerDeck.deck;
        canBeSummon = false;
        summoned = false;
        Zone = true;
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
        // Verificar si tienes suficiente mana para invocar cartas
        Draggable h = GetComponent<Draggable>();
        if (TurnSystem.CurrentMana >= Cost && summoned == false && TurnSystem.IsYourTurn == true)
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
    public void Summon(Card SumonedCard)
    {
        TurnSystem.CurrentMana -= Cost;
        PowerTotal+=SumonedCard.Power;
        summoned = true;
        CardSummon.Add(SumonedCard);
        if (SumonedCard.Efect == "3")
        {
            MaxMana(1);
        }
        // if (SumonedCard.Efect == "1")
        // {
        //     StartCoroutine(DrawE(1));
        // }
    }

    // Diccionario para almacenar las zonas de batalla según su tag
    Dictionary<string, GameObject> battleZones = new Dictionary<string, GameObject>();

    // Llenar el diccionario con las zonas de batalla encontradas
    void FindBattleZones()
    {
        string[] zoneTags = { "Melee", "Range", "Clima", "Siege", "Leader", "Increase" };
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
        string[] battleZoneNames = { "MeleeZone 1", "RangeZone 1", "ClimaZone 1", "ClimaZone 2", "ClimaZone 3", "LeaderZone 1", "SiegeZone 1", "IncrementoZone 1", "IncrementoZone 2", "IncrementoZone 3" };
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
                        
                        Debug.Log("Carta: " + card.CardName);
                        Debug.Log(CardSummon.Count);
                        Debug.Log(PowerTotal);
                    }
                }
            }
        }
        if (TurnSystem.Round == 2 && Zone == true)
        {
            Debug.Log("Ronda 2 a comenzado");
            CardSummon.Clear();
            Zone = false;
        }
        if (TurnSystem.Round == 3 && Zone == false)
        {
            Debug.Log("Ronda 3 a comenzado");
            CardSummon.Clear();
            Zone = true;
        }
    }
    public void MaxMana(int x = 1)
    {
        TurnSystem.MaxMana += x;
        TurnSystem.CurrentMana += x;
    }

    //  IEnumerator DrawE(int x)
    // {
    //     for (int i = 0; i < x; i++)
    //     {
    //         //Debug.Log("Mas 2");
    //         yield return new WaitForSeconds(1);
    //         Instantiate(CardToHand, transform.position, transform.rotation);
    //     }
    // }
}




