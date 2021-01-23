using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private EffectEvent winEvent;

    [SerializeField]
    private EffectEvent coinEvent;

    public static EffectManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance == this) {
            Destroy(gameObject); 
        }

        DontDestroyOnLoad(gameObject);

        coinEvent.PreloadEffect();
        winEvent.PreloadEffect();
    }

    public void PlayCoinEffect(Vector3 position) {
        coinEvent.Play(position);
    }

    public void PlayWinEffect(Vector3 position) {
        winEvent.Play(position);
    }
}
