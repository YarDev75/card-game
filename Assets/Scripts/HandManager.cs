using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private bool Enemy;               //whether or not the card belongs to enemy
    [SerializeField] private Card[] Deck;              //all available cards (assigned through inspector for now)
    [SerializeField] private GameObject EmptyCard;     //prefab
    [SerializeField] private Transform CardSpawn;      //where a drawn card will spawn (transform, so it's easier to move in editor)
    [SerializeField] private int StartCards;           //how many cards to draw at the start of the game
    [SerializeField] private float DrawDelay;          
    [SerializeField] private Transform HandOrigin;     //left border of hand
    [SerializeField] private float HandSize;
    private List<Card> RemainingDeck;
    public List<CardObjectScript> Hand;                //cards in hand, public so the enemyAI can use it
    private bool InitialDraw;                          //just a flag to check whether or not the initial draw is happening now
    private float timer;

    private void Start()
    {
        Initialize();
        InitialDraw = true;
    }

    private void Update()
    {
        if (InitialDraw)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                DrawCard();
                timer = DrawDelay;
                if (Hand.Count >= StartCards) InitialDraw = false;
            }
        }
    }

    //initializes lists, made it a separate function in case it will be used somewhere else
    void Initialize()
    {
        RemainingDeck = new List<Card>();
        Hand = new List<CardObjectScript>();
        for (int i = 0; i < Deck.Length; i++) RemainingDeck.Add(Deck[i]);
    }

    void PositionCards() 
    {
        float gap = HandSize / (Hand.Count + 1);
        for (int i = 0; i < Hand.Count; i++)
        {
            Hand[i].TargetPos = new Vector3(HandOrigin.position.x + gap * (i + 1), HandOrigin.position.y);
            Hand[i].sr.sortingOrder = i * 2;
            Hand[i].transform.position = new Vector3(Hand[i].transform.position.x, Hand[i].transform.position.y, i * -0.1f);
            Hand[i].HandID = i;
        }
    }

    public void DrawCard()
    {
        if (RemainingDeck.Count > 0)
        {
            var newCard = Instantiate(EmptyCard, CardSpawn.position, Quaternion.identity).GetComponent<CardObjectScript>();
            var i = Random.Range(0, RemainingDeck.Count);   //used in 2 places, therefore a variable
            newCard.content = RemainingDeck[i];
            newCard.transform.parent = transform;
            newCard.HandID = Hand.Count;
            newCard.PlayerCard = !Enemy;
            RemainingDeck.RemoveAt(i);
            Hand.Add(newCard);
            PositionCards();
        }
        else print("deck empty");
    }

    public void RemoveCardFromHand(int ind)
    {
        Hand.RemoveAt(ind);
        PositionCards();
    }
}
