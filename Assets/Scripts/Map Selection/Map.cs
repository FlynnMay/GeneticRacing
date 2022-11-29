using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Maps/Single", fileName = "New Map")]
public class Map : ScriptableObject
{
    [SerializeField] string mapName;
    [SerializeField][TextArea] string description;
    [SerializeField] Sprite previewSprite;
    [SerializeField] string sceneName;
    
    public string MapName { get => mapName; }
    public string Description { get => description; }
    public Sprite Preview { get => previewSprite; }
    public string Scene { get => sceneName; }
}
