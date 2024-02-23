using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemtAI : MonoBehaviour
{
    public Transform playerTransform; // Assign your player's transform here in the inspector
    public float detectionRange = 5f; // The range within which the snake detects the player
    public float moveSpeed = 2f; // How fast the snake moves towards the player
    private bool playerDetected = false;
    public float smallSnakeDamageAmount;




    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
       // grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //if (!grounded) ApplyGravity();
        // Check distance between snake and player
        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            // Rotate towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else
        {
            playerDetected = false;
        }

        if (playerDetected)
        {
            // Move towards the player
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    // Optionally, use OnTriggerEnter or OnTriggerStay to detect when the player is close
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            playerDetected = true;
           // Debug.Log("Attack");
            collision.gameObject.TryGetComponent<PlayerMovementCombat>(out PlayerMovementCombat playerMovementCombat);

            if (playerMovementCombat != null)
                if (playerMovementCombat.CanTakeDamage())
                {
                    collision.gameObject.TryGetComponent<HealthManager>(out HealthManager manager);
                    manager.TakeDamage(smallSnakeDamageAmount);
                }
        }

    }
    

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
        }
    }
}
