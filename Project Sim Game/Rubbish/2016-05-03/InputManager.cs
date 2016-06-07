using UnityEngine;
using System.Collections;

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
        platform = Platform.PC;
#endif

#if UNITY_ANDROID
        platform = Platform.Android;
#endif

#if UNITY_IOS
        platform = Platform.iOS;
#endif
    }

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

        movementMagnitude = movementVector.magnitude;
    }

    private static Vector2 movementVector;
    public static Vector2 MovementVector
    {
        get
        {
            return movementVector;
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

    private static Vector2 movementDirection;
    public static Vector2 MovementDirection
    {
        get
        {
            return movementVector / movementMagnitude;
        }
    }

    public float doubleTapTime = 0.5f;
    [SerializeField]
    float secondsSinceLastTap = float.MaxValue;
    void PCInput()
    {
        if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (movementVector.magnitude < 0.25f)
            {
                movementVector *= 0f;
                secondsSinceLastTap += Time.deltaTime;
            }
            else
            {
                if (secondsSinceLastTap <= doubleTapTime && secondsSinceLastTap >= 0f)
                {
                    movementVector = movementVector.normalized;
                }
                else
                {
                    secondsSinceLastTap = 0f;
                }
            }
        }

        else
        {
            if (secondsSinceLastTap == -1f)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    movementVector.y += -1;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    movementVector.y += 1;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    movementVector.x += -1;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    movementVector.x += 1;
                }
                movementVector = movementVector.normalized;
            }
            else
            {
                movementVector = Vector2.zero;
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    movementVector.y += -1;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    movementVector.y += 1;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    movementVector.x += -1;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    movementVector.x += 1;
                }
                movementVector = movementVector.normalized * 0.79f;
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if(secondsSinceLastTap <= doubleTapTime && secondsSinceLastTap > 0f)
                    {
                        secondsSinceLastTap = -1f;
                    }
                    else
                    {
                        secondsSinceLastTap = 0f;
                    }
                }
                else
                {
                    secondsSinceLastTap += Time.deltaTime;
                }
            }
        }

        movementVector = Vector2.ClampMagnitude(movementVector, 1f);
    }

    void AndroidInput()
    {

    }

    void iOSInput()
    {

    }
}
