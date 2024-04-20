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
        cardList.Add(new Card(0, "none", 0, "none", "none", Resources.Load<Sprite>("1"), "none", 0));
        cardList.Add(new Card(1, "Baron Steelheard", 5, "none", "Golden", Resources.Load<Sprite>("22"), "Melee", 1));
        cardList.Add(new Card(2, "Sir Bladefury", 6, "3", "Golden", Resources.Load<Sprite>("4"), "Melee", 1));
        cardList.Add(new Card(3, "Siege Colossus", 4, "1", "Silver", Resources.Load<Sprite>("5"), "Siege", 1));
        cardList.Add(new Card(4, "Knight Ironclaver", 6, "3", "Silver", Resources.Load<Sprite>("6"), "Melee", 1));
        cardList.Add(new Card(5, "Sir Bananathor", 4, "1", "Silver", Resources.Load<Sprite>("7"), "Range", 1));
        cardList.Add(new Card(6, "Grand Wizard", 3, "none", "Silver", Resources.Load<Sprite>("10"), "Range", 1));
        cardList.Add(new Card(7, "Baron Spellweaver", 4, "none", "Silver", Resources.Load<Sprite>("11"), "Range", 1));
        cardList.Add(new Card(8, "Siege Golossus", 5, "none", "Golden", Resources.Load<Sprite>("12"), "Siege", 1));
        cardList.Add(new Card(9, "Knight Thunderclash", 7, "4", "Leader", Resources.Load<Sprite>("13"), "Leader", 1));
        cardList.Add(new Card(10, "Lord Shadowblade", 6, "3", "Golden", Resources.Load<Sprite>("14"), "Melee", 1));
        cardList.Add(new Card(11, "MoonBlade", 5, "none", "Golden", Resources.Load<Sprite>("17"), "Melee", 1));
        cardList.Add(new Card(12, "Swiftstriker", 5, "1", "Golden", Resources.Load<Sprite>("18"), "Range", 1));
        cardList.Add(new Card(13, "Cataclysmic Rumbler", 3, "none", "Silver", Resources.Load<Sprite>("19"), "Melee", 1));
        cardList.Add(new Card(14, "BattleFury", 4, "none", "Silver", Resources.Load<Sprite>("21"), "Melee", 1));
        cardList.Add(new Card(15, "Titan War Machine", 6, "none", "Golden", Resources.Load<Sprite>("15"), "Siege", 1));
        cardList.Add(new Card(16, "Deep Abyss Citadel", 0, "2", "Silver", Resources.Load<Sprite>("2"), "Increase", 1));
        cardList.Add(new Card(17, "Chaos Banana", 0, "2", "Silver", Resources.Load<Sprite>("3"), "Increase", 1));
        cardList.Add(new Card(18, "Master Sorcerer", 0, "3", "Silver", Resources.Load<Sprite>("8"), "Clima", 1));
        cardList.Add(new Card(19, "Citadel of Captive Stars", 0, "1", "Silver", Resources.Load<Sprite>("16"), "Clima", 1));
    }
}
