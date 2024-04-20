using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    public static bool IsYourTurn;

    public int YourTurn;
    public int YourOponnentTurn;
    public Text TurnText;
    public int Round;
    public Text RoundText;
    public static int MaxMana;
    public static int CurrentMana;
    public Text ManaText;

    public static bool StartTurn;

    // Contadores para llevar la cuenta de cuántos jugadores han cedido el turno en la ronda actual
    private int surrenderedPlayer1;
    private int surrenderedPlayer2;
    public GameObject CemeteryPanel;

    void Start()
    {
        IsYourTurn = true;
        StartTurn= false;

        YourTurn = 1;
        YourOponnentTurn = 0;
        MaxMana = 1;
        CurrentMana = 1;
        Round = 1;

        // Inicializar los contadores de rendición de jugadores
        surrenderedPlayer1 = 0;
        surrenderedPlayer2 = 0;
    }

    void Update()
    {
        if (IsYourTurn)
        {
            TurnText.text = "TurnoP1";
        }
        else
        {
            TurnText.text = "TurnoP2";
        }
        ManaText.text = CurrentMana + "/" + MaxMana;
        RoundText.text = Round + "/" + 3;
    }

    public void EndYourTurn()
    {    
        IsYourTurn = false;
        Debug.Log("False");
        YourOponnentTurn += 1;
    }

    public void EndYourOpponentTurn()
    {
        IsYourTurn = true;
        YourTurn += 1;
        MaxMana += 1;
        CurrentMana = MaxMana;
    }

    // Función para que un jugador ceda el turno
    public void Surrenderp1()
    {
        // Verificar el turno actual y marcar al jugador correspondiente como rendido
        if (IsYourTurn)
        {
            Debug.Log("es tu turno");
            surrenderedPlayer1 = 1;
            StartNextRound();
        }
    }
    public void Surrenderp2()
    {

        if (!IsYourTurn)
        {
            Debug.Log("no es tu turno");

            surrenderedPlayer2 = 1;
            StartNextRound();
        }
    }
    public void StartNextRound()
    {
        if (surrenderedPlayer1 == 1 && surrenderedPlayer2 == 1)
        {
            Debug.Log("Se cambio de ronda");
            NextRound();
        }

    }
    // Función para iniciar la próxima ronda
    private void NextRound()
    {
        // Reiniciar los contadores de turno y mana
        YourTurn = 1;
        YourOponnentTurn = 0;
        MaxMana = 1;
        CurrentMana = 1;
        Round ++;
        RoundText.text = Round + "/" + 3;
        StartTurn = true;

        // Reiniciar los indicadores de rendición de jugadores
        surrenderedPlayer1 = 0;
        surrenderedPlayer2 = 0;
        CardsCemetery("Melee");
        CardsCemetery("Range");
        CardsCemetery("Siege");
        CardsCemetery("Clima");
        CardsCemetery("Leader");
    }
 void CardsCemetery(string zoneTag)
{
    GameObject[] zones = GameObject.FindGameObjectsWithTag(zoneTag);
    foreach (GameObject zone in zones)
    {
        // Buscar todas las cartas dentro de la zona actual
        Transform zoneTransform = zone.transform;
        bool cardsRemaining = true; // Variable para controlar si quedan cartas por mover en la zona
        while (cardsRemaining)
        {
            // Buscar una carta con el tag "second"
            Transform cardToMove = null;
            foreach (Transform child in zoneTransform)
            {
                if (child.CompareTag("second"))
                {
                    cardToMove = child;
                    break; // Salir del bucle una vez que se encuentre una carta con el tag "second"
                }
            }

            // Verificar si se encontró una carta para mover
            if (cardToMove != null)
            {
                // Mover la carta al cementerio
                cardToMove.SetParent(CemeteryPanel.transform);
            }
            else
            {
                // No se encontraron más cartas con el tag "second" en esta zona
                cardsRemaining = false;
            }
        }
    }
}



}
