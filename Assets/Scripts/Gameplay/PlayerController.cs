using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls all the player's logic.
/// </summary>
public class PlayerController : MonoBehaviour
{

    private Rigidbody2D _myRb;
    private BoxCollider2D _myCollider;
    private SmoothSprite _mySR;

    private Room _curRoom;

    private bool _isHoldingZ;
    private bool _isHoldingX;
    private InteractableStateMachine _ism;
    private PlayerInputActions _pia;

    public bool RhythmLocked = true;

    public bool CanPlaceHeadMine;
    public bool CanPlaceStemMine;


    void Start()
    {
        _myRb = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _mySR = GetComponentInChildren<SmoothSprite>();
        _ism = GetComponent<InteractableStateMachine>();
        _pia = new PlayerInputActions();

        _pia.Player.Enable();
        _pia.Player.Interact.performed += Interact;
        _pia.Player.Interact.canceled += Interact;
        _pia.Player.Delete.performed += Delete;
        _pia.Player.Delete.canceled += Delete;
        _pia.Player.Movement.started += Movement;
    }

    void Update()
    {
        _ism.SetZPressed(_isHoldingZ);
        _ism.SetXPressed(_isHoldingX);

        this.CheckTileOn();

        if (Conductor.Instance.SongIsOnBeat())
        {
            _mySR.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            _mySR.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            RestartGame();
        }
    }

    public void RestartGame(){
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        StopCoroutine("SmoothSprite._moveCoroutine");
    }

    private void FixedUpdate()
    {
        Collider2D roomCollider = CheckRoomOverlap();
        if (roomCollider)
        {
            if (!_curRoom)
            {
                _curRoom = roomCollider.GetComponent<Room>();
                _curRoom.OnPlayerEnter(this);
            }
        }
        else
        {
            if (_curRoom)
            {
                _curRoom.OnPlayerExit(this);
            }

            _curRoom = null;
        }
    }

    /// <summary>
    /// Checks what rooms the player is in, if any
    /// </summary>
    /// <returns>The collider of the room the player is in</returns>
    private Collider2D CheckRoomOverlap()
    {
        Vector3 alpha = new Vector3(0.05f, 0.05f); //So the player touching the edge of the collider isn't counted as an overlap
        Vector2 topLeftCorner = _myCollider.bounds.min + alpha;
        Vector2 topRightCorner = _myCollider.bounds.max - alpha;
        Collider2D overlapCollider = Physics2D.OverlapArea(
            topLeftCorner,
            topRightCorner,
            LayerMask.GetMask("Room")
        );
        return overlapCollider;
    }

    /// <summary>
    /// At a given position, casts a vector up and down the z axis to find Components of type <typeparamref name="T"/>
    /// </summary>
    /// <param name="pos">Position</param>
    /// <typeparam name="T">Component</typeparam>
    /// <returns>Component to find</returns>
    public T OnComponent<T>(Vector3 pos) where T : MonoBehaviour {

        RaycastHit2D[] found = Physics2D.RaycastAll(
        pos,
        new Vector3(0, 0, 1),
        LayerMask.GetMask("Interactable")
        );
        T highestComponent = default(T);
        foreach (RaycastHit2D curCol in found)
        {
            T interact = curCol.transform.GetComponent<T>();
            if (interact != null) {
                // print(interact + " " + interact.transform.position.z);
                if(highestComponent == null || interact.transform.position.z < highestComponent.transform.position.z) {
                    highestComponent = interact;
                }
            }
        }

        return highestComponent;
    }

    public Interactable OnInteractable()
    {
        return OnComponent<Interactable>(transform.position);
    }

    public Destructable OnDestructable()
    {
        return OnComponent<Destructable>(transform.position);
    }

    #region Actions
    private void Movement(InputAction.CallbackContext context)
    {
        Vector2 inputVector = _pia.Player.Movement.ReadValue<Vector2>();
        Vector3 newPos = Vector3.zero;
        // bool moved = false;
        // bool attemptMove = inputVector != Vector2.zero;

        bool onBeat;

        if (!Conductor.Instance.RhythmLock)
        {
            onBeat = true;
            // Conductor.Instance.MachineTick();
        }
        else
        {
            if (this.RhythmLocked)
            {
                onBeat = Conductor.Instance.AttemptMove();
            }
            else
            {
                onBeat = true;
            }
        }

        if (onBeat)
        {
          if (this != null) {
              newPos.x = _myRb.position.x
              + (inputVector.x > 0 ? 1 : (inputVector.x < 0 ? -1 : 0));
              newPos.y = _myRb.position.y
              + (inputVector.y > 0 ? 1 : (inputVector.y < 0 ? -1 : 0));

              _mySR.Move(newPos);
              _myRb.MovePosition(newPos);
              _ism.Move(newPos);
          }
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        _isHoldingZ = context.performed;
    }

    private void Delete(InputAction.CallbackContext context)
    {
        _isHoldingX = context.performed;
    }

    public void CheckTileOn()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            new Vector3(0, 0, 1),
            10.0f,
            LayerMask.GetMask("Default"));
        if (hit.transform != null)
        {
            if (hit.transform.gameObject.CompareTag("StemTiles"))
            {
                CanPlaceStemMine = true;
                CanPlaceHeadMine = false;
            }
            else if (hit.transform.gameObject.CompareTag("HeadTiles"))
            {
                CanPlaceStemMine = false;
                CanPlaceHeadMine = true;
            }
            else
            {
                CanPlaceStemMine = false;
                CanPlaceHeadMine = false;
            }
        }
        else
        {
            CanPlaceStemMine = false;
            CanPlaceHeadMine = false;
        }

    }
    #endregion
}
