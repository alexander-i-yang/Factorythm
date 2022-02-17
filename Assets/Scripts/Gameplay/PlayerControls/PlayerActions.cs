using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    #region Sprite Structure
    private Rigidbody2D _myRb;
    private BoxCollider2D _myCollider;
    private SmoothSprite _mySR;

    #endregion

    #region Input Structure
    private InteractableStateMachine _ism;
    private PlayerInputActions _pia;
    #endregion

    private void Start()
    {
        _myRb = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _mySR = GetComponentInChildren<SmoothSprite>();

        _ism = GetComponent<InteractableStateMachine>();
        _pia = new PlayerInputActions();

        _pia.Player.Enable();
        _pia.Player.Interact.performed += Interact;
        _pia.Player.Movement.performed += Movement;
    }

    private void FixedUpdate()
    {

    }

    public void Movement(InputAction.CallbackContext context)
    {
        /// Movement ///
        if (context.performed)
        {
            Vector2 inputVector = _pia.Player.Movement.ReadValue<Vector2>();
            bool onBeat = true;

            if (onBeat)
            {
                Vector3 newPos = Vector3.zero;
                
                int offsetX = (inputVector.x > 0 ? 1 : 0) + (inputVector.x < 0 ? -1 : 0);
                int offsetY = (inputVector.y > 0 ? 1 : 0) + (inputVector.y < 0 ? -1 : 0);
                Vector2 offset = new Vector2(offsetX, offsetY);

                newPos = _myRb.position + offset;
                
                //TODO: if you end up using this method, you're going to need to implement this function
                Debug.LogError("if you end up using this method, you're going to" +
                               "need to implement CanMove in this class or have a reference to PlayerController." +
                               "Otherwise, the player won't be able to tell whether they can move to a spot or not.");
                // bool canMove = CanMoveTo(offset);
                
                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
                _ism.Move(newPos);
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Do something!");
            _ism.SetZPressed(true);
        }
        else if (context.canceled)
        {
            Debug.Log("Stop Doing it!");
            _ism.SetZPressed(false);
        }

    }
}
