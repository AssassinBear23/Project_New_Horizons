using UnityEngine;
using UnityEngine.UI;
public class ImageChanger : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Color dark;
    [SerializeField] private Color bright;
    public void DarkenImage()
    {
        image.color = dark;
    }
    public void UnDarkenImage()
    {
        image.color = bright;
    }
}
