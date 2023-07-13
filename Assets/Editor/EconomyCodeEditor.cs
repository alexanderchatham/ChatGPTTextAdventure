#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EconomyCode))]
public class EconomyCodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EconomyCode gameSettings = (EconomyCode)target;
        if (GUILayout.Button("SetTokensToZero"))
        {
            gameSettings.SetTokensToZero();
        }
        if (GUILayout.Button("SetPointsToZero"))
        {
            gameSettings.SetPointsToZero();
        }
    }
}
#endif