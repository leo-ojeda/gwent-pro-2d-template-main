using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    public bool IsYourTurn;
    public int YourTurn;
    public int YourOponnentTurn;
    public Text TurnText;
    public int MaxMana;
    public int CurrentMana;
    public Text ManaText;
    // Start is called before the first frame update
    void Start()
    {
        IsYourTurn = true;
        YourTurn = 1;
        YourOponnentTurn = 0;
        MaxMana = 1;
        CurrentMana = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsYourTurn == true)
        {
            TurnText.text = "TurnoP1";
        }
        else
        {
            TurnText.text = "TurnoP2";
        }
        ManaText.text= CurrentMana+"/"+MaxMana;
    }


    public void EndYourTurn()
    {
        IsYourTurn = false;
        YourOponnentTurn += 1;

    }
    public void EndYourOpponentTurn()
    {
        IsYourTurn = true;
        YourTurn += 1;
        MaxMana +=1;
        CurrentMana = MaxMana;

    }



}
