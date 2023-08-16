using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dogMovement : MonoBehaviour
{
    public float velocity = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        gameObject.transform.position = transform.position + new Vector3(h, v, 0) * Time.deltaTime * velocity;
    }
}
