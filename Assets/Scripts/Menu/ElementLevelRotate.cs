using UnityEngine;
using System.Collections;
using DG.Tweening;
public class ElementLevelRotate : MonoBehaviour {

    public Transform littleElement1;
    public Transform littleElement2;

    public Transform midElement1;
    public Transform midElement2;

    public Transform bigElement1;
    public Transform bigElement2;

    public Color activeColor;
    public Color disactiveColor;

    void Start()
    {
        SetRandomRotation(littleElement1, littleElement2);
        SetRandomRotation(midElement1, midElement2);
        SetRandomRotation(bigElement1, bigElement2);

        Rotate(littleElement1, 1, 5);
        Rotate(littleElement2, 1, 5);
        Rotate(midElement1, -1, 7);
        Rotate(midElement2, -1, 7);
        Rotate(bigElement1, 1, 9);
        Rotate(bigElement2, 1, 9);
    }

    void SetRandomRotation(Transform element1, Transform element2)
    {
        float z = Random.Range(0f,359f);
        element1.localEulerAngles = new Vector3(0, 0, z);
        element2.localEulerAngles = new Vector3(0, 0, z + 180);
    }

    void Rotate(Transform element,float dir, float speed)
    {
        Vector3 rotation = element.localEulerAngles;
        float z = rotation.z;
        DOTween.To(() => rotation, x => element.localEulerAngles = x, new Vector3(0, 0, z + dir*360), speed).SetEase(Ease.Linear).OnComplete(() => Rotate(element,dir, speed));
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            littleElement1.GetComponent<SpriteRenderer>().color = activeColor;
            littleElement2.GetComponent<SpriteRenderer>().color = activeColor;
            midElement1.GetComponent<SpriteRenderer>().color = activeColor;
            midElement2.GetComponent<SpriteRenderer>().color = activeColor;
            bigElement1.GetComponent<SpriteRenderer>().color = activeColor;
            bigElement2.GetComponent<SpriteRenderer>().color = activeColor;
        }
        else
        {
            littleElement1.GetComponent<SpriteRenderer>().color = disactiveColor;
            littleElement2.GetComponent<SpriteRenderer>().color = disactiveColor;
            midElement1.GetComponent<SpriteRenderer>().color = disactiveColor;
            midElement2.GetComponent<SpriteRenderer>().color = disactiveColor;
            bigElement1.GetComponent<SpriteRenderer>().color = disactiveColor;
            bigElement2.GetComponent<SpriteRenderer>().color = disactiveColor;
        }
    }

}
