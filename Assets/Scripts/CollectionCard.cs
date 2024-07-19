using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    public GameObject cardPrefab; // Prefab de la carta
    public Transform cardPanelParent; // Padre de los paneles de carta

    void Start()
    {
        InstantiateCards();
    }

    void InstantiateCards()
    {
        if (CardDatabase.cardList == null || CardDatabase.cardList.Count == 0)
        {
            Debug.LogError("CardDatabase.cardList está vacío o es nulo.");
            return;
        }


        foreach (Card card in CardDatabase.cardList)
        {
            if (card == null)
            {
                continue;
            }


            // Instancia el prefab de la carta en el panel
            GameObject cardInstance = Instantiate(cardPrefab, cardPanelParent);

            // Configura la imagen de la carta
            Image cardImage = cardInstance.GetComponentInChildren<Image>(); // Busca la imagen en los hijos del prefab4

            if (cardImage != null)
            {
                string imageName = card.Name;
                Sprite sprite = Resources.Load<Sprite>(imageName);
                if (sprite != null)
                {
                    cardImage.sprite = sprite;
                }
                else
                {
                    Debug.LogError("No se pudo cargar la imagen de la carta: " + imageName);
                }
            }
            else
            {
                Debug.LogError("No se encontró el componente Image en el prefab de la carta.");
            }
        }
    }
}
