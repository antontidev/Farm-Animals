using UnityEngine;

public abstract class EffectEvent : ScriptableObject {
    public GameObject effectParticles;

    public GameObject LoadedEffect {
        get; protected set;
    }

    public abstract void Play(Vector3 positoin);

    public void PreloadEffect() {
        LoadedEffect = Instantiate(effectParticles);

        LoadedEffect.SetActive(false);
    }
}
