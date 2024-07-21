using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardToHand : MonoBehaviour
{
    public GameObject Hand;
    public GameObject It;
    public static GameObject ItName;

    public static Card card;


    public static void InstantiateCard(GameObject cardPrefab, GameObject hand, Card cards)
    {
        // Instanciar el prefab de la carta
        GameObject cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        cardObject.name = cards.Name;

        // Configurar el objeto de la carta
        CardToHand cardToHand = cardObject.GetComponent<CardToHand>();
        if (cardToHand == null)
        {
            cardToHand = cardObject.AddComponent<CardToHand>();
        }

        card = cards;
        cardToHand.Hand = hand;
        cardToHand.It = cardObject;
        cardToHand.It.tag = "Three";

    }
    void Awake()
    {
        ItName=It;

    }

    void Update()
    {
        if (Hand == null)
        {
            Hand = GameObject.Find("Hand");
        }

        if (It.tag == "first" || It.tag == "Three")
        {

           // It.name = ItName;
            //Debug.Log(ItName);

            It.transform.SetParent(Hand.transform);
            It.transform.localScale = Vector3.one;
            It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
            It.transform.eulerAngles = new Vector3(25, 0, 0);
            It.tag = "second";
        }
    }
}