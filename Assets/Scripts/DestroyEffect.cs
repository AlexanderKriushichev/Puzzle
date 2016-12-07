using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
public class DestroyEffect : MonoBehaviour
{

    public List<ElementDestroyEffect> destroysSprite = new List<ElementDestroyEffect>();
    public TextMesh text;
    private float timer = 0.5f;
    private bool active = false;
    private bool complite = false;
    private GameObject crystal;
    public Cell cell;
    private bool isDestroy = true;
    private int score;
    public void Activate(GameObject _object, bool _isDestroy, int countScore)
    {
        if (!active)
        {
            crystal = _object;
            isDestroy = _isDestroy;
            score = countScore;
            for (int i = 0; i < destroysSprite.Count;i++ )
            {
                destroysSprite[i].sprite.transform.DOLocalMove(destroysSprite[i].position, 0.5f).SetEase(Ease.Flash);
                destroysSprite[i].sprite.transform.DOScale(destroysSprite[i].scale, 0.5f).SetEase(Ease.Flash);
                destroysSprite[i].sprite.DOColor(destroysSprite[i].endColor, 0.5f).SetEase(Ease.Flash);
                destroysSprite[i].sprite.transform.DOLocalRotate(destroysSprite[i].endRotation, 0.5f).SetEase(Ease.Flash).OnComplete(destroysSprite[i].Complite);
            }
            active = true;
        }
    }

    public void UpdateDestroy()
    {
        if (active)
        {
            bool objComplite = true;
            foreach (ElementDestroyEffect obj in destroysSprite)
            {
                if (!obj.complite)
                    objComplite = false;
            }
            if (objComplite)
            {
                foreach (ElementDestroyEffect obj in destroysSprite)
                {
                    obj.complite = false;
                }
                foreach (ElementDestroyEffect destObject in destroysSprite)
                {
                    destObject.sprite.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.InFlash);
                    destObject.sprite.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InFlash);
                    destObject.sprite.DOColor(destObject.startColor, 0.25f).SetEase(Ease.InFlash);
                    destObject.sprite.transform.DOLocalRotate(destObject.startRotation, 0.25f).SetEase(Ease.InFlash);
                }
                if (isDestroy)
                {
                    cell.isCrystalMove = false;
                    Destroy(crystal);
                    cell.cellInCombination.Clear();
                    cell.crystal = null;
                    cell.isCrystalIn = false;
                    if (score != 0)
                    {
                        text.text = "+" + score;
                        ScoreManager.AddScore(score);
                        DOTween.Kill(text);
                        text.transform.localScale = Vector3.one;
                        text.transform.DOLocalMove(new Vector3(0, 0.5f, 1), 1).OnComplete(delegate { text.transform.localScale = Vector3.zero; text.transform.localPosition = Vector3.zero; });
                    }
                }
                active = false;
                cell.gameField.Update();
            }
        }
    }
}

[System.Serializable]
public class ElementDestroyEffect
{
    public SpriteRenderer sprite;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 startRotation;
    public Vector3 endRotation;
    public Color startColor;
    public Color endColor;
    public bool complite;

    public void Complite()
    {
        complite = true;
    }
}