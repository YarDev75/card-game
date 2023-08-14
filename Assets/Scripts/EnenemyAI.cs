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
    [SerializeField] private Slider LuxMeter;
    [SerializeField] private float MaxUmbra;
    [SerializeField] private Slider UmbraMeter;
    public EnemyPerson personality;
    float Lux;
    float Umbra;
    float Timer;
    float TimerGoal;

    private void Start()
    {
        //setting up meters (sliders)
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
    }

    private void Update()
    {
        //a bit funky so it can do the dots thing
        if(TimerGoal > 0)
        {
            Timer += Time.deltaTime;
            if(Timer >= TimerGoal)
            {
                TimerGoal = -1;
                ThinkingBubble.text = " ";
                PlaceCards();
            }
        }
    }

    //called on turn start, starts the thinking thing
    public void doTurn()
    {
        TimerGoal = Random.Range(1f, 6f);
        Timer = 0;
        ThinkingBubble.text = personality.ThinkingDialogue[Random.Range(0, personality.ThinkingDialogue.Length)];
    }

    void ClearDialogueText()
    {
        ThinkingBubble.text = " ";
    }

    void PlaceCards() //that's where the actual card placement AI should be at
    {

        //needs some work
        if (Hand.Hand.Count > 0)
        {
            ThinkingBubble.text = personality.EndTurnDialogue[Random.Range(0, personality.EndTurnDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
            for (int i = 0; i < Random.Range(1, Hand.Hand.Count); i++)
            {
                var AffordableCards = new List<int>();
                for (int j = 0; j < Hand.Hand.Count; j++)
                {
                    if (Hand.Hand[j].content.cost <= (Hand.Hand[j].content.element == Card.elements.light ? Lux : Umbra)) AffordableCards.Add(j);
                }
                if (AffordableCards.Count == 0)
                {
                    boardManager.NextTurn();
                    ThinkingBubble.text = personality.OutOfManaDialogue[Random.Range(0, personality.OutOfManaDialogue.Length)];
                    Invoke("ClearDialogueText", 3f);
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
                if (boardManager.PlaceCard(Hand.Hand[cardInd].content, AvailablePlaces[placeInd]))
                {
                    Hand.Hand[cardInd].Send(boardManager.EnemySlots[AvailablePlaces[placeInd]].position);
                    if (Hand.Hand[cardInd].content.element == Card.elements.light) Lux -= Hand.Hand[cardInd].content.cost;
                    else Umbra -= Hand.Hand[cardInd].content.cost;
                    UpdateSliders();
                    Hand.RemoveCardFromHand(cardInd);
                }
            }
        }
        else
        {
            ThinkingBubble.text = personality.DeckEmptyDialogue[Random.Range(0, personality.DeckEmptyDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
        }

        boardManager.NextTurn();
    }

    void UpdateSliders()
    {
        LuxMeter.value = Lux;
        UmbraMeter.value = Umbra;
    }
}
