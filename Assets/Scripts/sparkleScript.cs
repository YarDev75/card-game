using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sparkleScript : MonoBehaviour
{
    [SerializeField] Transform TopRight;
    [SerializeField] Transform BottomLeft;
    float Timer;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Timer = Random.Range(1f, 7f);
            anim.SetTrigger(Random.Range(0, 2) == 0 ? "one" : "two");
            Invoke("Jump", 0.7f);
        }
    }

    void Jump()
    {
        transform.position = new Vector3(Random.Range(BottomLeft.position.x, TopRight.position.x), Random.Range(BottomLeft.position.x, TopRight.position.x));
    }
}
