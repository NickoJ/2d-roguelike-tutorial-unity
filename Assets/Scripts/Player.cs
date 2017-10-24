using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{

	public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.Instance.playerFoodPoints;

        base.Start();
    }

    public void LossFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(Vector2Int dir)
    {
        food -= 1;
        base.AttemptMove<T>(dir);

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
        }
        else if (other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0) GameManager.Instance.GameOver();
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