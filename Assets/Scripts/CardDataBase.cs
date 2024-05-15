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
        cardList.Add(new Card(0, "none", 0, "none", "none", Resources.Load<Sprite>("1"), "none", 0, "none"));
        cardList.Add(new Card(1, "Baron Steelheard", 5, "none", "Golden", Resources.Load<Sprite>("22"), "Melee", 1, "Torment"));
        cardList.Add(new Card(2, "Sir Bladefury", 6, "3", "Golden", Resources.Load<Sprite>("4"), "Melee", 1, "Torment"));
        cardList.Add(new Card(3, "Siege Colossus", 4, "1", "Silver", Resources.Load<Sprite>("5"), "Siege", 1, "Torment"));
        cardList.Add(new Card(4, "Knight Ironclaver", 6, "3", "Silver", Resources.Load<Sprite>("6"), "Melee", 1, "Torment"));
        cardList.Add(new Card(5, "Sir Bananathor", 4, "1", "Silver", Resources.Load<Sprite>("7"), "Range", 1, "Torment"));
        cardList.Add(new Card(6, "Grand Wizard", 3, "none", "Silver", Resources.Load<Sprite>("10"), "Range", 1, "Torment"));
        cardList.Add(new Card(7, "Baron Spellweaver", 4, "none", "Silver", Resources.Load<Sprite>("11"), "Range", 1, "Torment"));
        cardList.Add(new Card(8, "Siege Golossus", 5, "none", "Golden", Resources.Load<Sprite>("12"), "Siege", 1, "Torment"));
        cardList.Add(new Card(9, "Knight Thunderclash", 7, "4", "Leader", Resources.Load<Sprite>("13"), "Leader", 1, "Torment"));
        cardList.Add(new Card(10, "Lord Shadowblade", 6, "3", "Golden", Resources.Load<Sprite>("14"), "Melee", 1, "Torment"));
        cardList.Add(new Card(11, "MoonBlade", 5, "none", "Golden", Resources.Load<Sprite>("17"), "Melee", 1, "Torment"));
        cardList.Add(new Card(12, "Swiftstriker", 5, "1", "Golden", Resources.Load<Sprite>("18"), "Range", 1, "Torment"));
        cardList.Add(new Card(13, "Cataclysmic Rumbler", 3, "none", "Silver", Resources.Load<Sprite>("19"), "Melee", 1, "Torment"));
        cardList.Add(new Card(14, "BattleFury", 4, "none", "Silver", Resources.Load<Sprite>("21"), "Melee", 1, "Torment"));
        cardList.Add(new Card(15, "Titan War Machine", 6, "none", "Golden", Resources.Load<Sprite>("15"), "Siege", 1, "Torment"));
        cardList.Add(new Card(16, "Deep Abyss Citadel", 0, "2", "Silver", Resources.Load<Sprite>("2"), "Increase", 1, "Torment"));
        cardList.Add(new Card(17, "Chaos Banana", 0, "2", "Silver", Resources.Load<Sprite>("3"), "Increase", 1, "Torment"));
        cardList.Add(new Card(18, "Master Sorcerer", 0, "3", "Silver", Resources.Load<Sprite>("8"), "Clima", 1, "Torment"));
        cardList.Add(new Card(19, "Citadel of Captive Stars", 0, "1", "Silver", Resources.Load<Sprite>("16"), "Clima", 1, "Torment"));
        cardList.Add(new Card(20, "Catapulta de Fuego", 4, "1", "Silver", Resources.Load<Sprite>("23"), "Siege", 1, "Fire"));
        cardList.Add(new Card(21, "Minero", 3, "1", "Silver", Resources.Load<Sprite>("24"), "Melee", 1, "Fire"));
        cardList.Add(new Card(22, "Lanza Roca", 4, "1", "Silver", Resources.Load<Sprite>("25"), "Siege", 1, "Fire"));
        cardList.Add(new Card(23, "Cataclismo", 0, "1", "Silver", Resources.Load<Sprite>("26"), "Clima", 1, "Fire"));
        cardList.Add(new Card(24, "Antorcha", 4, "1", "Silver", Resources.Load<Sprite>("27"), "Range", 1, "Fire"));
        cardList.Add(new Card(25, "Gran Lanza Roca", 6, "1", "Golden", Resources.Load<Sprite>("28"), "Siege", 1, "Fire"));
        cardList.Add(new Card(26, "Ola de LLamas", 0, "1", "Silver", Resources.Load<Sprite>("29"), "Aumento", 1, "Fire"));
        cardList.Add(new Card(27, "LLuvia de fuego", 0, "1", "Silver", Resources.Load<Sprite>("30"), "Despeje", 1, "Fire"));
        cardList.Add(new Card(28, "Dragon Guardian", 4, "1", "Silver", Resources.Load<Sprite>("31"), "Melee", 1, "Fire"));
        cardList.Add(new Card(29, "Samurai", 8, "1", "Leader", Resources.Load<Sprite>("32"), "Leader", 1, "Fire"));
        cardList.Add(new Card(30, "Mini Ninja", 5, "1", "Golden", Resources.Load<Sprite>("33"), "Melee", 1, "Fire"));
        cardList.Add(new Card(31, "Arquero", 3, "1", "Silver", Resources.Load<Sprite>("34"), "Range", 1, "Fire"));
        cardList.Add(new Card(32, "Caballero Solar", 6, "1", "Golden", Resources.Load<Sprite>("35"), "Melee", 1, "Fire"));
        cardList.Add(new Card(33, "Horda de Fuego", 0, "1", "Silver", Resources.Load<Sprite>("36"), "Clima", 1, "Fire"));
        cardList.Add(new Card(34, "Escudero", 5, "1", "Golden", Resources.Load<Sprite>("37"), "Melee", 1, "Fire"));
        cardList.Add(new Card(35, "Esbirro", 0, "1", "Silver", Resources.Load<Sprite>("38"), "Señuelo", 1, "Fire"));
        cardList.Add(new Card(36, "bolero", 4, "1", "Silver", Resources.Load<Sprite>("39"), "Siege", 1, "Fire"));
        cardList.Add(new Card(37, "Gran Antorcha", 5, "1", "Silver", Resources.Load<Sprite>("40"), "Range", 1, "Fire"));
        cardList.Add(new Card(38, "Haku", 3, "1", "Silver", Resources.Load<Sprite>("41"), "Melee", 1, "Forest"));
        cardList.Add(new Card(39, "Lanza hojas", 4, "1", "Silver", Resources.Load<Sprite>("42"), "Siege", 1, "Forest"));
        cardList.Add(new Card(40, "Leñador", 7, "1", "Leader", Resources.Load<Sprite>("43"), "Leader", 1, "Forest"));
        cardList.Add(new Card(41, "Cazador", 5, "1", "Golden", Resources.Load<Sprite>("45"), "Range", 1, "Forest"));
        cardList.Add(new Card(42, "Lanza hojas inferior", 3, "1", "Silver", Resources.Load<Sprite>("46"), "Despeje", 1, "Forest"));
        cardList.Add(new Card(43, "Densidad", 0, "1", "Silver", Resources.Load<Sprite>("47"), "Clima", 1, "Forest"));
        cardList.Add(new Card(44, "Bosque Profundo", 0, "1", "Silver", Resources.Load<Sprite>("48"), "Clima", 1, "Forest"));
        cardList.Add(new Card(45, "Gran Cazador", 6, "1", "Golden", Resources.Load<Sprite>("49"), "Range", 1, "Forest"));
        cardList.Add(new Card(46, "Vigilante", 3, "1", "Silver", Resources.Load<Sprite>("50"), "Siege", 1, "Forest"));
        cardList.Add(new Card(47, "Francotirador", 5, "1", "Golden", Resources.Load<Sprite>("51"), "Range", 1, "Forest"));
        cardList.Add(new Card(48, "Cazador Menor", 3, "1", "Silver", Resources.Load<Sprite>("52"), "Range", 1, "Forest"));
        cardList.Add(new Card(49, "Ballestero", 4, "1", "Silver", Resources.Load<Sprite>("53"), "Range", 1, "Forest"));
        cardList.Add(new Card(50, "Arquero", 3, "1", "Silver", Resources.Load<Sprite>("54"), "Range", 1, "Forest"));
        cardList.Add(new Card(51, "Defensor", 5, "1", "Golden", Resources.Load<Sprite>("55"), "Melee", 1, "Forest"));
        cardList.Add(new Card(52, "Flecha Tactica", 4, "1", "Silver", Resources.Load<Sprite>("56"), "Range", 1, "Forest"));
        cardList.Add(new Card(53, "Tanque", 5, "1", "Silver", Resources.Load<Sprite>("57"), "Melee", 1, "Forest"));
        cardList.Add(new Card(54, "Asesino", 6, "1", "Golden", Resources.Load<Sprite>("58"), "Melee", 1, "Forest"));
        cardList.Add(new Card(55, "Espadachin", 6, "1", "Golden", Resources.Load<Sprite>("59"), "Melee", 1, "Forest"));
        cardList.Add(new Card(56, "Ballestero Superior", 5, "1", "Golden", Resources.Load<Sprite>("44"), "Range", 1, "Forest"));
        
    }
}
