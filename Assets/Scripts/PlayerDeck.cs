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
            Debug.Log(Leader.Name);

            // Contadores para el número de cartas golden, silver y clima con el mismo nombre que se han agregado al mazo
            Dictionary<string, int> goldenCount = new Dictionary<string, int>();
            Dictionary<string, int> silverCount = new Dictionary<string, int>();
            Dictionary<string, int> magicCount = new Dictionary<string, int>();

            for (int i = 0; i < deck; i++)
            {
                Card randomCard = null;
                bool cardAdded = false;

                while (!cardAdded)
                {
                    // Verifica que la lista de cartas no esté vacía
                    if (CardDatabase.cardList.Count == 0)
                    {
                        Debug.LogError("No cards available in CardDatabase.");
                        return;
                    }

                    // Obtener una carta aleatoria que no esté ya en el mazo
                    int randomIndex = Random.Range(0, CardDatabase.cardList.Count);
                    randomCard = CardDatabase.cardList[randomIndex];

                    // Lógica de selección y adición de cartas al mazo
                    if (Leader.Name == "DSL")
                    {
                        if (randomCard.Faction != "Fire" && randomCard.Faction != "Torment" && randomCard.Faction != "Forest")
                        {
                            if (randomCard.Type == "Leader")
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                cardAdded = true;
                            }
                            if (randomCard.Type == "Golden") //&& !goldenCount.ContainsKey(randomCard.Name))
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                goldenCount[randomCard.Name] = 1;
                                cardAdded = true;
                            }
                            else if (randomCard.Type == "Silver")// && (!silverCount.ContainsKey(randomCard.Name) || silverCount[randomCard.Name] < 3))
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                silverCount[randomCard.Name] = silverCount.GetValueOrDefault(randomCard.Name, 0) + 1;
                                cardAdded = true;
                            }
                            else if (randomCard.Type == "Clima")// && (!magicCount.ContainsKey(randomCard.Name) || magicCount[randomCard.Name] < 2))
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                magicCount[randomCard.Name] = magicCount.GetValueOrDefault(randomCard.Name, 0) + 1;
                                cardAdded = true;
                            }
                            else if (randomCard.Type == "Increase")
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                magicCount[randomCard.Name] = magicCount.GetValueOrDefault(randomCard.Name, 0) + 1;
                                cardAdded = true;
                            }
                            else
                            {
                                //Debug.LogError("la definicion de la carta no es correcta");
                                randomIndex = Random.Range(0, CardDatabase.cardList.Count);
                                randomCard = CardDatabase.cardList[randomIndex];
                                // Debug.Log(randomCard.Name);
                            }


                        }
                    }
                    else
                    {
                        // Lógica para otros casos de cartas y facciones
                        if (randomCard.Type == "Leader" && !leaderCardAdded && randomCard.Faction == Leader.Faction)
                        {
                            Deck[i] = randomCard;
                            context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                            context.playerDecks["Jugador 1"].Add(Deck[i]);
                            leaderCardAdded = true;
                            cardAdded = true;
                        }
                        else if (randomCard.Type == "Golden" && randomCard.Faction == Leader.Faction)
                        {
                            if (!goldenCount.ContainsKey(randomCard.Name))
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                goldenCount[randomCard.Name] = 1;
                                cardAdded = true;
                            }
                        }
                        else if ((randomCard.Type == "Increase" || randomCard.Type == "Clima") && randomCard.Faction == Leader.Faction)
                        {
                            if (!magicCount.ContainsKey(randomCard.Name) || magicCount[randomCard.Name] < 2)
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                magicCount[randomCard.Name] = magicCount.GetValueOrDefault(randomCard.Name, 0) + 1;
                                cardAdded = true;
                            }
                        }
                        else if (randomCard.Type == "Silver" && randomCard.Faction == Leader.Faction)
                        {
                            if (!silverCount.ContainsKey(randomCard.Name) || silverCount[randomCard.Name] < 3)
                            {
                                Deck[i] = randomCard;
                                context.playerDecks["Jugador 1"] = context.playerDecks.GetValueOrDefault("Jugador 1", new List<Card>());
                                context.playerDecks["Jugador 1"].Add(Deck[i]);
                                silverCount[randomCard.Name] = silverCount.GetValueOrDefault(randomCard.Name, 0) + 1;
                                cardAdded = true;
                            }
                        }
                    }

                    // Agregar la carta al mazo si no ha sido añadida
                    if (cardAdded)
                    {
                        deckCards.Add(randomCard);
                    }
                }
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
            //Debug.Log("Mas 2");
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }

}

