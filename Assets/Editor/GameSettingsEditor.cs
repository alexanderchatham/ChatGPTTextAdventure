#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameSettings gameSettings = (GameSettings)target;
        if (GUILayout.Button("Increment Font Index"))
        {
            gameSettings.IncrementFontIndex();
        }
        if (GUILayout.Button("Increment Gradient Index"))
        {
            gameSettings.IncrementGradientIndex();
        }
        if (GUILayout.Button("Increment Color Index"))
        {
            gameSettings.IncrementColorIndex();
        }
    }
}
#endif