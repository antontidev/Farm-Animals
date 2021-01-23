using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISimpleGameManager {
    void OnPlayerDeath();

    void OnCoinGrab(GameObject coin);

    void OnWin(Vector3 winPosition);

    void OnPlayerFlying(float capacity);
}

public class GameManager : MonoBehaviour, ISimpleGameManager {
    // --> Debug scene
    [SerializeField]
    private FadeManager fadeManager;

    [SerializeField]
    private ProgressBar staminaFlyBar;


    private void Awake() {
        SceneManager.LoadScene("Shared", LoadSceneMode.Additive);

        fadeManager.Unfade(null);
    }

    private void LoadScene() {
        var index = SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    private void ReloadScene() {
        var index = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    /// <summary>
    /// Invokes when player dies
    /// </summary>
    public void OnPlayerDeath() {
        ReloadScene();
    }

    /// <summary>
    /// Invokes when player reach the coin 
    /// </summary>
    /// <param name="coin">Coin GameObject</param>
    public void OnCoinGrab(GameObject coin) {
        var position = coin.transform.position;

        EffectManager.Instance.PlayCoinEffect(position);

        coin.SetActive(false);
    }

    public void OnWin(Vector3 winPosition) {
        EffectManager.Instance.PlayWinEffect(winPosition);
    }

    public void OnPlayerFlying(float capacity) {
        staminaFlyBar.ChangeProgress(capacity);
    }
}
