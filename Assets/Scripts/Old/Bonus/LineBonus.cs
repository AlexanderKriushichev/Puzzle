using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class LineBonus : Bonus {
    private float timer = 0.1f;
    private float downTimer = 0;

    private int index = 1;
    private int indexEffectPosition = 0;

    private bool destroyLine = false;
    private bool startEffect = false;

    private GameObject lineEffect;
    private LineEffectComplite lineEffectComplite;
    private Sprite lineSprite;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    private List<Vector3> effectPositionHorizontal = new List<Vector3> 
    { 
        new Vector3(0, 0.1f, -0.6f),
        new Vector3(0.5f,0,-0.1f),
        new Vector3(0,-0.1f,0.4f),
        new Vector3(-0.5f, 0, -0.1f)
    };

    private List<Vector3> effectPositionVertical = new List<Vector3>
    {
        new Vector3(0.1f, 0, -0.6f),
        new Vector3(0,0.5f,-0.1f),
        new Vector3(-0.1f,0,0.4f),
        new Vector3(0, -0.5f, -0.1f)
    };

    public void Line(TypeLineBonus _type, Field _field, Crystal _crystal)
    {
        type = _type;
        field = _field;
        crystal = _crystal;
    }

    public void SetEffect(GameObject _object, Sprite _sprite)
    {
        lineEffect = _object;
        spriteRenderer = lineEffect.GetComponent<SpriteRenderer>();
        trailRenderer = lineEffect.GetComponent<TrailRenderer>();
        lineEffect.transform.parent = crystal.transform;
        lineEffectComplite = new LineEffectComplite();
        lineEffect.transform.localPosition = new Vector3(-0.5f, 0, -0.1f);
        startEffect = true;
        lineSprite = _sprite;
    }

    public void OffAnim()
    {
        spriteRenderer.enabled = false;
        trailRenderer.enabled = false;
    }

    void OnDestroy()
    {
        Destroy(lineEffect);
    }

    public override void Acivate()
    {
        bounceStart = true;
        startEffect = false;

        lineEffect.transform.DOKill();
        trailRenderer.enabled = false;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = lineSprite;
        spriteRenderer.color = Color.white;
        lineEffect.transform.localScale = Vector3.forward;
        lineEffect.transform.localPosition = Vector3.zero;

        switch (type)
        {
            case TypeLineBonus.Horizontal:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[i, crystal.cell.y].isCrystalMove = true;
                        lineEffect.transform.localEulerAngles = Vector3.zero;
                    }
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[crystal.cell.x, i].isCrystalMove = true;
                        lineEffect.transform.localEulerAngles = new Vector3(0, 0, 90);
                    }
                    break;
                }
        }

        ActiveLazer();
        
    }

    void ActiveLazer()
    {
        float speed = 0.7f;

        switch (type)
        {
            case TypeLineBonus.Horizontal:
                {
                    if (Field.size / 2 > crystal.cell.x)
                    {
                        speed = Mathf.Abs((crystal.cell.x - Field.size / 2) + 1);
                    }
                    else
                    {
                        speed = (crystal.cell.x - Field.size / 2);
                    }
                    speed = 0.7f - speed / 10;
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    if (Field.size / 2 > crystal.cell.y)
                    {
                        speed = Mathf.Abs((crystal.cell.y - Field.size / 2) + 1);
                    }
                    else
                    {
                        speed = (crystal.cell.y - Field.size / 2);
                    }
                    speed = 0.7f - speed / 10;
                    break;
                }
        }

        lineEffect.transform.DOScale(new Vector3(10, 0.1f, 1), speed/1.5f).OnComplete(DestroyCells);

    }

    void DestroyCells()
    {
        bounceComplite = true;
        switch (type)
        {
            case TypeLineBonus.Horizontal:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[i, crystal.cell.y].DestroyCrystal();
                    }
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[crystal.cell.x, i].DestroyCrystal();
                    }
                    break;
                }
        }
    }

    static public void DestroyCell(int x, int y, TypeLineBonus typeLine)
    {
        Field field = BonusManager.GetField();
        GameObject lizer = (GameObject)Instantiate(BonusManager.GetLizerPrefab(), field.cells[x, y].transform.position, field.cells[x, y].transform.rotation);
        float speed = 0.7f;

        switch (typeLine)
        {
            case TypeLineBonus.Horizontal:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        lizer.transform.localEulerAngles = Vector3.zero;
                    }

                    if (Field.size / 2 > x)
                    {
                        speed = Mathf.Abs((x - Field.size / 2) + 1);
                    }
                    else
                    {
                        speed = (x - Field.size / 2);
                    }
                    speed = 0.7f - speed / 10;
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        lizer.transform.localEulerAngles = new Vector3(0, 0, 90);
                    }

                    if (Field.size / 2 > y)
                    {
                        speed = Mathf.Abs((y - Field.size / 2) + 1);
                    }
                    else
                    {
                        speed = (y - Field.size / 2);
                    }
                    speed = 0.7f - speed / 10;
                    break;
                }
        }

        lizer.transform.DOScale(new Vector3(10, 0.1f, 1), speed / 1.5f).OnComplete(delegate { DestroyCellInLine(x, y, typeLine); Destroy(lizer); });

    }

    static private void DestroyCellInLine(int x, int y, TypeLineBonus typeLine)
    {
        Field field = BonusManager.GetField();

        switch (typeLine)
        {
            case TypeLineBonus.Horizontal:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[i, y].DestroyCrystal();
                    }
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    for (int i = 0; i < Field.size; i++)
                    {
                        field.cells[x, i].DestroyCrystal();
                    }
                    break;
                }
        }
    }

    void Update()
    {
        if (startEffect)
        {
            if (lineEffectComplite.Check())
            {
                if (type == TypeLineBonus.Horizontal)
                {
                    lineEffect.transform.DOLocalMoveX(effectPositionHorizontal[indexEffectPosition].x, 0.1f).SetEase(Ease.Linear).OnComplete(() => lineEffectComplite.x = true);
                    if (indexEffectPosition % 2 == 0)
                        lineEffect.transform.DOLocalMoveY(effectPositionHorizontal[indexEffectPosition].y, 0.1f).SetEase(Ease.OutSine).OnComplete(() => lineEffectComplite.y = true);
                    else
                        lineEffect.transform.DOLocalMoveY(effectPositionHorizontal[indexEffectPosition].y, 0.1f).SetEase(Ease.InSine).OnComplete(() => lineEffectComplite.y = true);
                    lineEffect.transform.DOLocalMoveZ(effectPositionHorizontal[indexEffectPosition].z, 0.1f).SetEase(Ease.Linear).OnComplete(() => lineEffectComplite.z = true);
                    lineEffectComplite.Reset();
                    if (indexEffectPosition != effectPositionHorizontal.Count - 1)
                        indexEffectPosition++;
                    else
                        indexEffectPosition = 0;
                }
                else
                {
                    lineEffect.transform.DOLocalMoveX(effectPositionVertical[indexEffectPosition].x, 0.1f).SetEase(Ease.Linear).OnComplete(() => lineEffectComplite.x = true);
                    if (indexEffectPosition % 2 == 0)
                        lineEffect.transform.DOLocalMoveY(effectPositionVertical[indexEffectPosition].y, 0.1f).SetEase(Ease.InSine).OnComplete(() => lineEffectComplite.y = true);
                    else
                        lineEffect.transform.DOLocalMoveY(effectPositionVertical[indexEffectPosition].y, 0.1f).SetEase(Ease.OutSine).OnComplete(() => lineEffectComplite.y = true);
                    lineEffect.transform.DOLocalMoveZ(effectPositionVertical[indexEffectPosition].z, 0.1f).SetEase(Ease.Linear).OnComplete(() => lineEffectComplite.z = true);
                    lineEffectComplite.Reset();
                    if (indexEffectPosition != effectPositionVertical.Count - 1)
                        indexEffectPosition++;
                    else
                        indexEffectPosition = 0;
                }
            }
        }
    }

}

public class LineEffectComplite
{
    public bool x;
    public bool y;
    public bool z;

    public LineEffectComplite()
    {
        x = true;
        y = true;
        z = true;
    }

    public bool Check()
    {
        return z && x && y;
    }

    public void Reset()
    {
        x = false;
        y = false;
        z = false;
    }
}

public enum TypeLineBonus {Horizontal, Verical}