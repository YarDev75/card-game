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
    [SerializeField] private TextMeshProUGUI Damage;
    [SerializeField] private Animator anim;
    public SpriteRenderer sr;
    public Vector3 TargetPos;
    public CardTemplate content;
    private int sortOrder;
    

    private void Start()
    {
        canvas.worldCamera = Camera.main;
        Name.text = content.Name;
        Damage.text = content.damage.ToString();
    }

    private void Update()
    {
        transform.Translate((TargetPos - transform.position) * Time.deltaTime * Speed);
        canvas.sortingOrder = sr.sortingOrder + 1;                     //gonna change later, will be like this for now
    }

    private void OnMouseEnter()
    {
        anim.SetBool("selected", true);
        //sortOrder = sr.sortingOrder;
        //sr.sortingOrder = 100;
        //canvas.sortingOrder = 101;
    }
    private void OnMouseExit()
    {
        anim.SetBool("selected", false);
        //sr.sortingOrder = sortOrder;
        //canvas.sortingOrder = sr.sortingOrder + 1;
    }
}
