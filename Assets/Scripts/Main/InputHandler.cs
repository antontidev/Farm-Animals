using UnityEngine;
using Zenject;

/// <summary>
/// So smart
/// </summary>
public interface IPlayerMover {
    Vector2 Move {
        get; set;
    }

    bool Jump {
        get; set;
    }

    bool Attack {
        get; set;
    }

    bool Press {
        get; set;
    }
}

public class KeyboardMover : IPlayerMover {
    public Vector2 Move {
        get {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

            return new Vector2(horizontal, vertical);
        }
        set {
        }
    }
    public bool Jump {
        get {
            return Input.GetAxisRaw("Jump") > 0;
        }
        set {
        }
    }
    public bool Attack {
        get {
            return Input.GetMouseButtonDown(0);
        }
        set {
        }
    }

    public bool Press {
        get {
            return Input.GetKey(KeyCode.W);
        }
        set {
        }
    }
}

/// <summary>
/// Don't think that it's really good
/// </summary>
/// <remarks>
/// It's value sets in InputHandler class
/// </remarks>
public class JoystickMover : IPlayerMover {
    public Vector2 Move {
        get; set;
    }

    public bool Jump {
        get; set;
    }

    public bool Attack {
        get; set;
    }
    public bool Press {
        get; set;
    }
}

public class InputHandler : MonoBehaviour {
    [Inject]
    private IPlayerMover playerMover;

    public void JoystickInput_Changed(Vector2 value) {
        playerMover.Move = value;
    }

    public void JoystickPressState_Change(bool value) {
        playerMover.Press = value;
    }

    public void JoystickButtonA_Clicked() {
        playerMover.Attack = true;
    }

    public void JoystickButtonA_Up() {
        playerMover.Attack = false;
    }

    public void JoystickButtonB_Clicked() {
        playerMover.Jump = true;
    }

    public void JoystickButtonB_Up() {
        playerMover.Jump = false;
    }
}
