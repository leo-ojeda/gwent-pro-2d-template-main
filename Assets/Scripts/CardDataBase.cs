using System.Collections.Generic;
using Unity.VisualScripting;
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
        var boostEffect = new EffectActivation(EffectLibrary.BoostPower(1), new Selector("Field", false, ""));
        var damageEffect = new EffectActivation(EffectLibrary.Damage(1), new Selector("otherField", false, ""));
        var drawEffect = new EffectActivation(EffectLibrary.Draw(), new Selector("Deck", true, ""));
        var returnToDeckEffect = new EffectActivation(EffectLibrary.ReturnToDeck(), new Selector("Field", false, ""));
        var IncreaseEffect = new EffectActivation(EffectLibrary.Increase(1), new Selector("Field", false, ""));


        cardList.Add(new Card("none", 0, "none", new string[] { "none" }, "none", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Guerrero", 5, "Golden", new string[] { "Melee" }, "Torment", new List<EffectActivation> {}, " "));
        cardList.Add(new Card("Espadachin", 6, "Golden", new string[] { "Melee" }, "Torment", new List<EffectActivation> { }, " "));
        cardList.Add(new Card("Colosal", 4, "Silver", new string[] { "Siege" }, "Torment", new List<EffectActivation> {}, ""));
        cardList.Add(new Card("Caballero de Hacha", 6, "Golden", new string[] { "Melee" }, "Torment", new List<EffectActivation> {}, " "));
        cardList.Add(new Card("Tormenta", 4, "Silver", new string[] { "Ranged" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Gran Mago", 3, "Silver", new string[] { "Ranged" }, "Torment", new List<EffectActivation>(), "  "));
        cardList.Add(new Card("Defensor de Hechizo", 4, "Silver", new string[] { "Ranged" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Asediador", 5, "Golden", new string[] { "Siege" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Caballero del Trueno", 0, "Leader", new string[] { "Melee" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Espada Oscura", 6, "Golden", new string[] { "Melee" }, "Torment", new List<EffectActivation> {returnToDeckEffect }, " "));
        cardList.Add(new Card("Caballero Luna", 5, "Golden", new string[] { "Melee" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Aplastador", 5, "Golden", new string[] { "Ranged" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Carnicero", 3, "Silver", new string[] { "Melee" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Hacha de batalla", 4, "Silver", new string[] { "Melee" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Maquina de Guerra", 6, "Golden", new string[] { "Siege" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ciudad del Abismo", 0, "Increase", new string[] { "Melee", "Ranged", "Siege" }, "Torment", new List<EffectActivation> { IncreaseEffect }, ""));
        cardList.Add(new Card("Banana Del Caos", 0, "Increase", new string[] { "Melee", "Ranged", "Siege" }, "Torment", new List<EffectActivation> { IncreaseEffect }, ""));
        cardList.Add(new Card( "Sacerdote", 0,  "Clima", new string[] { "Melee", "Ranged", "Siege" },  "Torment", new List<EffectActivation> { damageEffect}, ""));
        cardList.Add(new Card( "Ciudad Nublada", 0,  "Clima",  new string[] { "Melee", "Ranged", "Siege" },  "Torment", new List<EffectActivation> { damageEffect}, ""));
        cardList.Add(new Card("Catapulta de Fuego", 4, "Silver", new string[] { "Siege" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Minero", 3, "Silver", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza Roca", 4, "Silver", new string[] { "Siege" }, "Fire", new List<EffectActivation>(), " "));
        // cardList.Add(new Card( "Cataclismo", 0,  "Silver", Resources.Load<Sprite>("26"), "Clima",  "Fire",new List<EffectActivation>()));
        cardList.Add(new Card("Antorcha", 4, "Silver", new string[] { "Ranged" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Gran Lanza Roca", 6, "Golden", new string[] { "Siege" }, "Fire", new List<EffectActivation>(), " "));
        //cardList.Add(new Card( "Ola de LLamas", 0,  "Silver",  "Aumento",  "Fire"));
        //cardList.Add(new Card( "LLuvia de fuego", 0,  "Silver", "Despeje",  "Fire"));
        cardList.Add(new Card("Dragon Guardian", 4, "Silver", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Samurai", 0, "Leader", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Mini Ninja", 5, "Golden", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Arquero", 3, "Silver", new string[] { "Ranged" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Caballero Solar", 6, "Golden", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        //cardList.Add(new Card( "Horda de Fuego", 0,  "Silver",  "Clima",  "Fire",new List<EffectActivation>()));
        cardList.Add(new Card("Escudero", 5, "Golden", new string[] { "Melee" }, "Fire", new List<EffectActivation>(), " "));
        // cardList.Add(new Card( "Esbirro", 0,  "Silver",  "Señuelo",  "Fire",new List<EffectActivation>()));
        cardList.Add(new Card("bolero", 4, "Silver", new string[] { "Siege" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Antorcha Superior", 5, "Silver", new string[] { "Ranged" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Haku", 3, "Silver", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza hojas", 4, "Silver", new string[] { "Siege" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Leñador", 0, "Leader", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Cazador", 5, "Golden", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        //cardList.Add(new Card("Lanza hojas inferior", 3,  "Silver",  "Despeje",  "Forest",new List<EffectActivation>()));
        // cardList.Add(new Card( "Densidad", 0,  "Silver",  "Clima",  "Forest",new List<EffectActivation>()));
        // cardList.Add(new Card( "Bosque Profundo", 0, " "Silver",  "Clima",  "Forest",new List<EffectActivation>()));
        cardList.Add(new Card("Gran Cazador", 6, "Golden", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza hojas inferior", 3, "Silver", new string[] { "Siege" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Francotirador", 5, "Golden", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Cazador Menor", 3, "Silver", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ballestero", 4, "Silver", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Arquero Hoja", 3, "Silver", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Defensor", 5, "Golden", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Flecha Tactica", 4, "Silver", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Tanque", 5, "Silver", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Asesino", 6, "Golden", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Espadachin H", 6, "Golden", new string[] { "Melee" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ballestero Superior", 5, "Golden", new string[] { "Ranged" }, "Forest", new List<EffectActivation>(), " "));

    }
}
