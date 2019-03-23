using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





[RequireComponent(typeof(Rigidbody))]
public class PlyerController : MonoBehaviour
{

    private Vector3 m_Velocity = Vector3.zero;
    Rigidbody2D body;
    bool grounded = true;
    Animator anim;
    
    bool land = true;
    bool jump = false;
    Vector2 swipeStart, swipeDelta;
    bool isDraging, tap, swipeUp;
    SwipeControl sc;
    bool crawling = false;
    bool reachEdge = false;



    [SerializeField] private float speed = 20;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private float jumpForce = 20;
    [SerializeField] private Transform overlapPos;
    [SerializeField] private float overlapRadius;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] bool falling = false;
    [SerializeField] public bool slide;
    [SerializeField] GameObject ground;
    [SerializeField] float rayRange;
    [SerializeField] Transform rayOrigin;
    [SerializeField] GameManager gm;
    [SerializeField] bool canCrawl = true;
    [SerializeField] float crawlTime = 2;
    [SerializeField] LayerMask whatIsEdge;
    [SerializeField] bool timeToCrawl;
    [SerializeField] bool timeToRun;
    

    void Start()
    {
        sc = GetComponent<SwipeControl>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

  


    // Update is called once per frame
    private void FixedUpdate()
    {
        

        Collider2D[] overlappedObjects = Physics2D.OverlapCircleAll(overlapPos.position, overlapRadius, whatIsGround);


        grounded = false;


        if (overlappedObjects.Length > 0)
        {
            for (int i = 0; i < overlappedObjects.Length; i++)
            {
                if (overlappedObjects[i].gameObject != gameObject)
                {
                    Debug.Log("Enter");

                    Debug.Log("Enter2");

                    grounded = true;
                    jump = false;

                    if (falling == true)
                    {
                        falling = false;
                        land = true;
                        anim.SetBool("Land", true);

                    }

                    crawling = false;
                    anim.SetBool("Crawl", false);

                    anim.SetBool("Jumping", false);
                    anim.SetBool("Fall", false);

                    reachEdge = false;
                    canCrawl = true;

                    StopAllCoroutines();

                }
            }

        }

        
        Collider2D[] overlappedObjectsWalls = Physics2D.OverlapCircleAll(overlapPos.position, overlapRadius, whatIsWall);
        


        if (overlappedObjectsWalls.Length > 0)
        {
            for (int i = 0; i < overlappedObjectsWalls.Length; i++)
            {
                if (overlappedObjectsWalls[i].gameObject != gameObject)
                {
                    crawling = true;
                    jump = false;

                    Debug.Log("Enter");
                    anim.SetBool("Crawl", true);

                    // Invoke("CheckIfStillCanCrawl", crawlTime);

                    StartCoroutine(CheckIfStillCrawling());
                    anim.SetBool("Jumping", false);
                    anim.SetBool("Fall", false);
                   
                    
                    reachEdge = false;
                    canCrawl = true;
                }
            }

        }


         Collider2D[] overlappedObjectsEdges = Physics2D.OverlapCircleAll(overlapPos.position, overlapRadius, whatIsEdge);
        
        
        if (overlappedObjectsEdges.Length > 0)
        {
            for (int i = 0; i < overlappedObjectsEdges.Length; i++)
            {
                if (overlappedObjectsEdges[i].gameObject != gameObject)
                {
                    reachEdge = true;
                    anim.SetBool("StopCrawl", true);
                    


                 }
            }

        }


            if(!slide)
            {

            anim.SetBool("Slide", false);
            }
    


        // crawl up 
        if(reachEdge)
        {
            canCrawl = true;
            if(timeToCrawl)
            {
                Vector3 targetVelocity = new Vector2(speed * 10f * Time.deltaTime, speed /2 * 10f * Time.deltaTime);
                // And then smoothing it out and applying it to the character
                body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }
            else
            {
              
                Vector3 targetVelocity = new Vector2(speed * 10f * Time.deltaTime, body.velocity.y);
                // And then smoothing it out and applying it to the character
                body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            }

          
       
        }


        if (!canCrawl)
            if(!reachEdge)
        {
            Invoke("Restart", 1);
        }


        if (timeToRun)
        {
            anim.SetBool("StopCrawl", false);
        }

        // control with swipes
        if (sc.swipeUp || Input.GetMouseButtonDown(0))
        {
            if (grounded)
            {
                Jump();
            }
        }

       


        if(sc.swipeDown)
        {
            if (grounded)
            {
                Slide();
            }
        }


        // run or crawl acording to conditions
        if (canCrawl && !reachEdge)
        {

            if (!crawling)
                Move();
            else
                CrawlUp();

        }

       
    }


    IEnumerator CheckIfStillCrawling()
    {
        yield return new WaitForSeconds(crawlTime);
        canCrawl = false;

    }

    void CheckIfStillCanCrawl()
    {
        if (crawling)
            if(!reachEdge)
        {
            canCrawl = false;
            Invoke("Restart", 1);
        }
    }

    void Restart()
    {
        gm.Again();
    }

    void CrawlUp()
    {
        Vector3 targetVelocity = new Vector2(body.velocity.x, speed * 10f * Time.deltaTime);
        // And then smoothing it out and applying it to the character
        body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    void Slide()
    {
        slide = true;
        anim.SetBool("Slide", true);
    }


    void Move()
    {

        Vector3 targetVelocity = new Vector2(speed * 10f * Time.deltaTime, body.velocity.y);
        // And then smoothing it out and applying it to the character
        body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

    }


    void Jump()
    {
        anim.SetBool("Jumping", true);
        grounded = false;
        body.AddForce(new Vector2(0f, jumpForce));
        anim.SetBool("Slide", false);

    }


  

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(overlapPos.position, overlapRadius);
    }





}
