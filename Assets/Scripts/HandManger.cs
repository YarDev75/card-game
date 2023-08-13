using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManger : MonoBehaviour
{
    [SerializeField] private CardTemplate[] Deck;
    [SerializeField] private GameObject EmptyCard;
    [SerializeField] private Transform CardSpawn;
    [SerializeField] private int StartCards;
    [SerializeField] private float DrawDelay;
    [SerializeField] private Vector2 HandOrigin;
    [SerializeField] private float HandSize;
    private List<CardTemplate> RemainingDeck;
    private List<CardObjectScript> Hand;
    private bool InitialDraw;
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
        if (Input.GetKeyDown(KeyCode.Space)) DrawCard(); //for testing
    }

    // made it a separate function in case it will be used somewhere else
    void Initialize()
    {
        RemainingDeck = new List<CardTemplate>();
        Hand = new List<CardObjectScript>();
        for (int i = 0; i < Deck.Length; i++) RemainingDeck.Add(Deck[i]);
    }

    void PositionCards()
    {
        float gap = HandSize / (Hand.Count + 1);
        for (int i = 0; i < Hand.Count; i++)
        {
            Hand[i].TargetPos = new Vector3(HandOrigin.x + gap * (i + 1), HandOrigin.y);
        }
    }

    void DrawCard()
    {
        if (RemainingDeck.Count > 0)
        {
            var newCard = Instantiate(EmptyCard, CardSpawn.position, Quaternion.identity).GetComponent<CardObjectScript>();
            var i = Random.Range(0, RemainingDeck.Count);
            newCard.content = RemainingDeck[i];
            RemainingDeck.RemoveAt(i);
            Hand.Add(newCard);
            PositionCards();
        }
        else print("deck empty");
    }
}
