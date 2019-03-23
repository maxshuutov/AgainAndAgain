using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwipeControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


     [HideInInspector]public bool swipeUp, tap, isDraging, swipeDown, swipeLeft, swipeRigth;
     Vector2 swipeStart, swipeDelta;

    [SerializeField]float swipeMagnitude = 100;



    public void ResetSwipe()
    {
        swipeDelta = swipeStart = Vector2.zero;
        isDraging = false;
    }

    void Update()
    {
        swipeDown = swipeUp = tap = swipeLeft = swipeRigth = false;

        // Mobile controls
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                isDraging = true;
                tap = true;
                swipeStart = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                ResetSwipe();
            }
        }

        // Calculate distanse
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touchCount > 0)
            {
                swipeDelta = Input.touches[0].position - swipeStart;
            }
        }

        // did we croos dead zone

        if (swipeDelta.magnitude > swipeMagnitude)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Math.Abs(x) > Math.Abs(y))
            {
                if (x > 0)
                    swipeRigth = true;
                else
                    swipeLeft = true;
                // Left or Right
            }
            else
            {
                if (y > 0)
                    swipeUp = true;
                else
                    swipeDown = true;
            }



        }

    }
}
