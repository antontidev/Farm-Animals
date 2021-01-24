using UnityEngine;

public class AudioManager : MonoBehaviour {
    [Header("Background")]
    public AudioSource backgroundSource;

    public AudioEvent backgroundEvent;

    [Header("Interactions")]
    public AudioSource objectInteractionSource;

    public AudioEvent coinEvent;

    public AudioEvent foodEvent;

    public AudioEvent winEvent;

    public AudioEvent deathEvent;

    public static AudioManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance == this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        backgroundEvent.Play(backgroundSource);
    }

    public void PlayCoinEvent() {
        coinEvent.Play(objectInteractionSource);
    }

    public void PlayFoodEvent() {
        foodEvent.Play(objectInteractionSource);
    }

    public void PlayWinEvent() {
        winEvent.Play(objectInteractionSource);
    }

    public void PlayDeathEvent() {
        deathEvent.Play(objectInteractionSource);
    }
}
