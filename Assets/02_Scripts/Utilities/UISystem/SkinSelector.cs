using UnityEngine;
using System.Collections.Generic;
public class SkinSelector : MonoBehaviour
{
    private int index = 0;
    private int selectedSkin = 0;
    [SerializeField] private List<SpriteRenderer> skins;
    [SerializeField] private SpriteRenderer checkMark;
    private void Awake()
    {
        PlayerPrefs.SetInt("SelectedSkin", 0);
    }
    private void Start()
    {
        index = PlayerPrefs.GetInt("SelectedSkin");
        selectedSkin = index;
        ShowSkin();
    }
    private void ShowSkin()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            if (i == index) skins[i].enabled = true;
            else skins[i].enabled = false;
        }
    }
    public void NextSkin()
    {
        skins[index].enabled = false;
        index++;
        if (index >= skins.Count) index = 0;
        skins[index].enabled = true;

        if (index == selectedSkin) checkMark.enabled = true;
        else checkMark.enabled = false;
    }
    public void PreviousSkin()
    {
        skins[index].enabled = false;
        index--;
        if (index < 0) index = skins.Count - 1;
        skins[index].enabled = true;

        if (index == selectedSkin) checkMark.enabled = true;
        else checkMark.enabled = false;
    }
    public void SelectSkin()
    {
        PlayerPrefs.SetInt("SelectedSkin", index);
        selectedSkin = index;

        checkMark.enabled = true;
    }
}