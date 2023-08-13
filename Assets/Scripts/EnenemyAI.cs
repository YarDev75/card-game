using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnenemyAI : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TextMeshProUGUI ThinkingBubble;
    [SerializeField] private float MaxLux;
    [SerializeField] private Slider LuxMeter;
    [SerializeField] private float MaxUmbra;
    [SerializeField] private Slider UmbraMeter;
    float Lux;
    float Umbra;
    float Timer;
    float TimerGoal;
    HandManager Hand;

    private void Start()
    {
        Hand = GetComponent<HandManager>();
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
    }

    private void Update()
    {
        if(TimerGoal > 0)
        {
            Timer += Time.deltaTime;
            if(Timer >= TimerGoal/3) ThinkingBubble.text = "..";
            if(Timer >= (TimerGoal/3)*2) ThinkingBubble.text = "...";
            if(Timer >= TimerGoal)
            {
                TimerGoal = -1;
                PlaceCards();
            }
        }
    }

    public void doTurn()
    {
        TimerGoal = Random.Range(1f, 6f);
        Timer = 0;
        ThinkingBubble.text = ".";
        Invoke("PlaceCards", Timer);
    }

    void PlaceCards() //that's where the actual card placement AI should be at
    {



        boardManager.NextTurn();
    }
}
