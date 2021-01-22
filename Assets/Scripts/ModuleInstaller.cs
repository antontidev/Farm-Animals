using System.Collections;
using UnityEngine;
using Zenject;
using Cinemachine;
using System.Reflection;
using SukharevShared;

public enum InputType {
    JoystickMover,
    KeyboardMover
}

public class ModuleInstaller : MonoInstaller
{
    public InputType inputType;

    public override void InstallBindings()
    {
        var typeInput = EnumResolver<InputType, IPlayerMover>.GetType(inputType);

        Container.Bind<IPlayerMover>().To(typeInput.UnderlyingSystemType).AsSingle();
    }
}
