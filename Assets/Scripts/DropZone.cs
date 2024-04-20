using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool allowMultipleCards = true; // Variable para controlar si se permiten múltiples cartas en esta zona
    private bool hasCard = false; // Variable para controlar si ya hay una carta en esta zona

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            ThisCard card = d.GetComponent<ThisCard>();
            if (card != null)
            {
                // Verificar si se permiten múltiples cartas en esta zona y si ya hay una carta en ella
                if (gameObject.tag == "Clima" && !allowMultipleCards && hasCard)
                {
                    Debug.Log("Solo se permite una carta en esta zona.");
                    return;
                }

                if (gameObject.tag == card.Attack)
                {
                    d.parentToReturnTo = this.transform;
                    d.OnEndDrag(eventData);

                    // Desactivar la funcionalidad de arrastrar y soltar para la carta
                    d.SetDraggable(false);

                    // Marcar que ya hay una carta en esta zona
                    hasCard = true;
                }
                else
                {
                    Debug.Log("La carta no puede ser soltada en esta zona.");
                }
            }
        }
    }

    // Método para restablecer hasCard cuando la carta se retira de la zona
    public void ResetHasCard()
    {
        hasCard = false;
    }
}
