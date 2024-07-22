using UnityEngine;

#if !NOT_UNITY3D

namespace Zenject
{
    public class SceneKernel : MonoKernel
    {
        // Only needed to set "script execution order" in unity project settings

#if ZEN_INTERNAL_PROFILING
        public override void Start()
        {
            base.Start();
            Debug.Log($"SceneContext.Awake detailed profiling: {ProfileTimers.FormatResults()}");
        }
#endif
    }
}

#endif
