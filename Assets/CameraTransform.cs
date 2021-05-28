using Cinemachine;
using UnityEngine;

public class CameraTransform : MonoBehaviour {
    public static CameraTransform Instance;

    [SerializeField]
    private CinemachineVirtualCamera cam;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance == this) {
            Destroy(gameObject);
        }
    }

    public Transform GetCameraTransform() {
        return cam.transform;
    }
}
