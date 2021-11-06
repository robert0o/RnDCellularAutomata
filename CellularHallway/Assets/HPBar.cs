using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public RectTransform Hpbar;
    public HpValue hp;
    Vector2 basePosition;
    Vector3 baseScale;
    void Start()
    {
        basePosition = Hpbar.anchoredPosition;
        baseScale = Hpbar.localScale;
    }

    void Update()
    {
        Vector2 pos = new Vector2((basePosition.x * hp.HpPercentage) * .5f + 5, basePosition.x);
        Vector3 scal = baseScale;
        scal.x = baseScale.x * hp.HpPercentage;
    }
}
