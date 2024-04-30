using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public List<Card> Deck = new List<Card>();
    public List<Card> Container = new List<Card>();
    public static List<Card> StaticEnemyDeck = new List<Card>();

    //New
    public List<Card> CardsInHand = new List<Card>();
    //end


    public GameObject Hand;
    public GameObject ZoneM;
    public GameObject ZoneR;
    public GameObject ZoneS;
    public GameObject ZoneL;

    public static int deckSize;

    public GameObject CardInDeck1;
    public GameObject CardInDeck2;
    public GameObject CardInDeck3;
    public GameObject CardInDeck4;

    public GameObject CardToHand;

    public GameObject[] Clones;

    public GameObject CardBack;

    //New
    public int CurrentMana;
    public bool[] AIcanSummon;

    public bool DrawPhase;
    public bool SummonPhase;
    public bool EndPhase;

    public int[] cardsID;
    public int SummonThisId;
    public AICardToHand aICardToHand;

    public int summonID;

    public int HowManyCards;
    public static int EnemyPowerTotal;

    void Start()
    {
        Hand = GameObject.Find("EnemyHand");
        ZoneM = GameObject.Find("MeleeZone AI");
        ZoneR = GameObject.Find("RangeZone AI");
        ZoneS = GameObject.Find("SiegeZone AI");
        ZoneL = GameObject.Find("LeaderZone AI");
        EnemyPowerTotal = 0;


        deckSize = 25;
        List<Card> deckCards = new List<Card>(); // Lista para almacenar las cartas que ya se han agregado al mazo
        bool leaderCardAdded = false; // Indica si se ha agregado la carta líder

        // Contadores para el número de cartas golden y silver con el mismo nombre que se han agregado al mazo
        Dictionary<string, int> goldenCountAI = new Dictionary<string, int>();
        Dictionary<string, int> silverCountAI = new Dictionary<string, int>();

        for (int i = 0; i < deckSize; i++)
        {
            Card randomCard = null;
            bool cardAdded = false;

            while (!cardAdded)
            {
                // Obtener una carta aleatoria que no esté ya en el mazo
                int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
                randomCard = CardDatabase.cardList[randomIndex];

                // Verificar si la carta seleccionada es una carta líder y si ya se ha agregado una al mazo
                if (randomCard.CardType == "Leader" && !leaderCardAdded)
                {
                    Deck[i] = randomCard;
                    leaderCardAdded = true;
                    cardAdded = true;
                }
                // Verificar si la carta seleccionada es golden
                else if (randomCard.CardType == "Golden")
                {
                    // Verificar si ya hay una carta golden con el mismo nombre en el mazo
                    if (!goldenCountAI.ContainsKey(randomCard.CardName))
                    {
                        Deck[i] = randomCard;
                        goldenCountAI[randomCard.CardName] = 1; // Registrar la presencia de esta carta golden en el mazo
                        cardAdded = true;
                    }
                }
                // Verificar si la carta seleccionada es silver
                else if (randomCard.CardType == "Silver" && randomCard.Attack != "Clima" && randomCard.Attack != "Increase")
                {
                    // Verificar si ya hay tres cartas silver con el mismo nombre en el mazo
                    if (!silverCountAI.ContainsKey(randomCard.CardName) || silverCountAI[randomCard.CardName] < 3)
                    {
                        Deck[i] = randomCard;
                        if (!silverCountAI.ContainsKey(randomCard.CardName))
                        {
                            silverCountAI[randomCard.CardName] = 1; // Registrar la presencia de esta carta silver en el mazo
                        }
                        else
                        {
                            silverCountAI[randomCard.CardName]++; // Incrementar el contador de esta carta silver en el mazo
                        }
                        cardAdded = true;
                    }
                }
            }
            // Agregar la carta al mazo
            deckCards.Add(randomCard);
        }

        StartCoroutine(StartGame());
    }


    void Update()
    {
        StaticEnemyDeck = Deck;

        if (deckSize < 20)
        {
            CardInDeck1.SetActive(false);
        }
        if (deckSize < 13)
        {
            CardInDeck2.SetActive(false);
        }
        if (deckSize < 6)
        {
            CardInDeck3.SetActive(false);
        }
        if (deckSize < 1)
        {
            CardInDeck4.SetActive(false);
        }
        if (TurnSystem.StartTurn == true && TurnSystem.Round <= 3)
        {
            StartCoroutine(Draw(2));
            Debug.Log("Draw");
        }
        //New
        CurrentMana = TurnSystem.CurrentEnemyMana;

        //Better
        if (0 == 0)
        {
            int j = 0;
            HowManyCards = 0;
            foreach (Transform child in Hand.transform)
            {
                HowManyCards++;
            }
            foreach (Transform child in Hand.transform)
            {

                CardsInHand[j] = child.GetComponent<AICardToHand>().ThisCard[0];
                j++;
            }
            for (int i = 0; i < 25; i++)
            {
                if (i >= HowManyCards)
                {
                    CardsInHand[i] = CardDatabase.cardList[0];
                }
            }
            j = 0;
        }
        //Really Better

        if (TurnSystem.IsYourTurn == false)
        {
            for (int i = 0; i < 25; i++)
            {
                if (CardsInHand[i].Id != 0)
                {
                    //Debug.Log(CurrentMana);
                    if (CurrentMana >= CardsInHand[i].Cost)
                    {
                        //Debug.Log("se puede invocar");
                        AIcanSummon[i] = true;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 25; i++)
            {
                AIcanSummon[i] = false;
            }
        }

        if (TurnSystem.IsYourTurn == false)
        {
            DrawPhase = true;
        }
        if (DrawPhase == true && SummonPhase == false)
        {
            StartCoroutine(WaitForSummonPhase());
        }
        if (TurnSystem.IsYourTurn == true)
        {
            DrawPhase = false;
            SummonPhase = false;
            EndPhase = false;

        }


        if (SummonPhase == true)
        {
            summonID = 0;
            SummonThisId = 0;

            int index = 0;
            for (int i = 0; i < 25; i++)
            {
                if (AIcanSummon[i] == true)
                {
                    //Debug.Log("Es verdadero");
                    cardsID[index] = CardsInHand[i].Id;
                    index++;
                }
            }

            //PLace
            for (int i = 0; i < 25; i++)
            {
                if (cardsID[i] != 0)
                {
                    //Debug.Log("vamo a calmarno 1");
                    if (cardsID[i] > summonID)
                    {
                        //Debug.Log("VAmo a calmarno 2");
                        summonID = cardsID[i];
                    }
                }
            }
            //end

        }
        SummonThisId = summonID;
        
        if (EnemyPowerTotal >= 13)
        {
            //Debug.Log("Suficiente");
            TurnSystem.surrenderedPlayer2 = true;
        }
        foreach (Transform child in Hand.transform)
        {

            if (child.GetComponent<AICardToHand>().Id == SummonThisId && CardDatabase.cardList[SummonThisId].Cost <= CurrentMana && TurnSystem.IsYourTurn == false && TurnSystem.surrenderedPlayer2 == false)
            {
                //Debug.Log(SummonThisId);
                if (CardDatabase.cardList[SummonThisId].Attack == "Melee")
                {
                    //Debug.Log("Zona Melee");
                    child.transform.SetParent(ZoneM.transform);
                }
                if (CardDatabase.cardList[SummonThisId].Attack == "Range")
                {
                    //Debug.Log("Zona Range");
                    child.transform.SetParent(ZoneR.transform);
                }
                if (CardDatabase.cardList[SummonThisId].Attack == "Siege")
                {
                    //Debug.Log("Zone Siege");
                    child.transform.SetParent(ZoneS.transform);
                }
                if (CardDatabase.cardList[SummonThisId].Attack == "Leader")
                {
                    Debug.Log("Zona Leader");
                    child.transform.SetParent(ZoneL.transform);
                }
                else
                {
                    Debug.Log("La carta es " + CardDatabase.cardList[SummonThisId].Attack);
                }
                TurnSystem.CurrentEnemyMana -= CardDatabase.cardList[SummonThisId].Cost;
                EnemyPowerTotal += CardDatabase.cardList[SummonThisId].Power;
                Debug.Log("PowerTotalEnemy" + EnemyPowerTotal);
                break;

            }

        }
        SummonPhase = false;

    }
    public void Shuffle()
    {
        for (int k = 0; k < deckSize; k++)
        {
            Container[0] = Deck[k];
            int randomIndex = Random.Range(k, deckSize);
            Deck[k] = Deck[randomIndex];
            Deck[randomIndex] = Container[0];
        }

        Instantiate(CardBack, transform.position, transform.rotation);

        StartCoroutine(ShuffleNow());

    }

    IEnumerator StartGame()
    {
        for (int i = 0; i <= 9; i++)
        {
            yield return new WaitForSeconds(1);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }
    IEnumerator ShuffleNow()
    {
        yield return new WaitForSeconds(1);
        Clones = GameObject.FindGameObjectsWithTag("Clone");

        foreach (GameObject Clone in Clones)
        {
            Destroy(Clone);
        }
    }

    IEnumerator Draw(int x)
    {
        for (int i = 0; i < x; i++)
        {
            //Debug.Log("Mas 2");
            yield return new WaitForSeconds(1);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }

    IEnumerator WaitForSummonPhase()
    {
        yield return new WaitForSeconds(5);
        SummonPhase = true;
    }

}


