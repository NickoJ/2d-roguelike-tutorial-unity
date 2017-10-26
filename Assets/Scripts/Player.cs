using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

	public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip[] moveSounds;
    public AudioClip[] eatSounds;
    public AudioClip[] drinkSounds;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.Instance.playerFoodPoints;
        foodText.text = $"Food: {food}";

        base.Start();
    }

    public void LossFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = $"-{loss} Food: {food}";
        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(Vector2Int dir)
    {
        food -= 1;
        foodText.text = $"Food: {food}";
        base.AttemptMove<T>(dir);

        RaycastHit2D hit;
        if (Move(dir, out hit)) SoundManager.Instance.RandomizeSfx(moveSounds);

        CheckIfGameOver();
        GameManager.Instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        var hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Update()
    {
        if (!GameManager.Instance.playersTurn) return;

        Vector2Int direction = new Vector2Int(
            (int)Input.GetAxisRaw("Horizontal"),
            (int)Input.GetAxisRaw("Vertical")
        );

        if (direction.x != 0) direction.y = 0;

        if (direction != Vector2Int.zero) AttemptMove<Wall>(direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            StartCoroutine(RestartDelayed(restartLevelDelay));
            enabled = false;
        }
        else if (other.CompareTag("Food"))
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
            foodText.text = $"+{pointsPerFood} Food: {food}";
            SoundManager.Instance.RandomizeSfx(eatSounds);
        }
        else if (other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
            foodText.text = $"+{pointsPerSoda} Food: {food}";
            SoundManager.Instance.RandomizeSfx(drinkSounds);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0) 
        {
            GameManager.Instance.GameOver();
            SoundManager.Instance.PlaySingle(gameOverSound);
            SoundManager.Instance.musicSource.Stop();
            enabled = false;
        }
    }

    private IEnumerator RestartDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Restart();
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}