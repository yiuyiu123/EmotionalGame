using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public event Action OnMoveLeft;
    public event Action OnMoveRight;
    public event Action OnJumpOrClimb;
    public event Action OnTryTakePhoto;
    public event Action OnTryMakePhoneCall;

    private void Update()
    {
        DetectMovementInput();
        DetectJumpInput();
        DetectMouseInput();
    }

    private void DetectMovementInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            OnMoveLeft?.Invoke();
        }

        if (Input.GetKey(KeyCode.D))
        {
            OnMoveRight?.Invoke();
        }
    }

    private void DetectJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnJumpOrClimb?.Invoke();
        }
    }

    private void DetectMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTryTakePhoto?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnTryMakePhoneCall?.Invoke();
        }
    }
}
