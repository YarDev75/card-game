using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public Card[] TheBoard;                                          //0-7 - enemy 8-15 - player

    [SerializeField] private EnenemyAI AI;
    [SerializeField] private Animator turnAnouncerAnim;
    [SerializeField] private TextMeshProUGUI turnAnouncerText;
    [SerializeField] private GameObject NextTurnButton;
    public Transform[] EnemySlots;                                   //positions for cards on enemy side
    public bool PlayersTurn;                                         //bool 'cause only 2 states

    //player stuff
    [SerializeField] private Transform[] PlayerSlots;                //positions for cards on Player side
    [SerializeField] private float SnapDistance;                     //for Drag&Drop mechanic
    [SerializeField] private float MaxLux;                           //Lux & Umbra -- resources for placing cards
    [SerializeField] private Slider LuxMeter;
    [SerializeField] private float MaxUmbra;
    [SerializeField] private Slider UmbraMeter;
    float Lux;
    float Umbra;

    private void Start()
    {
        TheBoard = new Card[16];
        PlayersTurn = true;
        //setting up meters (sliders)
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
    }

    void DoCombat()
    {

    }

    //anounces who's turn it is and let's that side place cards
    public void NextTurn()
    {
        PlayersTurn = !PlayersTurn;
        turnAnouncerText.text = (PlayersTurn ? "Player's" : "Enenemy's") + " turn!";
        turnAnouncerAnim.SetTrigger("go");
        if(!PlayersTurn) AI.doTurn();
        NextTurnButton.SetActive(PlayersTurn);
    }

    //for player
    public bool PlaceCard(Card card, GameObject obj)
    {
        //drag&drop stuff
        float Min = SnapDistance;
        Transform Slot = null;
        int ind = 0;
        for (int i = 0; i < PlayerSlots.Length; i++)
        {
            if(Vector2.Distance(obj.transform.position, PlayerSlots[i].position) < Min)
            {
                Min = Vector2.Distance(obj.transform.position, PlayerSlots[i].position);
                Slot = PlayerSlots[i];
                ind = i;
            }
        }
        //checks if the card can be placed on selected spot 
        if(Slot != null && (card.Primary == ind >= 4) && ((card.element == Card.elements.light && Lux >= card.cost) || (card.element == Card.elements.dark && Umbra >= card.cost)))
        {
            obj.transform.position = Slot.position;
            if (card.element == Card.elements.light)
            {
                Lux -= card.cost;
                LuxMeter.value = Lux;
            }
            else
            {
                Umbra -= card.cost;
                UmbraMeter.value = Umbra;
            }
            TheBoard[8 + ind] = card;

            return true;
        }
        return false;
    }

    //for enemy AI
    public bool PlaceCard(Card card, int ind)
    {
        if (TheBoard[ind] == null)
        {
            TheBoard[ind] = card;
            return true;
        }
        else return false;
    }
}
