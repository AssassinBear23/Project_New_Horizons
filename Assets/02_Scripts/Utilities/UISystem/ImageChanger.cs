using UnityEngine;
using UnityEngine.UI;
public class ImageChanger : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Color dark;
    [SerializeField] private Color bright;
    [SerializeField] private Texture texture1;
    [SerializeField] private Texture texture2;
    public void DarkenImage()
    {
        image.color = dark;
    }
    public void UnDarkenImage()
    {
        image.color = bright;
    }
    public void ToggleTexture()
    {
        if (image.texture == texture1) image.texture = texture2;
        else image.texture = texture1;
    }
}
