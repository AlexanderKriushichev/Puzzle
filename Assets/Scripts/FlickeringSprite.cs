using UnityEngine;
using DG.Tweening;

public class FlickeringSprite : MonoBehaviour {

    public float speed;

    public Ease ease;
    public Ease easeBack;

    public bool useCustomCurve = false;

    public AnimationCurve customCurve;
    public AnimationCurve customCurveBack;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    public void SetFlickering(float _speed, Ease _ease, Ease _easeBack)
    {
        speed = _speed;
        ease = _ease;
        easeBack = _easeBack;

        useCustomCurve = false;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Flickering();
    }

    [ContextMenu("Flickering")]
    void Flickering()
    {
        float a = spriteRenderer.color.a;
        if (useCustomCurve)
        {
            DOTween.To(() => a, x => spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, x), 0, 1 / speed).SetEase(customCurve).OnComplete(BackFlickering);
        }
        else
        {
            DOTween.To(() => a, x => spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, x), 0, 1 / speed).SetEase(ease).OnComplete(BackFlickering);
        }
    }

    void BackFlickering()
    {
        float a = spriteRenderer.color.a;
        if (useCustomCurve)
        {
            DOTween.To(() => a, x => spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, x), 1, 1 / speed).SetEase(customCurveBack).OnComplete(Flickering);
        }
        else
        {
            DOTween.To(() => a, x => spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, x), 1, 1 / speed).SetEase(easeBack).OnComplete(Flickering);
        }
    }
}
