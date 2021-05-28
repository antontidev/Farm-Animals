using SukharevShared;
using UnityEngine;
using Zenject;

public enum InputType {
    JoystickMover,
    KeyboardMover
}

public class ModuleInstaller : MonoInstaller {
    public InputType inputType;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private GameObject controls;

    public override void InstallBindings() {
        var typeInput = EnumResolver<InputType, IPlayerMover>.GetType(inputType);

        if (inputType == InputType.KeyboardMover) {
            controls.SetActive(false);
        }

        Container.Bind<IPlayerMover>().To(typeInput.UnderlyingSystemType).AsSingle();
        Container.Bind<ICollisionManager>().FromInstance(gameManager).AsSingle();
    }
}
