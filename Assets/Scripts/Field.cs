﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Field : MonoBehaviour
{

    public Cell[,] cells = new Cell[8, 8];

    public GameObject crystalPrefab;

    public static bool isCrystalMove;

    /// <summary>
    /// Комбинации
    /// </summary>
    private List<Combination> combinations = new List<Combination>();

    /// <summary>
    /// Флаг для проведения одной проверки на возможность хода за ход
    /// </summary>
    private bool onecheckOnStep;

    public bool inRotate { get; private set; }


    // Use this for initialization
    void Start()
    {

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
    /// Пометить все клетки на пути кристала как CrystalIn
    /// </summary>
    /// <param name="from">Начальная клетка</param>
    /// <param name="to">Конечная клетка</param>
    public void MarkCellCrystalIn(Cell from, Cell to)
    {
        if (from.x == to.x)
        {
            if (from.y < to.y)
            {
                for (int i = from.y; i <= to.y; i++)
                {
                    cells[from.x, i].isCrystalIn = true;
                }
            }

            if (from.y > to.y)
            {
                for (int i = to.y; i <= from.y; i++)
                {
                    cells[from.x, i].isCrystalIn = true;
                }
            }
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

        if (cells[x, y + 1].crystal == null && !cells[x, y + 1].isBarrier)
        {
            return GetEmptyCellDown(x, y + 1);
        }

        return cells[x, y];

    }

    /// <summary>
    /// Проверка клетки на нахождение под препятствием
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Возвращает true если находится не над препятствием, иначе false</returns>
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

    /// <summary>
    /// Выводит список клеток в комбинации
    /// </summary>
    /// <param name="x">Х</param>
    /// <param name="y">Y</param>
    /// <returns>Список клеток</returns>
    public List<Cell> GetAdjacentCells(int x, int y, ref List<Cell> checkedCell, ref bool waiting)
    {
        checkedCell.Add(cells[x, y]);
        cells[x, y].isChecked = true;
        List<Cell> nearbyCells = new List<Cell>();
        if (cells[x, y].isCrystalMove)
            return nearbyCells;
        if (cells[x, y].crystal == null)
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
                        nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell, ref waiting));
                    }
                    if (!cells[x + 1, y].isChecked)
                    {
                        nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell, ref waiting));
                    }
                }
                else
                {

                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && cells[x - 1, y].isCrystalMove && GetEmptyCellDown(x - 1, y) == cells[x - 1, y])
                    {
                        waiting = true;
                    }

                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && cells[x + 1, y].isCrystalMove && GetEmptyCellDown(x + 1, y) == cells[x + 1, y])
                    {
                        waiting = true;
                    }

                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && !cells[x - 1, y].isCrystalMove)
                    {
                        if (!cells[x - 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell, ref waiting));
                        }
                    }

                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && !cells[x + 1, y].isCrystalMove)
                    {
                        if (!cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell, ref waiting));
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
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell, ref waiting));
                        }
                    }

                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type && cells[x - 1, y].isCrystalMove && GetEmptyCellDown(x - 1, y) == cells[x - 1, y])
                    {
                        waiting = true;
                    }
                }
                if (cells[x + 1, y].crystal != null)
                {
                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && !cells[x + 1, y].isCrystalMove)
                    {
                        if (!cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell,ref waiting));
                        }
                    }

                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type && cells[x + 1, y].isCrystalMove && GetEmptyCellDown(x + 1, y) == cells[x + 1, y])
                    {
                        waiting = true;
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
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y, ref checkedCell,ref waiting));
                        }

                        if (cells[x - 1, y].isCrystalMove && GetEmptyCellDown(x - 1, y) == cells[x - 1, y])
                        {
                            waiting = true;
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
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y, ref checkedCell, ref waiting));
                        }

                        if (cells[x + 1, y].isCrystalMove && GetEmptyCellDown(x + 1, y) == cells[x + 1, y])
                        {
                            waiting = true;
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

                        nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell, ref waiting));
                    }
                    if (!cells[x, y + 1].isChecked)
                    {

                        nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell, ref waiting));
                    }
                }
                else
                {
                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type && cells[x, y - 1].isCrystalMove && GetEmptyCellDown(x, y - 1) == cells[x, y - 1])
                    {
                        waiting = true;
                    }

                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type && cells[x, y + 1].isCrystalMove && GetEmptyCellDown(x, y + 1) == cells[x, y + 1])
                    {
                        waiting = true;
                    }

                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y - 1].isChecked && !cells[x, y - 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell, ref waiting));
                        }
                    }
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell, ref waiting));
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
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell, ref waiting));
                        }

                        if (cells[x, y - 1].isCrystalMove && GetEmptyCellDown(x, y - 1) == cells[x, y - 1])
                        {
                            waiting = true;
                        }
                    }
                }
                if (cells[x, y + 1].crystal != null)
                {
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell, ref waiting));
                        }

                        if (cells[x, y + 1].isCrystalMove && GetEmptyCellDown(x, y + 1) == cells[x, y + 1])
                        {
                            waiting = true;
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
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1, ref checkedCell, ref waiting));
                        }

                        if (cells[x, y - 1].isCrystalMove && GetEmptyCellDown(x, y - 1) == cells[x, y - 1])
                        {
                            waiting = true;
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
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1, ref checkedCell, ref waiting));
                        }

                        if (cells[x, y + 1].isCrystalMove && GetEmptyCellDown(x, y + 1) == cells[x, y + 1])
                        {
                            waiting = true;
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
    //public void DestroyCrystal()
    //{
    //    if (!CheckMove())
    //    {
    //        return;
    //    }

    //    List<Cell> nearbyCells = new List<Cell>();
    //    List<Cell> checkedCell = new List<Cell>();
    //    for (int i = 0; i < 8; i++)
    //    {
    //        for (int j = 0; j < 8; j++)
    //        {
    //            if (!cells[i, j].isChecked)
    //                nearbyCells.AddRange(GetAdjacentCells(i, j, ref checkedCell));
    //        }
    //    }

    //    Debug.Log(nearbyCells.Count);
    //    foreach (Cell destroyCell in nearbyCells)
    //    {
    //        Destroy(destroyCell.crystal.gameObject);
    //        destroyCell.crystal = null;
    //    }
    //    ResetAllCells();
    //    //ResetCrystalMove();
    //    //MoveCrystals();

    //}

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

    /// <summary>
    /// Добавление комбинации от стартовой клетки
    /// </summary>
    /// <param name="cell">Стартовая клетка</param>
    public void AddCombination(Cell cell)
    {

        int x = cell.x;
        int y = cell.y;

        List<Cell> checkedCell = new List<Cell>();
        bool waiting = false;
        Combination combo = new Combination(cell, GetAdjacentCells(x, y, ref checkedCell, ref waiting));
        ResetCell(combo.cellsInCombination);
        ResetCell(checkedCell);

        if (!waiting)
        {
            foreach (Cell destroyCell in combo.cellsInCombination)
            {
                destroyCell.Destroy();
            }
        }
    }

    public void RoteteFieldRight()
    {
        if (!CheckMove())
            return;
        if (inRotate)
            return;
        inRotate = true;
        transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0, 0, -90), 0.5f).OnComplete(ReassignCellsPositionsRight);
    }

    private void ReassignCellsPositionsRight()
    {
        Cell[,] newList = new Cell[8, 8];
        foreach (Cell cell in cells)
        {
            int x;
            int y;
            x = 7 - cell.y;
            y = cell.x;
            cell.isCellGenerate = false;
            cell.x = x;
            cell.y = y;
            newList[x, y] = cell;
            if (y == 0)
                cell.isCellGenerate = true;
        }
        cells = new Cell[8, 8];
        cells = newList;
        inRotate = false;
    }

    public void RoteteFieldLeft()
    {
        if (!CheckMove())
            return;
        if (inRotate)
            return;
        inRotate = true;
        transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0, 0, 90), 0.5f).OnComplete(ReassignCellsPositionsLeft);
    }

    private void ReassignCellsPositionsLeft()
    {
        Cell[,] newList = new Cell[8, 8];
        foreach (Cell cell in cells)
        {
            int x;
            int y;
            x = cell.y;
            y = 7 - cell.x;
            cell.isCellGenerate = false;
            cell.x = x;
            cell.y = y;
            newList[x, y] = cell;
            if (y == 0)
                cell.isCellGenerate = true;
        }
        cells = new Cell[8, 8];
        cells = newList;
        inRotate = false;
    }

    void Update()
    {
        if (!CheckMove())
        {
            onecheckOnStep = true;
            return;
        }

        if (onecheckOnStep)
        {
            
            // Код для проверки на возможность следующего ход
            // если есть такая возможность записать все клетки будующей возможной комбинации в массив
            // в аоследствии их будем подствечивать как подсказку
            // иначе удалить все клетки и сгенерировать новое поле

            onecheckOnStep = false;
        }
    }
}

[System.Serializable]
public class Combination
{
    public List<Cell> cellsInCombination;
    public Cell activeCell;
    public Combination(Cell _activeCell, List<Cell> _cellsInCombination)
    {
        activeCell = _activeCell;
        cellsInCombination = _cellsInCombination;
    }
    public Combination()
    {
        cellsInCombination = new List<Cell>();
    }
    public void Destroy()
    {
        foreach (Cell cell in cellsInCombination)
        {
            Debug.Log(1);
            if (cell == null)
                return;
        }

        foreach (Cell cell in cellsInCombination)
        {
            cell.Destroy();
        }

        cellsInCombination.Clear();
    }
}
