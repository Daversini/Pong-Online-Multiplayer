using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text waitingText;
    
    private NetworkManagerPong networkManager;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManagerPong>();
        waitingText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts hosting a game.
    /// </summary>
    public void HostGame()
    {
        if (networkManager == null) return;
        networkManager.StartHost();
        Debug.Log("Host Game");
        
        // Hide the buttons 
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        
        // Show the waiting text
        waitingText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Joins an existing game.
    /// </summary>
    public void JoinGame()
    {
        if (networkManager == null) return;
        networkManager.StartClient();
        Debug.Log("Join Game");
    }
}