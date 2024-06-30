using UnityEngine;
[System.Serializable]


public class Card
{
    public string Name;
    public int Power;
    public string Efect;
    public string Type;
    public int Cost;

    public string[] Range;

    public Sprite Imagen;
    public string Faction;
    public Card()
    {

    }
    public Card( string name, int power, string efect, string type, Sprite imagen, string[] range, int cost,string faction)
    {

        Name = name;
        Power = power;
        Efect = efect;
        Type = type;
        Imagen = imagen;
        Range = range;
        Cost = cost;
        Faction = faction;
    }
}
