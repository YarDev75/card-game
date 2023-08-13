using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObjectScript : MonoBehaviour
{
    [SerializeField] private float Speed;
    public Vector3 TargetPos;
    public CardTemplate content;

    private void Update()
    {
        transform.Translate((TargetPos - transform.position) * Time.deltaTime * Speed);
    }
}
