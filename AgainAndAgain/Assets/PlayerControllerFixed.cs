using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFixed : MonoBehaviour
{

    SwipeControl sc;
    Rigidbody2D body;
    GameManager gm;
    Animator anim;




    public bool grounded;







    [SerializeField] LayerMask whatIsGround;
    [SerializeField] private float speed = 100;
    [SerializeField] private float crawlTime = 2;
   
    [SerializeField] float jumpForce = 100;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    Vector3 m_Velocity = Vector3.zero;
    public bool falling = false;
    AnimatorClipInfo[] animations;
    float jumpDuration;
    CapsuleCollider2D collider;
    Vector2 colliderOriginalOffset;
    Vector2 colliderOriginalSize;
    public Vector2 slideColliderOffset;
    public Vector2 slideColliderSize;
    public bool sliding = false;
    public float slideDuration = 1;
    public bool crawlling = false;
    public bool reachedEdge = false;
    public bool stoped = false;
    public bool cantCrawl = false;
    public bool goUp;
    public bool goRight;
    






    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        sc = GetComponent<SwipeControl>();
        body = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        anim.SetFloat("runSpeed", speed / 100);
        anim.SetFloat("slideDuration", 1 / slideDuration);

        animations = anim.GetNextAnimatorClipInfo(0);

        for(int i = 0; i < animations.Length; i++)
        {

            Debug.Log("Here");
            if (animations[i].clip.name == "Jump")
            {
                jumpDuration = animations[i].clip.averageDuration;
                Debug.Log(animations[i].clip.averageDuration);
            }
        }

        collider = GetComponent<CapsuleCollider2D>();
        colliderOriginalOffset = collider.offset;
        colliderOriginalSize = collider.size;

    }
    
    // Controlling states of character 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) || sc.swipeLeft)
        {
            gm.Again();
        }

        if (falling)
        {
            anim.SetBool("Falling", true);
            reachedEdge = false;
        }
        else
        {
            anim.SetBool("Falling", false);
        }

        Debug.Log(body.velocity.y);


        if (body.velocity.y < -10)
        {
            if (!grounded)
            {
                falling = true;
               
            }
        }
        else
        {
            falling = false;
        }

        if(sliding == true)
        {
            anim.SetBool("Sliding", true);
        }
        else
        {
            anim.SetBool("Sliding", false);
            ResetCollider();
            
        }

        if(crawlling == true)
        {
            anim.SetBool("Crawlling", true);
        }
        else
        {
            anim.SetBool("Crawlling", false);
        }

        if (stoped)
            gm.Again();

        if (grounded == true)
        {
         
            reachedEdge = false;
            anim.SetBool("OnGround", true);
        }

        else
            anim.SetBool("OnGround", false);


     
    }

    // Working with physics
    private void FixedUpdate()
    {
        if (!reachedEdge)
        {
            anim.SetBool("ReachEdge", false);

            if (!cantCrawl)
            {
                if (!crawlling)
                    Move();
                else
                    Crawl();
            }
            else
            {
                crawlling = false;
            }
        }
        else
        {
          
            anim.SetBool("ReachEdge", true);
            falling = false;

        }
        
        if (Input.GetKeyDown(KeyCode.Space) || sc.swipeUp)
        {
            if (grounded == true)
            {
                Jump();
            }

        }

        if (Input.GetKeyDown(KeyCode.T) || sc.swipeDown)
        {
            if (grounded)
            {
                Slide();
            }

        }


        if (reachedEdge)
        {
            body.simulated = true;
            if (goUp)
            {
                Vector3 targetVelocity = new Vector2(body.velocity.x, speed * 8f * Time.deltaTime);
                body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }
            else
            {
                Vector3 targetVelocity = new Vector2(speed * 10f * Time.deltaTime, body.velocity.y);
                body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            }


        }

    }


    IEnumerator Timer()
    {
        yield return new WaitForSeconds(crawlTime);
        cantCrawl = true;
    }


    void ResetCollider()
    {
        collider.size = colliderOriginalSize;
        collider.offset = colliderOriginalOffset;
    }

    public void StopSliding()
    {
        if (grounded)
        {
            sliding = false;
        }
    }


    void Fall()
    {
        falling = true;
    }

    void Jump()
    {
      
        body.AddForce(new Vector2(0f, jumpForce));
        anim.SetTrigger("Jump");
        grounded = false;
     
    }

    void Slide()
    {
        
            
            collider.size = slideColliderSize;
            collider.offset = slideColliderOffset;

            sliding = true;
        
    }

    void Move()
    {

        Vector3 targetVelocity = new Vector2(speed * 10f * Time.deltaTime, body.velocity.y);
        body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    void Crawl()
    {
        if (!cantCrawl)
        {
            Vector3 targetVelocity = new Vector2(body.velocity.x, speed * 10f * Time.deltaTime);
            body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log(collision.gameObject.layer);

        if (collision.gameObject.layer == 9)
        {
            grounded = true;
            falling = false;
           
           

        }
        else if(collision.gameObject.layer == 8)
        {
            if (!grounded)
            {

                crawlling = true;
                cantCrawl = false;
                falling = false;

                StartCoroutine(Timer());
            }
            else
            {
                Stop();
            }


            

        }
        else if(collision.gameObject.layer == 10)
        {
            if (!grounded)
            {
                reachedEdge = true;
                crawlling = false;

            }
        }
        else if(collision.gameObject.layer == 11)
        {
           
        }


    }

    void Stop()
    {
        body.simulated = false;
        anim.SetTrigger("Stop");
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        sliding = false;
      
    }



    
}
