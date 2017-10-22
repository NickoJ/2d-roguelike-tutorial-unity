using UnityEngine;

public class GameManager : MonoBehaviour
{	

	public static GameManager Instance { get; private set; }

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

	void InitGame()
	{
		boardScript.SetupScene(level);
	}

}