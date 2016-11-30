using UnityEngine;
using System.Collections;
using DG.Tweening;
public class BoxBonus : Bonus
{
    private GameObject area;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer crystalSpriteRenderer;
    private bool scaleComplite = true;
    private bool startEffect = true;

    private bool firstActive = false;
    private int areaSize = 1;

    private float timer = 0.5f;
    public int AreaSize
    {
        get { return areaSize; }
        set { areaSize = value; }
    }
    void Start()
    {
        crystalSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnDestroy()
    {
        Destroy(area);
    }



    public void Box(Field _field, Crystal _crystal)
    {
        field = _field;
        crystal = _crystal;
    }

    public void SetEffect(GameObject _object, Color _color)
    {
        area = _object;
        spriteRenderer = area.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = _color;
        area.transform.parent = crystal.transform;
        area.transform.localPosition = Vector3.zero;
        scaleComplite = true;
    }

    public override void Acivate()
    {
        if (firstActive)
            return;
        bounceStart = true;
        startEffect = false;
        area.transform.DOKill();
        area.transform.DOScale(new Vector3(6 * AreaSize, 6 * AreaSize, 1), 1).SetEase(Ease.InSine).OnComplete(DestroyCells);
        for (int i = crystal.cell.x - AreaSize; i <= crystal.cell.x + AreaSize; i++)
        {
            for (int j = crystal.cell.y - AreaSize; j <= crystal.cell.y + AreaSize; j++)
            {
                if (i >= 0 && i < Field.size && j >= 0 && j < Field.size)
                {
                    field.cells[i, j].isCrystalMove = true;
                }
            }
        }
    }

    void DestroyCells()
    {
        firstActive = true;
        for (int i = crystal.cell.x - AreaSize; i <= crystal.cell.x + AreaSize; i++)
        {
            for (int j = crystal.cell.y - AreaSize; j <= crystal.cell.y + AreaSize; j++)
            {
                if (i >= 0 && i < Field.size && j >= 0 && j < Field.size)
                {
                    field.cells[i,j].DestroyCrystal();
                }
            }
        }
        timer = 0.5f;
        area.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        startEffect = true;
        scaleComplite = true;
        field.Update();
    }
    // Update is called once per frame
    void Update()
    {
        if (startEffect)
        {
            if (scaleComplite)
            {
                if (firstActive)
                {
                    crystalSpriteRenderer.DOColor(new Color(1, 1, 1, 0.5f), 1).OnComplete(() => crystalSpriteRenderer.DOColor(Color.white, 1));
                }

                scaleComplite = false;
                area.transform.DOScale(new Vector3(1.7f, 1.7f, 1), 1).SetEase(Ease.InSine).OnComplete(() => area.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 1).SetEase(Ease.InOutSine).OnComplete(() => scaleComplite = true));
            }
        }
        
        if (firstActive )
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                bounceStart = false;
                field.Update();
                field.Update();

                if (field.CheckMove() && !field.inRotate && field.moveCrystals.Count == 0 && field.combinations.Count == 0)
                {
                    firstActive = false;
                    Acivate();
                    bounceComplite = true;
                }
            }
        }
    }
}
