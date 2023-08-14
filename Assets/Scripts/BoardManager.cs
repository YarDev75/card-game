using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] private Text PlayerHealth;
    [SerializeField] private Text EnemyHealth;

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

    //anounces who's turn it is and let's that side place cards
    public void NextTurn()
    {
        PlayersTurn = !PlayersTurn;
        turnAnouncerText.text = (PlayersTurn ? "Player's" : "Enenemy's") + " turn!";
        turnAnouncerAnim.SetTrigger("go");
        if(!PlayersTurn) AI.doTurn();
        CalculateTurn();
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

    public void CalculateTurn() {
        for(int i = TheBoard.Length / 4; i < TheBoard.Length/2; i++) { //This calculates primary row only on enemy's side
            Card enemyCard = TheBoard[i];
            int playerCardPos = i + TheBoard.Length / 4;
            Card playerCard = TheBoard[playerCardPos];
            if(playerCard != null && enemyCard != null)
            {
                ApplyCardEffects(playerCard);
                ApplyCardEffects(enemyCard);
                if(playerCard.damage >= enemyCard.damage) //playerCard is, at least, not weaker than enemyCard
                {
                    //Here we should trigger animation playerCard attacks enemyCard
                    playerCard.damage -= enemyCard.damage;
                    Destroy(enemyCard.GameObject());//Here we need to Destroy() the enemy card player has attacked thus removing it from the view
                    TheBoard[i] = null;
                    if(playerCard.damage == 0)
                    {
                        //Here we need to Destroy() the card that has attacked in this turn thus removing it from the view
                        TheBoard[playerCardPos] = null;
                    }else EnemyHealth.text = (int.Parse(EnemyHealth.text) - playerCard.damage).ToString();
                }
                else //enemyCard is stronger than playerCard
                {
                    //Here we should trigger animation enemyCard attacks playerCard
                    enemyCard.damage -= playerCard.damage;
                    //Here we need to Destroy() the player card enemy has attacked thus removing it from the view
                    TheBoard[i] = null;
                    PlayerHealth.text = (int.Parse(PlayerHealth.text) - enemyCard.damage).ToString();
                }
            }else if(enemyCard != null) //There's no player card in front of this enemy card
            {
                ApplyCardEffects(enemyCard);
                PlayerHealth.text = (int.Parse(PlayerHealth.text) - enemyCard.damage).ToString();
            }
            else if(playerCard != null) //There's no enemy card in front of this player card
            {
                ApplyCardEffects(playerCard);
                EnemyHealth.text = (int.Parse(EnemyHealth.text) - playerCard.damage).ToString();
            }
        }
        print("EnemyHealth: " + EnemyHealth.text + "  PlayerHealth: " + PlayerHealth.text);
    }

    private void ApplyCardEffects(Card card)
    {

    }
}
