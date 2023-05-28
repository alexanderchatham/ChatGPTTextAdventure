using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentHandler : MonoBehaviour
{
    public TextMeshProUGUI character;
    public TextMeshProUGUI paragraph;
    public CanvasGroup buttons;
    public TextMeshProUGUI a;
    public TextMeshProUGUI b;
    public TextMeshProUGUI c;
    private story current;
    public GameObject restartButton;
    private OpenAI.StreamResponse StreamResponse;

    private void Start()
    {
        StreamResponse = GetComponent<OpenAI.StreamResponse>();
        buttons.alpha = 0f;
        buttons.interactable = false;
    }


    private bool madeChoice = false;
    public string choiceFormatString;
    private string choice;
    public void selectOption(string option)
    {
        Debug.Log(option);
        paragraph.GetComponent<ContentSizeFitter>().enabled = false;
        buttons.alpha = 0;
        buttons.interactable = false;
        if (madeChoice)
            return;
        madeChoice = true;
        choice = choice_prompt_format(option);
        if (option == "A")
            choice += current.A;
        if (option == "B")
            choice += current.B;
        if (option =="B")
            choice+= current.B;
        Debug.Log(choice);
        StreamResponse.SendMessage(choice);
    }

    public string choice_prompt_format(string option)
    {
        choice = "The players name is " + StreamResponse.PlayerName + ". " + option + choiceFormatString;
        choice += " The current paragraph for the story is: " + current.paragraph + " The player has chosen this choice";
        return choice;
    }

    public string system_prompt()
    {
        return "You are now a text adventure game generator. " +
            "Generate a paragraph for a new text adventure " +
            "game along with 3 choices. Use this JSON format " +
            "for your response:\n" +
            "{\n" +
            " \"character\": \"character name\",\n" +
            " \"paragraph\":\"current part of the story goes here\",\n" +
            "     \"A\": \"New Adventure\",\n" +
            "     \"B\": \"New Adventure\",\n" +
            "     \"C\": \"New Adventure\",\n" +
            "   }\n" +
            "If using quotes in the story use ' instead of \"";
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
            print("no json found trying again");
            s += "}";
            // No valid JSON object found
            match = Regex.Match(s, @"\{.*\}", RegexOptions.Singleline);
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
    }

    public void contentLinker()
    {
        getContent();
    }

    async void getContent()
    {
        await Task.Delay(200);
        string s = StreamResponse.finalString;
        print(s);
        s = SanitizeJSONString(s);
        if (s != null)
        {


            try
            {
                current = JsonUtility.FromJson<story>(s);
                if (!String.IsNullOrEmpty(current.paragraph))
                {
                    paragraph.GetComponent<ContentSizeFitter>().enabled = true;
                    buttons.alpha = 1;
                    buttons.interactable = true;
                    madeChoice = false;
                    print(current.ToString());
                    character.text = "Character: " + current.character;
                    paragraph.text = current.paragraph;

                    if (String.IsNullOrEmpty(current.A) && String.IsNullOrEmpty(current.B) &&
                        String.IsNullOrEmpty(current.C))
                    {
                        Debug.Log("no options found");
                        if(String.IsNullOrEmpty(current.A))
                        a.transform.parent.gameObject.SetActive(false);
                        if(String.IsNullOrEmpty(current.B))
                        b.transform.parent.gameObject.SetActive(false);
                        if(String.IsNullOrEmpty(current.C))
                        c.transform.parent.gameObject.SetActive(false);
                        restartButton.SetActive(true);
                    }
                    else
                    {
                        restartButton.SetActive(false);
                        Debug.Log("options found");
                        a.text =  current.A;
                        b.text =  current.B;
                        c.text =  current.C;
                        if (String.IsNullOrEmpty(current.A))
                            a.transform.parent.gameObject.SetActive(false);
                        else
                            a.transform.parent.gameObject.SetActive(true);

                        if (String.IsNullOrEmpty(current.B))
                            b.transform.parent.gameObject.SetActive(false);
                        else
                            b.transform.parent.gameObject.SetActive(true);
                        if (String.IsNullOrEmpty(current.C))
                            c.transform.parent.gameObject.SetActive(false);
                        else
                            c.transform.parent.gameObject.SetActive(true);
                    }

                    var vert = paragraph.transform.parent.GetComponent<VerticalLayoutGroup>();
                    vert.enabled = false;
                    await Task.Delay(200);
                    vert.enabled = true;


                }
            }
            catch (Exception e)
            {
                a.transform.parent.gameObject.SetActive(false);
                b.transform.parent.gameObject.SetActive(false);
                c.transform.parent.gameObject.SetActive(false);
                restartButton.SetActive(true);
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