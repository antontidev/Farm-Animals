using UnityEngine;

public class EffectManager : MonoBehaviour {
    [SerializeField]
    private EffectEvent winEvent;

    [SerializeField]
    private EffectEvent coinEvent;

    [SerializeField]
    private EffectEvent foodEvent;

    public static EffectManager Instance;

    // Start is called before the first frame update
    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance == this) {
            Destroy(gameObject);
        }

        coinEvent.PreloadEffect();
        winEvent.PreloadEffect();
    }

    public void PlayCoinEffect(Vector3 position) {
        coinEvent.Play(position);
    }

    public void PlayWinEffect(Vector3 position) {
        winEvent.Play(position);
    }

    public void PlayFoodEffect(Vector3 position) {
        foodEvent.Play(position);
    }
}
