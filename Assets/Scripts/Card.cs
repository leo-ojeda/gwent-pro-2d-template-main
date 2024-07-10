
[System.Serializable]


public class Card
{
    public string Name;
    public int Power;
    public string Efect;
    public string Type;
    public string[] Range;
    public string Faction;
    public Card()
    {

    }
    public Card(string name, int power, string efect, string type, string[] range,string faction)
    {

        Name = name;
        Power = power;
        Efect = efect;
        Type = type;
        Range = range;
        Faction = faction;
    }
}
