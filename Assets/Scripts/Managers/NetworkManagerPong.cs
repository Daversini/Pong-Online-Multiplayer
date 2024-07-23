using UnityEngine;
using Mirror;

[AddComponentMenu("")]
public class NetworkManagerPong : NetworkManager
{
	[SerializeField] public Transform leftRacketSpawn;
	[SerializeField] public Transform rightRacketSpawn;
	private GameObject ball;
	private bool gameSceneLoaded = false;
	
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	/// <summary>
	/// Called on the server when the scene changes.
	/// </summary>
	/// <param name="sceneName">The name of the new scene.</param>
	public override void OnServerSceneChanged(string sceneName)
	{
		base.OnServerSceneChanged(sceneName);

		Debug.Log($"Scene changed to: {sceneName}");

		// Find the spawner references only if we are in the game scene
		if (sceneName == "Game")
		{
			leftRacketSpawn = GameObject.Find("RacketSpawnLeft")?.transform;
			rightRacketSpawn = GameObject.Find("RacketSpawnRight")?.transform;
		}
	}
	
	/// <summary>
	/// Called on the server when a new player is added.
	/// This method handles adding the player at the correct spawn position.
	/// If two players are connected, it starts the game.
	/// </summary>
	/// <param name="conn">The connection to the client.</param>
	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		Debug.Log("Added a new player...");
		
		// add player at correct spawn position
		Transform start = numPlayers == 0 ? leftRacketSpawn : rightRacketSpawn;
		GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
		NetworkServer.AddPlayerForConnection(conn, player);

		if (numPlayers == 2)
			StartGame();
	}
	
	/// <summary>
	/// Starts the game by spawning the ball and changing to the game scene if needed.
	/// </summary>
	public void StartGame()
	{
		ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
		NetworkServer.Spawn(ball);

		if (gameSceneLoaded) return;
		gameSceneLoaded = true;
		ServerChangeScene("Game");
	}

	/// <summary>
	/// Called on the server when a player disconnects.
	/// </summary>
	/// <param name="conn">The connection to the client.</param>
	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		Debug.Log("Player disconnected.");

		// Check if the host disconnected
		if (conn.connectionId == 0)
		{
			Debug.Log("Host disconnected. Returning to main menu...");
			ServerChangeScene("Menu");
		}

		// Destroy ball
		if (ball != null)
		{
			NetworkServer.Destroy(ball);
			ball = null;
		}

		// Call base functionality (actually destroys the player)
		base.OnServerDisconnect(conn);

		// Reset game if a player disconnects
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetGame();
		}
	}
}