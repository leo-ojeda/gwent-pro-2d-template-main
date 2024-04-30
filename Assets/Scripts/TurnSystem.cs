using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    public static bool IsYourTurn;

    //new
    public Text TurnText;
    public static int Round;
    public Text RoundText;
    public static int MaxMana;
    public static int EnemyMaxMana;
    public static int CurrentMana;
    public static int CurrentEnemyMana;
    public Text ManaText;
    public Text ManaEnemyText;
    private int RoundEV;
    private int RoundPV;
    public Text RoundVtext;
    public Text Victory;

    public static bool StartTurn;

    // Contadores para llevar la cuenta de cuántos jugadores han cedido el turno en la ronda actual
    private bool surrenderedPlayer1;
    public static bool surrenderedPlayer2;
    public GameObject CemeteryPanel;
    public GameObject EnemyCementeryPanel;
    public bool point;

    void Start()
    {
        point = true;
        RoundEV = 0;
        RoundPV = 0;
        IsYourTurn = true;
        StartTurn = false;

        MaxMana = 1;
        EnemyMaxMana = 1;
        CurrentMana = 1;
        CurrentEnemyMana = 1;
        Round = 1;

        // Inicializar los contadores
        surrenderedPlayer1 = false;
        surrenderedPlayer2 = false;

    }

    void Update()
    {
        Draggable K = GetComponent<Draggable>();
        if (IsYourTurn)
        {
            TurnText.text = "TurnoP1";
        }
        else
        {
            TurnText.text = "TurnoP2";
        }
        ManaText.text = CurrentMana + "/" + MaxMana;
        ManaEnemyText.text = CurrentEnemyMana + "/" + EnemyMaxMana;
        RoundText.text = Round + "/" + 3;
        RoundVtext.text = RoundPV + "      /      " + RoundEV;
        if (CurrentEnemyMana == 0 && IsYourTurn == false)
        {
            EndYourOpponentTurn();
        }
        if (surrenderedPlayer1 == true && surrenderedPlayer2 == true)
        {
            StartCoroutine(Wait());

        }
    }

    public void EndYourTurn()
    {
        if (surrenderedPlayer2 == true)
        {
            Debug.Log("Continuar TurnoP1");
            IsYourTurn = true;
            MaxMana += 1;
            CurrentMana = MaxMana;
        }
        else
        {
            IsYourTurn = false;
            Debug.Log("False");
        }
    }

    public void EndYourOpponentTurn()
    {
        if (surrenderedPlayer1 == true)
        {
            Debug.Log("Continuar Turno");
            IsYourTurn = false;
            MaxMana += 1;
            EnemyMaxMana += 1;
            CurrentEnemyMana = EnemyMaxMana;
        }
        else
        {
            IsYourTurn = true;
            MaxMana += 1;
            CurrentMana = MaxMana;
            EnemyMaxMana += 1;
            CurrentEnemyMana = EnemyMaxMana;
        }
    }

    // Función para que un jugador ceda el turno
    public void Surrenderp1()
    {
        // Verificar el turno actual y marcar al jugador correspondiente como rendido
        if (IsYourTurn)
        {
            Debug.Log("es tu turno");
            surrenderedPlayer1 = true;
            StartNextRound();
            IsYourTurn = false;
        }
    }
    public void Surrenderp2()
    {

        if (!IsYourTurn)
        {
            Debug.Log("no es tu turno");

            surrenderedPlayer2 = true;
            StartNextRound();
            IsYourTurn = true;
        }
    }
    public void StartNextRound()
    {
        if (surrenderedPlayer1 == true && surrenderedPlayer2 == true)
        {
            Debug.Log("Se cambio de ronda");
            NextRound();
            if (Round > 3 || RoundEV == 2 || RoundPV == 2)
            {
                //Debug.Log("Se acabo lo que se daba");
                //new
                if (RoundPV > RoundEV)
                {
                    Victory.text = "You Win";
                }
                else
                {
                    Victory.text = "You Lose";
                }
            }
        }

    }
    // Función para iniciar la próxima ronda
    private void NextRound()
    {
        //new
        if (ThisCard.PowerTotal > AI.EnemyPowerTotal && point == true)
        {
            RoundPV++;
            point = false;
        }
        if (ThisCard.PowerTotal == AI.EnemyPowerTotal && point == true)
        {
            RoundPV++;
            RoundEV++;
            point = false;
        }
        if (ThisCard.PowerTotal < AI.EnemyPowerTotal && point == true)
        {
            RoundEV++;
            point = false;
        }
        if (RoundPV >= RoundEV)
        {
            Debug.Log("Empieza P1");
            IsYourTurn = true;
        }
        else
        {
            Debug.Log("Empieza P2");
            IsYourTurn = false;
        }
        if (point == false)
        {

            AI.EnemyPowerTotal = 0;
            ThisCard.PowerTotal = 0;
        }
        MaxMana = 1;
        EnemyMaxMana = 1;
        CurrentEnemyMana = 1;
        CurrentMana = 1;
        Round++;
        RoundText.text = Round + "/" + 3;
        StartTurn = true;

        surrenderedPlayer1 = false;
        surrenderedPlayer2 = false;
        point = true;
        CardsCemetery("Melee");
        CardsCemetery("Range");
        CardsCemetery("Siege");
        CardsCemetery("Clima");
        CardsCemetery("Leader");
        CardsCemetery("Increase");
        CardsCemetery("Melee AI");

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
                // Buscar una carta con el tag "second" o "Untagged"
                Transform cardToMove = null;
                foreach (Transform child in zoneTransform)
                {
                    if (child.CompareTag("second") || child.CompareTag("Untagged"))
                    {
                        cardToMove = child;
                        break;
                    }
                }

                // Verificar si se encontró una carta para mover
                if (cardToMove != null)
                {
                    // Determinar el panel al que se moverá la carta
                    GameObject destinationPanel = (cardToMove.CompareTag("second")) ? CemeteryPanel : EnemyCementeryPanel;
                    // Mover la carta al panel correspondiente
                    cardToMove.SetParent(destinationPanel.transform);
                }
                else
                {
                    // No se encontraron más cartas con los tags especificados en esta zona
                    cardsRemaining = false;
                }
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        StartNextRound();
    }

}
