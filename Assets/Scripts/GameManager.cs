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

    [SerializeField]
    private ProgressBar hungerBar;

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
        AudioManager.Instance.PlayDeathEvent();
        // Temporary
        ReloadScene();
    }

    /// <summary>
    /// Invokes when player reach the coin 
    /// </summary>
    /// <param name="coin">Coin GameObject</param>
    public void OnCoinGrab(GameObject coin) {
        var position = coin.transform.position;

        EffectManager.Instance.PlayCoinEffect(position);
        AudioManager.Instance.PlayCoinEvent();

        coin.SetActive(false);
    }

    public void OnPlayerHungerChanged(float hunger) {
        hungerBar.ChangeProgress(hunger);
    }

    public void OnFoodReached(GameObject food) {
        var position = food.transform.position;

        EffectManager.Instance.PlayFoodEffect(position);
        AudioManager.Instance.PlayFoodEvent();

        food.SetActive(false);
    }

    public void OnWin(Vector3 winPosition) {
        EffectManager.Instance.PlayWinEffect(winPosition);
        AudioManager.Instance.PlayWinEvent();
    }

    public void OnPlayerFlying(float capacity) {
        staminaFlyBar.ChangeProgress(capacity);
    }
}
