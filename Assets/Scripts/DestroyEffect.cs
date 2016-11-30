using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
public class DestroyEffect : MonoBehaviour {

    public List<ElementDestroyEffect> destroysSprite = new List<ElementDestroyEffect>();
    private float timer = 0.5f;
    private bool active = false;
    private GameObject crystal;
    public Cell cell;
    private bool isDestroy = true;
    public void Activate(GameObject _object, bool _isDestroy)
    {
        if (!active)
        {
            crystal = _object;
            isDestroy = _isDestroy;
            foreach (ElementDestroyEffect destObject in destroysSprite)
            {
                destObject.sprite.transform.DOLocalMove(destObject.position, 0.5f).SetEase(Ease.Flash);
                destObject.sprite.transform.DOScale(destObject.scale, 0.5f).SetEase(Ease.Flash);
                destObject.sprite.DOColor(destObject.endColor, 0.5f).SetEase(Ease.Flash);
                destObject.sprite.transform.DOLocalRotate(destObject.endRotation, 0.5f).SetEase(Ease.Flash);
            }
            active = true;
        }
    }

    void Update()
    {
        if (active)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                active = false;
                timer = 0.5f;
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
                }
            }
        }
    }
}

[System.Serializable]
public struct ElementDestroyEffect
{
    public SpriteRenderer sprite;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 startRotation;
    public Vector3 endRotation;
    public Color startColor;
    public Color endColor;
}