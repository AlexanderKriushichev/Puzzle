using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Field : MonoBehaviour {

    public Cell[,] cells = new Cell[8,8];
    
    public GameObject crystalPrefab;

    public static bool isCrystalMove;


	// Use this for initialization
	void Start () {

        foreach (Cell cell in gameObject.GetComponentsInChildren<Cell>())
        {
            cells[cell.x, cell.y] = cell;
            
            GameObject initCell = (GameObject)Instantiate(crystalPrefab, cells[cell.x, cell.y].transform);
            initCell.GetComponent<Crystal>().SetRandomType(GenerateColor(cell.x,cell.y));
            //initCell.GetComponent<Crystal>().SwitchColor();
            cells[cell.x, cell.y].crystal = initCell.GetComponent<Crystal>();
            cells[cell.x, cell.y].crystal.transform.position = cells[cell.x, cell.y].transform.position + new Vector3(0, 1, 0);
            cells[cell.x, cell.y].crystal.transform.DOMove(cells[cell.x, cell.y].transform.position, 0.5f).OnComplete(delegate { ResetCrystalMove(); });
            cells[cell.x, cell.y].isCrystalMove = true;
            cells[cell.x, cell.y].gameField = this;
        }

       
        
	}

    void Reset()
    {
        foreach (Cell cell in cells)
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
            if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x - 2, y].crystal.type == target.crystal.type && cells[x - 1, y] != target)
                return true;
        if (x > 0 && x < 7)
            if (cells[x - 1, y].crystal.type == target.crystal.type && cells[x + 1, y].crystal.type == target.crystal.type && cells[x - 1, y] != target && cells[x + 1, y] != target)
                return true;
        if (x < 6)
            if (cells[x + 1, y].crystal.type == target.crystal.type && cells[x + 2, y].crystal.type == target.crystal.type && cells[x + 1, y] != target)
                return true;

        if (y > 1)
            if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y - 2].crystal.type == target.crystal.type && cells[x, y - 1] != target)
                return true;
        if (y > 0 && y < 7)
            if (cells[x, y - 1].crystal.type == target.crystal.type && cells[x, y + 1].crystal.type == target.crystal.type && cells[x, y - 1] != target && cells[x, y + 1] != target)
                return true;
        if (y < 6)
            if (cells[x, y + 1].crystal.type == target.crystal.type && cells[x, y + 2].crystal.type == target.crystal.type && cells[x, y + 1] != target)
                return true;

        return false;
    }

    /// <summary>
    /// Проверка на свободное место
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Положение пустого места</returns>
    public Cell GetEmptyCell(int x, int y)
    {
        if (y + 1 > 7) return cells[x, y];
        if (cells[x, y + 1].crystal == null)
        {
            return GetEmptyCell(x, y + 1);
        }
        else
        {
            return cells[x, y];
        }
    }

    /// <summary>
    /// Выводит список клеток в комбинации
    /// </summary>
    /// <param name="x">Х</param>
    /// <param name="y">Y</param>
    /// <returns>Список клеток</returns>
    public List<Cell> GetAdjacentCells(int x, int y)
    {
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
                if (cells[x - 1, y].crystal.type == cells[x,y].crystal.type && cells[x + 1, y].crystal.type == cells[x,y].crystal.type && !cells[x - 1, y].isCrystalMove && !cells[x + 1, y].isCrystalMove)
                {
                    if (!cells[x, y].isAddInList)
                    {
                        cells[x, y].isAddInList = true;
                        nearbyCells.Add(cells[x, y]);
                    }
                    if (!cells[x - 1, y].isAddInList)
                    {
                        nearbyCells.Add(cells[x - 1, y]);
                    }
                    if (!cells[x + 1, y].isAddInList)
                    {
                        nearbyCells.Add(cells[x + 1, y]);
                    }
                    if (!cells[x - 1, y].isChecked)
                    {
                        nearbyCells.AddRange(GetAdjacentCells(x-1,y));
                    }
                    if (!cells[x + 1, y].isChecked)
                    {
                        nearbyCells.AddRange(GetAdjacentCells(x + 1, y));
                    }
                }
                else
                {
                    if (cells[x - 1, y].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x - 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y));
                        }
                    }
                    if (cells[x + 1, y].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x + 1, y].isChecked)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y));
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
                            nearbyCells.AddRange(GetAdjacentCells(x - 1, y));
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
                            nearbyCells.AddRange(GetAdjacentCells(x + 1, y));
                        }
                    }
                }
            }
        }
        if (y > 0 && y < 7)
        {
            if ((cells[x, y - 1].crystal != null && cells[x, y + 1].crystal != null && !cells[x, y + 1].isCrystalMove && !cells[x, y - 1].isCrystalMove))
            {
                if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type && cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                {
                    if (!cells[x, y].isAddInList)
                    {
                        cells[x, y].isAddInList = true;
                        nearbyCells.Add(cells[x, y]);
                    }
                    if (!cells[x, y - 1].isAddInList)
                    {
                        nearbyCells.Add(cells[x, y - 1]);
                    }
                    if (!cells[x, y + 1].isAddInList)
                    {
                        nearbyCells.Add(cells[x, y + 1]);
                    }
                    if (!cells[x, y - 1].isChecked)
                    {

                        nearbyCells.AddRange(GetAdjacentCells(x, y - 1));
                    }
                    if (!cells[x, y + 1].isChecked)
                    {

                        nearbyCells.AddRange(GetAdjacentCells(x, y + 1));
                    }
                }
                else
                {
                    if (cells[x, y - 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y - 1].isChecked && !cells[x, y - 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1));
                        }
                    }
                    if (cells[x, y + 1].crystal.type == cells[x, y].crystal.type)
                    {
                        if (!cells[x, y + 1].isChecked && !cells[x, y + 1].isCrystalMove)
                        {
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1));
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
                            nearbyCells.AddRange(GetAdjacentCells(x, y - 1));
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
                            nearbyCells.AddRange(GetAdjacentCells(x, y + 1));
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

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (!cells[i, j].isChecked && !cells[i, j].isCrystalMove)
                    nearbyCells.AddRange(GetAdjacentCells(i,j));
            }
        }

        foreach (Cell destroyCell in nearbyCells)
        {
            Destroy(destroyCell.crystal.gameObject);
            destroyCell.crystal = null;
        }
        Reset();
        //ResetCrystalMove();
        //MoveCrystals();

    }

    /// <summary>
    /// Генерация массива с цифровыми кодами цветов ячеек
    /// </summary>
    private int GenerateColor(int x, int y)
    {
        List<int> colorList = new List<int> { 0, 1, 2, 3 };
        if ((x > 1) && (cells[x - 1, y].crystal.colorID == cells[x - 2, y].crystal.colorID))
        {
            colorList.Remove(cells[x - 1, y].crystal.colorID);
        }
        if ((y > 1) && (cells[x, y - 1].crystal.colorID == cells[x, y - 2].crystal.colorID))
        {
            colorList.Remove(cells[x, y - 1].crystal.colorID);
        }
        int newColor = Random.Range(0, colorList.Count);
        return colorList[newColor];
    }
}
