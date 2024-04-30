using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDeck : MonoBehaviour
{

    public List<Card> Deck = new List<Card>();
    public List<Card> Container = new List<Card>();
    public static List<Card> staticDeck = new List<Card>();
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


    public GameObject Hand;
    // Start is called before the first frame update

    void Start()
    {
        deck = 25;
        List<Card> deckCards = new List<Card>(); // Lista para almacenar las cartas que ya se han agregado al mazo
        bool leaderCardAdded = false; // Indica si se ha agregado la carta líder

        // Contadores para el número de cartas golden y silver con el mismo nombre que se han agregado al mazo
        Dictionary<string, int> goldenCount = new Dictionary<string, int>();
        Dictionary<string, int> silverCount = new Dictionary<string, int>();

        for (int i = 0; i < deck; i++)
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
                    if (!goldenCount.ContainsKey(randomCard.CardName))
                    {
                        Deck[i] = randomCard;
                        goldenCount[randomCard.CardName] = 1; // Registrar la presencia de esta carta golden en el mazo
                        cardAdded = true;
                    }
                }
                // Verificar si la carta seleccionada es silver
                else if (randomCard.CardType == "Silver")
                {
                    // Verificar si ya hay tres cartas silver con el mismo nombre en el mazo
                    if (!silverCount.ContainsKey(randomCard.CardName) || silverCount[randomCard.CardName] < 3)
                    {
                        Deck[i] = randomCard;
                        if (!silverCount.ContainsKey(randomCard.CardName))
                        {
                            silverCount[randomCard.CardName] = 1; // Registrar la presencia de esta carta silver en el mazo
                        }
                        else
                        {
                            silverCount[randomCard.CardName]++; // Incrementar el contador de esta carta silver en el mazo
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

    // Update is called once per frame
    void Update()
    {
        staticDeck = Deck;


        if (deck < 20)
        {
            CardInDeck1.SetActive(false);
        }
        if (deck < 13)
        {
            CardInDeck2.SetActive(false);
        }
        if (deck < 6)
        {
            CardInDeck3.SetActive(false);
        }
        if (deck < 1)
        {
            CardInDeck4.SetActive(false);
        }

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
            yield return new WaitForSeconds(1);
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
        StartCoroutine(Example());
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

}

