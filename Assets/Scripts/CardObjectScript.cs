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
    [SerializeField] private TextMeshProUGUI Primary;
    [SerializeField] private Image Damage;
    [SerializeField] private Image Arrow;
    [SerializeField] private Image Art;
    [SerializeField] private Image Cost;
    [SerializeField] private Animator anim;
    [SerializeField] private Sprite[] arrows;
    [SerializeField] private Sprite[] DamageNums;
    [SerializeField] private Sprite[] Costs;
    [SerializeField] private Sprite[] Themes;
    public SpriteRenderer sr;
    public Vector3 TargetPos;
    public CardTemplate content;
    public int HandID;
    BoardManager boardManager;
    int sortOrder;
    bool Dragin;              //Drag&Drop enabled

    private void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();
        canvas.worldCamera = Camera.main;
        Name.text = content.Name;
        Name.color = content.element == CardTemplate.elements.light ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        Damage.sprite = DamageNums[content.damage + (9 * (content.element == CardTemplate.elements.light ? 1 : 0))];
        Arrow.sprite = arrows[(int)content.direction + (4 * (content.element == CardTemplate.elements.light ? 1 : 0))];
        Cost.sprite = Costs[content.cost];
        Cost.color = content.element == CardTemplate.elements.dark ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        sr.sprite = Themes[(int)content.element];
        Primary.text = content.Primary ? "P" : "S";
        Primary.color = content.element == CardTemplate.elements.dark ? new Color(0.7960785f, 0.8588236f, 0.9882354f) : new Color(0.1294118f, 0.09411766f, 0.1058824f);
        Art.sprite = content.Pic;
    }

    private void Update()
    {
        if (Dragin)
        {
            var MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(MousePos.x,MousePos.y, 0);
        }
        else
        {
            var shift = (TargetPos - transform.position) * Time.deltaTime * Speed;
            transform.position += new Vector3(shift.x, shift.y, 0);
            canvas.sortingOrder = sr.sortingOrder + 1;                     //gonna change later, will be like this for now
        }
    }

    private void OnMouseEnter()
    {
        if (!Dragin)
        {
            anim.SetBool("selected", true);
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
        Dragin = true;
        anim.SetBool("selected", false);
    }

    private void OnMouseUp()
    {
        if (Dragin)
        {
            if(boardManager.PlaceCard(content, gameObject))
            {
                GetComponentInParent<HandManger>().RemoveCardFromHand(HandID);
                sr.sortingOrder = sortOrder;
                canvas.sortingOrder = sr.sortingOrder + 1;
                Destroy(this);
            }
            else
            {
                Dragin = false;
            }
        }
    }
}
