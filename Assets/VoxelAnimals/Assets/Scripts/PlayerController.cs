using SukharevShared;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using MyBox;
using UniRx;

public interface IJumper {
    void Jump(Vector3 jumpForce);

    Vector3 CalculateForce(Transform target);
}

public interface IMover {
    void Move(Vector3 movement);
}

public interface IFlyer {
    void Fly(float capacity);
}

public interface IHungry {
}

public class PlayerController : MonoBehaviour, IJumper, IMover, IFlyer, IHungry
{
    [Inject]
    private IPlayerMover playerMover;

    [Header("Camera movement")]
    [SerializeField]
    private bool cameraRelativeMovement;

    [SerializeField]
    [ConditionalField("cameraRelativeMovement")]
    private Camera cam;

    [Header("Audio events")]
    [SerializeField]
    private AudioEvent moveClip;

    [SerializeField]
    private AudioEvent jumpClip;

    [SerializeField]
    private AudioEvent flyClip;

    [SerializeField]
    private AudioSource playerSource;

    [Header("Movement")]
    public float movementSpeed = 3;

    [Header("Player components")]
    public Animator anim;
    public Rigidbody rb;
    public BoxCollider boxCollider;

    private float canJump = 0f;
    private bool flying = false;

    #region Tags
    [Header("Tags for interacting")]
    [SerializeField]
    private Tag interestTag;

    [SerializeField]
    private Tag killTag;

    [SerializeField]
    private Tag jumpTag;

    [SerializeField]
    private Tag flyTag;

    [SerializeField]
    private Tag winTag;

    [SerializeField]
    private Tag coinTag;
    #endregion

    // --> Collisions events
    [Header("Collisoin events")]
    public UnityEventGameObject coinReached;

    public UnityEventFloat playerFlying;

    public UnityEventVector3 playerWin;

    public UnityEvent dead;

    void Update()
    {
        ControllPlayer();
    }

    void ControllPlayer()
    {
        var move = playerMover.Move;

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Move(movement);

        // Obsolete for Last levels
        if (playerMover.Jump) {
            Jump(new Vector3(0f, 300f, 0f));
        }
    }

    public void Move(Vector3 movement) {
        if (movement != Vector3.zero) {
            //movement = Quaternion.AngleAxis(normalizeAngle, Vector3.up) * movement;

            if (cameraRelativeMovement) {
                movement = CameraRelativeMovement(movement);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.30f);

            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);

            if (!flying) {
                moveClip.Play(playerSource);
            }
        }

        anim.SetFloat("Walk", movement.magnitude);
    }

    private Vector3 CameraRelativeMovement(Vector3 movement) {
        var camTransform = cam.transform;

        var forward = camTransform.forward;
        var right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * movement.z + right * movement.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        var gameObj = other.gameObject;

        if (gameObj.HasTag(killTag)) {
            dead?.Invoke();
        }
        else if (gameObj.HasTag(coinTag)) {
            coinReached?.Invoke(gameObj);

            Debug.Log("Wow such coin");
        }
        else if (gameObj.HasTag(jumpTag)) {
            var jumper = gameObj.GetComponent<JumperLogic>() as IJumpPlatform;

            jumper.OnTryJump(this);
        }
        else if (gameObj.HasTag(flyTag)) {
            var flyer = gameObj.GetComponent<FlyerLogic>() as IFlyPlatform;

            flyer.OnTryFly(this);
        }
        else if (gameObj.HasTag(winTag)) {
            playerWin?.Invoke(transform.position);
        }
    }

    #region Jump
    public void Jump(Vector3 jumpForce) {
        rb.AddForce(jumpForce, ForceMode.Impulse);
         
        anim.SetTrigger("jump");

        jumpClip.Play(playerSource);
    }
    #endregion

    #region Fly
    public void Fly(float capacity) {
        anim.SetTrigger("wantFly");

        MainThreadDispatcher.StartUpdateMicroCoroutine(FlyCoroutine(capacity));
    }

    private IEnumerator FlyCoroutine(float capacity) {
        while (!playerMover.Press) {
            yield return null;
        }

        rb.useGravity = false;
        boxCollider.isTrigger = true;

        while (capacity > 0.18f && playerMover.Press) {
            anim.SetFloat("fly", capacity);

            playerFlying?.Invoke(capacity);

            capacity -= Time.deltaTime * movementSpeed * 9;

            flyClip.Play(playerSource);

            yield return null;
        }

        anim.SetTrigger("dontWantFly");
        anim.SetFloat("fly", 0);

        rb.useGravity = true;
        boxCollider.isTrigger = false;
    }

    #endregion

    /// <summary>
    /// Deprecated
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector3 CalculateForce(Transform target) {
        Vector3 direction = target.position - transform.position;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = 45 * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));

        return velocity * direction.normalized;
    }
}