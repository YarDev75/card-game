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
    [SerializeField] private Slider PlayerHealthBar;
    [SerializeField] private TextMeshProUGUI EnemyHealthText;
    [SerializeField] private Slider EnemyHealthBar;
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
        PlayerHealthBar.maxValue = PlayerMaxHealth;
        PlayerHealthBar.value = PlayerHealth;
        EnemyHealthBar.maxValue = EnemyMaxHealth;
        EnemyHealthBar.value = EnemyHealth;
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
        for (int i = 0; i < TheBoard.Length; i++)
        {
            if (TheBoard[i].damage <= 0) Destroy(TheBoard[i].gameObject);
        }
        for (int i = 4; i < 12; i++) TheBoard[i].TurnDamage = TheBoard[i].damage;
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
        for(int i = 0; i < TheBoard.Length/4; i++) //just goes through one row and then calculates all cards in the column
        {
            int enemySecondaryInd = i;
            int enemyPrimaryInd = i + TheBoard.Length / 4;
            CardObjectScript enemyPrimary = TheBoard[enemyPrimaryInd];
            int playerSecondaryInd = i + (TheBoard.Length / 4 * 3);
            int playerPrimaryInd = i + (TheBoard.Length / 4 * 2);
            CardObjectScript playerPrimary = TheBoard[playerPrimaryInd];
            ApplyCardEffects(playerPrimary);
            ApplyCardEffects(enemyPrimary);

            //new approach: treating damage as a shockwave going through a column
            int playerDamage = TheBoard[playerPrimaryInd].TurnDamage;
            int enemyDamage = TheBoard[enemyPrimaryInd].TurnDamage;

            //player card attack (here order doesn't matter, both of them attack and the damage is stored independently)

            //calculating offset (so the card attacks to the left or to the right, depending on the direction of the card)
            int offset = 0;
            switch (TheBoard[playerPrimaryInd].content.direction)
            {
                case Card.directions.front:
                    offset = 0;
                    break;
                case Card.directions.right:
                    offset = 1;
                    break;
                case Card.directions.left:
                    offset = -1;
                    break;
                case Card.directions.fork:
                    offset = -1;
                    CalculateCardAttack(playerPrimaryInd, enemyPrimaryInd, enemySecondaryInd, playerDamage, ref EnemyHealth, offset); //additional attack for fork type
                    offset = 1;
                    break;
            }
            CalculateCardAttack(playerPrimaryInd, enemyPrimaryInd, enemySecondaryInd, playerDamage, ref EnemyHealth, offset);

            //enemy card attacks
            switch (TheBoard[playerPrimaryInd].content.direction)
            {
                case Card.directions.front:
                    offset = 0;
                    break;
                case Card.directions.right:
                    offset = 1;
                    break;
                case Card.directions.left:
                    offset = -1;
                    break;
                case Card.directions.fork:
                    offset = -1;
                    CalculateCardAttack(enemyPrimaryInd, playerPrimaryInd, playerSecondaryInd, enemyDamage, ref PlayerHealth, offset); //additional attack for fork type
                    offset = 1;
                    break;
            }
            CalculateCardAttack(enemyPrimaryInd, playerPrimaryInd, playerSecondaryInd, enemyDamage, ref PlayerHealth, offset);


            //secondary cards do stuff


            PlayerHealthText.text = PlayerHealth.ToString();
            PlayerHealthBar.value = PlayerHealth;
            EnemyHealthText.text = EnemyHealth.ToString();
            EnemyHealthBar.value = EnemyHealth;
        }
        print($"EnemyHealth: {EnemyHealthText.text} PlayerHealth: {PlayerHealthText.text}");
    }

    void CalculateCardAttack(int boardID, int opposingPInd, int opposingSInd, int damage, ref int opposingHp, int offset)
    {
        //checks if the tearget is within boundries (so that direction doesn't lead out the board)
        if ((boardID % 4) + offset >= 0 && (boardID % 4) + offset < 4)
        {
            //damaging the primary
            if (TheBoard[opposingPInd + offset] != null)
            {
                TheBoard[opposingPInd + offset].damage -= damage;
                if (TheBoard[opposingPInd + offset].damage <= 0)
                {
                    damage = Mathf.Abs(TheBoard[opposingPInd + offset].damage); //Mathf.Abs removes the sign, so if the enemy has -3 damage, the damage variable becomes 3
                    //don't destroy the card before it can attack, cards with no damage will be destroyed at the end of the turn 
                }
                else damage = 0; //used it all up on an enemy, not killing it
            }
            //damaging the secondary, if enough damage left
            if (TheBoard[opposingSInd + offset] != null && damage > 0)
            {
                TheBoard[opposingSInd + offset].damage -= damage;
                if (TheBoard[opposingSInd + offset].damage <= 0)
                {
                    damage = Mathf.Abs(TheBoard[opposingSInd + offset].damage);
                    //don't destroy the card before it can attack, cards with no damage will be destroyed at the end of the turn
                }
                else damage = 0; //used it all up on an enemy, not killing it
            }
            //damaging the enemy, if still damage left
            if (damage > 0)
            {
                opposingHp -= damage;  //opposingHp is a 'ref' variable, so the operation is done directly to the EnemyHealth or PlayerHealth, whichever is the argument
            }

        }
    }

    private void ApplyCardEffects(CardObjectScript card)
    {
        if(card != null)
        {

        }
    }
}
