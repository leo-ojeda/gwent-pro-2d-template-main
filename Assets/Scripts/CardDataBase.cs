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
        cardList.Add(new Card( "none", 0, "none", "none", Resources.Load<Sprite>("1"), new string[]{"none"}, 0, "none"));
        cardList.Add(new Card("Baron Steelheard", 5, "none", "Golden", Resources.Load<Sprite>("22"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card("Sir Bladefury", 6, "3", "Golden", Resources.Load<Sprite>("4"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card("Siege Colossus", 4, "1", "Silver", Resources.Load<Sprite>("5"), new string[]{"Siege"}, 1, "Torment"));
        cardList.Add(new Card("Knight Ironclaver", 6, "3", "Silver", Resources.Load<Sprite>("6"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card("Sir Bananathor", 4, "1", "Silver", Resources.Load<Sprite>("7"), new string[]{"Ranged"}, 1, "Torment"));
        cardList.Add(new Card("Grand Wizard", 3, "none", "Silver", Resources.Load<Sprite>("10"), new string[]{"Ranged"}, 1, "Torment"));
        cardList.Add(new Card("Baron Spellweaver", 4, "none", "Silver", Resources.Load<Sprite>("11"), new string[]{"Ranged"}, 1, "Torment"));
        cardList.Add(new Card("Siege Golossus", 5, "none", "Golden", Resources.Load<Sprite>("12"),new string[]{"Siege"}, 1, "Torment"));
        cardList.Add(new Card("Knight Thunderclash", 7, "4", "Leader", Resources.Load<Sprite>("13"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card( "Lord Shadowblade", 6, "3", "Golden", Resources.Load<Sprite>("14"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card( "MoonBlade", 5, "none", "Golden", Resources.Load<Sprite>("17"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card( "Swiftstriker", 5, "1", "Golden", Resources.Load<Sprite>("18"), new string[]{"Ranged"}, 1, "Torment"));
        cardList.Add(new Card( "Cataclysmic Rumbler", 3, "none", "Silver", Resources.Load<Sprite>("19"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card( "BattleFury", 4, "none", "Silver", Resources.Load<Sprite>("21"), new string[]{"Melee"}, 1, "Torment"));
        cardList.Add(new Card( "Titan War Machine", 6, "none", "Golden", Resources.Load<Sprite>("15"), new string[]{"Ranged"}, 1, "Torment"));
        //cardList.Add(new Card(16, "Deep Abyss Citadel", 0, "2", "Silver", Resources.Load<Sprite>("2"), "Increase", 1, "Torment"));
        //cardList.Add(new Card(17, "Chaos Banana", 0, "2", "Silver", Resources.Load<Sprite>("3"), "Increase", 1, "Torment"));
        //cardList.Add(new Card(18, "Master Sorcerer", 0, "3", "Silver", Resources.Load<Sprite>("8"), "Clima", 1, "Torment"));
        //cardList.Add(new Card(19, "Citadel of Captive Stars", 0, "1", "Silver", Resources.Load<Sprite>("16"), "Clima", 1, "Torment"));
        cardList.Add(new Card( "Catapulta de Fuego", 4, "1", "Silver", Resources.Load<Sprite>("23"), new string[]{"Siege"}, 1, "Fire"));
        cardList.Add(new Card( "Minero", 3, "1", "Silver", Resources.Load<Sprite>("24"), new string[]{"Melee"}, 1, "Fire"));
        cardList.Add(new Card( "Lanza Roca", 4, "1", "Silver", Resources.Load<Sprite>("25"), new string[]{"Siege"}, 1, "Fire"));
       // cardList.Add(new Card(23, "Cataclismo", 0, "1", "Silver", Resources.Load<Sprite>("26"), "Clima", 1, "Fire"));
        cardList.Add(new Card( "Antorcha", 4, "1", "Silver", Resources.Load<Sprite>("27"), new string[]{"Ranged"}, 1, "Fire"));
        cardList.Add(new Card( "Gran Lanza Roca", 6, "1", "Golden", Resources.Load<Sprite>("28"), new string[]{"Siege"}, 1, "Fire"));
        //cardList.Add(new Card(26, "Ola de LLamas", 0, "1", "Silver", Resources.Load<Sprite>("29"), "Aumento", 1, "Fire"));
        //cardList.Add(new Card(27, "LLuvia de fuego", 0, "1", "Silver", Resources.Load<Sprite>("30"), "Despeje", 1, "Fire"));
        cardList.Add(new Card( "Dragon Guardian", 4, "1", "Silver", Resources.Load<Sprite>("31"), new string[]{"Melee"}, 1, "Fire"));
        cardList.Add(new Card( "Samurai", 8, "1", "Leader", Resources.Load<Sprite>("32"), new string[]{"Melee"}, 1, "Fire"));
        cardList.Add(new Card( "Mini Ninja", 5, "1", "Golden", Resources.Load<Sprite>("33"), new string[]{"Melee"}, 1, "Fire"));
        cardList.Add(new Card( "Arquero", 3, "1", "Silver", Resources.Load<Sprite>("34"), new string[]{"Ranged"}, 1, "Fire"));
        cardList.Add(new Card( "Caballero Solar", 6, "1", "Golden", Resources.Load<Sprite>("35"), new string[]{"Melee"}, 1, "Fire"));
        //cardList.Add(new Card(33, "Horda de Fuego", 0, "1", "Silver", Resources.Load<Sprite>("36"), "Clima", 1, "Fire"));
        cardList.Add(new Card( "Escudero", 5, "1", "Golden", Resources.Load<Sprite>("37"), new string[]{"Melee"}, 1, "Fire"));
       // cardList.Add(new Card(35, "Esbirro", 0, "1", "Silver", Resources.Load<Sprite>("38"), "Señuelo", 1, "Fire"));
        cardList.Add(new Card( "bolero", 4, "1", "Silver", Resources.Load<Sprite>("39"), new string[]{"Siege"}, 1, "Fire"));
        cardList.Add(new Card( "Gran Antorcha", 5, "1", "Silver", Resources.Load<Sprite>("40"), new string[]{"Ranged"}, 1, "Fire"));
        cardList.Add(new Card( "Haku", 3, "1", "Silver", Resources.Load<Sprite>("41"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Lanza hojas", 4, "1", "Silver", Resources.Load<Sprite>("42"), new string[]{"Siege"}, 1, "Forest"));
        cardList.Add(new Card( "Leñador", 7, "1", "Leader", Resources.Load<Sprite>("43"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Cazador", 5, "1", "Golden", Resources.Load<Sprite>("45"), new string[]{"Ranged"}, 1, "Forest"));
        //cardList.Add(new Card(42, "Lanza hojas inferior", 3, "1", "Silver", Resources.Load<Sprite>("46"), "Despeje", 1, "Forest"));
       // cardList.Add(new Card(43, "Densidad", 0, "1", "Silver", Resources.Load<Sprite>("47"), "Clima", 1, "Forest"));
       // cardList.Add(new Card(44, "Bosque Profundo", 0, "1", "Silver", Resources.Load<Sprite>("48"), "Clima", 1, "Forest"));
        cardList.Add(new Card( "Gran Cazador", 6, "1", "Golden", Resources.Load<Sprite>("49"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Vigilante", 3, "1", "Silver", Resources.Load<Sprite>("50"), new string[]{"Siege"}, 1, "Forest"));
        cardList.Add(new Card( "Francotirador", 5, "1", "Golden", Resources.Load<Sprite>("51"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Cazador Menor", 3, "1", "Silver", Resources.Load<Sprite>("52"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Ballestero", 4, "1", "Silver", Resources.Load<Sprite>("53"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Arquero", 3, "1", "Silver", Resources.Load<Sprite>("54"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Defensor", 5, "1", "Golden", Resources.Load<Sprite>("55"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Flecha Tactica", 4, "1", "Silver", Resources.Load<Sprite>("56"), new string[]{"Ranged"}, 1, "Forest"));
        cardList.Add(new Card( "Tanque", 5, "1", "Silver", Resources.Load<Sprite>("57"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Asesino", 6, "1", "Golden", Resources.Load<Sprite>("58"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Espadachin", 6, "1", "Golden", Resources.Load<Sprite>("59"), new string[]{"Melee"}, 1, "Forest"));
        cardList.Add(new Card( "Ballestero Superior", 5, "1", "Golden", Resources.Load<Sprite>("44"), new string[]{"Ranged"}, 1, "Forest"));
        
    }
}
