using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Field : MonoBehaviour {

    public Cell[,] cells = new Cell[8,8];
    
    public GameObject crystalPrefab;

    public static bool isCrystalMove;

    /// <summary>
    /// Комбинации
    /// </summary>
    private List<Combination> combinations;

	// Use this for initialization
	void Start () {

        foreach (Cell cell in gameObject.GetComponentsInChildren<Cell>())
        {
            cells[cell.x, cell.y] = cell;
            if (!cells[cell.x, cell.y].isBarrier)
            {
                GameObject initCell = (GameObject)Instantiate(crystalPrefab, cells[cell.x, cell.y].transform);
                initCell.GetComponent<Crystal>().SetRandomType(GenerateColor(cell.x, cell.y));
                //initCell.GetComponent<Crystal>().SwitchColor();
                cells[cell.x, cell.y].crystal = initCell.GetComponent<Crystal>();
                cells[cell.x, cell.y].crystal.transform.position = cells[cell.x, cell.y].transform.position + new Vector3(0, 1, 0);
                cells[cell.x, cell.y].crystal.transform.DOMove(cells[cell.x, cell.y].transform.position, 0.5f).OnComplete(delegate { ResetCrystalMove(); });
                cells[cell.x, cell.y].isCrystalMove = true;
                cells[cell.x, cell.y].gameField = this;
            }
            else
            {
                cells[cell.x, cell.y].gameField = this;
            }
        }

       
        
	}

    void ResetAllCells()
    {
        foreach (Cell cell in cells)
        {
            cell.isAddInList = false;
            cell.isChecked = false;
        }
    }

    void ResetCell(List<Cell> resetCells)
    {
        foreach (Cell cell in resetCells)
        {
            cell.isAddInList = false;
            cell.isChecked = false;
        }
    }

    public void MoveCrystals()
    {
        
        bool f = false;
        while (!f)
        {
            bool move = true;
            foreach (Cell cell in cells)
            {
                if (cell.crystal == null)
                {
                    cell.CrystalMove();
                    Debug.Log(1);
                    move = false;
                    break;
                }
            }
            f = move;
        }
    }

    void ResetCrystalMove()
    {
        foreach (Cell cell in cells)
        {
            cell.isCrystalMove = false;
        }
    }

    /// <summary>
    /// Проверка всех клеток на их неподвижность
    /// </summary>
    /// <returns>Если хоть одна клетка в движении возвращает fakse, иначе true</returns>
    public bool CheckMove()
    {
        foreach (Cell cell in cells)
        {
            if (cell.isCrystalMove)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Проверка на комбинацию
    /// </summary>
    /// <param name="x">Координата Х</param>
    /// <param name="y">Координата Y</param>
    /// <param name="typeOfCrystal">Тип кристала</param>
    /// Возвращает true если есть комбинация, иначе false
    /// <returns></returns>
    public bool CheckNearCombination(int x, int y, Cell target)
    {
        if (x > 1)
            if (cells[x - 1, y].crystal != null && cells[x - 2, y].crystal != null)
                if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x - 2, y].crystal.type == target.crystal.type && cells[x - 1, y] != target && !cells[x - 1, y].isCrystalMove)
                    return true;
        if (x > 0 && x < 7)
            if (cells[x - 1, y].crystal != null && cells[x + 1, y].crystal != null)
                if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x + 1, y].crystal.type == target.crystal.type && cells[x - 1, y] != target && cells[x + 1, y] != target && !cells[x - 1, y].isCrystalMove && !cells[x + 1, y].isCrystalMove)
                    return true;
        if (x < 6)
            if (cells[x + 1, y].crystal != null && cells[x + 2, y].crystal != null)
                if (cells[x + 1, y].crystal.type == target.crystal.type && cells[x + 2, y].crystal.type == target.crystal.type && cells[x + 1, y] != target && !cells[x + 1, y].isCrystalMove)
                    return true;

        if (y > 1)
            if (cells[x, y - 1].crystal != null && cells[x, y - 2].crystal != null)
                if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y - 2].crystal.type == target.crystal.type && cells[x, y - 1] != target && !cells[x, y - 1].isCrystalMove)
                    return true;
        if (y > 0 && y < 7)
            if (cells[x, y - 1].crystal != null && cells[x, y + 1].crystal != null)
                if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y + 1].crystal.type == target.crystal.type && cells[x, y - 1] != target && cells[x, y + 1] != target && !cells[x, y - 1].isCrystalMove && !cells[x, y + 1].isCrystalMove)
                    return true;
        if (y < 6)
            if (cells[x, y + 1].crystal != null && cells[x, y + 2].crystal != null)
                if (cells[x, y + 1].crystal.type == target.crystal.type && cells[x, y + 2].crystal.type == target.crystal.type && cells[x, y + 1] != target && !cells[x, y + 1].isCrystalMove)
                    return true;
        return false;
    }

    
    /// <summary>
    /// Проверка клетки на нахождение под препятствием
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Возвращает true если клетка находится под препятствием, иначе false</returns>
    public bool CheckCellUnderBarrier(int x, int y)
    {
        if (y < 0) return false;
        if (cells[x, y].isBarrier) return true;
        if (y == 0) return false;
        if (cells[x, y - 1].isBarrier)
            return true;
        else
            return CheckCellUnderBarrier(x, y - 1);
    }

    /// <summary>
    /// Когда клетка под препятствием ижет поиск диагональных спусков для кристалов
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public void FindDiagonalCellMove(int x, int y)
    {
        CheckNearCellFromFindDiagonal(x, y, -1);
        CheckNearCellFromFindDiagonal(x, y, 1);
    }

    private void CheckNearCellFromFindDiagonal(int x, int y, int index)
    {
        if (x == 0)
        {
            if (cells[x, y].crystal != null && !cells[x, y].isCrystalMove && cells[x, y - 1].crystal != null && !cells[x, y - 1].isCrystalMove && !cells[x, y - 1].isBarrier)
            {

            }
        }
        if (x == 7)
        {
            if (cells[x, y].crystal != null && !cells[x, y].isCrystalMove && cells[x, y - 1].crystal != null && !cells[x, y - 1].isCrystalMove && !cells[x, y - 1].isBarrier)
            {
            }
        }
        x += index;
        if (x < 0 || x > 7) return;
        if (cells[x, y].crystal != null)
        {
            if (cells[x, y - 1].isBarrier)
            {
                FindDiagonal(x, y + 1, index);
            }
            else
            {
                if (cells[x, y - 1].crystal != null)
                {
                    FindDiagonal(x, y - 1, index);
                }
            }
        }
        else
        {
            if (!cells[x, y].isBarrier)
            {
                if (cells[x, y - 1].isBarrier)
                {
                    CheckNearCellFromFindDiagonal(x, y, index);
                }
            }
        }
    }

    private void FindDiagonal(int x, int y, int index)
    {
        if (y == 7) return;
        if (x - index == 0 || x - index == 7) return;
        if (cells[x - index, y].crystal == null)
        {
            Cell.MoveToCell(cells[x - index,y],cells[x, y + 1]);
        }
        else
        {
            FindDiagonal(x-index, y + 1, index);
        }
    }

    /// <summary>
    /// Проверка на свободное место
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Положение пустого места</returns>
    public Cell GetEmptyCellDown(int x, int y)
    {
        if (y + 1 > 7) return cells[x, y];

        if (cells[x, y + 1].crystal == null  && !cells[x, y + 1].isBarrier)
        {
            return GetEmptyCellDown(x, y + 1);
        }

        return cells[x, y];
        
    }

    public bool UpCellCanDown(int x, int y)
    {
        if (y - 1 < 0) return true;

        if (cells[x, y - 1].crystal == null)
        {
            if (!cells[x, y - 1].isBarrier)
            { 
                return UpCellCanDown(x, y - 1);
            }
            else
            { 
                return false;
            }
        }

        return true;
    }


    public bool UpCell(int x, int y)
    {
        if (y == 0)
            return true;
        if (cells[x, y - 1].crystal == null && !cells[x, y - 1].isCrystalMove)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Выводит список клеток в комбинации
    /// </summary>
    /// <param name="x">Х</param>
    /// <param name="y">Y</param>
    /// <returns>Список клеток</returns>
    public List<Cell> GetAdjacentCells(int x, int y, ref List<Cell> checkedCell)
    {
        checkedCell.Add(cells[x,y]);
        cells[x,y].isChecked = true;
        List<Cell> nearbyCells = new List<Cell>();
        if (cells[x,y].isCrystalMove)
            return nearbyCells;
        if (cells[x,y].crystal == null)
            return nearbyCells;
        if (x > 0 && x < 7)
        {
            if (cells[x - 1, y].crystal != null && cells[x + 1, y].crystal != null)
            {
                if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && cells[x + 1, y].crystal.type == cells[x, y].crystal.type && !cells[x - 1, y].isCrystalMove && !cells[x + 1, y].isCrystalMove)
                {
                    if (!cells[x, y].isAddInList)
                    {
                        cells[x, y].isAddInList = true;
                        nearbyCells.Add(cells[x, y]);
                    }
                    if (!cells[x - 1, y].isAddInList)
                    {
                        cells[x - 1, y].isAddInList = true;
                        nearbyCells.Add(cells[x - 1, y]);
                    }
                    if (!cells[x + 1, y].isAddInList)
                    {
                        cells[x + 1, y].isAddInList = true;
                        nearbyCells.Add(cells[x + 1, y]);
                    }
                    if (!cells[x - 1, y].isChecked)
                    {
                        nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell));
                    }
                    if (!cells[x + 1, y].isChecked)
                    {
                        nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell));
                    }
                }
                else
                {
                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && !cells[x - 1, y].isCrystalMove)
                    {
                        if (!cells[x - 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell));
                        }
                    }
                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && !cells[x + 1, y].isCrystalMove)
                    {
                        if (!cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell));
                        }
                    }
                }
            }
            else
            {
                if (cells[x - 1, y].crystal != null)
                {
                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && !cells[x - 1, y].isCrystalMove)
                    {
                        if (!cells[x - 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell));
                        }
                    }
                }
                if (cells[x + 1, y].crystal != null)
                {
                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && !cells[x + 1, y].isCrystalMove)
                    {
                        if (!cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell));
                        }
                    }
                }
            }
        }
        else
        {
            if (x > 0)
            {
                if (cells[x - 1, y].crystal != null)
                {
                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x - 1, y].isChecked && !cells[x - 1, y].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell));
                        }
                    }
                }
            }
            if (x < 7)
            {
                if (cells[x + 1, y].crystal != null)
                {
                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x + 1, y].isChecked && !cells[x + 1, y].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell));
                        }
                    }
                }
            }
        }
        if (y > 0 && y < 7)
        {
            if ((cells[x, y - 1].crystal != null && cells[x, y + 1].crystal != null))
            {
                if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type && cells[x, y + 1].crystal.type == cells[x, y].crystal.type && !cells[x, y + 1].isCrystalMove && !cells[x, y - 1].isCrystalMove)
                {
                    if (!cells[x, y].isAddInList)
                    {
                        cells[x, y].isAddInList = true;
                        nearbyCells.Add(cells[x, y]);
                    }
                    if (!cells[x, y - 1].isAddInList)
                    {
                        cells[x, y - 1].isAddInList = true;
                        nearbyCells.Add(cells[x, y - 1]);
                    }
                    if (!cells[x, y + 1].isAddInList)
                    {
                        cells[x, y + 1].isAddInList = true;
                        nearbyCells.Add(cells[x, y + 1]);
                    }
                    if (!cells[x, y - 1].isChecked)
                    {

                        nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell));
                    }
                    if (!cells[x, y + 1].isChecked)
                    {

                        nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell));
                    }
                }
                else
                {
                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y - 1].isChecked && !cells[x, y - 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell));
                        }
                    }
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell));
                        }
                    }
                }
            }
            else
            {
                if (cells[x, y - 1].crystal != null)
                {
                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y - 1].isChecked && !cells[x, y - 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell));
                        }
                    }
                }
                if (cells[x, y + 1].crystal != null)
                {
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell));
                        }
                    }
                }
            }
        }
        else
        {
            if (y > 0)
            {
                if (cells[x, y - 1].crystal != null)
                {
                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y - 1].isChecked && !cells[x, y - 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell));
                        }
                    }
                }
            }
            if (y < 7)
            {
                if (cells[x, y + 1].crystal != null)
                {
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell));
                        }
                    }
                }
            }
        }
        return nearbyCells;
    }

    /// <summary>
    /// Уничтожение кристалов
    /// </summary>
    public void DestroyCrystal()
    {
        if (!CheckMove())
        {
            return;
        }

        List<Cell> nearbyCells = new List<Cell>();
        List<Cell> checkedCell = new List<Cell>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (!cells[i, j].isChecked)
                    nearbyCells.AddRange(GetAdjacentCells(i, j, ref checkedCell));
            }
        }

        Debug.Log(nearbyCells.Count);
        foreach (Cell destroyCell in nearbyCells)
        {
            Destroy(destroyCell.crystal.gameObject);
            destroyCell.crystal = null;
        }
        ResetAllCells();
        //ResetCrystalMove();
        //MoveCrystals();

    }

    /// <summary>
    /// Генерация массива с цифровыми кодами цветов ячеек
    /// </summary>
    private int GenerateColor(int x, int y)
    {
        List<int> colorList = new List<int> { 0, 1, 2, 3, 4, 5 };
        if (x > 1 && !cells[x - 1, y].isBarrier && !cells[x - 2, y].isBarrier)
        {
            if (cells[x - 1, y].crystal.colorID == cells[x - 2, y].crystal.colorID)
            {
                colorList.Remove(cells[x - 1, y].crystal.colorID);
            }
        }
        if (y > 1 && !cells[x, y - 1].isBarrier && !cells[x, y - 2].isBarrier)
        {
            if (cells[x, y - 1].crystal.colorID == cells[x, y - 2].crystal.colorID)
            {
                colorList.Remove(cells[x, y - 1].crystal.colorID);
            }
        }
        int newColor = Random.Range(0, colorList.Count);
        return colorList[newColor];
    }


    public void AddCombination(Cell cell)
    {

        int x = cell.x;
        int y = cell.y;

        List<Cell> checkedCell = new List<Cell>();

        Combination combo = new Combination(cell, GetAdjacentCells(x, y, ref checkedCell));
        ResetCell(combo.cellsInCombination);
        ResetCell(checkedCell);
        foreach (Cell destroyCell in combo.cellsInCombination)
        {
            destroyCell.Destroy();
        }
    }
}

public struct Combination
{
    public List<Cell> cellsInCombination;
    public Cell activeCell;
    public Combination(Cell _activeCell, List<Cell> _cellsInCombination)
    {
        activeCell = _activeCell;
        cellsInCombination = _cellsInCombination;
    }
}