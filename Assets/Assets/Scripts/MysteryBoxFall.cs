using UnityEngine;

public class MysteryBoxFall : MonoBehaviour
{
    public float fallSpeed = 1f;

    void Update()
    {
        transform.Translate(0, -fallSpeed * Time.deltaTime, 0);

        // Despawn when off-screen
        if (transform.position.y < -Camera.main.orthographicSize - transform.localScale.y)
        {
            Destroy(gameObject);
        }
    }
}