using UnityEngine;

public static class CardDestroyer
{
    public static void DestroyCard(string cardName)
    {
        // Encuentra todos los GameObjects con el tag "second"
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("second");

        // Recorre cada objeto encontrado
        foreach (GameObject obj in objectsWithTag)
        {
          
            ThisCard thiscardComponent = obj.GetComponent<ThisCard>();

            if (thiscardComponent != null && thiscardComponent.CardName == cardName)
            {
                Object.Destroy(obj);
            }
        }
    }
}