using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpUpgrade : MonoBehaviour
{


    [SerializeField] float fallMultiplyer = 2.5f;
    [SerializeField] float lowJumpMultiplyer = 2;


    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplyer - 1) * Time.deltaTime;
        }


        
    }
}
