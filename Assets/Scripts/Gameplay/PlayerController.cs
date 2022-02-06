using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _pia.Player.Movement.performed += Movement;
    }

    void Update()
    {
        _ism.SetZPressed(_isHoldingZ);
        _ism.SetXPressed(_isHoldingX);

        // print("Cur: " + _ism.CurInput.CurInteractable);
        // print("Z press: " + curZ);
        // print("On top of: " + (OnInteractable(newPos) != null));

        if (Conductor.Instance.SongIsOnBeat())
        {
            _mySR.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            _mySR.GetComponent<SpriteRenderer>().color = Color.blue;
        }
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
    /// At a given position, casts a vector up and down the z axis to find colliders in the Interactable layer.
    /// </summary>
    /// <returns>Interactable with highest z value</returns>
    public Interactable OnInteractable(Vector3 pos) {

        /*
        RaycastHit2D[] foundColliders = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        int number = Physics2D.Raycast(pos, new Vector3(0, 0, 1), filter.NoFilter(), foundColliders);
        for (int i = 0; i < foundColliders.Length; i++)
        {
            if(foundColliders[i].transform == null)
            {
                continue;
            }
            print(foundColliders[i].transform);
        }
        */

        RaycastHit2D[] found = Physics2D.RaycastAll(pos, new Vector3(0, 0, 1), LayerMask.GetMask("Interactable"));
        Interactable highestCollider = null;
        foreach (RaycastHit2D collider in found)
        {
            Interactable interact = collider.transform.GetComponent<Interactable>();
            print(interact);
            if (interact != null
                && (highestCollider == null || interact.transform.position.z > highestCollider.transform.position.z))
            {
                highestCollider = interact;
            }
        }

        return highestCollider;
        /*
        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            new Vector3(0, 0, 1),
            10.0f,
            LayerMask.GetMask("Interactable"));
        if (hit.transform != null)
        {
            return hit.transform.GetComponent<Interactable>();
        }
        else
        {
            return null;
        }
        */
    }

    public Interactable OnInteractable()
    {
        return OnInteractable(transform.position);
    }

    public Destructable OnDestructable(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            new Vector3(0, 0, 1),
            10.0f,
            LayerMask.GetMask("Interactable"));
        if (hit.transform != null)
        {
            return hit.transform.gameObject.GetComponent<Destructable>();
        }
        else
        {
            return null;
        }
    }

    public Destructable OnDestructable()
    {
        return OnDestructable(transform.position);
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
            newPos.x = _myRb.position.x
            + (inputVector.x > 0 ? 1 : (inputVector.x < 0 ? -1 : 0));
            newPos.y = _myRb.position.y
            + (inputVector.y > 0 ? 1 : (inputVector.y < 0 ? -1 : 0));

            _mySR.Move(newPos);
            _myRb.MovePosition(newPos);
            _ism.Move(newPos);
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

    #endregion

    int checkInputH()
    {
        bool leftPress = Input.GetKeyDown("left");
        bool rightPress = Input.GetKeyDown("right");
        return leftPress ? -1 : (rightPress ? 1 : 0);
    }

    int checkInputV()
    {
        bool upPress = Input.GetKeyDown("up");
        bool downPress = Input.GetKeyDown("down");
        return upPress ? 1 : (downPress ? -1 : 0);
    }
}
