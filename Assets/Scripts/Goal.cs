using UnityEngine;

public class Goal : MonoBehaviour
{
    public enum GoalSide
    {
        Left,
        Right
    }
    
    [SerializeField] public GoalSide side;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        if (ball == null) return;
        switch (side)
        {
            case GoalSide.Left:
                Debug.Log("Goal by Right Player");
                GameManager.Instance.UpdateScore(GameManager.Player.RightPlayer);
                break;
            case GoalSide.Right:
                Debug.Log("Goal by Left Player");
                GameManager.Instance.UpdateScore(GameManager.Player.LeftPlayer);
                break;
        }
    }
}