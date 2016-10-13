using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Cell : MonoBehaviour {

    [Header("Position")]
    public int x;
    public int y;
    [Space(10)]

    public bool isCellGenerate;

    public Crystal crystal;

    public Field gameField;

    public bool isCrystalMove;

    public bool isChecked;

    public bool isAddInList;

    private Vector2 startPositionOfMouse;


    void Start()
    {
        
    }

    void OnMouseDown()
    {
        startPositionOfMouse = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (!gameField.CheckMove())
            return;

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
      
        if (gameField.CheckNearCombination(x, y, target) || gameField.CheckNearCombination(target.x, target.y, this))
        {
            crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(delegate { isCrystalMove = false; });
            target.crystal.transform.DOMove(transform.position, 0.2f).SetEase(Ease.InSine);

            Crystal obm;
            obm = crystal;
            crystal = target.crystal;
            target.crystal = obm;
        }
        else
        {
            crystal.transform.DOMove(target.transform.position, 0.2f).SetEase(Ease.InSine).OnComplete(delegate { ReturnCrystal(target); });
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

        crystal.transform.DOMove(transform.position, 0.5f).OnComplete(delegate { isCrystalMove = false; });
        target.crystal.transform.DOMove(target.transform.position, 0.5f);

    }

    public List<Cell> CheckAdjacentCells()
    {
        isChecked = true;
        List<Cell> nearbyCells = new List<Cell>();
        if (isCrystalMove)
            return nearbyCells;
        if (crystal == null)
            return nearbyCells;
        if (x > 0 && x < 7)
        {
            if (gameField.cells[x - 1, y].crystal != null && gameField.cells[x + 1, y].crystal != null)
            {
                if (gameField.cells[x - 1, y].crystal.type == crystal.type && gameField.cells[x + 1, y].crystal.type == crystal.type && !gameField.cells[x - 1, y].isCrystalMove && !gameField.cells[x + 1, y].isCrystalMove)
                {
                    if (!isAddInList)
                    {
                        isAddInList = true;
                        nearbyCells.Add(gameField.cells[x, y]);
                    }
                    if (!gameField.cells[x - 1, y].isAddInList)
                    {
                        nearbyCells.Add(gameField.cells[x - 1, y]);
                    }
                    if (!gameField.cells[x + 1, y].isAddInList)
                    {
                        nearbyCells.Add(gameField.cells[x + 1, y]);
                    }
                    if (!gameField.cells[x - 1, y].isChecked)
                    {
                        nearbyCells.AddRange(gameField.cells[x - 1, y].CheckAdjacentCells());
                    }
                    if (!gameField.cells[x + 1, y].isChecked)
                    {
                        nearbyCells.AddRange(gameField.cells[x + 1, y].CheckAdjacentCells());
                    }
                }
                else
                {
                    if (gameField.cells[x - 1, y].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x - 1, y].isChecked)
                        {
                            nearbyCells.AddRange(gameField.cells[x - 1, y].CheckAdjacentCells());
                        }
                    }
                    if (gameField.cells[x + 1, y].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(gameField.cells[x + 1, y].CheckAdjacentCells());
                        }
                    }
                }
            }
        }
        else
        {
            if (x > 0)
            {
                if (gameField.cells[x - 1, y].crystal != null)
                {
                    if (gameField.cells[x - 1, y].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x - 1, y].isChecked && !gameField.cells[x - 1, y].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x - 1, y].CheckAdjacentCells());
                        }
                    }
                }
            }
            if (x < 7)
            {
                if (gameField.cells[x + 1, y].crystal != null)
                {
                    if (gameField.cells[x + 1, y].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x + 1, y].isChecked && !gameField.cells[x + 1, y].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x + 1, y].CheckAdjacentCells());
                        }
                    }
                }
            }
        }
        if (y > 0 && y < 7)
        {
            if ((gameField.cells[x, y - 1].crystal != null && gameField.cells[x, y + 1].crystal != null && !gameField.cells[x, y + 1].isCrystalMove && !gameField.cells[x, y - 1].isCrystalMove))
            {
                if (gameField.cells[x, y - 1].crystal.type == crystal.type && gameField.cells[x, y + 1].crystal.type == crystal.type)
                {
                    if (!isAddInList)
                    {
                        isAddInList = true;
                        nearbyCells.Add(gameField.cells[x, y]);
                    }
                    if (!gameField.cells[x, y - 1].isAddInList)
                    {
                        nearbyCells.Add(gameField.cells[x, y - 1]);
                    }
                    if (!gameField.cells[x, y + 1].isAddInList)
                    {
                        nearbyCells.Add(gameField.cells[x, y + 1]);
                    }
                    if (!gameField.cells[x, y - 1].isChecked)
                    {

                        nearbyCells.AddRange(gameField.cells[x, y - 1].CheckAdjacentCells());
                    }
                    if (!gameField.cells[x, y + 1].isChecked)
                    {

                        nearbyCells.AddRange(gameField.cells[x, y + 1].CheckAdjacentCells());
                    }
                }
                else
                {
                    if (gameField.cells[x, y - 1].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x, y - 1].isChecked && !gameField.cells[x, y - 1].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x, y - 1].CheckAdjacentCells());
                        }
                    }
                    if (gameField.cells[x, y + 1].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x, y + 1].isChecked && !gameField.cells[x, y + 1].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x, y + 1].CheckAdjacentCells());
                        }
                    }
                }
            }
        }
        else
        {
            if (y > 0)
            {
                if (gameField.cells[x, y - 1].crystal != null)
                {
                    if (gameField.cells[x, y - 1].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x, y - 1].isChecked && !gameField.cells[x, y - 1].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x, y - 1].CheckAdjacentCells());
                        }
                    }
                }
            }
            if (y < 7)
            {
                if (gameField.cells[x, y + 1].crystal != null)
                {
                    if (gameField.cells[x, y + 1].crystal.type == crystal.type)
                    {
                        if (!gameField.cells[x, y + 1].isChecked && !gameField.cells[x, y + 1].isCrystalMove)
                        {
                            //nearbyCells.Add(gameField.cells[x, y]);
                            nearbyCells.AddRange(gameField.cells[x, y + 1].CheckAdjacentCells());
                        }
                    }
                }
            }
        }

        //isChecked = false;
        return nearbyCells;
    }

    
	void Update () {

        if (isCellGenerate && crystal == null )
        {
            int countOfCrystal = 1;
            while (crystal == null)
            {
                GameObject initCell = (GameObject)Instantiate(gameField.crystalPrefab, gameObject.transform);
                Cell emptyCell = gameField.GetEmptyCell(x, y);
                emptyCell.crystal = initCell.GetComponent<Crystal>();
                emptyCell.crystal.SetRandomType();
                //crystal.SwitchColor();
                emptyCell.crystal.transform.position = gameObject.transform.position + new Vector3(0, 1 * countOfCrystal, 0);
                emptyCell.crystal.transform.DOMove(emptyCell.transform.position, 0.5f).SetEase(Ease.InSine).OnComplete(delegate { emptyCell.isCrystalMove = false; });
                countOfCrystal++;
            }            
        }

        if (crystal == null && !isCrystalMove && !isCellGenerate)
        {
            if (gameField.cells[x, y - 1].crystal != null && !gameField.cells[x, y - 1].isCrystalMove)
            {
                Cell emptyCell = gameField.GetEmptyCell(x, y);
                emptyCell.crystal = gameField.cells[x, y - 1].crystal;
                emptyCell.crystal.transform.parent = gameObject.transform;
                emptyCell.crystal.transform.DOMove(emptyCell.transform.position, 0.5f).SetEase(Ease.InSine).OnComplete(delegate { emptyCell.isCrystalMove = false; });
                gameField.cells[x, y - 1].crystal = null;
                emptyCell.isCrystalMove = true;
            }
        }
	}
}
