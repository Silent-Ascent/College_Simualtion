using UnityEngine;

public class InfoData : MonoBehaviour
{
    [SerializeField] private string objectTitle;

    [TextArea(5, 10)]
    [SerializeField] private string objectDescription;

    [SerializeField] private Sprite objectSprite;

    public string Title => objectTitle;
    public string Description => objectDescription;
    public Sprite ObjectSprite => objectSprite;
}