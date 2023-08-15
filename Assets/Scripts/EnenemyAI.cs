using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnenemyAI : MonoBehaviour
{
    [SerializeField] private HandManager Hand;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TextMeshProUGUI ThinkingBubble;      //the three dots thing
    [SerializeField] private float MaxLux;
    [SerializeField] public Slider LuxMeter;
    [SerializeField] private float MaxUmbra;
    [SerializeField] public Slider UmbraMeter;
    public EnemyPerson personality;
    public float Lux;
    public float Umbra;
    float Timer;
    int CardsToPlace;
    bool Placing;

    private void Start()
    {
        //setting up meters (sliders)
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
        Invoke("doTurn", 3f);
    }

    private void Update()
    {
        //a bit funky so it can do the dots thing
        if(Placing)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                if (CardsToPlace > 0)
                {
                    Timer = Random.Range(1f, 3f);
                    PlaceCard();
                    CardsToPlace--;
                }
                else
                {
                    Placing = false;
                    ThinkingBubble.text = personality.EndTurnDialogue[Random.Range(0, personality.EndTurnDialogue.Length)];
                    Invoke("ClearDialogueText", 3f);
                    boardManager.Ready(false);
                }
            }
        }
    }

    //called on turn start, starts the thinking thing
    public void doTurn()
    {
        if (Hand.Hand.Count > 0)
        {
            CardsToPlace = Random.Range(1, Hand.Hand.Count);
            Timer = Random.Range(1f, 3f);
            Placing = true;
            ThinkingBubble.text = personality.ThinkingDialogue[Random.Range(0, personality.ThinkingDialogue.Length)];
        }
        else
        {
            ThinkingBubble.text = personality.DeckEmptyDialogue[Random.Range(0, personality.DeckEmptyDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
            boardManager.Ready(false);
        }
    }

    void ClearDialogueText()
    {
        ThinkingBubble.text = " ";
    }

    void PlaceCard() //that's where the actual card placement AI should be at
    {
        var AffordableCards = new List<int>();
        for (int j = 0; j < Hand.Hand.Count; j++)
        {
            if (Hand.Hand[j].content.cost <= (Hand.Hand[j].content.element == Card.elements.light ? Lux : Umbra)) AffordableCards.Add(j);
        }
        if (AffordableCards.Count == 0)
        {
            ThinkingBubble.text = personality.OutOfManaDialogue[Random.Range(0, personality.OutOfManaDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
            boardManager.Ready(false);
            Placing = false;
            return;
        }
        var cardInd = AffordableCards[Random.Range(0, AffordableCards.Count)];
        var AvailablePlaces = new List<int>();
        if (Hand.Hand[cardInd].content.Primary)
        {
            for (int j = 4; j < 8; j++)
            {
                if (boardManager.TheBoard[j] == null) AvailablePlaces.Add(j);
            }
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                if (boardManager.TheBoard[j] == null) AvailablePlaces.Add(j);
            }
        }
        var placeInd = Random.Range(0, AvailablePlaces.Count);
        if (boardManager.PlaceCard(Hand.Hand[cardInd], AvailablePlaces[placeInd]))
        {
            Hand.Hand[cardInd].Send(boardManager.EnemySlots[AvailablePlaces[placeInd]].position);
            if (Hand.Hand[cardInd].content.element == Card.elements.light) Lux -= Hand.Hand[cardInd].content.cost;
            else Umbra -= Hand.Hand[cardInd].content.cost;
            UpdateSliders();
            Hand.RemoveCardFromHand(cardInd);
        }
    }

    void UpdateSliders()
    {
        LuxMeter.value = Lux;
        UmbraMeter.value = Umbra;
    }
}
