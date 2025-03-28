using UnityEngine.Events;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public UnityEvent OnSpacePressed = new();
    public UnityEvent<char> OnMousePressed = new();
    public UnityEvent OnMouseRightPressed = new();
    public UnityEvent<Vector2> OnShiftPressed = new();
    public UnityEvent<Vector2> OnMove = new();
    public UnityEvent<Vector2> OnLook = new();

    //public UnityEvent OnResetPressed = new UnityEvent();

    void LateUpdate()
    {
        Vector2 look = Vector2.zero;
        if (Input.GetAxisRaw("Mouse X") != 0)
        {
            look.x = Input.GetAxisRaw("Mouse X");
        }
        if (Input.GetAxisRaw("Mouse Y") != 0)
        {
            look.y = Input.GetAxisRaw("Mouse Y");
        }

        OnLook?.Invoke(look);

        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            input += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input += Vector2.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            input += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input += Vector2.down;
        }

        OnMove?.Invoke(input);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpacePressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnShiftPressed?.Invoke(input);
        }

        if (Input.GetMouseButton(0))
        {
            OnMousePressed?.Invoke('a');
        }
        if (Input.GetMouseButton(1))
        {
            OnMousePressed?.Invoke('b');
        }
    }
}