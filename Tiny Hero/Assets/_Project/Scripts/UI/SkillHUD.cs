using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

//Bug
[System.Serializable]
public class SkillUI
{
    public Image activeIcon;
    public Image inactiveIcon;
    public TMPro.TextMeshProUGUI textTimer;
    public ISkill skill;
    public CountdownTimer timer;
}
public class SkillHUD : MonoBehaviour
{
    public SkillUI skillQ;
    public SkillUI skillE;
    public SkillUI skillR;

    public void InitSkillUI()
    {
        foreach (var skillUI in new[] { skillQ, skillE, skillR })
        {
            if (skillUI.inactiveIcon != null)
            {
                skillUI.inactiveIcon.fillAmount = 0f;
            }
            if(skillUI.textTimer != null)
            {
                skillUI.textTimer.enabled = false;
            }
        }
    }

    public void SetSkillCooldown(SkillUI skillUI)
    {
        skillUI.inactiveIcon.fillAmount = 1;
    }

    public IEnumerator UpdateCooldownUI(SkillUI skillUI)
    {
        skillUI.textTimer.enabled = true;
        skillUI.inactiveIcon.fillAmount = 1;

        while (skillUI.timer.IsRunning)
        {
            skillUI.inactiveIcon.fillAmount = skillUI.timer.progress;
            skillUI.textTimer.text = skillUI.timer.Time.ToString("F1"); //Get time in seconds with one decimal place
            yield return null;
        }
        skillUI.inactiveIcon.fillAmount = 0;
        skillUI.textTimer.enabled = false;
    }
}
