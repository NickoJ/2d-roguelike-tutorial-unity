using UnityEngine;

public class SoundManager : MonoBehaviour
{

	public static SoundManager Instance { get; private set; }

	public AudioSource efxSource;
	public AudioSource musicSource;

	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;

	private float defaultPitchRange;

	void Awake()
	{
		if (Instance == null) Instance = this;
		if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		defaultPitchRange = efxSource.pitch;
	}

	public void PlaySingle(AudioClip clip)
	{
		efxSource.pitch = defaultPitchRange;
		efxSource.clip = clip;
		efxSource.Play();
	}

	public void RandomizeSfx(params AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips[randomIndex];
		efxSource.Play();
	}

}