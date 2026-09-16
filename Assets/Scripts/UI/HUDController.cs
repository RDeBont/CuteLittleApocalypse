using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthBarImage;
    [Tooltip("Op volgorde: hp_0 (leeg), hp_1, hp_2, hp_3 (vol)")]
    [SerializeField] private Sprite[] healthFrames = new Sprite[4];

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Color ammoColor = new Color32(0xF2, 0xB1, 0x38, 0xFF);      // #F2B138
    [SerializeField] private Color ammoEmptyColor = new Color32(0xC9, 0x3B, 0x3B, 0xFF); // #C93B3B

    private void Start()
    {
        // Startwaarden tot PlayerHealth/WeaponController gekoppeld zijn
        SetHealth(3, 3);
        SetAmmo(14, 14);
    }

    /// <summary>Aanroepen vanuit PlayerHealth (CLA-28).</summary>
    public void SetHealth(int current, int max)
    {
        if (healthBarImage == null || healthFrames == null || healthFrames.Length == 0) return;

        // Rekent HP om naar een frame: 3/3 -> hp_3, 0/3 -> hp_0
        int lastFrame = healthFrames.Length - 1;
        float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
        int index = Mathf.CeilToInt(ratio * lastFrame);

        healthBarImage.sprite = healthFrames[index];
    }

    /// <summary>Aanroepen vanuit WeaponController (CLA-20).</summary>
    public void SetAmmo(int current, int max)
    {
        if (ammoText == null) return;

        current = Mathf.Max(0, current);
        ammoText.text = $"{current:D2} / {max:D2}";
        ammoText.color = current == 0 ? ammoEmptyColor : ammoColor;
    }
}