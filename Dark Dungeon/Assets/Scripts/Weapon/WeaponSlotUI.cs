using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image iconImage;
    public GameObject highlight;
    public Sprite emptyIcon;

    public void SetWeaponIcon(Sprite icon)
    {
        if (icon != null)
        {
            iconImage.sprite = icon;
        }
        else
        {
            iconImage.sprite = emptyIcon;
            // iconImage.enabled = true;
        }

        iconImage.enabled = true;
        iconImage.gameObject.SetActive(true);
    }

    public void SetHighlight(bool isActive)
    {
        highlight.SetActive(isActive);
    }
}
