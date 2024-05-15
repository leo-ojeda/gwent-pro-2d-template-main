using UnityEngine;
using UnityEngine.EventSystems;

public class CardPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject previewPanel; // Panel donde se mostrará la vista previa de la carta
    public GameObject cardPrefab; // Prefab de la carta

    private GameObject instantiatedCard; // Carta instanciada en el panel de vista previa

    // Este método se llama cuando el puntero del mouse entra en el área de la carta
    public void OnPointerEnter(PointerEventData eventData)
    {

        previewPanel = GameObject.Find("Preview");
        // Verificar si la carta prefab y el panel de vista previa son válidos
        if (cardPrefab != null && previewPanel != null)
        {
            // Obtener la carta sobre la cual el mouse está encima
            GameObject hoveredCard = eventData.pointerEnter;
            if (hoveredCard != null)
            {
                // Instanciar una copia de la carta en el panel de vista previa
                instantiatedCard = Instantiate(hoveredCard, previewPanel.transform);
                // Ajustar el tamaño y posición de la carta para que se ajuste al panel
                RectTransform cardRectTransform = instantiatedCard.GetComponent<RectTransform>();
                RectTransform panelRectTransform = previewPanel.GetComponent<RectTransform>();
                cardRectTransform.sizeDelta = panelRectTransform.sizeDelta;
                cardRectTransform.localPosition = Vector3.zero;
                // Activar el panel de vista previa
                previewPanel.SetActive(true);
            }
        }
    }

    // Este método se llama cuando el puntero del mouse sale del área de la carta
    public void OnPointerExit(PointerEventData eventData)
    {
        // Destruir la carta instanciada
        if (instantiatedCard != null)
        {
            Destroy(instantiatedCard);
        }

    }
}
