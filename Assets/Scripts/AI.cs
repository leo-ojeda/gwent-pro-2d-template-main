using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class AI : MonoBehaviour
{
    Context context;
    public GameObject CardPrefab;
    private string Owner;
    public List<Card> Deck = new List<Card>();
    public List<Card> CardsInHand = new List<Card>();
    public static List<Card> StaticEnemyDeck = new List<Card>();

    private AudioSource audioSource;
    public GameObject CardInDeck1;
    public GameObject CardInDeck2;
    public GameObject CardInDeck3;
    public GameObject CardInDeck4;

    public GameObject Hand;
    public GameObject ZoneM;
    public GameObject ZoneR;
    public GameObject ZoneS;
    public GameObject ZoneI;
    public GameObject ZoneC;

    public static int deckSize;
    public GameObject CardToHand;
    public GameObject CardBack;

    public int CurrentMana;
    // public bool[] AIcanSummon;
    public bool DrawPhase;
    public bool SummonPhase;
    public bool EndPhase;
    public static bool Control;

    public int HowManyCards;
    public static int EnemyPowerTotal;
    private Card Leader;

    private Card cardToSummon;
    private List<Card> summonableCards = new List<Card>();

    public TextMeshProUGUI AINcard;

    void Start()
    {

        Control = true;
        Owner = "Jugador 2";
        context = FindObjectOfType<Context>();
        if (!context.playerDecks.ContainsKey(Owner))
        {
            context.playerDecks[Owner] = new List<Card>();
        }
        if (!context.playerHands.ContainsKey(Owner))
        {
            context.playerHands[Owner] = new List<Card>();
        }
        if (!context.playerFields.ContainsKey(Owner))
        {
            context.playerFields[Owner] = new List<Card>();
        }
        Hand = GameObject.Find("EnemyHand");
        ZoneM = GameObject.Find("MeleeZone AI");
        ZoneR = GameObject.Find("RangeZone AI");
        ZoneS = GameObject.Find("SiegeZone AI");
        ZoneI = GameObject.Find("IncreaseZone AI");
        ZoneC = GameObject.Find("ClimaZone AI");
        EnemyPowerTotal = 0;

        deckSize = 25;
        List<Card> deckCards = new List<Card>();
        bool leaderCardAdded = false;

        Dictionary<string, int> goldenCountAI = new Dictionary<string, int>();
        Dictionary<string, int> silverCountAI = new Dictionary<string, int>();
        Dictionary<string, int> magicCountAI = new Dictionary<string, int>();

        while (!leaderCardAdded)
        {
            int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
            Card randomCard = CardDatabase.cardList[randomIndex];
            if (randomCard.Type == "Leader" && Menuinicial.cardList[0].Name != randomCard.Name)
            {
                Leader = randomCard;
                leaderCardAdded = true;
                Debug.Log(Leader.Name);
            }
        }

        for (int i = 0; i < deckSize; i++)
        {
            bool cardAdded = false;
            while (!cardAdded)
            {
                int randomIndex = Random.Range(1, CardDatabase.cardList.Count);
                Card randomCard = CardDatabase.cardList[randomIndex];

                if (randomCard.Type == "Leader" && randomCard.Faction == Leader.Faction)
                {
                    if (!goldenCountAI.ContainsKey(randomCard.Name))
                    {
                        Deck[i] = randomCard;

                        context.playerDecks[Owner].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }

                if (randomCard.Type == "Golden" && randomCard.Faction == Leader.Faction)
                {
                    if (!goldenCountAI.ContainsKey(randomCard.Name))
                    {
                        Deck[i] = randomCard;

                        context.playerDecks[Owner].Add(Deck[i]);

                        goldenCountAI[randomCard.Name] = 1;
                        cardAdded = true;
                    }
                }
                else if (randomCard.Type == "Silver" && randomCard.Faction == Leader.Faction)
                {
                    if (!silverCountAI.ContainsKey(randomCard.Name) || silverCountAI[randomCard.Name] < 3)
                    {
                        Deck[i] = randomCard;
                        context.playerDecks[Owner].Add(Deck[i]);

                        if (!silverCountAI.ContainsKey(randomCard.Name))
                        {
                            silverCountAI[randomCard.Name] = 1;
                        }
                        else
                        {
                            silverCountAI[randomCard.Name]++;
                        }
                        cardAdded = true;
                    }
                }
                else if (randomCard.Type == "Increase" && randomCard.Faction == Leader.Faction || randomCard.Type == "Clima" && randomCard.Faction == Leader.Faction)
                {
                    if (!magicCountAI.ContainsKey(randomCard.Name) || magicCountAI[randomCard.Name] < 2)
                    {
                        Deck[i] = randomCard;
                        context.playerDecks[Owner].Add(Deck[i]);

                        if (!magicCountAI.ContainsKey(randomCard.Name))
                        {
                            magicCountAI[randomCard.Name] = 1;
                        }
                        else
                        {
                            magicCountAI[randomCard.Name]++;
                        }
                        cardAdded = true;
                    }
                }
            }
            deckCards.Add(Deck[i]);
        }

        StartCoroutine(StartGame());
    }

    void Update()
    {
        //Debug.Log(TurnSystem.surrenderedPlayer2);
        CalculatePowerTotal();
        context.playerHands[Owner] = CardsInHand;
        context.playerDecks[Owner] = Deck;
        AINcard.text = "" + Deck.Count;

        CardInDeck1.SetActive(deckSize >= 20);
        CardInDeck2.SetActive(deckSize >= 13);
        CardInDeck3.SetActive(deckSize >= 6);
        CardInDeck4.SetActive(deckSize >= 1);

        if (TurnSystem.StartTurn && TurnSystem.Round <= 3)
        {
            StartCoroutine(Draw(2));
            Debug.Log("Draw");
            Control = true;
        }

        CurrentMana = TurnSystem.CurrentEnemyMana;
        UpdateHand();

        if (!TurnSystem.IsYourTurn)
        {
            UpdateSummonableCards();
        }
        else
        {
            summonableCards.Clear();
        }

        if (!TurnSystem.IsYourTurn)
        {
            DrawPhase = true;
        }

        if (DrawPhase && !SummonPhase)
        {
            StartCoroutine(WaitForSummonPhase());
        }

        if (TurnSystem.IsYourTurn)
        {
            DrawPhase = false;
            SummonPhase = false;
            EndPhase = false;
        }

        if (SummonPhase)
        {
            ExecuteSummonPhase();
        }

        if (TurnSystem.surrenderedPlayer1 && ShouldSurrender())
        {
            TurnSystem.surrenderedPlayer2 = true;
            DrawPhase = false;
            SummonPhase = false;
        }

        if (DrawPhase)
        {
            SummonAction();
        }
    }

    void ExecuteSummonPhase()
    {
        cardToSummon = summonableCards.OrderByDescending(card => card.Power).FirstOrDefault();
    }

    void SummonAction()
    {
        if (cardToSummon != null && context.playerHands[Owner].Count > 0)
        {
            Control = true;
            foreach (Transform child in Hand.transform)
            {
                AICardToHand cardComponent = child.GetComponent<AICardToHand>();
                Card cardInHand = cardComponent.ThisCard[0];


                if ((cardInHand.Type == "Clima" || cardInHand.Type == "Increase") && Mathf.Abs(ThisCard.PowerTotal - EnemyPowerTotal) > 8 && context.playerHands[Owner].Count != 0)
                {
                    cardToSummon = cardInHand;
                }

                if (cardInHand == cardToSummon && !TurnSystem.IsYourTurn && !TurnSystem.surrenderedPlayer2)
                {
                    foreach (var range in cardToSummon.Range)
                    {
                        Transform targetZone = null;
                        if (cardToSummon.Type == "Increase")
                        {
                            targetZone = ZoneI.transform;
                        }
                        else if (cardToSummon.Type == "Clima")
                        {
                            targetZone = ZoneC.transform;
                        }
                        else
                        {
                            switch (range)
                            {
                                case "M":
                                    targetZone = ZoneM.transform;
                                    break;

                                case "R":
                                    targetZone = ZoneR.transform;
                                    break;

                                case "S":
                                    targetZone = ZoneS.transform;
                                    break;

                                default:
                                    Debug.LogWarning("Rango desconocido: " + range);
                                    break;
                            }
                        }

                        if (targetZone != null)
                        {
                            child.transform.SetParent(targetZone);
                            switch (cardInHand.Type)
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
                            break;
                        }
                    }

                    TurnSystem.CurrentEnemyMana = 0;
                    //context.board.Add(cardInHand);
                    context.playerFields[cardInHand.Owner].Add(cardInHand);
                    // context.playerHands[cardInHand.Owner].Remove(cardInHand);
                    ActivateEffects(cardInHand);

                    // Resetea la variable después de invocar
                    cardToSummon = null;
                    break;
                }
            }

            SummonPhase = false;
            DrawPhase = false;
        }
        if (context.playerHands[Owner].Count == 0 && Control)
        {
            TurnSystem.surrenderedPlayer2 = true;
            Control = false;
            Debug.Log("AI se rinde porque no tiene cartas en la mano.");
        }
    }



    private void CalculatePowerTotal()
    {
        EnemyPowerTotal = 0;

        List<Card> fieldCards = context.playerFields[Owner].ToList();

        // Recorre todas las cartas en el campo
        foreach (var card in fieldCards.ToList())
        {
            // Verifica si la carta debe influir en el poder total
            if (card.Type == "Leader" || card.Type == "Clima" || card.Type == "Increase" || card.Power < 0)
            {
                card.Power = 0;
                continue;
            }


            var CardBoard = context.board.FirstOrDefault(c => c.Name == card.Name && c.Power == card.Power);

            if (CardBoard == null)
            {
                // Si no hay coincidencias, eliminar la carta del campo
                context.playerFields[Owner].Remove(card);
                Debug.Log("Carta eliminada del campo: " + card.Name + " con poder: " + card.Power);
                continue; // Saltar a la siguiente carta
            }

            // Sumar el poder de la carta al total
            EnemyPowerTotal += card.Power;
        }


    }

    void UpdateHand()
    {
        CardsInHand.Clear();
        foreach (Transform child in Hand.transform)
        {
            CardsInHand.Add(child.GetComponent<AICardToHand>().ThisCard[0]);
        }
    }

    void UpdateSummonableCards()
    {
        summonableCards.Clear();
        foreach (var card in CardsInHand)
        {
            if (CurrentMana > 0)
            {
                summonableCards.Add(card);
            }
        }
    }

    bool ShouldSurrender()
    {
        return EnemyPowerTotal > ThisCard.PowerTotal || EnemyPowerTotal >= 20;

    }

    IEnumerator StartGame()
    {
        for (int i = 0; i <= 9; i++)
        {
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);
        }
    }

    IEnumerator Draw(int x)
    {
        for (int i = 0; i < x; i++)
        {
            yield return new WaitForSeconds(0.4f);
            Instantiate(CardToHand, transform.position, transform.rotation);

            TurnSystem.surrenderedPlayer2 = false;
            Control = true;

        }
    }

    IEnumerator WaitForSummonPhase()
    {
        yield return new WaitForSeconds(0.4f);
        SummonPhase = true;
    }

    string PlayMusic(string x)
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip musicClip = Resources.Load<AudioClip>(x);
        audioSource.clip = musicClip;
        audioSource.Play();
        return x;
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
                    context.TriggerPlayer = "Jugador 1";
                    targets = context.Field;
                    break;
                case "otherDeck":
                    context.TriggerPlayer = "Jugador 1";
                    targets = context.Deck;
                    break;
                case "otherHand":
                    context.TriggerPlayer = "Jugador 1";
                    targets = context.Hand;
                    break;
                case "board":
                    targets = context.board;
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
                        context.TriggerPlayer = "Jugador 1";
                        postActionTargets = context.Field;
                        break;
                    case "otherDeck":
                        context.TriggerPlayer = "Jugador 1";
                        postActionTargets = context.Deck;
                        break;
                    case "otherHand":
                        context.TriggerPlayer = "Jugador 1";
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
            newCard.Owner = "Jugador 2"; // Asignar el dueño a "Jugador 2"
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
                        // Debug antes de destruir el objeto
                        Debug.Log("Destruyendo carta: " + cardObj.name + " de la zona: " + zone.name);

                        Destroy(cardObj.gameObject); // Destruir el GameObject
                        removedCount++;

                        // Si hemos eliminado suficientes cartas, dejamos de buscar
                        if (removedCount >= countToRemove)
                        {
                            Debug.Log("Se han eliminado suficientes instancias de la carta: " + card.Name);
                            break;
                        }
                    }
                }

                // Si hemos eliminado suficientes cartas, dejamos de buscar en otras zonas
                if (removedCount >= countToRemove)
                {
                    break;
                }
            }

            // Debug si no se encuentra ninguna carta para eliminar
            if (removedCount == 0)
            {
                Debug.LogWarning("No se encontraron cartas para eliminar con el nombre: " + card.Name);
            }
        }
    }


}
