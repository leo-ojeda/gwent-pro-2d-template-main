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
    public string Efect;
    public string CardType;

    public string Attack;

    public Sprite Imagen;
    public Card()
    {

    }
    public Card(int id,string cardName,int power, string efect,string cardtype,Sprite imagen,string attack)
    {
        Id=id;
        CardName=cardName;
        Power=power;
        Efect=efect;
        CardType=cardtype;
        Imagen = imagen;
        Attack = attack;
    }
}
