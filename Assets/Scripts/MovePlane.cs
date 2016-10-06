using UnityEngine;
using System.Collections;

public class MovePlane : MonoBehaviour
{
    public float speed = 0.005f;
    public Vector2 direction = new Vector2(-1, 0);

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }    
}