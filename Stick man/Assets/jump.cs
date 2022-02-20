using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump : MonoBehaviour
{

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = rb.velocity + new Vector2(0.1f, 0);
        }
        if (Input.GetKey(KeyCode.A)) {
            rb.velocity = rb.velocity + new Vector2(-0.1f, 0);
        }
    }
}
