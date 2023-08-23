using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnenemyAI : MonoBehaviour
{
    public static EnemyPerson person;

    [SerializeField] public AudioSource ThemePlayer;
    [SerializeField] private HandManager Hand;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TextMeshProUGUI ThinkingBubble;      //the three dots thing
    [SerializeField] private Image Looks;
    [Range(0,3)]
    [SerializeField] public int Difficulty;
    public Sprite[] Animation;
    public float frametime;
    public float MaxLux;
    public Slider LuxMeter;
    public TextMeshProUGUI LuxText;
    public float MaxUmbra;
    public Slider UmbraMeter;
    public TextMeshProUGUI UmbraText;
    public EnemyPerson personality;
    public float Lux;
    public float Umbra;
    float Timer;
    float AnimTimer;
    int CardsToPlace;
    bool Placing;
    int ind;

    private void Awake()
    {
        if(person!=null) personality = person;
        Hand.SetDeck(personality);
    }

    private void Start()
    {
        //setting up meters (sliders)
        if(personality.Intro != null)
        {
            ThemePlayer.clip = personality.Intro;
            ThemePlayer.loop = false;
            ThemePlayer.Play();
        }
        Animation = personality.Art;
        Lux = MaxLux;
        LuxMeter.maxValue = MaxLux;
        LuxMeter.value = Lux;
        Umbra = MaxUmbra;
        UmbraMeter.maxValue = MaxUmbra;
        UmbraMeter.value = Umbra;
        Invoke("doTurn", 3.5f);
    }

    private void Update()
    {
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
                    CancelInvoke();
                    ThinkingBubble.text = personality.EndTurnDialogue[Random.Range(0, personality.EndTurnDialogue.Length)];
                    Invoke("ClearDialogueText", 3f);
                    boardManager.Ready(false);
                }
            }
        }
        if(AnimTimer < 0)
        {
            AnimTimer = frametime;
            Looks.sprite = Animation[ind];
            ind = (ind+1)%Animation.Length;
        }
        AnimTimer -= Time.deltaTime;
        if(ThemePlayer.clip == personality.Intro && !ThemePlayer.isPlaying)
        {
            ThemePlayer.clip = personality.Music;
            ThemePlayer.loop = true;
            ThemePlayer.Play();
        }
    }

    //called on turn start, starts the thinking thing
    public void doTurn()
    {
        if (Hand.Hand.Count > 0)
        {
            CardsToPlace = Hand.Hand.Count;
            Timer = Random.Range(1f, 3f);
            Placing = true;
            ThinkingBubble.text = personality.ThinkingDialogue[Random.Range(0, personality.ThinkingDialogue.Length)];
        }
        else
        {
            CancelInvoke();
            ThinkingBubble.text = personality.DeckEmptyDialogue[Random.Range(0, personality.DeckEmptyDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
            boardManager.Ready(false);
        }
    }

    public void Hurt()
    {
        CancelInvoke();
        ThinkingBubble.text = personality.TakenDamageDialogue[Random.Range(0, personality.TakenDamageDialogue.Length)];
        Invoke("ClearDialogueText", 2f);
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
            CancelInvoke();
            ThinkingBubble.text = personality.OutOfManaDialogue[Random.Range(0, personality.OutOfManaDialogue.Length)];
            Invoke("ClearDialogueText", 3f);
            boardManager.Ready(false);
            Placing = false;
            return;
        }
        var AvailablePlaces = new List<int>();
        for (int j = 0; j < 8; j++)
        {
            if (boardManager.TheBoard[j] == null) AvailablePlaces.Add(j);
        }
        if (AvailablePlaces.Count > 0)
        {
            var cardInd = 0;
            var placeInd = 0;
            //thinking part
            switch (Difficulty)
            {
                case 0:
                    //places cards Randomly
                    cardInd = AffordableCards[Random.Range(0, AffordableCards.Count)];
                    var goodPlaces = new List<int>();
                    for (int i = 0; i < AvailablePlaces.Count; i++)
                    {
                        if (Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] > 3) goodPlaces.Add(AvailablePlaces[i]);
                        else if (!Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] < 4) goodPlaces.Add(AvailablePlaces[i]);
                    }
                    if (goodPlaces.Count == 0) return;
                    placeInd = goodPlaces[Random.Range(0, goodPlaces.Count)];
                    break;
                case 1:
                    //prioritizes primaries
                    var goodCards = new List<int>();
                    for (int i = 0; i < AffordableCards.Count; i++)
                    {
                        if (Hand.Hand[AffordableCards[i]].content.Primary || Random.Range(0, 3) == 0) goodCards.Add(AffordableCards[i]);
                    }
                    if (goodCards.Count == 0) cardInd = AffordableCards[Random.Range(0, AffordableCards.Count)];
                    else cardInd = goodCards[Random.Range(0, goodCards.Count)];
                    
                    goodPlaces = new List<int>();
                    for (int i = 0; i < AvailablePlaces.Count; i++)
                    {
                        if (Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] > 3) goodPlaces.Add(AvailablePlaces[i]);
                        else if (!Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] < 4) goodPlaces.Add(AvailablePlaces[i]);
                    }
                    if (goodPlaces.Count == 0) return;
                    placeInd = goodPlaces[Random.Range(0, goodPlaces.Count)];
                    break;
                case 2:
                    // prioritize healers when low hp, check primaries' direction
                    goodCards = new List<int>();
                    for (int i = 0; i < AffordableCards.Count; i++)
                    {
                        if(boardManager.EnemyHealth <= boardManager.EnemyMaxHealth / 2)
                        {
                            for (int j = 0; j < Hand.Hand[AffordableCards[i]].content.effects.Length; j++)
                            {
                                if (Hand.Hand[AffordableCards[i]].content.effects[j] == Card.SpecialEffects.HealOwner)
                                {
                                    goodCards.Add(AffordableCards[i]);
                                    break;
                                }
                            }
                            if(goodCards.Count > 0 && !(goodCards[goodCards.Count-1] == AffordableCards[i]) && ((Hand.Hand[AffordableCards[i]].content.Primary && Random.Range(0, 3) == 0) || Random.Range(0, 5) == 0)) goodCards.Add(AffordableCards[i]);
                        }
                        else
                        {
                            if (Hand.Hand[AffordableCards[i]].content.Primary || Random.Range(0, 3) == 0) goodCards.Add(AffordableCards[i]);
                        }
                    }
                    if (goodCards.Count == 0) cardInd = AffordableCards[Random.Range(0, AffordableCards.Count)];
                    else cardInd = goodCards[Random.Range(0, goodCards.Count)];

                    goodPlaces = new List<int>();
                    for (int i = 0; i < AvailablePlaces.Count; i++)
                    {
                        if (Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] > 3)
                        {
                            switch (Hand.Hand[cardInd].content.direction)
                            {
                                case Card.directions.right:
                                    if ((AvailablePlaces[i] % 4) + 1 < 4) goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                                case Card.directions.left:
                                    if ((AvailablePlaces[i] % 4) - 1 >= 0) goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                                default:
                                    goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                            }
                        }
                        else if (!Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] < 4) goodPlaces.Add(AvailablePlaces[i]);
                    }
                    if (goodPlaces.Count == 0) return;
                    placeInd = goodPlaces[Random.Range(0, goodPlaces.Count)];
                    break;
                case 3:
                    //places buffers under primaries, prioritezes lux an umbra generators if said values are below 4
                    goodCards = new List<int>();
                    for (int i = 0; i < AffordableCards.Count; i++)
                    {
                        for (int j = 0; j < Hand.Hand[AffordableCards[i]].content.effects.Length; j++)
                        {
                            if (Hand.Hand[AffordableCards[i]].content.effects[j] == Card.SpecialEffects.HealOwner && (boardManager.EnemyHealth <= boardManager.EnemyMaxHealth / 2))
                            {
                                goodCards.Add(AffordableCards[i]);
                                break;
                            }
                            else if (Hand.Hand[AffordableCards[i]].content.effects[j] == Card.SpecialEffects.RecoverLux && Lux < 4)
                            {
                                goodCards.Add(AffordableCards[i]);
                                break;
                            }
                            else if (Hand.Hand[AffordableCards[i]].content.effects[j] == Card.SpecialEffects.RecoverUmbra && Umbra < 4)
                            {
                                goodCards.Add(AffordableCards[i]);
                                break;
                            }
                        }
                        if (goodCards.Count > 0 && !(goodCards[goodCards.Count - 1] == AffordableCards[i]) && (Hand.Hand[AffordableCards[i]].content.Primary || Random.Range(0, 3) == 0)) goodCards.Add(AffordableCards[i]);
                        
                    }
                    if (goodCards.Count == 0) cardInd = AffordableCards[Random.Range(0, AffordableCards.Count)];
                    else cardInd = goodCards[Random.Range(0, goodCards.Count)];

                    goodPlaces = new List<int>();
                    bool booster = false;
                    for (int j = 0; j < Hand.Hand[cardInd].content.effects.Length; j++)
                    {
                        if (Hand.Hand[cardInd].content.effects[j] == Card.SpecialEffects.BoostPrimary)
                        {
                            booster = true;
                        }
                    }
                    for (int i = 0; i < AvailablePlaces.Count; i++)
                    {
                        if (Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] > 3)
                        {
                            switch (Hand.Hand[cardInd].content.direction)
                            {
                                case Card.directions.right:
                                    if ((AvailablePlaces[i] % 4) + 1 < 4) goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                                case Card.directions.left:
                                    if ((AvailablePlaces[i] % 4) - 1 >= 0) goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                                default:
                                    goodPlaces.Add(AvailablePlaces[i]);
                                    break;
                            }
                        }
                        else if (!Hand.Hand[cardInd].content.Primary && AvailablePlaces[i] < 4)
                        {
                            if (booster && boardManager.TheBoard[AvailablePlaces[i] + 4] != null) goodPlaces.Add(AvailablePlaces[i]);
                            else if(!booster) goodPlaces.Add(AvailablePlaces[i]);
                        }
                    }
                    if (goodPlaces.Count == 0) return;
                    placeInd = goodPlaces[Random.Range(0, goodPlaces.Count)];
                    break;
            }


            //placing
            if (boardManager.PlaceCard(Hand.Hand[cardInd], placeInd))
            {
                Hand.Hand[cardInd].Send(boardManager.EnemySlots[placeInd].position);
                if (Hand.Hand[cardInd].content.element == Card.elements.light) Lux -= Hand.Hand[cardInd].content.cost;
                else Umbra -= Hand.Hand[cardInd].content.cost;
                boardManager.UpdateUIStats();
                Hand.RemoveCardFromHand(cardInd);
            }
        }

    }
}
