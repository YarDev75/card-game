using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckScreenCardObject : MonoBehaviour
{
    public Card content;

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
    [SerializeField] private SpriteRenderer sr;
    DeckOrganizingManager boardManager;
    bool Dragin;
    bool zoomed;
    int sortOrder;
    Vector3 BoardPos;
    public Vector3 TargetPos;

    private void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<DeckOrganizingManager>();
        anim.SetBool("placed", true);
        DrawStats();
    }

    private void Update()
    {
        if (Dragin)
        {
            var MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(MousePos.x, MousePos.y, 0);
        }
        else
        {
            var shift = (TargetPos - transform.position) * Time.deltaTime * Speed;
            transform.position += new Vector3(shift.x, shift.y, 0);
            canvas.sortingOrder = sr.sortingOrder + 1;                                  //gonna change later, will be like this for now
        }


        if (zoomed && Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("ZoomOut");
            Invoke("changeZoomed", 1f);
            TargetPos = BoardPos;
        }
    }

    void DrawStats()
    {
        canvas.gameObject.SetActive(true);
        canvas.worldCamera = Camera.main;
        Description.text = content.description;
        Name.text = content.Name;
        Name.color = content.element == Card.elements.light ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        if (content.damage >= 0 && content.damage < 10) Damage.sprite = DamageNums[content.damage + (10 * (content.element == Card.elements.light ? 1 : 0))];
        else Destroy(gameObject);
        Arrow.sprite = arrows[(int)content.direction + (4 * (content.element == Card.elements.light ? 1 : 0))];
        Cost.sprite = Costs[content.cost];
        Cost.color = content.element == Card.elements.dark ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        sr.sprite = ThemesFront[(int)content.element];
        Primary.sprite = RowInds[(content.Primary ? 0 : 1) + (content.element == Card.elements.light ? 0 : 2)];
        Art.sprite = content.Pic;
    }

    private void OnMouseEnter()
    {
        if (!Dragin)
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
        if (!Dragin)
        {
            anim.SetBool("selected", false);
            sr.sortingOrder = sortOrder;
            canvas.sortingOrder = sr.sortingOrder + 1;
        }
    }

    private void OnMouseDown()
    {
        sr.sortingOrder = 102;
        canvas.sortingOrder = 103;
        Dragin = true;
        anim.SetBool("selected", false);
        
        if (!zoomed && Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("ZoomIn");
            Description.transform.parent.gameObject.SetActive(Description.text != " ");
            BoardPos = transform.position;
            TargetPos = new Vector3(0, -1.4f, 0);
            sr.sortingOrder = 104;
            canvas.sortingOrder = 105;
            Invoke("changeZoomed", 1f);
            gameObject.GetComponents<Collider2D>()[0].enabled = false;         //delayed the enabling of the collider so the player doesn't interrupt the animation
        }
    }

    void changeZoomed()
    {
        zoomed = !zoomed;
    }

    private void OnMouseUp()
    {
        if (Dragin)
        {
            boardManager.Place(this);
            Dragin = false;
        }
    }
}
