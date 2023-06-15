using UnityEngine;
using System.Collections;
using TMPro;

public class LerpTextMeshPro : MonoBehaviour
{
    public float duration;

    private TextMeshProUGUI textMesh;
    private bool isLerping = false;
    private int currentCharIndex = 0;
    [SerializeField] private TextMeshProUGUI ghostText;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateText());
    }

    IEnumerator UpdateText()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (textMesh.text != ghostText.text)
            {
                textMesh.text = ghostText.text;
                SetFinalColorForPreviousCharacters();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isLerping && textMesh.text.Length > currentCharIndex)
        {
            
            isLerping = true;
            StartCoroutine(LerpCharacter());
        }
    }

    IEnumerator LerpCharacter()
    {
        if (textMesh.textInfo.characterCount > currentCharIndex)
        {
            float t = 0f;

            
            while (t < duration)
            {
                t += Time.deltaTime;

                // Lerp alpha
                Color32[] newVertexColors = textMesh.textInfo.meshInfo[0].colors32;
                Color32 c = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, t));
                int materialIndex = textMesh.textInfo.characterInfo[currentCharIndex].materialReferenceIndex;
                int vertexIndex = textMesh.textInfo.characterInfo[currentCharIndex].vertexIndex;
                newVertexColors[vertexIndex + 0] = c;
                newVertexColors[vertexIndex + 1] = c;
                newVertexColors[vertexIndex + 2] = c;
                newVertexColors[vertexIndex + 3] = c;

                SetFinalColorForPreviousCharacters();

                yield return null;
            }

            currentCharIndex++;
            SetFinalColorForPreviousCharacters();
            isLerping = false;
        }
    }
    void SetFinalColorForPreviousCharacters()
    {
        Color32[] newVertexColors = textMesh.textInfo.meshInfo[0].colors32;
        Color32 finalColor = new Color32(255, 255, 255, 255);

        for (int i = 0; i < currentCharIndex; i++)
        {
            int vertexIndex = textMesh.textInfo.characterInfo[i].vertexIndex;
            newVertexColors[vertexIndex + 0] = finalColor;
            newVertexColors[vertexIndex + 1] = finalColor;
            newVertexColors[vertexIndex + 2] = finalColor;
            newVertexColors[vertexIndex + 3] = finalColor;
        }

        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void Reset()
    {
        currentCharIndex = 0;
        textMesh.text = "";
    }
}