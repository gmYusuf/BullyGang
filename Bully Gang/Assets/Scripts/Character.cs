using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Character : MonoBehaviour
{
    public GameObject player;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveSpeed;
    private float dirX;
    private bool facingRight = true;
    private Vector3 localScale;
    private double nextUpdate = 2;
    private string serverCharacterName = "Deer L";
    private string clientCharacterName = "Deer R";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
       // player = GetComponent<GameObject>();

        localScale = transform.localScale;
        moveSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {

        // Call your fonction
        if (!GameManager.Instance.PlayerSide)
        {
             //Debug.Log("[HMS] NearbyManager server");
            if(String.Compare(player.name, serverCharacterName) == 0)
            {
                // all same
                updateCharacter();

            }
            else if (String.Compare(player.name, clientCharacterName) == 0)
            {
                string clientPosition = NearbyServer.Instance.ClientPosition;
                // change client character
                if (String.Compare(clientPosition, "no") == 0)
                {
                    
                }else
                {
                    dirX = float.Parse(clientPosition) * moveSpeed;
                }
            }
                 

        }
        else
        { 

            if (String.Compare(player.name, serverCharacterName) == 0)
            {
                string serverPosition = NearbyClient.Instance.ServerPosition;

                // change server character
                if (String.Compare(serverPosition, "no") == 0)
                {

                }
                else
                {
                    dirX = float.Parse(serverPosition) * moveSpeed;
                }
            }
            else if (String.Compare(player.name, clientCharacterName) == 0)
            {
                // all same
                updateCharacter();

            }
        }




    }

    private void updateCharacter()
    {
        dirX = CrossPlatformInputManager.GetAxis("Horizontal") * moveSpeed;

        if (CrossPlatformInputManager.GetButtonDown("Jump") && rb.velocity.y == 0)
            rb.AddForce(Vector2.up * 700f);

        if (Mathf.Abs(dirX) > 0 && rb.velocity.y == 0)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);

        if (rb.velocity.y == 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        if (rb.velocity.y > 0)
        {
            anim.SetBool("isJumping", true);
        }
        if (rb.velocity.y < 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(dirX, rb.velocity.y);
    }
    private void LateUpdate()
    {
        if (dirX > 0)
            facingRight = true;
        else if (dirX < 0)
            facingRight = false;

        if (((facingRight) && (localScale.x < 0)) || (!facingRight) && (localScale.x > 0))
            localScale.x *= -1;

        transform.localScale = localScale;
    }
}
