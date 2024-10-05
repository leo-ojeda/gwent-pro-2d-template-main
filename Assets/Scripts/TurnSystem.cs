
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


public class TurnSystem : MonoBehaviour
{
    Context context;
    public static bool IsYourTurn;
    private AudioSource AudioSource;

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
    public Text Power;

    public static bool StartTurn;

    // Contadores para llevar la cuenta de cuántos jugadores han cedido el turno en la ronda actual
    public static bool surrenderedPlayer1;
    public static bool surrenderedPlayer2;
    public GameObject CemeteryPanel;
    public GameObject EnemyCementeryPanel;
    public bool point;

    public string menu;



    void Start()
    {
        context = FindObjectOfType<Context>();
        if (!context.playerGraveyards.ContainsKey("Jugador 1"))
        {
            context.playerGraveyards["Jugador 1"] = new List<Card>();
        }
        if (!context.playerGraveyards.ContainsKey("Jugador 2"))
        {
            context.playerGraveyards["Jugador 2"] = new List<Card>();
        }

        AudioSource = GetComponent<AudioSource>();
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

        surrenderedPlayer1 = false;
        surrenderedPlayer2 = false;

    }

    void Update()
    {
        Power.text = ThisCard.PowerTotal + "            " + AI.EnemyPowerTotal;

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
        if (surrenderedPlayer2 == true)
        {
            //Debug.Log("Continuar TurnoP1");
            IsYourTurn = true;

            CurrentMana = MaxMana;
        }
        else if (CurrentMana == 0)
        {
            IsYourTurn = false;
        }
    }


    public void EndYourOpponentTurn()
    {
        if (surrenderedPlayer1 == true)
        {
            //Debug.Log("Continuar Turno");
            IsYourTurn = false;
            CurrentEnemyMana = EnemyMaxMana;
        }
        else
        {
            IsYourTurn = true;
            CurrentMana = MaxMana;
            CurrentEnemyMana = EnemyMaxMana;
        }
    }

    // Función para que un jugador ceda el turno
    public void Surrenderp1()
    {
        // Verificar el turno actual y marcar al jugador correspondiente como rendido
        if (IsYourTurn)
        {
            PlayMusic("s468");
            // Debug.Log("es tu turno");
            surrenderedPlayer1 = true;
            StartNextRound();
            IsYourTurn = false;
        }
    }
    public void Surrenderp2()
    {

        if (!IsYourTurn)
        {
            //Debug.Log("no es tu turno");

            surrenderedPlayer2 = true;
            StartNextRound();
            IsYourTurn = true;
        }
    }
    public void StartNextRound()
    {
        if (surrenderedPlayer1 == true && surrenderedPlayer2 == true)
        {
            //Debug.Log("Se cambio de ronda");
            NextRound();
            if (Round > 3 || RoundEV == 2 || RoundPV == 2)
            {
                //Debug.Log("Se acabo lo que se daba");
                //new
                Victory.gameObject.SetActive(true);
                if (RoundPV > RoundEV)
                {
                    Victory.text = "Has Ganado";
                    StartCoroutine(ReturnToMenu());
                }
                else
                {
                    Victory.text = "Has Perdido";
                    StartCoroutine(ReturnToMenu());
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
        if (RoundPV < RoundEV)
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
        CardsCemetery("M");
        CardsCemetery("R");
        CardsCemetery("S");
        CardsCemetery("Clima");
        CardsCemetery("Increase");
        CardsCemetery("Melee AI");

    }
    void CardsCemetery(string zoneTag)
    {

        foreach (Card card in context.board)
        {

            if (card.Owner == "Jugador 1")
            {
                context.playerGraveyards[card.Owner].Add(card);
            }
            else if (card.Owner == "Jugador 2")
            {
                context.playerGraveyards[card.Owner].Add(card);
            }
        }

        GameObject[] zones = GameObject.FindGameObjectsWithTag(zoneTag);
        foreach (GameObject zone in zones)
        {
            // Buscar todas las cartas dentro de la zona actual
            Transform zoneTransform = zone.transform;
            bool cardsRemaining = true; // Variable para controlar si quedan cartas por mover en la zona
            while (cardsRemaining)
            {
                
                Transform cardToMove = null;
                foreach (Transform child in zoneTransform)
                {
                    if (child.CompareTag("Hand") || child.CompareTag("Untagged"))
                    {
                        cardToMove = child;
                        break;

                    }
                }

                // Verificar si se encontró una carta para mover
                if (cardToMove != null)
                {
                    // Determinar el panel al que se moverá la carta
                    GameObject destinationPanel = (cardToMove.CompareTag("Hand")) ? CemeteryPanel : EnemyCementeryPanel;
                    // Mover la carta al panel correspondiente
                    cardToMove.SetParent(destinationPanel.transform);
                }
                else
                {

                    PlayMusic("s3200");
                    //context.board.Clear();
                    context.playerFields.Clear();
                    context.playerFields["Jugador 1"] = new List<Card>();
                    context.playerFields["Jugador 2"] = new List<Card>();
                    cardsRemaining = false;
                }
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        StartNextRound();
    }
    string PlayMusic(string x)
    {
        AudioSource = GetComponent<AudioSource>();
        AudioClip musicClip = Resources.Load<AudioClip>(x);
        AudioSource.clip = musicClip;
        AudioSource.Play();
        return x;
    }
    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(menu);
    }

}
