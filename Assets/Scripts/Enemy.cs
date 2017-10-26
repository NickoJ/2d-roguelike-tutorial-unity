using UnityEngine;

public class Enemy : MovingObject
{

	public int playerDamage;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	public AudioClip[] attackClips;

	protected override void Start()
	{
		GameManager.Instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
	}

	public void MoveEnemy()
	{
		Vector2Int dir = Vector2Int.zero;
		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
			dir.y = target.position.y > transform.position.y ? 1 : -1;
		else
			dir.x = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player>(dir);
	}

	protected override void AttemptMove<T>(Vector2Int dir)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}

		base.AttemptMove<T>(dir);
		skipMove = true;
	}

	protected override void OnCantMove<T>(T component)
	{
		Player hitPlayer = component as Player;
		animator.SetTrigger("enemyAttack");
		SoundManager.Instance.RandomizeSfx(attackClips);
		hitPlayer.LossFood(playerDamage);
	}

}