using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image healthBarSprite;

    public void UpdateHealthBar(float hp, float maxHp){
        healthBarSprite.fillAmount = hp / maxHp;
    }
}
