using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour {

    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text comboNumberText;
    [SerializeField] private List<Color> comboColors;

    public void ComboTextAnimation(int combo) {
        gameObject.SetActive(true);

        if(combo > 1) {
            if(combo - 2 >= comboColors.Count) {
                comboNumberText.color = comboColors[comboColors.Count - 1];
            } else {
                comboNumberText.color = comboColors[combo - 2]; // combo - 2 : bcoz combo text show will always start from 2
            }
            comboNumberText.text = combo.ToString();
            comboNumberText.gameObject.SetActive(true);
        } else {
            comboNumberText.gameObject.SetActive(false);
        }

        comboText.gameObject.SetActive(true);

        transform.DOScale(Vector3.one, .25f)
            .SetEase(Ease.OutExpo)
            .SetUpdate(true)
            .OnComplete(() => {
                StartCoroutine(OnComboShown());
            });
    }

    private IEnumerator OnComboShown() {
        yield return new WaitForSecondsRealtime(.5f);
        transform.DOScale(Vector3.zero, .25f).SetEase(Ease.InExpo).SetUpdate(true).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

}