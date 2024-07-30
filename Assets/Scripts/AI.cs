using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class AI : MonoBehaviour
{
    Context context;
    private string Owner;
    public List<Card> Deck = new List<Card>();
    public List<Card> CardsInHand = new List<Card>();
    public static List<Card> StaticEnemyDeck = new List<Card>();

    private AudioSource audioSource;
    public GameObject CardInDeck1;
    public GameObject CardInDeck2;
    public GameObject CardInDeck3;
    public GameObject CardInDeck4;

    public GameObject Hand;
    public GameObject ZoneM;
    public GameObject ZoneR;
    public GameObject ZoneS;
    public GameObject ZoneL;

    public static int deckSize;
    public GameObject CardToHand;
    public GameObject CardBack;

    public int CurrentMana;
    // public bool[] AIcanSummon;
    public bool DrawPhase;
    public bool SummonPhase;
    public bool EndPhase;
    public static bool Control;

    // public int[] cardsID;
    // public int SummonThisId;
    // public AICardToHand aICardToHand;
    // public int summonID;
    public int HowManyCards;
    public static int EnemyPowerTotal;
    private Card Leader;

    private Card cardToSummon;  // Nueva variable para la carta a invocar
    private List<Card> summonableCards = new List<Card>();

    public TextMeshProUGUI AINcard;

    void Start()
    {

        Control = true;
        Owner = "Jugador 2";
        context = FindObjectOfType<Context>();
        if (!context.playerDecks.ContainsKey(Owner))
        {
            context.playerDecks[Owner] = new List<Card>();
        }
        if (!context.playerHands.ContainsKey(Owner))
        {
            context.playerHands[Owner] = new List<Card>();
        }
        if (!context.playerFields.ContainsKey(Owner))
        {
            context.playerFields[Owner] = new List<Card>();
        }
        Hand = GameObject.Find("EnemyHand");
        ZoneM = GameObject.Find("MeleeZone AI");
        ZoneR = GameObject.Find("RangeZone AI");
        ZoneS = GameObject.Find("SiegeZone AI");
        ZoneL = GameObject.Find("LeaderZone AI");
        EnemyPowerTotal = 0;

        deckSize = 25;
        List<Card> deckCards = new List<Card>();
        bool leaderCardAdded = false;

        Dictionary<string, int> goldenCountAI = new Dictionary<string, int>();
        Dictionary<string, int> silverCountAI = new Dictionary<string, int>();

        while (!leaderCardAdded)
        {
            int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
            Card randomCard = CardDatabase.cardList[randomIndex];
            if (randomCard.Type == "Leader")
            {
                Leader = randomCard;
                leaderCardAdded = true;
                Debug.Log(Leader.Name);
            }
        }

        for (int i = 0; i < deckSize; i++)
        {
            bool cardAdded = false;
            while (!cardAdded)
            {
                int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
                Card randomCard = CardDatabase.cardList[randomIndex];

                if (randomCard.Type == "Leader" && randomCard.Faction == Leader.Faction)
                {
                    if (!goldenCountAI.ContainsKey(randomCard.Name))
                    {
                        Deck[i] = randomCard;

                        context.playerDecks[Owner].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }

                if (randomCard.Type == "Golden" && randomCard.Faction == Leader.Faction)
                {
                    if (!goldenCountAI.ContainsKey(randomCard.Name))
                    {
                        Deck[i] = randomCard;

                        context.playerDecks[Owner].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }
                else if (randomCard.Type == "Silver" && randomCard.Faction == Leader.Faction)
                {
                    if (!silverCountAI.ContainsKey(randomCard.Name) || silverCountAI[randomCard.Name] < 3)
                    {
                        Deck[i] = randomCard;
                        context.playerDecks[Owner].Add(Deck[i]);

                        if (!silverCountAI.ContainsKey(randomCard.Name))
                        {
                            silverCountAI[randomCard.Name] = 1;
                        }
                        else
                        {
                            silverCountAI[randomCard.Name]++;
                        }
                        cardAdded = true;
                    }
                }
            }
            deckCards.Add(Deck[i]);
        }

        StartCoroutine(StartGame());
    }

    void Update()
    {
        //Debug.Log(TurnSystem.surrenderedPlayer2);
        CalculatePowerTotal();
        context.playerDecks[Owner] = Deck;
        AINcard.text = "" + Deck.Count;

        CardInDeck1.SetActive(deckSize >= 20);
        CardInDeck2.SetActive(deckSize >= 13);
        CardInDeck3.SetActive(deckSize >= 6);
        CardInDeck4.SetActive(deckSize >= 1);

        if (TurnSystem.StartTurn && TurnSystem.Round <= 3)
        {
            StartCoroutine(Draw(2));
            Debug.Log("Draw");
            Control = true;
        }

        CurrentMana = TurnSystem.CurrentEnemyMana;
        UpdateHand();

        if (!TurnSystem.IsYourTurn)
        {
            UpdateSummonableCards();
        }
        else
        {
            summonableCards.Clear();
        }

        if (!TurnSystem.IsYourTurn)
        {
            DrawPhase = true;
        }

        if (DrawPhase && !SummonPhase)
        {
            StartCoroutine(WaitForSummonPhase());
        }

        if (TurnSystem.IsYourTurn)
        {
            DrawPhase = false;
            SummonPhase = false;
            EndPhase = false;
        }

        if (SummonPhase)
        {
            ExecuteSummonPhase();
        }

        if (TurnSystem.surrenderedPlayer1 && ShouldSurrender())
        {
            TurnSystem.surrenderedPlayer2 = true;
            DrawPhase = false;
            SummonPhase = false;
        }

        if (DrawPhase)
        {
            SummonAction();
        }
    }

    void ExecuteSummonPhase()
    {
        cardToSummon = summonableCards.OrderByDescending(card => card.Power).FirstOrDefault();
    }

    void SummonAction()
    {
        if (cardToSummon != null)
        {
            foreach (Transform child in Hand.transform)
            {
                AICardToHand cardComponent = child.GetComponent<AICardToHand>();
                Card cardInHand = cardComponent.ThisCard[0];

                if (cardInHand == cardToSummon && !TurnSystem.IsYourTurn && !TurnSystem.surrenderedPlayer2)
                {
                    //Debug.Log("Invocando carta...");

                    foreach (var range in cardToSummon.Range)
                    {
                        Transform targetZone = null;

                        switch (range)
                        {
                            case "M":
                                targetZone = ZoneM.transform;
                                break;

                            case "R":
                                targetZone = ZoneR.transform;
                                break;

                            case "S":
                                targetZone = ZoneS.transform;
                                break;

                            case "L":
                                targetZone = ZoneL.transform;
                                break;

                            default:
                                Debug.LogWarning("Rango desconocido: " + range);
                                break;
                        }

                        if (targetZone != null)
                        {
                            child.transform.SetParent(targetZone);
                            PlayMusic();
                            break; // Solo movemos la carta a una zona
                        }
                    }

                    //EnemyPowerTotal += cardToSummon.Power;
                    TurnSystem.CurrentEnemyMana = 0;
                    context.board.Add(cardInHand);
                    context.playerFields[cardInHand.Owner].Add(cardInHand);
                    context.playerHands[cardInHand.Owner].Remove(cardInHand);

                    // Resetea la variable después de invocar
                    cardToSummon = null;
                    break;
                }
            }

            SummonPhase = false;
            DrawPhase = false;
        }
        if (context.playerHands[Owner].Count == 0 && Control)
        {
            TurnSystem.surrenderedPlayer2 = true;
            Control = false;
            Debug.Log("AI se rinde porque no tiene cartas en la mano.");

        }

    }


    private void CalculatePowerTotal()
    {
        EnemyPowerTotal = 0;

        List<Card> fieldCards = context.playerFields[Owner];
        foreach (var card in fieldCards)
        {
            EnemyPowerTotal += card.Power;
        }

        //Debug.Log("PowerTotal recalculado a: " + PowerTotal);
    }


    void UpdateHand()
    {
        CardsInHand.Clear();
        foreach (Transform child in Hand.transform)
        {
            CardsInHand.Add(child.GetComponent<AICardToHand>().ThisCard[0]);
        }
    }

    void UpdateSummonableCards()
    {
        summonableCards.Clear();
        foreach (var card in CardsInHand)
        {
            if (CurrentMana > 0)
            {
                summonableCards.Add(card);
            }
        }
    }

    bool ShouldSurrender()
    {
        return EnemyPowerTotal > ThisCard.PowerTotal || EnemyPowerTotal >= 20;

    }

    IEnumerator StartGame()
    {
        for (int i = 0; i <= 9; i++)
        {
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }

    IEnumerator Draw(int x)
    {
        for (int i = 0; i < x; i++)
        {
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);

            TurnSystem.surrenderedPlayer2 = false;
            Control = true;

        }
    }

    IEnumerator WaitForSummonPhase()
    {
        yield return new WaitForSeconds(0.4f);
        SummonPhase = true;
    }

    void PlayMusic()
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip musicClip = Resources.Load<AudioClip>("100");
        audioSource.clip = musicClip;
        audioSource.Play();
    }
}
