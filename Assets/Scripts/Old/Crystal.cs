using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
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
    [HideInInspector]
    public SpriteRenderer spriteRenderer;

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
    public Cell previousCell;

    [HideInInspector]
    public Bonus bonus;

    [Header("LineBonus")]
    public GameObject lineEffectPrefab;
    public Sprite lineSprite;
    public TypeLineBonus typeOfLine = TypeLineBonus.Verical;

    [Header("BoxBonus")]
    public GameObject boxEffectPrefab;
    public List<Color> colorsBoxBonus = new List<Color>();
    public List<Sprite> spriteOfBoxEffect = new List<Sprite>();

    [Header("StarBonus")]
    public GameObject lizerPrefab;
    public Sprite starSprite;
    public GameObject starArial;
    public Color firstColorArial;
    public Color secondColorArial;

    public List<Cell> moveWayPoints = new List<Cell>();

    public bool compliteMove;

    private bool isMove;

    public AnimationCurve curve;

    private Color color;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
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

    /// <summary>
    /// Перемещение по точкам
    /// </summary>
    public void MoveOnWay()
    {
        //if (SideMove())
        //{
        //    if (moveWayPoints.Count != 0)
        //        if (moveWayPoints[0].isCrystalIn)
        //            return;
        //}

        if (moveWayPoints.Count != 0)
        {
            if (moveWayPoints[0].listCrystalMove.Count > 0)
                if (moveWayPoints[0].listCrystalMove[0] != this)
                    return;
            Cell target = moveWayPoints[0];
            if (moveWayPoints.Count > 1)
            {
                if (!isMove && !moveWayPoints[0].isCrystalMove && !moveWayPoints[0].isCrystalIn)
                {
                    transform.DOMove(moveWayPoints[0].transform.position, 0.15f).SetEase(Ease.Linear).OnComplete(
                        delegate
                        {
                            moveWayPoints[0].listCrystalMove.RemoveAt(0);
                            moveWayPoints.RemoveAt(0);
                            isMove = false;
                            target.isCrystalMove = false;
                            target.isCrystalIn = true;
                        });
                    compliteMove = false;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (moveWayPoints.Count == 1)
                {
                    if (!isMove && !moveWayPoints[0].isCrystalMove && !moveWayPoints[0].isCrystalIn)
                    {
                        transform.DOMove(moveWayPoints[0].transform.position, 0.15f).SetEase(curve).OnComplete(
                            delegate
                            {
                                moveWayPoints[0].listCrystalMove.RemoveAt(0);
                                moveWayPoints.RemoveAt(0);
                                isMove = false;
                                target.isCrystalMove = false;
                                cell = target;
                                compliteMove = true;
                                cell.gameField.AddCombination(cell);
                                transform.parent = cell.transform;
                                cell.isCrystalIn = true;
                                cell.isCrystalMove = false;
                            });
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            isMove = true;
            target.isCrystalIn = true;
            target.isCrystalMove = true;
            if (previousCell != target)
                previousCell.isCrystalIn = false;
            previousCell = target;
        }
    }

    public void MoveToGeneratorCell()
    {
        transform.DOMove(cell.transform.position, 0.15f).SetEase(curve).OnComplete(
            delegate
            {
                cell.listCrystalMove.RemoveAt(0);
                isMove = false;
                compliteMove = true;
                cell.gameField.AddCombination(cell);
                transform.parent = cell.transform;
                cell.isCrystalIn = true;
                cell.isCrystalMove = false;
            });         
    }

    public GameObject InitLineEffect()
    {
        return (GameObject)Instantiate(lineEffectPrefab, this.transform.position, this.transform.rotation);
    }

    public GameObject InitBoxEffect()
    {
        return (GameObject)Instantiate(boxEffectPrefab, this.transform.position, this.transform.rotation);
    }

    public GameObject InitStarArialEffect()
    {
        return (GameObject)Instantiate(starArial, this.transform.position, this.transform.rotation);
    }

    /// <summary>
    /// Перемещается ли кристалл вбок
    /// </summary>
    /// <returns></returns>
    public bool SideMove()
    {
        if (moveWayPoints.Count == 0) return false;
        if (transform.position.x == moveWayPoints[0].transform.position.x)
            return false;
        else return true;
    }

    /// <summary>
    /// Добавить точку маршрута перемещения
    /// </summary>
    /// <param name="point">Клетка</param>
    public void AddPoint(Cell point)
    {
        moveWayPoints.Add(point);
    }

    public void DestroySprite()
    {
        Destroy(spriteRenderer);
    }

    void OnDestroy()
    {
        if (cell != null)
        {
            cell.isCrystalIn = false;
        }
        //if (bonus != null)
        //{
        //    bonus.Acivate();
        //}
    }

    void Update()
    {
        if (!cell.gameField.CheckMove())
            return;
        if (cell.gameField.inRotate)
            return;
        if (cell.gameField.moveCrystals.Count != 0)
            return;
        if (type == TypeOfCrystal.starOfDeath && cell.targetOfDeathStar)
        {
            type = TypeOfCrystal.None;
            ScoreManager.AddDeathStar();
            cell.destroyEffect.Activate(gameObject, true, 1000);
        }
    }
}

/// <summary>
/// Тип цвета кристалла
/// </summary>
public enum TypeOfCrystal { triangle, sphere, rhomb, rectangle, pentagon, hexagon, star, starOfDeath, None }
