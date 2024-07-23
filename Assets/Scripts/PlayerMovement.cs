using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 1500;
    [SerializeField] private Rigidbody2D rigidbody2d;
    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
            rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * (speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.QuitGame();
        }
    }
}