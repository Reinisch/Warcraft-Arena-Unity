using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game;
using UnityEngine;

//This should become BasePlayer LOL.
public class BaseCreature : MonoBehaviour
{
    Animator m_Animator;
    Rigidbody m_Rigidbody;

    private bool m_Jump;

    private bool m_Walking;

    public float speed = 1.0f;

    private Transform target;

    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Walking = m_Animator.GetBool("IsWalking");
        Debug.Log("Start, walking = " + m_Walking);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.name == InitClient.GetCurrentPlayerName())
        {
            if (Input.GetKeyUp(KeyCode.U))
            {
                Debug.Log("Pressed U!!");
                m_Walking = !m_Walking;
                SetAnimState(m_Walking ? CharAnimState.Walk : CharAnimState.Stand);
                Debug.Log("Walking: " + m_Walking);
            }

            if (m_Walking)
            {
                float step = speed * Time.deltaTime; //calculate distance to move
                var newPos = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, newPos, step);

                if (Vector3.Distance(transform.position, newPos) < 0.001f)
                {
                    //Swap the position
                    //newPos *= -1.0f;
                    Debug.Log("Arrived.");
                }
                //Send my actual position to all connected clients.
                InitClient.Instance.MoveAsync(transform.position, transform.rotation);
            }

        }
        
    }

    void SetAnimState(CharAnimState state)
    {
        m_Animator.SetInteger("CharAnimState", (int)state);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
    }
}
