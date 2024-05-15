using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    public GameObject cardPrefab; // Prefab de la carta
    public Transform cardPanelParent; // Padre de los paneles de carta
    public List<Card> cardList = new List<Card>(); // Lista de cartas

    void Start()
    {
        // Instancia cada carta en el panel
        InstantiateCards();
    }

    void InstantiateCards()
    {

        // Instancia cada carta y las posiciona una al lado de la otra en el panel
        for (int i = 0; i < cardList.Count; i++)
        {
            // Instancia el prefab de la carta en el panel
            GameObject cardInstance = Instantiate(cardPrefab, cardPanelParent);

            // Configura la imagen de la carta
            Image cardImage = cardInstance.GetComponentInChildren<Image>(); // Busca la imagen en los hijos del prefab
            if (cardImage != null)
            {
                // Carga la imagen de la carta desde la carpeta de recursos
                string imageName = (i + 2).ToString(); // El nombre de la imagen es el número de la carta
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
