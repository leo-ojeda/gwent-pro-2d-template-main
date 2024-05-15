using UnityEngine;
[System.Serializable]


public class Card
{
    public int Id;
    public string CardName;
    public int Power;
    public string Efect;
    public string CardType;
    public int Cost;

    public string Attack;

    public Sprite Imagen;
    public string Faccion;
    public Card()
    {

    }
    public Card(int id, string cardName, int power, string efect, string cardtype, Sprite imagen, string attack, int cost,string faccion)
    {
        Id = id;
        CardName = cardName;
        Power = power;
        Efect = efect;
        CardType = cardtype;
        Imagen = imagen;
        Attack = attack;
        Cost = cost;
        Faccion = faccion;
    }
}
