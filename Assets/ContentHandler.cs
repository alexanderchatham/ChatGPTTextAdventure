using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatGPTWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentHandler : MonoBehaviour
{
    public TextMeshProUGUI character;
    public TextMeshProUGUI paragraph;
    public TextMeshProUGUI a;
    public TextMeshProUGUI b;
    public TextMeshProUGUI c;

    public GameObject restartButton;
    private ChatGPTConversation chat;

    private void Start()
    {
        chat = GetComponent<ChatGPTConversation>();
    }


    private bool madeChoice = false;
    public string choiceFormatString;
    private string choice;
    public void selectOption(string option)
    {
        if (madeChoice)
            return;
        madeChoice = true;
        choice = option + choiceFormatString;
        chat.SendToChatGPT(choice);
    }

    public static string SanitizeJSONString(string s)
    {
        // Find the first opening brace '{' and last closing brace '}'
        Match match = Regex.Match(s, @"\{.*\}", RegexOptions.Singleline);
        if (match.Success)
        {
            print("json found");
            string json = match.Value;
            return json;
        }
        else
        {
            print("no json found");
            // No valid JSON object found
            return null;
        }
    }

    public void contentLinker(string s)
    {
        getContent(s);
    }

    async void getContent(string s)
    {
        print(s);
        s = SanitizeJSONString(s);
        if (s != null)
        {


            try
            {
                story current = JsonUtility.FromJson<story>(s);
                if (!String.IsNullOrEmpty(current.paragraph))
                {
                    madeChoice = false;
                    print(current.ToString());
                    character.text = "Character: " + current.character;
                    paragraph.text = current.paragraph;

                    if (String.IsNullOrEmpty(current.A) || String.IsNullOrEmpty(current.B) ||
                        String.IsNullOrEmpty(current.C))
                    {
                        a.transform.parent.gameObject.SetActive(false);
                        b.transform.parent.gameObject.SetActive(false);
                        c.transform.parent.gameObject.SetActive(false);
                        restartButton.SetActive(true);
                    }
                    else
                    {
                        a.text = "A: " + current.A;
                        b.text = "B: " + current.B;
                        c.text = "C: " + current.C;
                    }

                    var vert = paragraph.transform.parent.GetComponent<VerticalLayoutGroup>();
                    vert.enabled = false;
                    await Task.Delay(200);
                    vert.enabled = true;


                }
            }
            catch (Exception e)
            {
                chat.SendToChatGPT(choice);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

[System.Serializable]
public class story
{
    public string character ;
    public string paragraph ;
    public string A ;
    public string B ;
    public string C ;
}