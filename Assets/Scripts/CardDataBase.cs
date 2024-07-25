using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {

        InitializeCardList();

    }

    void InitializeCardList()
    {
        var boostEffect = new EffectActivation(EffectLibrary.BoostPower(1), new Selector("Field", false, ""));
        var damageEffect = new EffectActivation(EffectLibrary.Damage(1), new Selector("otherField", false, ""));
        var drawEffect = new EffectActivation(EffectLibrary.Draw(), new Selector("Deck", true, ""));
        var returnToDeckEffect = new EffectActivation(EffectLibrary.ReturnToDeck(), new Selector("Field", false, ""));
        var IncreaseEffect = new EffectActivation(EffectLibrary.Increase(1), new Selector("Field", false, ""));


        cardList.Add(new Card("none", 0, "none", new string[] { "none" }, "none", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Guerrero", 5, "Golden", new string[] { "M" }, "Torment", new List<EffectActivation> {}, " "));
        cardList.Add(new Card("Espadachin", 6, "Golden", new string[] { "M" }, "Torment", new List<EffectActivation> { }, " "));
        cardList.Add(new Card("Colosal", 4, "Silver", new string[] { "S" }, "Torment", new List<EffectActivation> {}, ""));
        cardList.Add(new Card("Caballero de Hacha", 6, "Golden", new string[] { "M" }, "Torment", new List<EffectActivation> {}, " "));
        cardList.Add(new Card("Tormenta", 4, "Silver", new string[] { "R" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Gran Mago", 3, "Silver", new string[] { "R" }, "Torment", new List<EffectActivation>(), "  "));
        cardList.Add(new Card("Defensor de Hechizo", 4, "Silver", new string[] { "R" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Asediador", 5, "Golden", new string[] { "S" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Caballero del Trueno", 0, "Leader", new string[] { "M" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Espada Oscura", 6, "Golden", new string[] { "M" }, "Torment", new List<EffectActivation> {returnToDeckEffect }, " "));
        cardList.Add(new Card("Caballero Luna", 5, "Golden", new string[] { "M" }, "Torment", new List<EffectActivation>(), ""));
        cardList.Add(new Card("Aplastador", 5, "Golden", new string[] { "R" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Carnicero", 3, "Silver", new string[] { "M" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Hacha de batalla", 4, "Silver", new string[] { "M" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Maquina de Guerra", 6, "Golden", new string[] { "S" }, "Torment", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ciudad del Abismo", 0, "Increase", new string[] { "M", "R", "S" }, "Torment", new List<EffectActivation> { IncreaseEffect }, ""));
        cardList.Add(new Card("Banana Del Caos", 0, "Increase", new string[] { "M", "R", "S" }, "Torment", new List<EffectActivation> { IncreaseEffect }, ""));
        cardList.Add(new Card( "Sacerdote", 0,  "Clima", new string[] { "M", "R", "S" },  "Torment", new List<EffectActivation> { damageEffect}, ""));
        cardList.Add(new Card( "Ciudad Nublada", 0,  "Clima",  new string[] { "M", "R", "S" },  "Torment", new List<EffectActivation> { damageEffect}, ""));
        cardList.Add(new Card("Catapulta de Fuego", 4, "Silver", new string[] { "S" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Minero", 3, "Silver", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza Roca", 4, "Silver", new string[] { "S" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card( "Cataclismo", 0,  "Clima", new string[] { "M", "R", "S" },  "Fire",new List<EffectActivation>(), " "));
        cardList.Add(new Card("Antorcha", 4, "Silver", new string[] { "R" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Gran Lanza Roca", 6, "Golden", new string[] { "S" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card( "Ola de LLamas", 0,  "Increase", new string[] { "M", "R", "S" },  "Fire",new List<EffectActivation>(), " "));
        cardList.Add(new Card( "LLuvia de fuego", 0,  "Clima", new string[] { "M", "R", "S" },  "Fire",new List<EffectActivation>(), " "));
        cardList.Add(new Card("Dragon Guardian", 4, "Silver", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Samurai", 0, "Leader", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Mini Ninja", 5, "Golden", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Arquero", 3, "Silver", new string[] { "R" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Caballero Solar", 6, "Golden", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card( "Horda de Fuego", 0,   "Clima",   new string[] { "M", "R", "S" },"Fire",new List<EffectActivation>()," "));
        cardList.Add(new Card("Escudero", 5, "Golden", new string[] { "M" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card( "Esbirro", 0,  "Golden",  new string[] { "M", "R", "S" },  "Fire",new List<EffectActivation>()," "));
        cardList.Add(new Card("bolero", 4, "Silver", new string[] { "S" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Antorcha Superior", 5, "Silver", new string[] { "R" }, "Fire", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Haku", 3, "Silver", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza hojas", 4, "Silver", new string[] { "S" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Leñador", 0, "Leader", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Cazador", 5, "Golden", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza hojas inferior", 3,  "Silver",  new string[] { "R" },  "Forest",new List<EffectActivation>()," "));
        cardList.Add(new Card( "Densidad", 0,   "Clima",   new string[] { "M", "R", "S" },"Forest",new List<EffectActivation>()," "));
        cardList.Add(new Card( "Bosque Profundo", 0, "Clima" , new string[] { "M", "R", "S" },"Forest",new List<EffectActivation>()," "));
        cardList.Add(new Card("Gran Cazador", 6, "Golden", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Lanza hojas inferior", 3, "Silver", new string[] { "Siege" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Francotirador", 5, "Golden", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Cazador Menor", 3, "Silver", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ballestero", 4, "Silver", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Arquero Hoja", 3, "Silver", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Defensor", 5, "Golden", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Flecha Tactica", 4, "Silver", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Tanque", 5, "Silver", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Asesino", 6, "Golden", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Espadachin H", 6, "Golden", new string[] { "M" }, "Forest", new List<EffectActivation>(), " "));
        cardList.Add(new Card("Ballestero Superior", 5, "Golden", new string[] { "R" }, "Forest", new List<EffectActivation>(), " "));

    }
}
