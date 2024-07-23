using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float initialSpeed = 30f;
    [SerializeField] private float speedIncrement = 2.5f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private float speed = 0;
    
    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        rigidbody2d.simulated = true;
        
        SpawnBall(Vector2.right);
    }
    
    [ClientRpc]
    public void RpcHideBall()
    {
        rigidbody2d.simulated = false;
        spriteRenderer.enabled = false;
    }

    [ClientRpc]
    public void RpcShowBall()
    {
        rigidbody2d.simulated = true;
        spriteRenderer.enabled = true;
    }
    
    /// <summary>
    /// Spawns the ball at the center of the map with an initial velocity.
    /// </summary>
    [Server]
    public void SpawnBall(Vector2 direction)
    {
        speed = initialSpeed;
        transform.position = Vector2.zero;
        rigidbody2d.velocity = Vector2.zero;

        rigidbody2d.velocity = direction * speed;
    }
    
    /// <summary>
    /// Calculates the hit factor to determine the direction of the ball after collision.
    /// </summary>
    /// <param name="ballPos">Position of the ball.</param>
    /// <param name="racketPos">Position of the racket.</param>
    /// <param name="racketHeight">Height of the racket.</param>
    /// <returns>Hit factor as a float.</returns>
    private float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
    {
        return (ballPos.y - racketPos.y) / racketHeight;
    }
    
    /// <summary>
    /// Checks if the collision was with a racket.
    /// </summary>
    /// <param name="col">Collision data.</param>
    /// <returns>True if collision is with a racket, otherwise false.</returns>
    private bool IsRacketCollision(Collision2D col)
    {
        return col.transform.GetComponent<PlayerMovement>() != null;
    }
    
    /// <summary>
    /// Handles collision with the racket to determine the new velocity of the ball.
    /// </summary>
    /// <param name="col">Collision data.</param>
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (IsRacketCollision(col))
        {
            // Increase the speed
            speed += speedIncrement;
            speed = Mathf.Min(speed, maxSpeed);
            
            // Update the velocity and direction
            float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
            float x = col.relativeVelocity.x > 0 ? 1 : -1;
            Vector2 dir = new Vector2(x, y).normalized;
            rigidbody2d.velocity = dir * speed;
        }
    }
}