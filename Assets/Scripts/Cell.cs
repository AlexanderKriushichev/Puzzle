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
                if (x + 1 < 8)
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
                if (y + 1 < 8)
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

    private void EndMove()
    {
        isCrystalMove = false;
        //gameField.DestroyCrystal();
        gameField.AddCombination(this);
    }

    public void Destroy()
    {
        Destroy(crystal.gameObject);
        crystal = null;
    }

    public static void MoveToCell(Cell from, Cell target)
    {
        if (from == target)
        {
            from.isCrystalMove = false;
            target.EndMove();
            return;
        }
        target.crystal = from.crystal;
        target.crystal.transform.parent = target.transform;
        target.crystal.transform.DOMove(target.transform.position, ((int)(target.crystal.transform.position - target.transform.position).magnitude) * 0.2f).SetEase(Ease.Linear).OnComplete(target.EndMove);
        from.crystal = null;
        from.isCrystalMove = false;
        target.isCrystalMove = true;
    }



    /// <summary>
    /// Перемещение кристалов
    /// </summary>
    public void CrystalMove()
    {
        if (isCellGenerate && crystal == null && !isBarrier)
        {
            int countOfCrystal = 1;
            while (crystal == null)
            {
                GameObject initCell = (GameObject)Instantiate(gameField.crystalPrefab, gameObject.transform);
                Cell emptyCell = gameField.GetEmptyCellDown(x, y);
                emptyCell.crystal = initCell.GetComponent<Crystal>();
                emptyCell.crystal.SetRandomType();
                //crystal.SwitchColor();
                emptyCell.crystal.transform.position = gameObject.transform.position + new Vector3(0, 1 * countOfCrystal, 0);
                emptyCell.crystal.transform.DOMove(emptyCell.transform.position, ((int)(emptyCell.crystal.transform.position - emptyCell.transform.position).magnitude) * 0.2f).SetEase(Ease.Linear).OnComplete(emptyCell.EndMove);
                emptyCell.isCrystalMove = true;
                countOfCrystal++;
            }
            return;
        }

        if (crystal == null && !isCrystalMove && !isCellGenerate)
        {
            if (gameField.cells[x, y - 1].crystal != null && !gameField.cells[x, y - 1].isCrystalMove)
            {
                MoveToCell(gameField.cells[x, y - 1], gameField.GetEmptyCellDown(x, y));
                return;
            }
            if (gameField.cells[x, y - 1].isBarrier)
            {
                if (x > 0)
                {
                    if (gameField.cells[x - 1, y - 1].crystal != null && !gameField.cells[x - 1, y - 1].isCrystalMove && !gameField.cells[x - 1, y - 1].isBarrier)
                    {
                        MoveToCell(gameField.cells[x - 1, y - 1], this);
                        return;
                    }
                }
                if (x < 7)
                {
                    if (gameField.cells[x + 1, y - 1].crystal != null && !gameField.cells[x + 1, y - 1].isCrystalMove && !gameField.cells[x + 1, y - 1].isBarrier)
                    {
                        MoveToCell(gameField.cells[x + 1, y - 1], this);
                        return;
                    }
                }
            }
        }

        
        }



    void LateUpdate()
    {
        if (crystal != null && !isCrystalMove && gameField.UpCellCanDown(x, y) == false && gameField.GetEmptyCellDown(x, y) == this)
        {
            if (y < 7)
            {
                if (x > 0)
                {
                    if (gameField.cells[x - 1, y + 1].crystal == null && !gameField.cells[x - 1, y + 1].isCrystalMove && !gameField.cells[x - 1, y + 1].isBarrier && gameField.UpCellCanDown(x - 1, y + 1) == false && !gameField.cells[x - 1, y + 1].isCrystalMove)
                    {
                        if (gameField.GetEmptyCellDown(x - 1, y + 1) == gameField.cells[x - 1, y + 1])
                        {
                            MoveToCell(gameField.cells[x, y], gameField.cells[x - 1, y + 1]);
                            return;
                        }
                        else
                        {
                            gameField.cells[x - 1, y + 1].crystal = crystal;
                            gameField.cells[x - 1, y + 1].crystal.transform.parent = gameField.cells[x - 1, y + 1].transform;
                            gameField.cells[x - 1, y + 1].crystal.transform.DOMove(gameField.cells[x - 1, y + 1].transform.position, ((int)(gameField.cells[x - 1, y + 1].crystal.transform.position - gameField.cells[x - 1, y + 1].transform.position).magnitude) * 0.2f).SetEase(Ease.Linear).OnComplete(() => MoveToCell(gameField.cells[x - 1, y + 1], gameField.GetEmptyCellDown(x - 1, y + 1)));
                            crystal = null;
                            isCrystalMove = false;
                            gameField.cells[x - 1, y + 1].isCrystalMove = true;
                        }
                    }
                }
                if (x < 7)
                {
                    if (gameField.cells[x + 1, y + 1].crystal == null && !gameField.cells[x + 1, y + 1].isCrystalMove && !gameField.cells[x + 1, y + 1].isBarrier && gameField.UpCellCanDown(x + 1, y + 1) == false && !gameField.cells[x + 1, y + 1].isCrystalIn)
                    {
                        if (gameField.GetEmptyCellDown(x + 1, y + 1) == gameField.cells[x + 1, y + 1])
                        {
                            MoveToCell(gameField.cells[x, y], gameField.cells[x + 1, y + 1]);
                            return;
                        }
                        else
                        {
                            gameField.cells[x + 1, y + 1].crystal = crystal;
                            gameField.cells[x + 1, y + 1].crystal.transform.parent = gameField.cells[x + 1, y + 1].transform;
                            gameField.cells[x + 1, y + 1].crystal.transform.DOMove(gameField.cells[x + 1, y + 1].transform.position, ((int)(gameField.cells[x + 1, y + 1].crystal.transform.position - gameField.cells[x + 1, y + 1].transform.position).magnitude) * 0.2f).SetEase(Ease.Linear).OnComplete(() => MoveToCell(gameField.cells[x + 1, y + 1], gameField.GetEmptyCellDown(x + 1, y + 1)));
                            crystal = null;
                            isCrystalMove = false;
                            gameField.cells[x + 1, y + 1].isCrystalMove = true;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        if (isBarrier) return;
        CrystalMove();
    }
}
