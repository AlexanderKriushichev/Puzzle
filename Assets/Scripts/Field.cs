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

	// Update is called once per frame
	void Update () {

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
                    nearbyCells.AddRange(cells[i, j].CheckAdjacentCells());
            }
        }

        foreach (Cell destroyCell in nearbyCells)
        {
            Destroy(destroyCell.crystal.gameObject);
        }

        Reset();
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
