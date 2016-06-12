using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private enum Platform
    {
        PC, Android, iOS
    }

    Platform platform;

    void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        //platform = Platform.PC;
#endif

#if UNITY_ANDROID //&& !UNITY_EDITOR
        platform = Platform.Android;
#endif

#if UNITY_IOS
        platform = Platform.iOS;
#endif
    }

    public Text text;

    // Update is called once per frame
    void Update ()
    {
	    switch(platform)
        {
            case (Platform.PC):
                PCInput();
                break;
            case (Platform.Android):
                AndroidInput();
                break;
            case (Platform.iOS):
                iOSInput();
                break;
        }

        movementMagnitude = inputVector.magnitude;

        text.text = System.DateTime.Now.ToString();
     }

    private static Vector2 inputVector;
    public static Vector2 InputVector
    {
        get
        {
            return inputVector;
        }
    }

    private static float movementMagnitude;
    public static float MovementMagnitude
    {
        get
        {
            return movementMagnitude;
        }
    }

    private static Vector2 inputDirection;
    public static Vector2 InputDirection
    {
        get
        {
            return inputVector / movementMagnitude;
        }
    }

    public float doubleTapTime = 0.5f;
    [SerializeField]
    float secondsSinceLastTap = float.MaxValue;
    public static float secondsSinceZero = 0f;
    public static float maxQuickTapTime = 0.06f;
    public float deadZone = 0.25f;
    public static bool isDoubleTap = false;
    public static bool isQuickTap = false;
    public static bool joystickUsed = false;

    void PCInput()
    {
        Vector2 joystickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        joystickUsed = joystickInput.magnitude > deadZone;

        bool up = Input.GetKey(KeyCode.UpArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool keyPressed = up || left || down || right;

        inputVector = Vector2.zero;

        secondsSinceLastTap += Time.deltaTime;

        if (keyPressed)
        {
            if (up)
            {
                inputVector.y += 1;
            }
            if (left)
            {
                inputVector.x -= 1;
            }
            if (down)
            {
                inputVector.y -= 1;
            }
            if (right)
            {
                inputVector.x += 1;
            }

            inputVector = inputVector.normalized;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (secondsSinceLastTap <= doubleTapTime)
                {
                    isDoubleTap = true;
                }
                else
                {
                    isDoubleTap = false;
                    secondsSinceLastTap = 0f;
                }
            }

            if (!isDoubleTap)
            {
                inputVector *= 0.79f;
            }
            else
            {
                secondsSinceLastTap = 0f;
            }
        }
        else if (joystickUsed)
        {
            if (secondsSinceZero < maxQuickTapTime && MovementMagnitude > 0.95f)
            {
                inputVector = joystickInput.normalized;
                isQuickTap = true;
            }
            else
            {
                isQuickTap = false;
                secondsSinceZero += Time.deltaTime;
                inputVector = Vector2.ClampMagnitude(joystickInput, 1f);
            }
        }
        else
        {
            inputVector *= 0f;
            secondsSinceLastTap += Time.deltaTime;
            secondsSinceZero = 0f;

            isQuickTap = false;
            isDoubleTap = false;
        }
    }

    public static Vector2 touchStart, touchPosition, touchStartWorld, touchPositionWorld;

    public enum Phase { Begin, Intermediate, End, NoTouch };
    public static Phase phase;

    void AndroidInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case (TouchPhase.Began):
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            touchStart = touchPosition = touch.position;
                            touchStartWorld = touchPositionWorld = CameraManager.UICam.ScreenToWorldPoint(touchPosition);
                            phase = Phase.Begin;
                        }
                        break;
                    }
                case (TouchPhase.Moved):
                    {
                        if (!float.IsNaN(touchStart.x))
                        {
                            phase = Phase.Intermediate;
                            touchPosition = touch.position;
                            touchPositionWorld = CameraManager.UICam.ScreenToWorldPoint(touchPosition);
                            inputVector = touchPosition - touchStart;
                            inputDirection = inputVector.normalized;
                        }
                        break;
                    }
                case (TouchPhase.Ended):
                    {
                        phase = Phase.End;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            touchPositionWorld = CameraManager.UICam.ScreenToWorldPoint(touchPosition);
            touchStartWorld = CameraManager.UICam.ScreenToWorldPoint(touchStart);
        }
        else
        {
            touchStart = touchPosition = touchStartWorld = touchPositionWorld = inputVector = inputDirection = new Vector2(float.NaN, float.NaN);
            phase = Phase.NoTouch;
        }
    }

    void iOSInput()
    {

    }
}
