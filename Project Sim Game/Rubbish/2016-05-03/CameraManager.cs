using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static Camera MainCam;
    private Rigidbody2D rb2dMainCam;
    //public static Camera UICam;
    public PlayerController player;
    private float lerpCoefficient = 2.5f;
    private float horizontalOffset = 0f;//2.5f;
    private Vector3 offset = new Vector3(0, 10, 0);
    private BoxCollider2D bounds;
    
    void Start()
    {
        MainCam = Camera.main;

        rb2dMainCam = MainCam.gameObject.AddComponent<Rigidbody2D>();
        rb2dMainCam.gravityScale = 0f;
        rb2dMainCam.freezeRotation = true;

        //UICam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        bounds = MainCam.GetComponent<BoxCollider2D>();
    }

    public float updateSpeed = 5f;
    public float cameraDeadZone = 1f;
    void FixedUpdate()
    {
        offset.x = horizontalOffset * player.transform.localScale.x;
        Vector2 pos = MainCam.transform.position;
        Vector2 playerPos = player.transform.position;

        Vector2 velVec = (playerPos - rb2dMainCam.position);

        if(Mathf.Abs(playerPos.x - rb2dMainCam.position.x) < cameraDeadZone)
        {
            velVec.x = 0f;
        }

        if(Mathf.Abs(playerPos.y - rb2dMainCam.position.y) < cameraDeadZone)
        {
            velVec.y = 0f;
        }

        rb2dMainCam.velocity = velVec * updateSpeed * Time.deltaTime;
    }

    void Update()
    {
        float height = 2f * MainCam.orthographicSize;
        bounds.size = new Vector2(height * MainCam.aspect, height);
    }
}
