using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Subsystems;

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
        x = 0;
        for (int i = 0; i < deck; i++)
        {
            x = Random.Range(1, 15);
            Deck[i] = CardDatabase.cardList[x];

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
            Container[0] = Deck[i];
            int RandomIndex = Random.Range(i, deck);
            Deck[i] = Deck[RandomIndex];
            Deck[RandomIndex] = Container[0];
        }
        Instantiate(CardBack, transform.position, transform.rotation);
        StartCoroutine(Example());
    }
}

