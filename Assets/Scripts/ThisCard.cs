using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;



public class ThisCard : MonoBehaviour
{
    Context context;
    //CardToHand cardToHand;
    public GameObject CardPrefab;
    public List<Card> thisCard;
    public List<Card> Card;
    public int n;
    public static List<Card> CardSummon = new List<Card>();

    public static int Id;
    //new
    public string Owner;
    //end
    public string CardName;
    public string CardType;
    public int Power;
    public int InitialPower;

    public List<EffectActivation> Efect;
    public string[] Range;
    public string Faction;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PowerText;
    public TextMeshProUGUI EfectText;
    public TextMeshProUGUI RangedText;
    public TextMeshProUGUI TypeText;



    public Sprite ThisSprite;
    public Image ThatImage;
    public bool cardBack;
    public static bool cardB;


    public GameObject Hand;
    public int NumberOfCardsIdDeck;


    public bool canBeSummon;
    public bool summoned;
    public GameObject BattleZone;

    public static int PowerTotal;
    private bool Zone;
    private AudioSource audioSource;
    public GameObject State;
    public GameObject FieldP1;
    public GameObject Cards;
    public List<Card> wcard;



    void SelectCard()
    {


        // Se asigna la carta correspondiente desde la base de datos
        thisCard.Add(CardDatabase.cardList[0]);
        wcard.Add(CardDatabase.cardList[0]);


    }
    void Awake()
    {
        thisCard = new List<Card>();
        wcard = new List<Card>();
    }
    void Start()
    {
        InitialPower = -1;


        FieldP1 = GameObject.Find("Field P1");


        context = FindObjectOfType<Context>();
        if (!context.playerHands.ContainsKey("Jugador 1"))
        {
            context.playerHands["Jugador 1"] = new List<Card>();
        }
        if (!context.playerFields.ContainsKey("Jugador 1"))
        {
            context.playerFields["Jugador 1"] = new List<Card>();
        }
        SelectCard();
        NumberOfCardsIdDeck = PlayerDeck.deck;
        canBeSummon = false;
        summoned = false;
        Zone = true;
        Owner = "Jugador 1";
        PowerTotal = 0;




    }
    void Update()
    {


        CalculatePowerTotal();
        Hand = GameObject.Find("Hand");
        if (this.transform.parent == Hand.transform.parent)
        {
            cardBack = false;
        }
        if (CardToHand.ItName != null)
        {
            CardToHand.ItName.name = thisCard[0].Name;
        }


        thisCard[0].Owner = Owner;
        CardName = thisCard[0].Name;
        CardType = thisCard[0].Type;
        Power = thisCard[0].Power;
        Range = thisCard[0].Range;
        Faction = thisCard[0].Faction;
        Efect = thisCard[0].OnActivation;





        ThisSprite = Resources.Load<Sprite>(thisCard[0].Name);
        if (ThisSprite == null)
        {

            if (thisCard[0].Type == "Clima")
            {
                ThisSprite = Resources.Load<Sprite>("Clima");
            }
            else if (thisCard[0].Type == "increase")
            {
                ThisSprite = Resources.Load<Sprite>("Incremento");
            }
            else if (thisCard[0].Type == "Silver")
            {

                if (thisCard[0].Range.Contains("M"))
                {
                    ThisSprite = Resources.Load<Sprite>("Melee");
                }
                else if (thisCard[0].Range.Contains("R"))
                {
                    ThisSprite = Resources.Load<Sprite>("Ranged");
                }
                else if (thisCard[0].Range.Contains("S"))
                {
                    ThisSprite = Resources.Load<Sprite>("Siege");
                }
            }
            else if (thisCard[0].Type == "Golden")
            {

                if (thisCard[0].Range.Contains("M"))
                {
                    ThisSprite = Resources.Load<Sprite>("MeleeG");
                }
                else if (thisCard[0].Range.Contains("R"))
                {
                    ThisSprite = Resources.Load<Sprite>("RangedG");
                }
                else if (thisCard[0].Range.Contains("S"))
                {
                    ThisSprite = Resources.Load<Sprite>("SiegeGif");
                }
            }
            else if (thisCard[0].Type == "Leader")
            {
                ThisSprite = Resources.Load<Sprite>("Leader");
            }
            else
            {
                ThisSprite = Resources.Load<Sprite>("none");
            }
        }

        NameText.text = "" + CardName;
        PowerText.text = "" + Power;
        TypeText.text = "" + CardType;

        RangedText.text = string.Join(", ", Range);

        string efectText = "";
        foreach (var efectActivation in Efect)
        {
            efectText += "Effect: " + efectActivation.effect.name;
            // efectText += ":  " + efectActivation.selector.source;
        }
        EfectText.text = efectText;

        ThatImage.sprite = ThisSprite;

        cardB = cardBack;
        if (tag == "Three")
        {
            thisCard[0] = wcard[0];
            Debug.Log(wcard[0].Name);
        }

        if (tag == "first")
        {
            // Debug.Log("entro");
            thisCard[0] = context.playerDecks[Owner].Pop().Clone();
            context.board.Add(thisCard[0]);
            context.playerHands[Owner].Add(thisCard[0]);
            NumberOfCardsIdDeck -= 1;
            PlayerDeck.deck -= 1;
            cardBack = false;
        }
        if (State != null && InitialPower != thisCard[0].Power)
        {
            if (InitialPower == -1)
            {
                InitialPower = thisCard[0].Power;
            }
            else
            {
                if (InitialPower > thisCard[0].Power)
                {
                    State.GetComponent<Image>().color = Color.red;
                    PowerText.color = Color.red;
                }
                else if (InitialPower < thisCard[0].Power)
                {
                    State.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    State.GetComponent<Image>().color = Color.white;
                    PowerText.color = Color.black;
                }
            }

            // Actualizar el poder anterior
            //InitialPower = thisCard[0].Power;
        }


        // Verificar si tienes suficiente mana para invocar cartas
        Draggable h = GetComponent<Draggable>();
        if (summoned == false && TurnSystem.IsYourTurn == true && TurnSystem.surrenderedPlayer1 == false)
        {
            canBeSummon = true;
            if (h != null)
            {
                h.SetDraggable(true); // Activar la función de arrastrar la carta
            }
        }
        else
        {
            canBeSummon = false;
            if (h != null)
            {
                h.SetDraggable(false); // Desactivar la función de arrastrar la carta
            }
        }

        // Realizar operaciones para cada zona de batalla
        OperationsForBattleZones();
        //if (tag == "Three")
        //{
        //    thisCard[0] = wcard;
        //    Debug.Log("ultima"+wcard.Name);
        //}


    }
    public void Summon(Card SummonedCard)
    {
        TurnSystem.CurrentMana = 0;
        summoned = true;
        CardSummon.Add(SummonedCard);
        PlayMusic("100");

        context.TriggerPlayer = SummonedCard.Owner;
        //context.board.Add(SummonedCard);
        context.playerFields[SummonedCard.Owner].Add(SummonedCard);
        // Field.Add(SummonedCard);
        context.playerHands[Owner].Remove(SummonedCard);

        switch (SummonedCard.Type)
        {
            case "Clima":
                PlayMusic("s2033");
                break;
            case "Increase":
                PlayMusic("s554");
                break;
            default:
                PlayMusic("100");
                break;
        }
        if (SummonedCard.OnActivation != new List<EffectActivation>())
        {

        }

        ActivateEffects(SummonedCard);

    }

