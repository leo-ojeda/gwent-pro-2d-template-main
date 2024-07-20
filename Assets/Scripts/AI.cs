using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
    Context context;
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

    // public int[] cardsID;
    // public int SummonThisId;
    // public AICardToHand aICardToHand;
    // public int summonID;
    public int HowManyCards;
    public static int EnemyPowerTotal;
    private Card Leader;

    private Card cardToSummon;  // Nueva variable para la carta a invocar
    private List<Card> summonableCards = new List<Card>();

    void Start()
    {
        context = FindObjectOfType<Context>();
        if (!context.playerDecks.ContainsKey("Jugador 2"))
        {
            context.playerDecks["Jugador 2"] = new List<Card>();
        }
        if (!context.playerHands.ContainsKey("Jugador 2"))
        {
            context.playerHands["Jugador 2"] = new List<Card>();
        }
        if (!context.playerFields.ContainsKey("Jugador 2"))
        {
            context.playerFields["Jugador 2"] = new List<Card>();
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

                        context.playerDecks["Jugador 2"].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }

                if (randomCard.Type == "Golden" && randomCard.Faction == Leader.Faction)
                {
                    if (!goldenCountAI.ContainsKey(randomCard.Name))
                    {
                        Deck[i] = randomCard;

                        context.playerDecks["Jugador 2"].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }
                else if (randomCard.Type == "Silver" && randomCard.Faction == Leader.Faction)
                {
                    if (!silverCountAI.ContainsKey(randomCard.Name) || silverCountAI[randomCard.Name] < 3)
                    {
                        Deck[i] = randomCard;
                        context.playerDecks["Jugador 2"].Add(Deck[i]);

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
        context.playerDecks["Jugador 2"] = Deck;
        CardInDeck1.SetActive(deckSize >= 20);
        CardInDeck2.SetActive(deckSize >= 13);
        CardInDeck3.SetActive(deckSize >= 6);
        CardInDeck4.SetActive(deckSize >= 1);

        if (TurnSystem.StartTurn && TurnSystem.Round <= 3)
        {
            StartCoroutine(Draw(2));
            Debug.Log("Draw");
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
            ExecuteSummonAction();
        }
    }

    void ExecuteSummonPhase()
    {
        cardToSummon = summonableCards.OrderByDescending(card => card.Power).FirstOrDefault();
    }

    void ExecuteSummonAction()
    {
        if (cardToSummon != null)
        {
            foreach (Transform child in Hand.transform)
            {
                AICardToHand cardComponent = child.GetComponent<AICardToHand>();
                Card cardInHand = cardComponent.ThisCard[0];

                if (cardInHand == cardToSummon && !TurnSystem.IsYourTurn && !TurnSystem.surrenderedPlayer2)
                {
                    Debug.Log("Invocando carta...");

                    foreach (var range in cardToSummon.Range)
                    {
                        Transform targetZone = null;

                        switch (range)
                        {
                            case "Melee":
                                targetZone = ZoneM.transform;
                                break;

                            case "Ranged":
                                targetZone = ZoneR.transform;
                                break;

                            case "Siege":
                                targetZone = ZoneS.transform;
                                break;

                            case "Leader":
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

                    EnemyPowerTotal += cardToSummon.Power;
                    TurnSystem.CurrentEnemyMana = 0;
                    context.board.Add(cardInHand);
                    context.playerFields[cardInHand.Owner].Add(cardInHand);
                    context.playerHands[cardInHand.Owner].Remove(cardInHand);
                    Debug.Log("PowerTotalEnemy: " + EnemyPowerTotal);

                    // Resetea la variable después de invocar
                    cardToSummon = null;
                    break;
                }
            }

            SummonPhase = false;
            DrawPhase = false;
        }
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
        return EnemyPowerTotal > ThisCard.PowerTotal || EnemyPowerTotal >= 20;//|| HowManyCards == 0;
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
