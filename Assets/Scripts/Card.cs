using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]


public class Card 
{
    public int Id;
    public string CardName;
    public int Power;
    public string CardDescription;
    public int Cost;

    public Sprite Imagen;
    public Card()
    {

    }
    public Card(int id,string cardName,int power, string cardDescription,int cost,Sprite imagen)
    {
        Id=id;
        CardName=cardName;
        Power=power;
        CardDescription=cardDescription;
        Cost=cost;
        Imagen = imagen;
    }
}
