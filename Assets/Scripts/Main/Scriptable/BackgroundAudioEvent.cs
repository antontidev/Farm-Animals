using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Scriptable {

    [CreateAssetMenu(menuName = "AudioEvent/Background")]
    public class BackgroundAudioEvent : AudioEvent {
        private AudioClip sourceClip;

        private void PickSourceCLip() {
            var index = Random.Range(0, clips.Length);

            sourceClip = clips[index];
        }

        public override void Play(AudioSource source) {
            PickSourceCLip();

            source.clip = sourceClip;
            source.volume = Random.Range(volume.min, volume.max);
            source.pitch = Random.Range(pitch.min, pitch.max);
            source.loop = true;
            source.Play();
        }
    }
}