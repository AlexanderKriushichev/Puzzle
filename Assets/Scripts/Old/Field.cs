using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Field : MonoBehaviour
{

    public Cell[,] cells = new Cell[size, size];

    public GameObject crystalPrefab;

    public static bool isCrystalMove;

    /// <summary>
    /// Комбинации
    /// </summary>
    public List<Combination> combinations = new List<Combination>();

    /// <summary>
    /// Флаг для проведения одной проверки на возможность хода за ход
    /// </summary>
    private bool onecheckOnStep;

    public List<Crystal> moveCrystals = new List<Crystal>();

    private bool moveComplite = true;

    public bool findComplite = true;

    public bool inRotate { get; private set; }

    public const int size = 8;

    // Use this for initialization
    void Start()
    {

        foreach (Cell cell in gameObject.GetComponentsInChildren<Cell>())
        {
            cells[cell.x, cell.y] = cell;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!cells[i, j].isBarrier && cells[i, j].crystal==null)
                {
                    GameObject initCell = (GameObject)Instantiate(crystalPrefab, cells[i, j].transform);
                    initCell.GetComponent<Crystal>().SetRandomType(GenerateColor(i, j));
                    cells[i, j].crystal = initCell.GetComponent<Crystal>();
                    cells[i, j].crystal.previousCell = cells[i, j];
                    cells[i, j].crystal.cell = cells[i, j];
                    cells[i, j].isCrystalIn = true;
                    cells[i, j].crystal.transform.position = cells[i, j].transform.position + new Vector3(0, 1, 0);
                    cells[i, j].crystal.transform.DOMove(cells[i, j].transform.position, 0.15f).SetEase(cells[i, j].crystal.curve).OnComplete(delegate { ResetCrystalMove(); });
                    cells[i, j].isCrystalMove = true;
                    cells[i, j].gameField = this;
                }
                else
                {
                    cells[i, j].gameField = this;
                }
            }
        }

    }

    //void GenerateField()
    //{
    //    foreach (Cell cell in cells)
    //    {
    //        if (!cell.isBarrier && cell.crystal==null)
    //        {
    //            GameObject initCell = (GameObject)Instantiate(crystalPrefab, cells[cell.x, cell.y].transform);
    //            initCell.GetComponent<Crystal>().SetRandomType(GenerateColor(cell.x, cell.y));
    //            cell.crystal = initCell.GetComponent<Crystal>();
    //            cell.crystal.previousCell = cell;
    //            cell.crystal.cell = cell;
    //            cell.isCrystalIn = true;
    //            cell.crystal.transform.position = cell.transform.position + new Vector3(0, 1, 0);
    //            cell.crystal.transform.DOMove(cell.transform.position, 0.15f).SetEase(cell.crystal.curve).OnComplete(delegate { ResetCrystalMove(); });
    //            cell.isCrystalMove = true;
    //        }
    //    }
    //}

    void DeleteField()
    {
        foreach (Cell cell in cells)
        {
            if (!cell.isBarrier && cell.crystal != null)
            {
                cell.DestroyCrystal();
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
            if (cell.isCrystalMove || cell.listCrystalMove.Count != 0)
            {
                isCrystalMove = false;
                return false;
            }
        }
        isCrystalMove = true;
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
            if (cells[x - 1, y].crystal != null && cells[x - 2, y].crystal != null && target.crystal != null)
                if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x - 2, y].crystal.type == target.crystal.type && cells[x - 1, y] != target && !cells[x - 1, y].isCrystalMove)
                    return true;
        if (x > 0 && x < 7)
            if (cells[x - 1, y].crystal != null && cells[x + 1, y].crystal != null && target.crystal != null)
                if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x + 1, y].crystal.type == target.crystal.type && cells[x - 1, y] != target && cells[x + 1, y] != target && !cells[x - 1, y].isCrystalMove && !cells[x + 1, y].isCrystalMove)
                    return true;
        if (x < 6)
            if (cells[x + 1, y].crystal != null && cells[x + 2, y].crystal != null && target.crystal != null)
                if (cells[x + 1, y].crystal.type == target.crystal.type && cells[x + 2, y].crystal.type == target.crystal.type && cells[x + 1, y] != target && !cells[x + 1, y].isCrystalMove)
                    return true;

        if (y > 1)
            if (cells[x, y - 1].crystal != null && cells[x, y - 2].crystal != null && target.crystal!=null)
                if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y - 2].crystal.type == target.crystal.type && cells[x, y - 1] != target && !cells[x, y - 1].isCrystalMove)
                    return true;
        if (y > 0 && y < 7)
            if (cells[x, y - 1].crystal != null && cells[x, y + 1].crystal != null && target.crystal != null)
                if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y + 1].crystal.type == target.crystal.type && cells[x, y - 1] != target && cells[x, y + 1] != target && !cells[x, y - 1].isCrystalMove && !cells[x, y + 1].isCrystalMove)
                    return true;
        if (y < 6)
            if (cells[x, y + 1].crystal != null && cells[x, y + 2].crystal != null && target.crystal != null)
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
        cell.cellInCombination.Clear();
        if (!waiting)
        {
            
            for (int j = 0; j < combo.cellsInCombination.Count; j++)
            {
                for (int i = 0; i < combinations.Count; i++)
                {
                    for (int z = 0; z < combinations[i].activeCell.cellInCombination.Count; z++)
                    {
                        if (combo.cellsInCombination[j] == combinations[i].activeCell || combinations[i].activeCell.cellInCombination[z] == combo.cellsInCombination[j])
                        {
                            combinations[i].activeCell.cellInCombination.Clear();
                            combinations.RemoveAt(i);
                            i = -1;
                            break;
                        }
                    }
                }
            }
            foreach (Cell cellsInCombo in combo.cellsInCombination)
            {
                if (cellsInCombo != cell)
                    cell.cellInCombination.Add(cellsInCombo);
            }
            if (combo.cellsInCombination.Count!=0)
                combinations.Add(combo);

        }
    }

    private void SortCombination()
    {
        List<Combination> sortCombination = new List<Combination>();
        while (combinations.Count!=0)
        {
            int maxCount = 0;
            int id = 0;
            for (int j = 0; j < combinations.Count; j++)
            {
                if (maxCount <= combinations[j].cellsInCombination.Count)
                {
                    maxCount = combinations[j].cellsInCombination.Count;
                    id = j;
                }
            }
            sortCombination.Add(combinations[id]);
            combinations.RemoveAt(id);
        }
        combinations = sortCombination;
    }
    /// <summary>
    /// Проверка на активный бонус
    /// </summary>
    /// <returns></returns>
    private bool CheckCellFromBonusActive()
    {
        foreach (Cell cell in cells)
        {
            if (cell.crystal != null)
                if (cell.crystal.bonus != null)
                    if (cell.crystal.bonus.bounceStart)
                        return false;
        }
        return true;
    }

    public void RoteteFieldRight()
    {
        if (!CheckMove())
            return;
        if (inRotate)
            return;
        inRotate = true;
        foreach(Cell cell in cells)
        {
            cell.destroyEffect.transform.localEulerAngles += new Vector3(0, 0, -90);
        }
        transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0, 0, -90), 0.5f).OnComplete(ReassignCellsPositionsRight);
    }

    private void ReassignCellsPositionsRight()
    {
        Cell[,] newList = new Cell[size, size];
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
            if (cell.crystal != null)
            {
                if (cell.crystal.bonus != null)
                {
                    if (cell.crystal.bonus is LineBonus)
                    {
                        if ((cell.crystal.bonus as LineBonus).type == TypeLineBonus.Horizontal)
                        {
                            (cell.crystal.bonus as LineBonus).type = TypeLineBonus.Verical;
                        }
                        else
                        {
                            (cell.crystal.bonus as LineBonus).type = TypeLineBonus.Horizontal;
                        }
                    }
                }
            }
            if (y == 0)
                cell.isCellGenerate = true;
        }
        cells = new Cell[size, size];
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
        foreach (Cell cell in cells)
        {
            cell.destroyEffect.transform.localEulerAngles += new Vector3(0, 0, 90);
        }
        transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0, 0, 90), 0.5f).OnComplete(ReassignCellsPositionsLeft);
    }

    private void ReassignCellsPositionsLeft()
    {
        Cell[,] newList = new Cell[size, size];
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
        cells = new Cell[size, size];
        cells = newList;
        inRotate = false;
    }

    /// <summary>
    /// Поиск возможной комбинации
    /// </summary>
    /// <param name="cell">Клетка</param>
    /// <returns>Цель движения</returns>
    public Cell FindPossibleCombination(Cell cell)
    {
        if (cell.crystal == null)
            return null;
        if (cell.x > 0)
        {
            if (cell.crystal != null && cells[cell.x - 1, cell.y].crystal != null)
            {
                if (cell.crystal.bonus != null && cells[cell.x - 1, cell.y].crystal.bonus != null)
                {
                    if (cell.crystal.bonus.GetType() == typeof(LineBonus) && cells[cell.x - 1, cell.y].crystal.bonus.GetType() == typeof(LineBonus))
                    {
                        return cells[cell.x - 1, cell.y];
                    }
                    if ((cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x - 1, cell.y].crystal.bonus.GetType() == typeof(LineBonus)) || (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x - 1, cell.y].crystal.bonus.GetType() == typeof(BoxBonus)))
                    {
                        return cells[cell.x - 1, cell.y];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x - 1, cell.y].crystal.bonus.GetType() == typeof(BoxBonus))
                    {
                        return cells[cell.x - 1, cell.y];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(StarBonus) || cells[cell.x - 1, cell.y].crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        return cells[cell.x - 1, cell.y];
                    }
                }
            }

            if (CheckNearCombination(cell.x, cell.y, cells[cell.x - 1, cell.y]))
                return cells[cell.x - 1, cell.y];
        }
        if (cell.x < size-1)
        {
            if (cell.crystal != null && cells[cell.x + 1, cell.y].crystal != null)
            {
                if (cell.crystal.bonus != null && cells[cell.x + 1, cell.y].crystal.bonus != null)
                {
                    if (cell.crystal.bonus.GetType() == typeof(LineBonus) && cells[cell.x + 1, cell.y].crystal.bonus.GetType() == typeof(LineBonus))
                    {
                        return cells[cell.x + 1, cell.y];
                    }
                    if ((cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x + 1, cell.y].crystal.bonus.GetType() == typeof(LineBonus)) || (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x + 1, cell.y].crystal.bonus.GetType() == typeof(BoxBonus)))
                    {
                        return cells[cell.x + 1, cell.y];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x + 1, cell.y].crystal.bonus.GetType() == typeof(BoxBonus))
                    {
                        return cells[cell.x + 1, cell.y];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(StarBonus) || cells[cell.x + 1, cell.y].crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        return cells[cell.x + 1, cell.y];
                    }
                }
            }
            if (CheckNearCombination(cell.x, cell.y, cells[cell.x + 1, cell.y]))
                return cells[cell.x + 1, cell.y];
        }

        if (cell.y > 0)
        {
            if (cell.crystal != null && cells[cell.x, cell.y - 1].crystal != null)
            {
                if (cell.crystal.bonus != null && cells[cell.x, cell.y - 1].crystal.bonus != null)
                {
                    if (cell.crystal.bonus.GetType() == typeof(LineBonus) && cells[cell.x, cell.y - 1].crystal.bonus.GetType() == typeof(LineBonus))
                    {
                        return cells[cell.x, cell.y - 1];
                    }
                    if ((cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x , cell.y-1].crystal.bonus.GetType() == typeof(LineBonus)) || (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x , cell.y-1].crystal.bonus.GetType() == typeof(BoxBonus)))
                    {
                        return cells[cell.x , cell.y-1];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x, cell.y - 1].crystal.bonus.GetType() == typeof(BoxBonus))
                    {
                        return cells[cell.x, cell.y - 1];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(StarBonus) || cells[cell.x, cell.y - 1].crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        return cells[cell.x, cell.y - 1];
                    }
                }
            }

            if (CheckNearCombination(cell.x, cell.y, cells[cell.x, cell.y - 1]))
                return cells[cell.x, cell.y - 1];
        }
        if (cell.y < size - 1)
        {
            if (cell.crystal != null && cells[cell.x, cell.y + 1].crystal != null)
            {
                if (cell.crystal.bonus != null && cells[cell.x, cell.y + 1].crystal.bonus != null)
                {
                    if (cell.crystal.bonus.GetType() == typeof(LineBonus) && cells[cell.x, cell.y + 1].crystal.bonus.GetType() == typeof(LineBonus))
                    {
                        return cells[cell.x, cell.y + 1];
                    }
                    if ((cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x, cell.y + 1].crystal.bonus.GetType() == typeof(LineBonus)) || (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x, cell.y + 1].crystal.bonus.GetType() == typeof(BoxBonus)))
                    {
                        return cells[cell.x, cell.y + 1];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(BoxBonus) && cells[cell.x, cell.y + 1].crystal.bonus.GetType() == typeof(BoxBonus))
                    {
                        return cells[cell.x, cell.y + 1];
                    }
                    if (cell.crystal.bonus.GetType() == typeof(StarBonus) || cells[cell.x, cell.y + 1].crystal.bonus.GetType() == typeof(StarBonus))
                    {
                        return cells[cell.x, cell.y + 1];
                    }
                }
            }
            if (CheckNearCombination(cell.x, cell.y, cells[cell.x, cell.y + 1]))
                return cells[cell.x, cell.y + 1];
        }

        return null;
    }

    /// <summary>
    /// Поиск пути движения для кристалов
    /// </summary>
    private void FindWayFromCrystals()
    {
        bool move = false;

        if (!CheckCellFromBonusActive())
            return;

        if (moveComplite)
        {   

            if (combinations.Count != 0)
            {

                SortCombination();

                foreach (Combination combo in combinations)
                {
                    switch (combo.cellsInCombination.Count)
                    {
                        case 3:
                            {
                                foreach (Cell cell in combo.cellsInCombination)
                                {
                                    cell.DestroyCrystal();
                                }
                                break;
                            }
                        case 4:
                            {
                                if (combo.activeCell.crystal.bonus == null)
                                {
                                    LineBonus lineBonus = combo.activeCell.crystal.gameObject.AddComponent<LineBonus>();
                                    lineBonus.Line(combo.activeCell.crystal.typeOfLine, this, combo.activeCell.crystal);
                                    lineBonus.SetEffect(combo.activeCell.crystal.InitLineEffect(), combo.activeCell.crystal.lineSprite);
                                    combo.activeCell.crystal.bonus = lineBonus;
                                    combo.activeCell.destroyEffect.Activate(combo.activeCell.gameObject, false,250);
                                    foreach (Cell cell in combo.cellsInCombination)
                                    {
                                        if (cell != combo.activeCell)
                                            cell.DestroyCrystal();
                                    }
                                    combo.activeCell.cellInCombination.Clear();
                                }
                                else
                                {
                                    foreach (Cell cell in combo.cellsInCombination)
                                    {
                                        cell.DestroyCrystal();
                                    }
                                    break;
                                }
                                break;
                            }
                        default:
                            {
                                int x = combo.activeCell.x;
                                int y = combo.activeCell.y;
                                int xCount = 0;
                                int yCount = 0;
                                for (int i = 0; i < combo.cellsInCombination.Count; i++)
                                {
                                    if (combo.cellsInCombination[i].x != x)
                                    {
                                        xCount++;
                                    }
                                    if (combo.cellsInCombination[i].y != y)
                                    {
                                        yCount++;
                                    }
                                }
                                if (xCount<4 && yCount<4)
                                {
                                    if (combo.activeCell.crystal.bonus == null)
                                    {
                                        BoxBonus lineBonus = combo.activeCell.crystal.gameObject.AddComponent<BoxBonus>();
                                        lineBonus.Box(this, combo.activeCell.crystal);
                                        lineBonus.SetEffect(combo.activeCell.crystal.InitBoxEffect(), combo.activeCell.crystal.colorsBoxBonus[combo.activeCell.crystal.colorID]);
                                        combo.activeCell.crystal.bonus = lineBonus;
                                        combo.activeCell.crystal.spriteRenderer.sortingOrder += 2;
                                        combo.activeCell.crystal.spriteRenderer.sprite = combo.activeCell.crystal.spriteOfBoxEffect[combo.activeCell.crystal.colorID];
                                        combo.activeCell.destroyEffect.Activate(combo.activeCell.gameObject, false,500);
                                        foreach (Cell cell in combo.cellsInCombination)
                                        {
                                            if (cell != combo.activeCell)
                                                cell.DestroyCrystal();
                                        }
                                        combo.activeCell.cellInCombination.Clear();
                                    }
                                    else
                                    {
                                        foreach (Cell cell in combo.cellsInCombination)
                                        {
                                            cell.DestroyCrystal();
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    if (combo.activeCell.crystal.bonus == null)
                                    {
                                        StarBonus starBonus = combo.activeCell.crystal.gameObject.AddComponent<StarBonus>();
                                        combo.activeCell.crystal.bonus = starBonus;
                                        starBonus.Star(this, combo.activeCell.crystal);
                                        starBonus.SetType(combo.activeCell.crystal.type);
                                        starBonus.SetEffect(combo.activeCell.crystal.lizerPrefab, combo.activeCell.crystal.InitStarArialEffect(), combo.activeCell.crystal.InitStarArialEffect(), combo.activeCell.crystal.firstColorArial, combo.activeCell.crystal.secondColorArial);
                                        combo.activeCell.crystal.spriteRenderer.sprite = combo.activeCell.crystal.starSprite;
                                        combo.activeCell.crystal.spriteRenderer.sortingOrder = 1;
                                        combo.activeCell.crystal.type = TypeOfCrystal.star;
                                        combo.activeCell.destroyEffect.Activate(combo.activeCell.gameObject, false,1000);
                                        foreach (Cell cell in combo.cellsInCombination)
                                        {
                                            if (cell != combo.activeCell)
                                                cell.DestroyCrystal();
                                        }
                                        combo.activeCell.cellInCombination.Clear();
                                    }
                                    else
                                    {
                                        foreach (Cell cell in combo.cellsInCombination)
                                        {
                                            cell.DestroyCrystal();
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        
                    }
                    
                }

                combinations.Clear();

                if (!CheckCellFromBonusActive())
                    return;
            }

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    if (cells[i, j].crystal != null)
                    {
                        if (cells[i, j].isCrystalMove)
                            return;

                        if (j < size - 1)
                        {
                            if (cells[i, j + 1].crystal == null && !cells[i, j + 1].isBarrier )
                            {
                                move = true;
                                if (!AddWayPoint(i,j,0,1))
                                {
                                    findComplite = !move && moveCrystals.Count == 0;
                                    return;
                                } 
                            }
                            else
                            {
                                if (i > 0)
                                {
                                    if (cells[i - 1, j].crystal == null && cells[i - 1, j + 1].crystal == null && !cells[i - 1, j + 1].isBarrier && !cells[i - 1, j ].isCellGenerate)
                                    {
                                        move = true;
                                        if (!AddWayPoint(i, j, -1, 1))
                                        {
                                            findComplite = !move && moveCrystals.Count == 0;
                                            return;
                                        } 
                                    }
                                    else
                                    {
                                        if (i < size - 1)
                                        {
                                            if (cells[i + 1, j].crystal == null && cells[i + 1, j + 1].crystal == null && !cells[i + 1, j + 1].isBarrier && !cells[i + 1, j].isCellGenerate)
                                            {
                                                move = true;
                                                if (!AddWayPoint(i, j, 1, 1))
                                                {
                                                    findComplite = !move && moveCrystals.Count == 0;
                                                    return;
                                                } 
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (i < size - 1)
                                    {
                                        if (cells[i + 1, j].crystal == null && cells[i + 1, j + 1].crystal == null && !cells[i + 1, j + 1].isBarrier && !cells[i + 1, j].isCellGenerate)
                                        {
                                            move = true;
                                            if (!AddWayPoint(i, j, 1, 1))
                                            {
                                                findComplite = !move && moveCrystals.Count == 0;
                                                return;
                                            } 
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!AddWayPoint(i, j, 0, 0))
                        {
                            findComplite = !move && moveCrystals.Count == 0;
                            return;
                        } 
                    }
                }
            }
        }

        findComplite = !move && moveCrystals.Count == 0;

        if (!move)
        {
            moveComplite = false;
            if (moveCrystals.Count == 0)
            {
                moveComplite = true;
                return;
            }
            for (int i = 0; i < moveCrystals.Count; i++)
            {
                if (moveCrystals[i].moveWayPoints.Count == 0)
                {
                    moveCrystals.RemoveAt(i);
                    break;
                }
                moveCrystals[i].MoveOnWay();

            }
        }
    }

    /// <summary>
    /// Добавление пути движения
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool AddWayPoint(int i, int j, int x, int y)
    {
        Crystal crystal = cells[i, j].crystal;
        if (crystal != null)
        {
            crystal.typeOfLine = TypeLineBonus.Verical;
            crystal.AddPoint(cells[i + x, j + y]);
            cells[i, j].crystal = null;
            cells[i + x, j + y].crystal = crystal;

            cells[i + x, j + y].listCrystalMove.Add(cells[i + x, j + y].crystal);

            if (moveCrystals.Find(element => element == cells[i + x, j + y].crystal) == null)
            {
                moveCrystals.Add(cells[i + x, j + y].crystal);
            }
        }
        if (cells[i, j].isCellGenerate)
        {
            GameObject initCell = (GameObject)Instantiate(crystalPrefab, cells[i, j].transform);
            cells[i, j].crystal = initCell.GetComponent<Crystal>();
            cells[i, j].crystal.SetRandomType();
            cells[i, j].crystal.transform.position = cells[i, j].transform.position + new Vector3(0, 1, 0);
            cells[i, j].crystal.previousCell = cells[i, j];
            cells[i, j].crystal.cell = cells[i, j];
            cells[i, j].crystal.AddPoint(cells[i, j]);
            cells[i, j].listCrystalMove.Add(cells[i, j].crystal);
            if (moveCrystals.Find(element => element == cells[i, j].crystal) == null)
            {
                moveCrystals.Add(cells[i, j].crystal);
            }
            return false;
        }
        return true;
    }

    public void Update()
    {

        FindWayFromCrystals();

        foreach (Cell cell in cells)
        {
            cell.destroyEffect.UpdateDestroy();
        }

        if (findComplite && CheckMove() && combinations.Count == 0)
        {
            bool check = false;
            foreach (Cell cell in cells)
            {
                Cell target = FindPossibleCombination(cell);
                if (target != null)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                Debug.Log("Нет ходов");
                DeleteField();
                //GenerateField();
            }
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
            cell.DestroyCrystal();
        }

        cellsInCombination.Clear();
    }
    public bool FullCombo()
    {
        foreach (Cell cell in cellsInCombination)
        {
            if (cell.crystal == null)
                return false;  
        }
        return true;
    }
}
