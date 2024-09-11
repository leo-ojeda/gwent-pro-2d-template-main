using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public GameObject Hand;
    public GameObject Graveyard;
    public GameObject It;
    public static GameObject ItName;

    public static Card card;
    private Context context;


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
        if (cards.Owner == "Jugador 1")
        {
            cardToHand.It.tag = "Three";
            cardToHand.Hand = hand;
            card = cards;
            cardToHand.It = cardObject;

        }
        if (cards.Owner == "Jugador 2")
        {
            cardToHand.tag = "AIClone";
            card = cards;
            Debug.Log(card.Name);
        }


    }
    void Awake()
    {
        ItName = It;
        context = FindObjectOfType<Context>();

    }

    void Update()
    {
        if (Hand == null)
        {
            Hand = GameObject.Find("Hand");
        }
        if (Graveyard == null)
        {
            Graveyard = GameObject.Find("Cementery");

        }

        if (It.tag == "first" || It.tag == "Three")
        {
            var hand = context.HandOfPlayer("Jugador 1");

            if (hand.Count < 11)
            {
                // Añadir carta a la mano
                It.transform.SetParent(Hand.transform);
                It.transform.localScale = Vector3.one;
                It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
                It.transform.eulerAngles = new Vector3(25, 0, 0);
                It.tag = "Hand";
            }
            else
            {
                
                context.GraveyardOfPlayer("Jugador 1").Add(hand[hand.Count - 1]); 
                hand.RemoveAt(hand.Count - 1); 

                It.transform.SetParent(Graveyard.transform);
                It.transform.localScale = Vector3.one;
                It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
                It.transform.eulerAngles = new Vector3(25, 0, 0);
                It.tag = "cementery";
            }
        }

    }
}