using UnityEngine;
using System.Collections.Generic;
public class PlayerSkinSelector : MonoBehaviour
{
    [SerializeField] private List<Material> skins;
    [SerializeField] private List<MeshRenderer> bodyParts;
    private void Start()
    {
        Material skin = skins[PlayerPrefs.GetInt("SelectedSkin")];
        foreach(MeshRenderer bodyPart in bodyParts)
        {
            bodyPart.material = skin;
        }
    }
}