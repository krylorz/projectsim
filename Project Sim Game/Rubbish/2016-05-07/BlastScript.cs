using UnityEngine;
using System.Collections;

public class BlastScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator ani;
    Collider2D col2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
    }

    float blastSpeed = 15f;
    Vector2 vect = Vector2.zero;
    public void Initialize(Vector2 shotVector, Vector2 playerPosition, PlayerBackend.PlayerDirection dir)
    {
        transform.position = playerPosition;
        Vector3 offset = Vector2.zero;
        switch (dir)
        {
            case PlayerBackend.PlayerDirection.Up:
                transform.position += Vector3.up * 1.5f;
                offset = Vector3.up;
                break;
            case PlayerBackend.PlayerDirection.Left:
                offset = Vector3.left;
                transform.position += Vector3.up * 3f / 4f + Vector3.left / 2f;
                break;
            case PlayerBackend.PlayerDirection.Right:
                offset = Vector3.right;
                transform.position += Vector3.up * 3f/4f + Vector3.right / 2f;
                break;
            case PlayerBackend.PlayerDirection.Down:
                transform.position += Vector3.down / 9f;
                offset = Vector3.down;
                break;
        }
        transform.position += offset;
        if (!float.IsNaN(shotVector.x))
        {
            vect = ((Vector2)shotVector + new Vector2(Random.Range(-1f, 1f) * 0.1f, Random.Range(-1f, 1f) * 0.1f)).normalized;
            transform.localEulerAngles = new Vector3(0, 0, -Mathf.Atan2(vect.x, vect.y) * 180f / Mathf.PI + 90f);
            vect *= blastSpeed;
        }
        else
        {
            vect = ((Vector2)offset + new Vector2(Random.Range(-1f, 1f) * 0.1f, Random.Range(-1f, 1f) * 0.1f)).normalized;
            transform.localEulerAngles = new Vector3(0, 0, -Mathf.Atan2(vect.x, vect.y) * 180f / Mathf.PI + 90f);
            vect *= blastSpeed;
        }
        
    }

    void Update()
    {
        rb2d.velocity = vect;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        vect *= 0f;
        col2d.enabled = false;
        ani.Play("BlastImpact");
        StartCoroutine(WaitAndDestroy());
    }

    private float impactSeconds = 0.25f;

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(impactSeconds);
        Destroy(gameObject);
    }
}