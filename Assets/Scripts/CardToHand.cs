using UnityEngine;

public class CardToHand : MonoBehaviour
{
    public GameObject Hand;
    public GameObject HandAI;
    public GameObject Graveyard;
    public GameObject It;
    public static GameObject ItName;

    public static Card card;
    private Context context;
    private int HandCount;


    void Awake()
    {
        ItName = It;
        context = FindObjectOfType<Context>();
        HandCount = 0;
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
                HandCount++;
                It.tag = "Hand"; // Actualizar el tag
            }
            else
            {
                // Mover la última carta al cementerio si la mano está llena
                context.GraveyardOfPlayer("Jugador 1").Add(hand[hand.Count - 1]);
                hand.RemoveAt(hand.Count - 1);
                It.transform.SetParent(Graveyard.transform);
                It.transform.localScale = Vector3.one;
                It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
                It.transform.eulerAngles = new Vector3(25, 0, 0);
                HandCount--;
                It.tag = "cementery"; // Actualizar el tag
            }
        }
    }
}
