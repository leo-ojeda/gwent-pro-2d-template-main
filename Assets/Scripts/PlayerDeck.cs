using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    //new
    private Context context;
    //
    public List<Card> Deck = new List<Card>();
    public List<Card> Container = new List<Card>();
    //public static List<Card> staticDeck = new List<Card>();
    public int x;

    public static int deck;
    public GameObject CardInDeck1;
    public GameObject CardInDeck2;
    public GameObject CardInDeck3;
    public GameObject CardInDeck4;
    public GameObject CardToHand;
    public GameObject CardBack;
    public GameObject Dek;
    public GameObject[] Clones;
    private Card Leader;
    public TextMeshProUGUI Ncard;

    

    void Start()
    {

        context = FindObjectOfType<Context>();
        deck = 25;
        List<Card> deckCards = Menuinicial.cardList; // Lista para almacenar las cartas que ya se han agregado al mazo
        bool leaderCardAdded = false; // Indica si se ha agregado la carta líder
        if (deckCards != null && deckCards.Count > 0)
        {

            Leader = deckCards[0];
            Debug.Log(deckCards[0].Name);


            // Contadores para el número de cartas golden y silver con el mismo nombre que se han agregado al mazo
            Dictionary<string, int> goldenCount = new Dictionary<string, int>();
            Dictionary<string, int> silverCount = new Dictionary<string, int>();
            Dictionary<string, int> magicCount = new Dictionary<string, int>();

            for (int i = 0; i < deck; i++)
            {
                Card randomCard = null;
                bool cardAdded = false;

                while (!cardAdded)
                {
                    // Obtener una carta aleatoria que no esté ya en el mazo
                    int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
                    randomCard = CardDatabase.cardList[randomIndex];
                    //Debug.Log(randomCard.CardName);

                    // Verificar si la carta seleccionada es una carta líder y si ya se ha agregado una al mazo
                    if (randomCard.Type == "Leader" && !leaderCardAdded && randomCard.Faction == Leader.Faction)
                    {

                        Deck[i] = randomCard;
                        if (!context.playerDecks.ContainsKey("Jugador 1"))
                        {
                            context.playerDecks["Jugador 1"] = new List<Card>();
                        }
                        context.playerDecks["Jugador 1"].Add(Deck[i]);
                        //Debug.Log(context.playerDecks.Count);
                        leaderCardAdded = true;
                        cardAdded = true;
                    }
                    // Verificar si la carta seleccionada es golden
                    else if (randomCard.Type == "Golden" && randomCard.Faction == Leader.Faction)
                    {
                        // Verificar si ya hay una carta golden con el mismo nombre en el mazo
                        if (!goldenCount.ContainsKey(randomCard.Name))
                        {
                            Deck[i] = randomCard;
                            if (!context.playerDecks.ContainsKey("Jugador 1"))
                            {
                                context.playerDecks["Jugador 1"] = new List<Card>();
                            }
                            context.playerDecks["Jugador 1"].Add(Deck[i]);
                            // Debug.Log(context.playerDecks.Count);
                            goldenCount[randomCard.Name] = 1; // Registrar la presencia de esta carta golden en el mazo
                            cardAdded = true;
                        }
                    }
                    else if (randomCard.Type == "Increase" && randomCard.Faction == Leader.Faction || randomCard.Type == "Clima" && randomCard.Faction == Leader.Faction)
                    {

                        if (!magicCount.ContainsKey(randomCard.Name) || magicCount[randomCard.Name] < 2)
                        {
                            Deck[i] = randomCard;
                            if (!context.playerDecks.ContainsKey("Jugador 1"))
                            {
                                context.playerDecks["Jugador 1"] = new List<Card>();
                            }
                            context.playerDecks["Jugador 1"].Add(Deck[i]);
                            //Debug.Log(randomCard.Owner);
                            //Debug.Log(context.playerDecks.Count);
                            if (!magicCount.ContainsKey(randomCard.Name))
                            {
                                magicCount[randomCard.Name] = 1;
                            }
                            else
                            {
                                magicCount[randomCard.Name]++;
                            }
                            cardAdded = true;
                        }
                    }

                    // Verificar si la carta seleccionada es silver
                    else if (randomCard.Type == "Silver" && randomCard.Faction == Leader.Faction)
                    {
                        // Verificar si ya hay tres cartas silver con el mismo nombre en el mazo
                        if (!silverCount.ContainsKey(randomCard.Name) || silverCount[randomCard.Name] < 3)
                        {
                            Deck[i] = randomCard;
                            if (!context.playerDecks.ContainsKey("Jugador 1"))
                            {
                                context.playerDecks["Jugador 1"] = new List<Card>();
                            }
                            context.playerDecks["Jugador 1"].Add(Deck[i]);
                            //Debug.Log(randomCard.Owner);
                            //Debug.Log(context.playerDecks.Count);
                            if (!silverCount.ContainsKey(randomCard.Name))
                            {
                                silverCount[randomCard.Name] = 1; // Registrar la presencia de esta carta silver en el mazo
                            }
                            else
                            {
                                silverCount[randomCard.Name]++; // Incrementar el contador de esta carta silver en el mazo
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

    }

    // Update is called once per frame
    void Update()
    {
        //staticDeck = Deck;
        context.playerDecks["Jugador 1"] = Deck;
        Ncard.text = "" + Deck.Count;

        CardInDeck1.SetActive(deck >= 20);
        CardInDeck2.SetActive(deck >= 13);
        CardInDeck3.SetActive(deck >= 6);
        CardInDeck4.SetActive(deck >= 1);

        if (TurnSystem.StartTurn == true && TurnSystem.Round <= 3)
        {
            StartCoroutine(Draw(2));
            TurnSystem.StartTurn = false;
        }
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(1);
        Clones = GameObject.FindGameObjectsWithTag("Clone");

        foreach (GameObject Clone in Clones)
        {
            Destroy(Clone);
        }
    }

    IEnumerator StartGame()
    {
        for (int i = 0; i <= 9; i++)
        {

            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }
    public void Shuffle()
    {
        for (int i = 0; i < deck; i++)
        {
            Debug.Log("shuffle");
            Container[0] = Deck[i];
            int RandomIndex = Random.Range(i, deck);
            Deck[i] = Deck[RandomIndex];
            Deck[RandomIndex] = Container[0];
        }
        Instantiate(CardBack, transform.position, transform.rotation);
        //StartCoroutine(Example());
    }
    IEnumerator Draw(int x)
    {
        for (int i = 0; i < x; i++)
        {
            //Debug.Log("Mas 2");
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }

}

