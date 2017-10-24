using UnityEngine;

public class GameManager : MonoBehaviour
{	

	public static GameManager Instance { get; private set; }

	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private BoardManager boardScript;

	private int level = 3;

	void Awake()
	{
		if (Instance == null) Instance = this;
		
		if (Instance != this) 
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}

	public void GameOver()
	{
		enabled = false;
	}

	void InitGame()
	{
		boardScript.SetupScene(level);
	}

}