using UnityEngine;
using Mirror;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    
    private void Update()
    {
        if (isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            Vector3 playerMovement = new Vector2(h, v) * (moveSpeed * Time.deltaTime);
            
            transform.position = transform.position + playerMovement;
        }
    }
}
