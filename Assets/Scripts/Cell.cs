using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{

    [Header("Position")]
    public int x;
    public int y;
    [Space(10)]

    public bool isCellGenerate;
    [Space(10)]

    public bool isBarrier;
    [Space(10)]
    public Crystal crystal;

    public Field gameField;

    public bool isCrystalMove;

    public bool isCrystalIn;

    public bool isChecked;

    public bool isAddInList;

    public List<Crystal> listCrystalMove = new List<Crystal>();

    public List<Cell> cellInCombination = new List<Cell>();

    private Vector2 startPositionOfMouse;

    private SpriteRenderer spriteRenderer;

    private ParticleSystem particleSystem;

    [HideInInspector]
    public DestroyEffect destroyEffect; 

    private bool isDestoy;

    private float timerToDestroySprite;
    private float timerDestroy;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        GetComponent<ParticleSystemRenderer>().sortingOrder = 10;
        destroyEffect = GetComponentInChildren<DestroyEffect>();
        destroyEffect.cell = this;
    }
        

    void OnMouseDown()
    {
        startPositionOfMouse = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (!gameField.CheckMove())
            return;
        if (gameField.inRotate)
            return;
        if (gameField.moveCrystals.Count != 0)
            return;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == gameObject)
                return;
        }

        if (ScoreManager.countMove <= 0)
            return;

        ScoreManager.MakeMove();

        Vector2 endPositionOfMouse = Input.mousePosition;
        Vector2 moveVector = endPositionOfMouse - startPositionOfMouse;

        if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y))
        {
            if (moveVector.x > 0)
            {
                if (x + 1 < Field.size)
                {
                    ExchangeCrystal(gameField.cells[x + 1, y]);
                }
            }
            else
            {
                if (x - 1 >= 0)
                {
                    ExchangeCrystal(gameField.cells[x - 1, y]);
                }
            }
        }
        else
        {
            if (moveVector.y > 0)
            {
                if (y - 1 >= 0)
                {
                    ExchangeCrystal(gameField.cells[x, y - 1]);
                }
            }
            else
            {
                if (y + 1 < Field.size)
                {
                    ExchangeCrystal(gameField.cells[x, y + 1]);
                }
            }

        }
    }

    /// <summary>
    /// Меняет местами клетки
    /// </summary>
    /// <param name="target">Клетка</param>
    public void ExchangeCrystal(Cell target)
    {
        if (target.crystal != null && crystal!= null)
        {
            isCrystalMove = true;
            target.isCrystalMove = true;

            if (crystal.bonus != null && target.crystal.bonus != null)
            {
                if (crystal.bonus.GetType() == typeof(LineBonus) && target.crystal.bonus.GetType() == typeof(LineBonus))
                {
                    crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(delegate { DestroyCrystal(); target.DestroyCrystal(); });
                    if (target.x == x)
                    {
                        crystal.bonus.type = TypeLineBonus.Verical;
                        target.crystal.bonus.type = TypeLineBonus.Horizontal;
                    }
                    else
                    {
                        target.crystal.bonus.type = TypeLineBonus.Verical;
                        crystal.bonus.type = TypeLineBonus.Horizontal;
                    }
                    return;
                }

                if ((crystal.bonus.GetType() == typeof(BoxBonus) && target.crystal.bonus.GetType() == typeof(LineBonus)) || (crystal.bonus.GetType() == typeof(LineBonus) && target.crystal.bonus.GetType() == typeof(BoxBonus)))
                {
                    Destroy(crystal.bonus);
                    Destroy(target.crystal.bonus);

                    List<Cell> destroyCellList = new List<Cell>();
                    destroyCellList.Add(this);

                    crystal.spriteRenderer.sortingOrder = 0;
                    target.crystal.spriteRenderer.sortingOrder = 0;

                    destroyCellList.Add(target);
                    if (target.x == x)
                    {
                        LineBonus lineBonus = crystal.gameObject.AddComponent<LineBonus>();
                        lineBonus.Line(TypeLineBonus.Verical, gameField, crystal);
                        lineBonus.SetEffect(crystal.InitLineEffect(), crystal.lineSprite);
                        lineBonus.OffAnim();
                        crystal.bonus = lineBonus;

                        lineBonus = target.crystal.gameObject.AddComponent<LineBonus>();
                        lineBonus.Line(TypeLineBonus.Horizontal, gameField, target.crystal);
                        lineBonus.SetEffect(target.crystal.InitLineEffect(), target.crystal.lineSprite);
                        lineBonus.OffAnim();
                        target.crystal.bonus = lineBonus;

                    }
                    else
                    {
                        LineBonus lineBonus = crystal.gameObject.AddComponent<LineBonus>();
                        lineBonus.Line(TypeLineBonus.Horizontal, gameField, crystal);
                        lineBonus.SetEffect(crystal.InitLineEffect(), crystal.lineSprite);
                        lineBonus.OffAnim();
                        crystal.bonus = lineBonus;

                        lineBonus = target.crystal.gameObject.AddComponent<LineBonus>();
                        lineBonus.Line(TypeLineBonus.Verical, gameField, target.crystal);
                        lineBonus.SetEffect(target.crystal.InitLineEffect(), target.crystal.lineSprite);
                        lineBonus.OffAnim();
                        target.crystal.bonus = lineBonus;
                    }

                    if (target.x > 0)
                    {
                        LineBonus.DestroyCell(target.x - 1, target.y, TypeLineBonus.Verical);
                    }

                    if (target.x < Field.size - 1)
                    {
                        LineBonus.DestroyCell(target.x + 1, target.y, TypeLineBonus.Verical);
                    }

                    if (target.y > 0)
                    {
                        LineBonus.DestroyCell(target.x, target.y - 1, TypeLineBonus.Horizontal);
                    }

                    if (target.y < Field.size - 1)
                    {
                        LineBonus.DestroyCell(target.x, target.y + 1, TypeLineBonus.Horizontal);
                    }
                    crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(
                        delegate
                        {
                            crystal.spriteRenderer.sortingOrder = 0;
                            target.crystal.spriteRenderer.sortingOrder = 0;

                            foreach (Cell cellDestroy in destroyCellList)
                            {
                                cellDestroy.DestroyCrystal();
                            }
                        });
                    return;

                }
                if (crystal.bonus.GetType() == typeof(BoxBonus) && target.crystal.bonus.GetType() == typeof(BoxBonus))
                {
                    Destroy(crystal.bonus);
                    Destroy(target.crystal.bonus);

                    BoxBonus lineBonus = target.crystal.gameObject.AddComponent<BoxBonus>();
                    lineBonus.Box(gameField, target.crystal);
                    lineBonus.SetEffect(target.crystal.InitBoxEffect(), target.crystal.colorsBoxBonus[target.crystal.colorID]);
                    lineBonus.AreaSize = 2;
                    target.crystal.bonus = lineBonus;
                    target.crystal.spriteRenderer.sortingOrder += 2;
                    target.crystal.spriteRenderer.sprite = target.crystal.spriteOfBoxEffect[target.crystal.colorID];
                    crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(target.DestroyCrystal);
                    return;
                }
                
            }
            else
            {
                if (crystal.bonus != null)
                {
                    if (crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        crystal.GetComponent<StarBonus>().SetType(target.crystal.type);
                        crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(DestroyCrystal);
                        return;
                    }
                }
                if (target.crystal.bonus != null)
                {
                    if (target.crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        target.crystal.GetComponent<StarBonus>().SetType(crystal.type);
                        crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(target.DestroyCrystal);
                        return;
                    }
                }
            }



            if (gameField.CheckNearCombination(x, y, target) || gameField.CheckNearCombination(target.x, target.y, this))
            {
                crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(EndMove);
                target.crystal.transform.DOMove(transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(target.EndMove);

                Crystal obm;
                obm = crystal;
                crystal = target.crystal;
                target.crystal = obm;
                target.crystal.cell = this;
                target.crystal.previousCell = this;
                crystal.cell = target;
                crystal.previousCell = target;

                if (y == target.y)
                {
                    target.crystal.typeOfLine = TypeLineBonus.Horizontal;
                    crystal.typeOfLine = TypeLineBonus.Horizontal;
                }
            }
            else
            {
                crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(() => ReturnCrystal(target));
                target.crystal.transform.DOMove(transform.position, 0.2f).SetEase(Ease.InSine);
            }
        }
    }

    /// <summary>
    /// Возвращает помененые клетки обратно на места
    /// </summary>
    /// <param name="target">Клатка</param>
    private void ReturnCrystal(Cell target)
    {
        isCrystalMove = true;

        crystal.transform.DOMove(transform.position, 0.5f).OnComplete(EndMove);
        target.crystal.transform.DOMove(target.transform.position, 0.5f).OnComplete(target.EndMove);

    }

    /// <summary>
    /// Конец перемещения кристала
    /// </summary>
    private void EndMove()
    {
        isCrystalMove = false;
        crystal.cell = this;
        crystal.previousCell = this;
        gameField.AddCombination(this);
    }

    /// <summary>
    /// Уничтожение кристала
    /// </summary>
    public void DestroyCrystal()
    {
        if (crystal != null)
        {
            if (crystal.bonus == null)
            {
                isCrystalMove = true;
                particleSystem.Play();
                destroyEffect.Activate(crystal.gameObject, true,100);

            }
            else
            {
                if (!crystal.bonus.bounceComplite)
                {
                    if (!crystal.bonus.bounceStart)
                    {
                        crystal.bonus.Acivate();
                        cellInCombination.Clear();
                        isCrystalMove = false;

                    }
                }
                else
                {
                    isCrystalMove = true;
                    destroyEffect.Activate(crystal.gameObject, true,100);
                    particleSystem.Play();
                }
            }
        }
        else
        {
            isCrystalMove = false;
        }
    }

    
}
