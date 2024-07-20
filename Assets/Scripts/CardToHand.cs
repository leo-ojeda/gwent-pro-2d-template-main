using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardToHand : MonoBehaviour
{
    public GameObject Hand;
    public GameObject It;
    public Card thisCard;
    public string CardName;
    public string CardType;
    public int Power;
    public string[] Range;
    public string Faction;
    public List<EffectActivation> Efect;
    public Sprite ThisSprite;

    public static void InstantiateCard(GameObject cardPrefab, GameObject hand, Card cardData)
    {
        // Instanciar el prefab de la carta
        GameObject cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        // Configurar el objeto de la carta
        CardToHand cardToHand = cardObject.GetComponent<CardToHand>();
        if (cardToHand == null)
        {
            cardToHand = cardObject.AddComponent<CardToHand>();
        }
        
        cardToHand.Hand = hand;
        cardToHand.It = cardObject;
        cardToHand.It.tag = "first"; // Marcar la carta con la etiqueta "first"

        // Asignar las propiedades del objeto de la carta
        cardToHand.thisCard = cardData;
        cardToHand.CardName = cardData.Name;
        cardToHand.CardType = cardData.Type;
        cardToHand.Power = cardData.Power;
        cardToHand.Range = cardData.Range;
        cardToHand.Faction = cardData.Faction;
        cardToHand.Efect = cardData.OnActivation;
        cardToHand.ThisSprite = Resources.Load<Sprite>(cardData.Name);

        // Asignar el sprite al componente Image del objeto de la carta
        Image imageComponent = cardObject.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = cardToHand.ThisSprite;
        }
    }

    void Update()
    {
        if (Hand == null)
        {
            Hand = GameObject.Find("Hand");
        }

        if (It.tag == "first")
        {
            It.transform.SetParent(Hand.transform);
            It.transform.localScale = Vector3.one;
            It.transform.position = new Vector3(transform.position.x, transform.position.y, -48);
            It.transform.eulerAngles = new Vector3(25, 0, 0);
            It.tag = "second";
        }
    }
}