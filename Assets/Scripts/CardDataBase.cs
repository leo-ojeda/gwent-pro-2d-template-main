using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    // Awake es un método de Unity que se llama cuando se inicia el script
    void Awake()
    {
                
        // Inicializa la lista de cartas
        InitializeCardList();

    }

    // Método para inicializar la lista de cartas
    void InitializeCardList()
      {
        cardList.Add(new Card(0, "none", 0, "none", "none", Resources.Load <Sprite>("1"), "none", 0));
        cardList.Add(new Card(1, "Baron Steelheard", 5, "none", "Golden", Resources.Load <Sprite>("2"), "Melee", 1));
        cardList.Add(new Card(2, "Sir Bladefury", 6, "3", "Golden", Resources.Load <Sprite>("3"), "Melee", 1));
        cardList.Add(new Card(3, "Siege Colossus", 4, "1", "Silver", Resources.Load <Sprite>("4"), "Siege", 1));
        cardList.Add(new Card(4, "Knight Ironclaver", 6, "3", "Silver", Resources.Load <Sprite>("5"), "Melee", 1));
        cardList.Add(new Card(5, "Sir Bananathor", 4, "1", "Silver", Resources.Load <Sprite>("6"), "Clima", 1));
        cardList.Add(new Card(6, "Grand Wizard", 3, "none", "Silver", Resources.Load <Sprite>("7"), "Range", 1));
        cardList.Add(new Card(7, "Baron Spellweaver", 4, "none", "Silver", Resources.Load <Sprite>("8"), "Range", 1));
        cardList.Add(new Card(8, "Siege Golossus", 5, "none", "Golden", Resources.Load <Sprite>("9"), "Siege", 1));
        cardList.Add(new Card(9, "Knight Thunderclash", 7, "4", "Leader", Resources.Load <Sprite>("10"), "Leader", 1));
        cardList.Add(new Card(10, "Lord Shadowblade", 6, "3", "Golden", Resources.Load <Sprite>("11"), "Melee", 1));
        cardList.Add(new Card(11, "MoonBlade", 5, "none", "Golden", Resources.Load <Sprite>("12"), "Melee", 1));
        cardList.Add(new Card(12, "Swiftstriker", 5, "1", "Golden", Resources.Load <Sprite>("13"), "Range", 1));
        cardList.Add(new Card(13, "Cataclysmic Rumbler", 3, "none", "Silver", Resources.Load <Sprite>("14"), "Range", 1));
        cardList.Add(new Card(14, "BattleFury", 4, "none", "Silver", Resources.Load <Sprite>("15"), "Melee", 1));
        cardList.Add(new Card(15, "Titan War Machine", 6, "none", "Golden", Resources.Load <Sprite>("16"), "Siege", 1));
    }
}
