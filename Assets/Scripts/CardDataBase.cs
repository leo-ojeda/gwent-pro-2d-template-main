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
        cardList.Add(new Card("none", 0, "none", "none",  new string[] { "none" }, "none"));
        cardList.Add(new Card("Guerrero", 5, "none", "Golden",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Espadachin", 6, "3", "Golden",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Colosal", 4, "1", "Silver",  new string[] { "Siege" }, "Torment"));
        cardList.Add(new Card("Caballero de Hacha", 6, "3", "Silver",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Tormenta", 4, "1", "Silver",  new string[] { "Ranged" }, "Torment"));
        cardList.Add(new Card("Gran Mago", 3, "none", "Silver",  new string[] { "Ranged" }, "Torment"));
        cardList.Add(new Card("Defensor de Hechizo", 4, "none", "Silver",  new string[] { "Ranged" }, "Torment"));
        cardList.Add(new Card("Asediador", 5, "none", "Golden", new string[] { "Siege" }, "Torment"));
        cardList.Add(new Card("Caballero del Trueno", 0, "4", "Leader",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Espada Oscura", 6, "3", "Golden",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Caballero Luna", 5, "none", "Golden",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Aplastador", 5, "1", "Golden",  new string[] { "Ranged" }, "Torment"));
        cardList.Add(new Card("Carnicero", 3, "none", "Silver",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Hacha de batalla", 4, "none", "Silver",  new string[] { "Melee" }, "Torment"));
        cardList.Add(new Card("Maquina de Guerra", 6, "none", "Golden",  new string[] { "Siege" }, "Torment"));
        //cardList.Add(new Card( "Ciudad del Abismo", 0, "2", "Silver",  "Increase",  "Torment"));
        //cardList.Add(new Card( "Banana Del Caos", 0, "2", "Silver", "Increase",  "Torment"));
        //cardList.Add(new Card( "Sacerdote", 0, "3", "Silver",  "Clima",  "Torment"));
        //cardList.Add(new Card( "Ciudad Nublada", 0, "1", "Silver",  "Clima",  "Torment"));
        cardList.Add(new Card("Catapulta de Fuego", 4, "1", "Silver",  new string[] { "Siege" }, "Fire"));
        cardList.Add(new Card("Minero", 3, "1", "Silver",  new string[] { "Melee" }, "Fire"));
        cardList.Add(new Card("Lanza Roca", 4, "1", "Silver",  new string[] { "Siege" }, "Fire"));
        // cardList.Add(new Card( "Cataclismo", 0, "1", "Silver", Resources.Load<Sprite>("26"), "Clima",  "Fire"));
        cardList.Add(new Card("Antorcha", 4, "1", "Silver",  new string[] { "Ranged" }, "Fire"));
        cardList.Add(new Card("Gran Lanza Roca", 6, "1", "Golden",  new string[] { "Siege" }, "Fire"));
        //cardList.Add(new Card( "Ola de LLamas", 0, "1", "Silver",  "Aumento",  "Fire"));
        //cardList.Add(new Card( "LLuvia de fuego", 0, "1", "Silver", "Despeje",  "Fire"));
        cardList.Add(new Card("Dragon Guardian", 4, "1", "Silver", new string[] { "Melee" }, "Fire"));
        cardList.Add(new Card("Samurai", 0, "1", "Leader",  new string[] { "Melee" }, "Fire"));
        cardList.Add(new Card("Mini Ninja", 5, "1", "Golden",  new string[] { "Melee" }, "Fire"));
        cardList.Add(new Card("Arquero", 3, "1", "Silver",  new string[] { "Ranged" }, "Fire"));
        cardList.Add(new Card("Caballero Solar", 6, "1", "Golden",  new string[] { "Melee" }, "Fire"));
        //cardList.Add(new Card( "Horda de Fuego", 0, "1", "Silver",  "Clima",  "Fire"));
        cardList.Add(new Card("Escudero", 5, "1", "Golden", new string[] { "Melee" }, "Fire"));
        // cardList.Add(new Card( "Esbirro", 0, "1", "Silver",  "Señuelo",  "Fire"));
        cardList.Add(new Card("bolero", 4, "1", "Silver",  new string[] { "Siege" }, "Fire"));
        cardList.Add(new Card("Antorcha Superior", 5, "1", "Silver", new string[] { "Ranged" }, "Fire"));
        cardList.Add(new Card("Haku", 3, "1", "Silver",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Lanza hojas", 4, "1", "Silver",  new string[] { "Siege" }, "Forest"));
        cardList.Add(new Card("Leñador", 0, "1", "Leader",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Cazador", 5, "1", "Golden", new string[] { "Ranged" }, "Forest"));
        //cardList.Add(new Card("Lanza hojas inferior", 3, "1", "Silver",  "Despeje",  "Forest"));
        // cardList.Add(new Card( "Densidad", 0, "1", "Silver",  "Clima",  "Forest"));
        // cardList.Add(new Card( "Bosque Profundo", 0, "1", "Silver",  "Clima",  "Forest"));
        cardList.Add(new Card("Gran Cazador", 6, "1", "Golden",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Lanza hojas inferior", 3, "1", "Silver", new string[] { "Siege" }, "Forest"));
        cardList.Add(new Card("Francotirador", 5, "1", "Golden",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Cazador Menor", 3, "1", "Silver",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Ballestero", 4, "1", "Silver",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Arquero Hoja", 3, "1", "Silver",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Defensor", 5, "1", "Golden",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Flecha Tactica", 4, "1", "Silver",  new string[] { "Ranged" }, "Forest"));
        cardList.Add(new Card("Tanque", 5, "1", "Silver",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Asesino", 6, "1", "Golden",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Espadachin H", 6, "1", "Golden",  new string[] { "Melee" }, "Forest"));
        cardList.Add(new Card("Ballestero Superior", 5, "1", "Golden",  new string[] { "Ranged" }, "Forest"));

    }
}
