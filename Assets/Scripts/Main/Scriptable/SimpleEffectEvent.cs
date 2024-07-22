using UnityEngine;

[CreateAssetMenu(menuName = "EffectEvent/Simple")]
public class SimpleEffectEvent : EffectEvent {
    public override void Play(Vector3 position) {
        // If effect wasn't preloaded
        if (LoadedEffect == null) {
            LoadedEffect = Instantiate(effectParticles);
        }

        LoadedEffect.SetActive(false);

        LoadedEffect.transform.position = position;

        LoadedEffect.SetActive(true);
    }
}
