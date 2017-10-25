using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{	

	public static GameManager Instance { get; private set; }

	public float levelStartDelay = 2f;
	public float turnDelay = 0.1f;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private BoardManager boardScript;
	private bool doingSetup;

	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;

	void Awake()
	{
		if (Instance == null) Instance = this;
		
		if (Instance != this) 
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
	}

	public void GameOver()
	{
		levelText.text = $"After {level} days, you starved.";
		levelImage.SetActive(true);
		enabled = false;
	}

	public void AddEnemyToList(Enemy enemy)
	{
		enemies.Add(enemy);
	}

	void OnEnable()
	{	
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void Update()
	{
		if (playersTurn || enemiesMoving || doingSetup) return;

		StartCoroutine(MoveEnemies());
	}

	void InitGame()
	{
		doingSetup = true;

		// Never do this in real projects
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = $"Day: {level}";
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();
		boardScript.SetupScene(level);
	}

	void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		level += 1;
		InitGame();
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSecondsRealtime(turnDelay);
		if (enemies.Count == 0)
		{
			yield return new WaitForSecondsRealtime(turnDelay);
		}
		
		for (int i = 0; i < enemies.Count; ++i)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSecondsRealtime(enemies[i].moveTime);
		}
		playersTurn = true;
		enemiesMoving = false;
	}

}