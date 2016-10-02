using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Crystal : MonoBehaviour {

    public TypeOfCrystal type;
    private SpriteRenderer spriteRenderer;

    public Sprite[] spritesOfCrystal = new Sprite[4];


	// Use this for initialization
	void Start () {
        //SetRandomType();
        //SwitchColor();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    //public void SetType(TypeOfCrystal typeOfCrystal);

    public void SetRandomType()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();


        switch (Random.Range(0, 4))
        {
            case 0:
                {
                    type = TypeOfCrystal.Red;
                    spriteRenderer.sprite = spritesOfCrystal[0];
                    break;
                }
            case 1:
                {
                    type = TypeOfCrystal.Blue;
                    spriteRenderer.sprite = spritesOfCrystal[1];

                    break;
                }
            case 2:
                {
                    type = TypeOfCrystal.Green;
                    spriteRenderer.sprite = spritesOfCrystal[2];

                    break;
                }
            case 3:
                {
                    type = TypeOfCrystal.Yellow;
                    spriteRenderer.sprite = spritesOfCrystal[3];

                    break;
                }
        }
    }

}

public enum TypeOfCrystal { Red, Blue, Green, Yellow}