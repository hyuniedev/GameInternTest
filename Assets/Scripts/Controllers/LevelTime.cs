using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;

    public override void Setup(float value, Text txt, GameManager mngr)
    {
        base.Setup(value, txt, mngr);

        m_mngr = mngr;

        m_time = value;

        UpdateText();

        StartCoroutine(DecTime());
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        UpdateText();

        if (m_time <= 0)
        {
            OnConditionComplete();
        }
    }
    private IEnumerator DecTime()
    {
        while(m_time > 0)
        {
            yield return new WaitForSeconds(1f);
            m_time--;
        }
    }
    protected override void UpdateText()
    {
        Debug.Log(m_time);
        if (m_time < 0f) return;

        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }
}
