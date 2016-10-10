using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Crystal : MonoBehaviour {


    /// <summary>
    /// Цвет кристалла
    /// </summary>
    public TypeOfCrystal type;
    /// <summary>
    /// Цифровой код цвета
    /// </summary>
    public int colorID;

    /// <summary>
    /// Отрисовка
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    ///  Массив спрайтов для отрисовки кристаллов
    /// </summary>
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

    /// <summary>
    /// Смена цвета кристалла
    /// </summary>
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

    /// <summary>
    /// Получить цвет кристалла по цифровому коду
    /// </summary>
    /// <returns> Цвет кристалла </returns>
    public TypeOfCrystal GetColor()
    { 
        switch (colorID)
        {
            case 0: { return TypeOfCrystal.Red;    }
            case 1: { return TypeOfCrystal.Blue;   }
            case 2: { return TypeOfCrystal.Green;  }
            case 3: { return TypeOfCrystal.Yellow; }
        }
// Если цифровой код отличается от [0..3] - возвращает красный
        return TypeOfCrystal.Red;
    }

    /// <summary>
    /// Рандомная генерация цвета кристалла
    /// </summary>
    public void SetRandomType()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        colorID = Random.Range(0, 4);
        type = GetColor();
        spriteRenderer.sprite = spritesOfCrystal[colorID];        
    }

    /// <summary>
    /// Генерация цвета по цифровому коду
    /// </summary>
    /// <param name="ID"></param>
    public void SetRandomType(int ID)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        colorID = ID;
        type = GetColor();
        spriteRenderer.sprite = spritesOfCrystal[colorID];
    }
}

/// <summary>
/// Тип цвета кристалла
/// </summary>
public enum TypeOfCrystal { Red, Blue, Green, Yellow}