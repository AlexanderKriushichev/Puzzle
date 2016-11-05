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

    /// <summary>
    /// Клетка в которой находится кристал
    /// </summary>
    public Cell cell;

    /// <summary>
    /// Предыдущий кристал во время движения
    /// </summary>
    private Cell previousCell;

    public Bonus bonus;

	// Use this for initialization
	void Start () {
        //SetRandomType();
        //SwitchColor();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    

    /// <summary>
    /// Получить цвет кристалла по цифровому коду
    /// </summary>
    /// <returns> Цвет кристалла </returns>
    public TypeOfCrystal GetColor()
    { 
        switch (colorID)
        {
            case 0: { return TypeOfCrystal.triangle; }
            case 1: { return TypeOfCrystal.sphere;   }
            case 2: { return TypeOfCrystal.rectangle;  }
            case 3: { return TypeOfCrystal.pentagon; }
            case 4: { return TypeOfCrystal.hexagon; }
            case 5: { return TypeOfCrystal.rhomb; }
            default: { return TypeOfCrystal.triangle; }
        }

    }

    /// <summary>
    /// Рандомная генерация цвета кристалла
    /// </summary>
    public void SetRandomType()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        colorID = Random.Range(0, 6);
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

    void OnDestroy()
    {
        if (previousCell != null)
        {
            previousCell.isCrystalIn = false;
            previousCell.CrystalMove();
        }
        if (bonus != null)
        {
            bonus.Acivate();
        }
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            if (previousCell != hit.collider.GetComponent<Cell>())
            {
                if (previousCell != null)
                {
                    previousCell.isCrystalIn = false;
                }
                previousCell = hit.collider.GetComponent<Cell>();
                previousCell.isCrystalIn = true;
            }
            else
            {
                if (previousCell != null)
                {
                    previousCell.isCrystalIn = true;
                }
            }
        }
    }
}

/// <summary>
/// Тип цвета кристалла
/// </summary>
public enum TypeOfCrystal { triangle, sphere, rhomb, rectangle, pentagon, hexagon }