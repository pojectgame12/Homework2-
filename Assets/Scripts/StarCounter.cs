using UnityEngine;

public class StarCounter : MonoBehaviour
{
    public static int Collected { get; set; }
    [SerializeField, Min(1)] private int totalStars = 5;

    private void Awake() => Collected = 0;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 18;
        style.alignment = TextAnchor.MiddleLeft;
        GUI.Box(new Rect(15f, 15f, 400f, 40f),
            $"  Stars: {Collected}/{totalStars}   |   Move: Arrow Keys or WASD", style);
    }
}