    private void CalculatePowerTotal()
    {
        PowerTotal = 0;


        List<Card> fieldCards = context.playerFields[Owner].ToList();
        List<Card> HandCards = context.playerHands[Owner].ToList();


        foreach (var card in fieldCards.ToList())
        {

            if (card.Type == "Leader" || card.Type == "Clima" || card.Type == "Increase" || card.Power < 0)
            {
                card.Power = 0;
                continue;
            }


            var CardBoard = context.board.FirstOrDefault(c => c.Name == card.Name && c.Power == card.Power);

            if (CardBoard == null)
            {

                context.playerFields[Owner].Remove(card);
                context.playerHands[Owner].Remove(card);
                Debug.Log("Carta eliminada del campo: " + card.Name + " con poder: " + card.Power);
                continue;
            }

            // Sumar el poder de la carta al total
            PowerTotal += card.Power;
        }

        // Si quieres depurar, puedes habilitar este mensaje para revisar el total de poder
        //Debug.Log("PowerTotal Recalculado a: " + PowerTotal);
    }
    void ActivateEffects(Card card)
    {
        // Asegurarse de que TriggerPlayer esté configurado
        context.TriggerPlayer = card.Owner;
        // int handCountBefore = context.Hand.Count;
        var handBefore = new List<Card>(context.Hand);
        var boardBefore = new List<Card>(context.board);

        foreach (var effectActivation in card.OnActivation)
        {
            List<Card> targets;

            // Obtener la lista de objetivos según el source del selector
            switch (effectActivation.selector.source)
            {
                case "Field":
                    targets = context.Field;
                    break;
                case "Deck":
                    targets = context.Deck;
                    break;
                case "Hand":
                    targets = context.Hand;
                    break;
                case "Graveyard":
                    targets = context.Graveyard;
                    break;
                case "otherField":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Field;
                    break;
                case "otherDeck":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Deck;
                    break;
                case "otherHand":
                    context.TriggerPlayer = "Jugador 2";
                    targets = context.Hand;
                    break;
                case "board":
                    targets = context.board;
                    break;
                case "parent":
                    // Aquí podrías obtener los targets del padre, si es necesario
                    targets = new List<Card>(); // Este debe ser reemplazado con el contexto adecuado para "parent"
                    break;
                default:
                    targets = new List<Card>(); // Lista vacía si el source no se reconoce
                    break;
            }

            // Filtrar los objetivos según el Predicate
            if (effectActivation.selector.predicate != null)
            {
                targets = targets.Where(effectActivation.selector.predicate).ToList();
            }

            // Si Single es verdadero, selecciona solo el primer objetivo (si hay alguno)
            if (effectActivation.selector.single && targets.Count > 0)
            {
                targets = targets.Take(1).ToList();
            }

            // Aplicar el efecto a los objetivos seleccionados
            effectActivation.Activate(targets, context);

            // Si hay un PostAction, se aplica a los mismos targets
            if (effectActivation.postAction != null)
            {
                // Definir el source del PostAction
                string postActionSource = effectActivation.postAction.selector.source;

                List<Card> postActionTargets;

                // Manejar el source del PostAction
                switch (postActionSource)
                {
                    case "Field":
                        postActionTargets = context.Field;
                        break;
                    case "Deck":
                        postActionTargets = context.Deck;
                        break;
                    case "Hand":
                        postActionTargets = context.Hand;
                        break;
                    case "Graveyard":
                        postActionTargets = context.Graveyard;
                        break;
                    case "otherField":
                        context.TriggerPlayer = "Jugador 2";
                        postActionTargets = context.Field;
                        break;
                    case "otherDeck":
                        context.TriggerPlayer = "Jugador 2";
                        postActionTargets = context.Deck;
                        break;
                    case "otherHand":
                        context.TriggerPlayer = "Jugador 2";
                        postActionTargets = context.Hand;
                        break;
                    case "board":
                        postActionTargets = context.board;
                        break;
                    case "parent":
                        // Se refiere al mismo source que el del padre
                        postActionTargets = targets; // Usa los mismos targets del efecto principal
                        break;
                    default:
                        postActionTargets = new List<Card>(); // Lista vacía si el source no se reconoce
                        break;
                }

                // Filtrar los objetivos del PostAction según el Predicate
                if (effectActivation.postAction.selector.predicate != null)
                {
                    postActionTargets = postActionTargets.Where(effectActivation.postAction.selector.predicate).ToList();
                }

                // Si Single es verdadero, selecciona solo el primer objetivo (si hay alguno)
                if (effectActivation.postAction.selector.single && postActionTargets.Count > 0)
                {
                    postActionTargets = postActionTargets.Take(1).ToList();
                }

                // Activar el PostAction en los objetivos seleccionados
                effectActivation.postAction.action?.Invoke(postActionTargets, context);
            }
        }
        context.TriggerPlayer = card.Owner;
        var handAfter = new List<Card>(context.Hand);
        var newCards = FindNewCards(handBefore, handAfter);


        var boardAfter = new List<Card>(context.board);
        // Buscar las cartas que se eliminaron del campo
        var removedCards = FindRemovedCards(boardBefore, boardAfter);
        // Eliminar solo la cantidad correcta de GameObjects correspondientes a las cartas eliminadas
        RemoveCardsboard(removedCards);

        // Iterar sobre todas las nuevas cartas encontradas
        foreach (var newCard in newCards)
        {
            if (newCard != null)
            {
                Debug.Log("Nueva carta añadida: " + newCard.Name);

                // Instanciar el prefab para la nueva carta
                Instantiate(CardPrefab, transform.position, transform.rotation);
                // Configurar el prefab si es necesario, por ejemplo:
                //newCardPrefab.GetComponent<ThisCard>().thisCard.Add(newCard);
            }
        }
    }


