using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineTraceEffect : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private TypeLineBonus typeLineBonus;

    public void Move(TypeLineBonus type)
    {
        transform.localPosition = new Vector3(0, -0.5f, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        typeLineBonus = type;
        MoveForwardUp();
    }

    void MoveForwardUp()
    {
        spriteRenderer.sortingOrder = 1;

        switch (typeLineBonus)
        {
            case TypeLineBonus.Horizontal:
                {
                    transform.DOLocalMoveX(0.5f, 0.1f).SetEase(Ease.Linear).OnComplete(MoveBackUp);
                    transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InSine);
                    transform.DOLocalMoveZ(0, 0.1f).SetEase(Ease.InSine);
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.InSine).OnComplete(MoveBackUp);
                    transform.DOLocalMoveY(0.5f, 0.1f).SetEase(Ease.Linear);
                    transform.DOLocalMoveZ(0, 0.1f).SetEase(Ease.InSine);
                    break;
                }
        }
        
    }

    void MoveBackUp()
    {
        spriteRenderer.sortingOrder = -1;

        switch (typeLineBonus)
        {
            case TypeLineBonus.Horizontal:
                {
                    transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.Linear).OnComplete(MoveBackDown);
                    transform.DOLocalMoveY(0.1f, 0.1f).SetEase(Ease.OutSine);
                    transform.DOLocalMoveZ(0.5f, 0.1f).SetEase(Ease.OutSine);
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    transform.DOLocalMoveX(0.1f, 0.1f).SetEase(Ease.OutSine).OnComplete(MoveBackDown);
                    transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.Linear);
                    transform.DOLocalMoveZ(0.5f, 0.1f).SetEase(Ease.OutSine);
                    break;
                }
        }
        

    }

    void MoveBackDown()
    {
        spriteRenderer.sortingOrder = -1;

        switch (typeLineBonus)
        {
            case TypeLineBonus.Horizontal:
                {
                    transform.DOLocalMoveX(-0.5f, 0.1f).SetEase(Ease.Linear).OnComplete(MoveForwardDown);
                    transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InSine);
                    transform.DOLocalMoveZ(0, 0.1f).SetEase(Ease.InSine);
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.InSine).OnComplete(MoveForwardDown);
                    transform.DOLocalMoveY(-0.5f, 0.1f).SetEase(Ease.Linear);
                    transform.DOLocalMoveZ(0, 0.1f).SetEase(Ease.InSine);
                    break;
                }
        }


    }

    void MoveForwardDown()
    {
        spriteRenderer.sortingOrder = 1;

        switch (typeLineBonus)
        {
            case TypeLineBonus.Horizontal:
                {
                    transform.DOLocalMoveX(0, 0.1f).SetEase(Ease.Linear).OnComplete(MoveForwardUp);
                    transform.DOLocalMoveY(-0.1f, 0.1f).SetEase(Ease.OutSine);
                    transform.DOLocalMoveZ(-0.5f, 0.1f).SetEase(Ease.OutSine);
                    break;
                }
            case TypeLineBonus.Verical:
                {
                    transform.DOLocalMoveX(-0.1f, 0.1f).SetEase(Ease.OutSine).OnComplete(MoveForwardUp);
                    transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.Linear);
                    transform.DOLocalMoveZ(-0.5f, 0.1f).SetEase(Ease.OutSine);
                    break;
                }
        }


    }
}
