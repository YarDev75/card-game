using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BoardManager : MonoBehaviour
{
    public CardObjectScript[] TheBoard;                                          //0-7 - enemy 8-15 - player

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
    [SerializeField] private int PlayerMaxHealth = 10;
    [SerializeField] private int EnemyMaxHealth = 10;
    [SerializeField] private TextMeshProUGUI PlayerHealthText;
    [SerializeField] private TextMeshProUGUI EnemyHealthText;
    int PlayerHealth;                                                //I think we should just store them in a seperate variable instead of Parsing text every time
    int EnemyHealth;                                                   // Yeah, I was gonna change that, it was just for debugging
    float Lux;
    float Umbra;

    private void Start()
    {
        PlayerHealth = PlayerMaxHealth;
        EnemyHealth = EnemyMaxHealth;
        PlayerHealthText.text = PlayerHealth.ToString();
        EnemyHealthText.text = EnemyHealth.ToString();
        TheBoard = new CardObjectScript[16];
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
        if(EnemyHealth <= 0)
        {
            turnAnouncerText.text = "Victory";
            if(PlayerHealth <= 0) turnAnouncerText.text = "Draw";
            turnAnouncerAnim.SetTrigger("go");
        }else if(PlayerHealth <= 0)
        {
            turnAnouncerText.text = "Defeat";
            turnAnouncerAnim.SetTrigger("go");
        }
        
        NextTurnButton.SetActive(PlayersTurn);
    }

    //for player
    public bool PlaceCard(CardObjectScript card, GameObject obj)
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
        if(Slot != null && (card.content.Primary == ind < 4) && ((card.content.element == Card.elements.light && Lux >= card.content.cost) || (card.content.element == Card.elements.dark && Umbra >= card.content.cost)))
        {
            obj.transform.position = Slot.position;
            if (card.content.element == Card.elements.light)
            {
                Lux -= card.content.cost;
                LuxMeter.value = Lux;
            }
            else
            {
                Umbra -= card.content.cost;
                UmbraMeter.value = Umbra;
            }
            TheBoard[8 + ind] = card;

            return true;
        }
        return false;
    }

    //for enemy AI
    public bool PlaceCard(CardObjectScript card, int ind)
    {
        if (TheBoard[ind] == null)
        {
            TheBoard[ind] = card;
            return true;
        }
        else return false;
    }

    //Cards are doing double damage for some reason, I really need to go to bed now, so please fix it :)
    public void CalculateTurn() 
    {
        for(int i = TheBoard.Length / 4; i < TheBoard.Length/2; i++) { //This calculates primary row only on enemy's side
            CardObjectScript enemyCard = TheBoard[i];
            CardObjectScript playerCard = TheBoard[i + TheBoard.Length / 4];
            ApplyCardEffects(playerCard);
            ApplyCardEffects(enemyCard);

            //TODO: make the hit dirrection actualy do stuff

            if (playerCard != null && enemyCard != null)
            {
                int UnmodifiedPlayerDamage = playerCard.content.damage;                 //playerCard damage before it gets hit
                playerCard.content.damage -= enemyCard.content.damage;
                enemyCard.content.damage -= UnmodifiedPlayerDamage;

                if (playerCard.content.damage <= 0)
                {
                    Destroy(playerCard.gameObject);                      //gameobject gets destroyed and so is the script attached to it, automaticly making TheBoard value null
                }
                if (enemyCard.content.damage <= 0)
                {
                    Destroy(playerCard.gameObject);
                }
            }
            if (enemyCard != null) //If you eliminate the elses you only apply the damage of the cards with remaining damage points as they are not destroyed
            {
                PlayerHealth -= enemyCard.content.damage;
                enemyCard.UpdateStats(); //This updates the damage points
            }
            else if (playerCard != null) 
            { 
                EnemyHealth -= playerCard.content.damage;playerCard.UpdateStats();
            }
            PlayerHealthText.text = PlayerHealth.ToString();
            EnemyHealthText.text = EnemyHealth.ToString();
        }
        print($"EnemyHealth: {EnemyHealthText.text} PlayerHealth: {PlayerHealthText.text}");
    }

    private void ApplyCardEffects(CardObjectScript card)
    {
        if(card != null)
        {

        }
    }
}
