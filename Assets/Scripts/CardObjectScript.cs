using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardObjectScript : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private Image Primary;
    [SerializeField] private Image Damage;
    [SerializeField] private Image Arrow;
    [SerializeField] private Image Art;
    [SerializeField] private Image Cost;
    [SerializeField] public Animator anim;
    [SerializeField] private Sprite[] arrows;          //arrays store all the sprites for all the states
    [SerializeField] private Sprite[] DamageNums;
    [SerializeField] private Sprite[] RowInds;
    [SerializeField] private Sprite[] Costs;
    [SerializeField] private Sprite[] ThemesFront;
    [SerializeField] private Sprite[] ThemesBack;
    public bool PlayerCard;                            //whether or not the card belongs to a player
    public SpriteRenderer sr;
    public Vector3 TargetPos;                          //where the card is going
    public Card content;  
    public int HandID;                                 //Index in the Hand array in HandManager
    public int damage;                                 //actual damage stat of the card
    public int TurnDamage;                             //damage for the current turn (so everyone attacks at the same time, noone gets the damage lowered before attacking)
    Vector3 BoardPos;
    BoardManager boardManager;
    int sortOrder;                                     //SpriteRenderer's sorting order, keeping track cause changing it in a few places
    bool Dragin;                                       //whether or not the card is currently draged
    bool sent;                                         //funky thing for the enemy AI
    bool placed;
    bool zoomed = false;

    private void Start()
    {
        damage = content.damage;
        boardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();
        if (PlayerCard) DrawStats();
        else
        {
            sr.sprite = ThemesBack[(int)content.element];
            canvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!placed)
        {
            if (Dragin)
            {
                if (!boardManager.PlacingTurn) Dragin = false;
                var MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(MousePos.x, MousePos.y, 0);
            }
            else
            {
                var shift = (TargetPos - transform.position) * Time.deltaTime * Speed;
                transform.position += new Vector3(shift.x, shift.y, 0);
                canvas.sortingOrder = sr.sortingOrder + 1;                                  //gonna change later, will be like this for now
            }

            if (sent && Vector2.Distance(transform.position, TargetPos) < 0.01f) Placed();
        }
        else
        {
            //put all the code for card behavior on board here if needed
            var shift = (TargetPos - transform.position) * Time.deltaTime * Speed;
            transform.position += new Vector3(shift.x, shift.y, 0);
            if(zoomed && Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("ZoomOut");
                Invoke("changeZoomed", 1f);
                TargetPos = BoardPos;
                boardManager.Zooming = false;
                gameObject.GetComponents<Collider2D>()[1].enabled = false;
            }
        }


    }



    void DrawStats()
    {
        canvas.gameObject.SetActive(true);
        canvas.worldCamera = Camera.main;
        Description.text = content.description;
        Name.text = content.Name;
        Name.color = content.element == Card.elements.light ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        if (damage >= 0 && damage < 10) Damage.sprite = DamageNums[damage + (10 * (content.element == Card.elements.light ? 1 : 0))];
        else Destroy(gameObject);
        Arrow.sprite = arrows[(int)content.direction + (4 * (content.element == Card.elements.light ? 1 : 0))];
        Cost.sprite = Costs[content.cost];
        Cost.color = content.element == Card.elements.dark ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        sr.sprite = ThemesFront[(int)content.element];
        Primary.sprite = RowInds[(content.Primary ? 0 : 1) + (content.element == Card.elements.light ? 0 : 2)];
        Art.sprite = content.Pic;
    }

    //called when enemy AI sends a card to its place
    public void Send(Vector2 pos)
    {
        TargetPos = pos;
        sent = true;
    }

    //called when card is placed on the board
    void Placed()
    {
        if(PlayerCard) GetComponentInParent<HandManager>().RemoveCardFromHand(HandID);
        TargetPos = transform.position;
        sr.sortingOrder = sortOrder;
        canvas.sortingOrder = sr.sortingOrder + 1;
        anim.SetBool("placed", true);
        placed = true;
        //GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnMouseEnter()
    {
        if (!Dragin && PlayerCard)
        {
            anim.SetBool("selected", true);
            Description.transform.parent.gameObject.SetActive(Description.text != " ");
            sortOrder = sr.sortingOrder;
            sr.sortingOrder = 100;
            canvas.sortingOrder = 101;
        }
    }
    private void OnMouseExit()
    {
        if (!Dragin && PlayerCard)
        {
            anim.SetBool("selected", false);
            sr.sortingOrder = sortOrder;
            canvas.sortingOrder = sr.sortingOrder + 1;
        }
    }

    private void OnMouseDown()
    {
        if (PlayerCard && boardManager.PlacingTurn && !placed)
        {
            sr.sortingOrder = 102;
            canvas.sortingOrder = 103;
            Dragin = true;
            anim.SetBool("selected", false);
        }
        if (placed && !zoomed && !boardManager.Zooming)
        {
            anim.SetTrigger("ZoomIn");
            Description.transform.parent.gameObject.SetActive(Description.text != " ");
            boardManager.Zooming = true;
            BoardPos = transform.position;
            TargetPos = new Vector3(0, -1.4f, 0);
            sr.sortingOrder = 104;
            canvas.sortingOrder = 105;
            Invoke("changeZoomed", 1f); 
            gameObject.GetComponents<Collider2D>()[0].enabled = false;         //delayed the enabling of the collider so the player doesn't interrupt the animation
        }
    }

    private void OnMouseUp()
    {
        if (Dragin && !placed)
        {
            if(boardManager.PlaceCard(this, gameObject)) Placed();
            else Dragin = false;
        }
    }

    public void UpdateStats()
    {
        DrawStats(); //This is just a patch, if you have a better solution, change it you know this part of the code better
    }
    private void changeZoomed() 
    { 
        zoomed = !zoomed;
        if (zoomed) gameObject.GetComponents<Collider2D>()[1].enabled = true;
        else
        {
            gameObject.GetComponents<Collider2D>()[0].enabled = true;
            sr.sortingOrder = 100;
            canvas.sortingOrder = 101;
        }
    }
}
