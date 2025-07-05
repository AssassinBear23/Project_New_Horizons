using UnityEngine;
using System.Collections.Generic;
public class SkinSelector : MonoBehaviour
{
    private int index = 0;
    private int selectedSkin = 0;
    [SerializeField] private List<Transform> skins;
    [SerializeField] private Transform checkMark;
    private void Start()
    {
        index = PlayerPrefs.GetInt("SelectedSkin", 0);
        selectedSkin = index;
        ShowSkin();
    }
    private void ShowSkin()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            if (i == index) skins[i].gameObject.SetActive(true);
            else skins[i].gameObject.SetActive(false);
        }
        checkMark.gameObject.SetActive(true);
    }
    public void NextSkin()
    {
        // Disable current skin
        skins[index].gameObject.SetActive(false);

        // Go to next skin
        index++;
        if (index >= skins.Count) index = 0;
        skins[index].gameObject.SetActive(true);

        // Check if this skin is equipped
        if (index == selectedSkin) checkMark.gameObject.SetActive(true);
        else checkMark.gameObject.SetActive(false);
    }
    public void PreviousSkin()
    {
        // Disable current skin
        skins[index].gameObject.SetActive(false);

        // Go to previous skin
        index--;
        if (index < 0) index = skins.Count - 1;
        skins[index].gameObject.SetActive(true);

        // Check if this skin is equipped
        if (index == selectedSkin) checkMark.gameObject.SetActive(true);
        else checkMark.gameObject.SetActive(false);
    }
    public void SelectSkin()
    {
        PlayerPrefs.SetInt("SelectedSkin", index);
        selectedSkin = index;

        checkMark.gameObject.SetActive(true);
    }
}