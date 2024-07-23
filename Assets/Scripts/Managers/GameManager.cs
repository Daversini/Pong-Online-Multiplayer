using UnityEngine;
using System.Collections;
using Mirror;
using TMPro;

public class GameManager : NetworkSingleton<GameManager>
{
    [SerializeField] TMP_Text leftPlayerScoreText;
    [SerializeField] TMP_Text rightPlayerScoreText;
    [SerializeField] float waitSecondsBeforeRespawn = 1f;

    [SyncVar(hook = nameof(OnLeftPlayerScoreChanged))]
    private int leftPlayerScore = 0;

    [SyncVar(hook = nameof(OnRightPlayerScoreChanged))]
    private int rightPlayerScore = 0;
    
    public enum Player
    {
        LeftPlayer,
        RightPlayer
    }
    
    /// <summary>
    /// Updates the score for the specified player.
    /// </summary>
    /// <param name="player">Player identifier (LeftPlayer or RightPlayer).</param>
    [Server]
    public void UpdateScore(Player player)
    {
        switch (player)
        {
            case Player.LeftPlayer:
                leftPlayerScore++;
                StartCoroutine(WaitAndRespawnBall(Vector2.right));
                break;
            case Player.RightPlayer:
                rightPlayerScore++;
                StartCoroutine(WaitAndRespawnBall(Vector2.left));
                break;
        }
    }
    
    /// <summary>
    /// SyncVar hook that updates the left player score text when the score changes.
    /// </summary>
    private void OnLeftPlayerScoreChanged(int oldScore, int newScore)
    {
        leftPlayerScoreText.text = newScore.ToString();
    }

    /// <summary>
    /// SyncVar hook that updates the right player score text when the score changes.
    /// </summary>
    private void OnRightPlayerScoreChanged(int oldScore, int newScore)
    {
        rightPlayerScoreText.text = newScore.ToString();
    }
    
    /// <summary>
    /// Waits for a specified time and then respawns the ball in the given direction.
    /// </summary>
    /// <param name="direction">The direction to spawn the ball.</param>
    [Server]
    private IEnumerator WaitAndRespawnBall(Vector2 direction)
    {
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.RpcHideBall();
        }
        
        yield return new WaitForSeconds(waitSecondsBeforeRespawn);

        if (ball != null)
        {
            ball.SpawnBall(direction);
            ball.RpcShowBall();
        }
    }
    
    /// <summary>
    /// Resets the game, including player scores and ball position.
    /// </summary>
    [Server]
    public void ResetGame()
    {
        leftPlayerScore = 0;
        rightPlayerScore = 0;
        RpcResetScores();

        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.RpcHideBall();
            ball.SpawnBall(Vector2.right);
            ball.RpcShowBall();
        }
    }

    /// <summary>
    /// ClientRpc that resets the score texts for both players.
    /// </summary>
    [ClientRpc]
    private void RpcResetScores()
    {
        leftPlayerScoreText.text = "0";
        rightPlayerScoreText.text = "0";
    }

    /// <summary>
    /// Quits the game and returns to the main menu.
    /// This method handles the disconnection for both server and client.
    /// </summary>
    public void QuitGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // If the server is active and the client is connected, stop the host
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkServer.active)
        {
            // If only the server is active, stop the server
            NetworkManager.singleton.StopServer();
        }
        else if (NetworkClient.isConnected)
        {
            // If only the client is active, stop the client
            NetworkManager.singleton.StopClient();
        }

        // Load the main menu scene for all clients
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.ServerChangeScene("MainMenu");
        }
    }
}