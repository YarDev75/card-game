using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabsScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        anim.SetBool("Active", true);
    }
    public void OnPointerExit(PointerEventData data)
    {
        anim.SetBool("Active", false);
    }
}
