using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Inspector
    public Image iconSwipe;
    // Inspector

    private TMPro.TextMeshProUGUI textSwipe;


    void Start()
    {
        textSwipe = iconSwipe.transform.GetChild(0)
            .GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void ShowSwipeIcon()
    {
        iconSwipe.DOFade(1, 0.5f);
        textSwipe.DOFade(1, 0.5f);
    }

    public void HideSwipeIcon()
    {
        iconSwipe.DOFade(0, 1.2f);
        textSwipe.DOFade(0, 1.2f);
    }
}
