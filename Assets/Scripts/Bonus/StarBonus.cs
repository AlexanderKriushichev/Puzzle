using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class StarBonus : Bonus {

    private Transform firstArial;
    private Transform secondArial;
    private SpriteRenderer spriteRenderer;

    private bool animLoopComplite = true;
    private bool halfLoop = true;

    private GameObject line;

    private TypeOfCrystal typeOfCrystal;

    private List<Cell> cellsToDestroy = new List<Cell>();

    private bool compliteMove = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Star(Field _field, Crystal _crystal)
    {
        field = _field;
        crystal = _crystal;
        typeOfCrystal = crystal.type;
    }

    public void SetType(TypeOfCrystal type)
    {
        typeOfCrystal = type;
    }

    public void SetEffect(GameObject _object, GameObject _arial, GameObject _arial1, Color firstColor, Color secondColor)
    {
        line = _object;
        firstArial = _arial.transform;
        secondArial = _arial1.transform;
        firstArial.parent = this.transform;
        secondArial.parent = this.transform;
        firstArial.GetComponent<SpriteRenderer>().color = firstColor;
        secondArial.GetComponent<SpriteRenderer>().color = secondColor;
    }

    public override void Acivate()
    {
        bounceStart = true;
        spriteRenderer.sortingOrder = 2;
        foreach (Cell cell in field.cells)
        {
            if (cell.crystal != null)
            {
                if (cell.crystal.type == typeOfCrystal)
                {
                    cellsToDestroy.Add(cell);
                    GameObject initLine = (GameObject)Instantiate(line, transform.position, transform.rotation);
                    initLine.GetComponent<TrailRenderer>().sortingOrder = 1;
                    initLine.transform.DOMove(cell.transform.position, 0.5f).OnComplete(delegate { DestroyCells(); Destroy(initLine); });
                }
            }
        }
    }

    void DestroyCells()
    {
        if (!compliteMove)
        {
            compliteMove = true;
            foreach (Cell cell in cellsToDestroy)
            {
                cell.DestroyCrystal();
            }
            bounceComplite = true;
            crystal.cell.DestroyCrystal();
        }
    }

    void Update()
    {
        if (animLoopComplite)
        {
            if (halfLoop)
            {
                animLoopComplite = false;
                halfLoop = false;
                firstArial.DOScale(new Vector3(0.5f, 0.5f, 1), 2).SetEase(Ease.Linear);
                firstArial.DORotate(firstArial.localEulerAngles + new Vector3(0, 0, 90), 2).SetEase(Ease.Linear);
                secondArial.DOScale(new Vector3(1, 1, 1), 2).SetEase(Ease.Linear);
                secondArial.DORotate(secondArial.localEulerAngles - new Vector3(0, 0, 90), 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    animLoopComplite = true;
                });
            }
            else
            {
                animLoopComplite = false;
                halfLoop = true;
                firstArial.DOScale(new Vector3(1, 1, 1), 2).SetEase(Ease.Linear);
                firstArial.DORotate(firstArial.localEulerAngles + new Vector3(0, 0, 90), 2).SetEase(Ease.Linear);
                secondArial.DOScale(new Vector3(0.5f, 0.5f, 1), 2).SetEase(Ease.Linear);
                secondArial.DORotate(secondArial.localEulerAngles - new Vector3(0, 0, 90), 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    animLoopComplite = true;
                });
            }
        }
    }
}
