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
            if(Timer >= TimerGoal/3) ThinkingBubble.text = "..";
            if(Timer >= (TimerGoal/3)*2) ThinkingBubble.text = "...";
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
        ThinkingBubble.text = ".";
    }

    void PlaceCards() //that's where the actual card placement AI should be at
    {
        //needs some work
        for (int i = 0; i < Random.Range(0,Hand.Hand.Count); i++)
        {
            var placeInd = Random.Range(0, 8);
            var cardInd = Random.Range(0, Hand.Hand.Count);
            boardManager.PlaceCard(Hand.Hand[cardInd].content, placeInd);
            Hand.Hand[cardInd].Send(boardManager.EnemySlots[placeInd].position);
            Hand.RemoveCardFromHand(cardInd);
        }


        boardManager.NextTurn();
    }
}
