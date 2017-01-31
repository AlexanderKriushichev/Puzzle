using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
public class ProgressScore : MonoBehaviour {
    [SerializeField]
    private Image background;

    [SerializeField]
    private RectTransform firstStar;
    [SerializeField]
    private Vector2 sizeStartFirstStar;
    [SerializeField]
    private Vector2 sizeEndFirstStar;
    private bool firstStarShow;

    [SerializeField]
    private RectTransform secondStar;
    [SerializeField]
    private Vector2 sizeStartSecondStar;
    [SerializeField]
    private Vector2 sizeEndSecondStar;
    private bool secondStarShow;

    [SerializeField]
    private RectTransform thirdStar;
    [SerializeField]
    private Vector2 sizeStartThirdStar;
    [SerializeField]
    private Vector2 sizeEndThirdStar;
    private bool thirdStarShow;

    public void SetScore(float score)
    {
        background.fillAmount = score;

        if (score >= 0.325f && !firstStarShow)
        {
            firstStar.DOSizeDelta(sizeStartFirstStar, 1).OnComplete(() => firstStar.DOSizeDelta(sizeEndFirstStar, 1));
            firstStarShow = true;
        }

        if (score >= 0.665 && !secondStarShow)
        {
            secondStar.DOSizeDelta(sizeStartSecondStar, 1).OnComplete(() => secondStar.DOSizeDelta(sizeEndSecondStar, 1));
            secondStarShow = true;
        }

        if (score >= 1 && !thirdStarShow)
        {
            thirdStar.DOSizeDelta(sizeStartThirdStar, 1).OnComplete(() => thirdStar.DOSizeDelta(sizeEndThirdStar, 1));
            thirdStarShow = true;
        }
    }

 
}
