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

    private Vector2 startPositionOfMouse;

    private SpriteRenderer spriteRenderer;

    private bool isDestoy;

    private float timerToDestroySprite;
    private float timerDestroy;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == gameObject)
                return;
        }


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
            if (gameField.CheckNearCombination(x, y, target) || gameField.CheckNearCombination(target.x, target.y, this))
            {
                crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(EndMove);
                target.crystal.transform.DOMove(transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(target.EndMove); ;

                Crystal obm;
                obm = crystal;
                crystal = target.crystal;
                target.crystal = obm;
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
        gameField.AddCombination(this);
    }



    /// <summary>
    /// Уничтожение кристала
    /// </summary>
    public void DestroyCrystal()
    {
        if (crystal != null)
        {
            isCrystalMove = false;
            Destroy(crystal.gameObject);
            crystal = null;
            isCrystalIn = false;
        }
        isCrystalMove = false;
    }
}
