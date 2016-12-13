using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private bool pressed = false;
    private Vector3 previousMousePosition;
    private Vector3 previousCameraPosition;

    public float sensitivity;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    public List<Background> backgrounds;
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonUp(1))
        {
            pressed = false;
        }

        if (Input.GetMouseButton(1))
        {
            if (!pressed)
            {
                previousMousePosition = Input.mousePosition;
                previousCameraPosition = transform.position;
                pressed = true;
            }
            else
            {
                Vector3 pos = transform.position + (previousMousePosition - Input.mousePosition) / sensitivity;
                transform.position = new Vector3(Mathf.Clamp(pos.x, minPosition.x, maxPosition.x), Mathf.Clamp(pos.y, minPosition.y, maxPosition.y), transform.position.z);
                for (int i = 0; i < backgrounds.Count; i++)
                {
                    backgrounds[i].background.position += (previousCameraPosition - transform.position) / backgrounds[i].sensitivity;
                }
                previousMousePosition = Input.mousePosition;
                previousCameraPosition = transform.position;
            }
        }
	}
}

[System.Serializable]
public struct Background
{
    public Transform background;
    public float sensitivity;
}