using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Crystal : MonoBehaviour {

    public TypeOfCrystal type;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        //SetRandomType();
        //SwitchColor();
	}

    

	// Update is called once per frame
	void Update () {



	}

    public void SwitchColor()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case TypeOfCrystal.Red:
                {
                    spriteRenderer.color = Color.red;
                    break;
                }
            case TypeOfCrystal.Blue:
                {
                    spriteRenderer.color = Color.blue;
                    break;
                }
            case TypeOfCrystal.Green:
                {
                    spriteRenderer.color = Color.green;
                    break;
                }
            case TypeOfCrystal.Yellow:
                {
                    spriteRenderer.color = Color.yellow;
                    break;
                }
        }
    }

    public void SetRandomType()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                {
                    type = TypeOfCrystal.Red;
                    break;
                }
            case 1:
                {
                    type = TypeOfCrystal.Blue;
                    break;
                }
            case 2:
                {
                    type = TypeOfCrystal.Green;
                    break;
                }
            case 3:
                {
                    type = TypeOfCrystal.Yellow;
                    break;
                }
        }
    }

}

public enum TypeOfCrystal { Red, Blue, Green, Yellow}