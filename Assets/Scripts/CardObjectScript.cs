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
    [SerializeField] private Image Primary;
    [SerializeField] private Image Damage;
    [SerializeField] private Image Arrow;
    [SerializeField] private Image Art;
    [SerializeField] private Image Cost;
    [SerializeField] private Animator anim;
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
    BoardManager boardManager;
    int sortOrder;                                     //SpriteRenderer's sorting order, keeping track cause changing it in a few places
    bool Dragin;                                       //whether or not the card is currently draged
    bool sent;                                         //funky thing for the enemy AI

    private void Start()
    {
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
        if (Dragin)
        {
            if (!boardManager.PlayersTurn) Dragin = false;
            var MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(MousePos.x,MousePos.y, 0);
        }
        else
        {
            var shift = (TargetPos - transform.position) * Time.deltaTime * Speed;
            transform.position += new Vector3(shift.x, shift.y, 0);
            canvas.sortingOrder = sr.sortingOrder + 1;                                  //gonna change later, will be like this for now
        }

        if (sent && Vector2.Distance(transform.position, TargetPos) < 0.01f) Placed();
    }

    void DrawStats()
    {
        canvas.gameObject.SetActive(true);
        canvas.worldCamera = Camera.main;
        Name.text = content.Name;
        Name.color = content.element == Card.elements.light ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        Damage.sprite = DamageNums[content.damage + (9 * (content.element == Card.elements.light ? 1 : 0))];
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
        DrawStats();
    }

    //called when card is placed on the board
    void Placed()
    {
        if(PlayerCard) GetComponentInParent<HandManager>().RemoveCardFromHand(HandID);
        sr.sortingOrder = sortOrder;
        canvas.sortingOrder = sr.sortingOrder + 1;
        sr.transform.localPosition = Vector2.zero;
        anim.enabled = false;
        Destroy(this);
    }

    private void OnMouseEnter()
    {
        if (!Dragin && PlayerCard)
        {
            anim.SetBool("selected", true);
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
        if (PlayerCard && boardManager.PlayersTurn)
        {
            sr.sortingOrder = 102;
            canvas.sortingOrder = 103;
            Dragin = true;
            anim.SetBool("selected", false);
        }
    }

    private void OnMouseUp()
    {
        if (Dragin)
        {
            if(boardManager.PlaceCard(content, gameObject)) Placed();
            else Dragin = false;
        }
    }


}