    void OperationsForBattleZones()
    {
        string[] battleZoneNames = { "MeleeZone 1", "RangeZone 1", "ClimaZone", "SiegeZone 1", "IncreaseZone" };
        foreach (string zoneName in battleZoneNames)
        {
            BattleZone = GameObject.Find(zoneName);
            if (transform.parent == BattleZone.transform)
            {
                // Si la carta puede ser invocada en esta zona, la invocamos y la agregamos a CardSummon
                if (canBeSummon)
                {
                    Summon(thisCard[0]);


                    foreach (var card in CardSummon)
                    {

                        //   Debug.Log("Carta: " + card.Name);
                        //   Debug.Log(CardSummon.Count);
                        //   Debug.Log(PowerTotal);
                    }
                }
            }
        }
        if (TurnSystem.Round == 2 && Zone == true)
        {
            //Debug.Log("Ronda 2 a comenzado");
            CardSummon.Clear();
            Zone = false;
        }
        if (TurnSystem.Round == 3 && Zone == false)
        {
            //Debug.Log("Ronda 3 a comenzado");
            CardSummon.Clear();
            Zone = true;
        }
    }


    string PlayMusic(string x)
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip musicClip = Resources.Load<AudioClip>(x);
        audioSource.clip = musicClip;
        audioSource.Play();
        return x;
    }
    //mejorar por si la carta tiene el mismo nombre
    List<Card> FindNewCards(List<Card> before, List<Card> after)
    {
        // Crear una copia de la lista "after" para no modificar el original
        var newCards = new List<Card>(after);

        // Remover las cartas de "before" que también están en "after"
        foreach (var card in before)
        {
            // Remover solo una instancia de la carta que coincida por nombre
            var cardToRemove = newCards.FirstOrDefault(c => c.Name == card.Name);
            if (cardToRemove != null)
            {
                newCards.Remove(cardToRemove);
            }
        }

        // Actualizar cada carta nueva encontrada
        foreach (var newCard in newCards)
        {
            newCard.Owner = "Jugador 1"; // Asignar el dueño a "Jugador 1"
            context.playerDecks[Owner].Insert(0, newCard); // Insertar la carta en el mazo del jugador
            context.playerHands[Owner].Remove(newCard); // Remover la carta de la mano del jugador
        }

        // Retornar la lista de cartas nuevas (las que no estaban en "before")
        return newCards;
    }
    List<Card> FindRemovedCards(List<Card> before, List<Card> after)
    {
        var removedCards = new List<Card>(before);

        // Eliminar las cartas que están tanto en "before" como en "after"
        foreach (var card in after)
        {
            var cardToRemove = removedCards.FirstOrDefault(c => c.Name == card.Name);
            if (cardToRemove != null)
            {

                removedCards.Remove(cardToRemove);
            }
        }
        //  Debug.Log("Cartas removidas:");
        //  foreach (var removedCard in removedCards)
        //  {
        //      Debug.Log("Carta removida: " + removedCard.Name);
        //  }

        return removedCards; // Lista de cartas que ya no están en el campo
    }

    // Método para eliminar solo la cantidad correcta de cartas del campo
    // Método para eliminar solo la cantidad correcta de cartas del campo y moverlas al cementerio
    void RemoveCardsboard(List<Card> removedCards)
    {
        GameObject[] zones = {
        GameObject.Find("Field P1/MeleeZone 1"),
        GameObject.Find("Field P1/RangeZone 1"),
        GameObject.Find("Field P1/SiegeZone 1"),
        GameObject.Find("Field P1/IncreaseZone"),
        GameObject.Find("Field P1/ClimaZone"),
        GameObject.Find("Field P2/MeleeZone AI"),
        GameObject.Find("Field P2/RangeZone AI"),
        GameObject.Find("Field P2/SiegeZone AI"),
        GameObject.Find("Field P2/IncreaseZone AI"),
        GameObject.Find("Field P2/ClimaZone AI")
    };

        // Cementerios para Jugador 1 y Jugador 2
        GameObject playerCemetery = GameObject.Find("Cementery");
        GameObject enemyCemetery = GameObject.Find("EnemyCementery");

        if (playerCemetery == null || enemyCemetery == null)
        {
            Debug.LogError("Cementerios no encontrados.");
            return;
        }

        // Debug para ver las cartas que se intentan remover
        Debug.Log("Intentando remover las siguientes cartas:");
        foreach (var card in removedCards)
        {
            Debug.Log("Carta: " + card.Name);
        }

        foreach (var card in removedCards)
        {
            int countToRemove = removedCards.Count(c => c.Name == card.Name);
            int removedCount = 0;

            // Debug para saber cuántas veces se tiene que remover una carta
            Debug.Log("Intentando remover " + countToRemove + " instancias de la carta: " + card.Name);

            foreach (var zone in zones)
            {
                if (zone == null)
                {
                    Debug.LogWarning("Zona no encontrada o no asignada correctamente.");
                    continue;
                }

                // Debug para ver en qué zona estamos buscando
                Debug.Log("Buscando en la zona: " + zone.name);

                // Buscar todos los objetos de carta en esta zona
                foreach (Transform cardObj in zone.transform)
                {
                    // Si el nombre del GameObject coincide con el nombre de la carta removida
                    if (cardObj.name == card.Name && removedCount < countToRemove)
                    {
                        // Decidir a qué cementerio mover la carta en función del propietario
                        GameObject cemetery = (card.Owner == "Jugador 1") ? playerCemetery : enemyCemetery;

                        // Cambiar la carta a la posición del cementerio
                        cardObj.SetParent(cemetery.transform);
                        cardObj.position = cemetery.transform.position;

                        // Actualizar la lista de cementerios del contexto
                        context.playerGraveyards[card.Owner].Add(card);

                        Debug.Log("Moviendo carta: " + cardObj.name + " al cementerio de: " + card.Owner);

                        removedCount++;

                        // Si hemos eliminado suficientes cartas, dejamos de buscar
                        if (removedCount >= countToRemove)
                        {
                            Debug.Log("Se han movido suficientes instancias de la carta: " + card.Name + " al cementerio.");
                            break;
                        }
                    }
                }

                // Si hemos movido suficientes cartas, dejamos de buscar en otras zonas
                if (removedCount >= countToRemove)
                {
                    break;
                }
            }

            // Debug si no se encuentra ninguna carta para mover al cementerio
            if (removedCount == 0)
            {
                Debug.LogWarning("No se encontraron cartas para mover con el nombre: " + card.Name);
            }
        }
    }






}




