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
    private string choiceFormatString = 
        " If using quotes in the story use ' instead of \"." +
        " Please continue the story by writing a new paragraph and updating the summary of the story and use this JSON from the last section of the story to format your response for the next section of the story. Remember to only send back the JSON. \n";


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
        /*
        if (option == "A")
            choice += current.A;
        if (option == "B")
            choice += current.B;
        if (option =="B")
            choice+= current.B;
        */
        StreamResponse.SendMessage(choice);
    }

    public string choice_prompt_format(string option)
    {
        choice = option + " is the option the player chose in the following paragraph, use this when creating the new paragraph. ";
        //choice += " The current paragraph for the story is: " + current.paragraph;
        choice += choiceFormatString + StreamResponse.finalString + "\n\n[Remember to ONLY return JSON] \n";
        return choice;
    }

    public static string SanitizeJSONString(string s)
    {
        // Find the first opening brace '{' and last closing brace '}'
        Match match = Regex.Match(s, @"\{.*\}", RegexOptions.Singleline);
        if (match.Success)
        {
            string json = match.Value;
            return json;
        }
        else
        {
            print("no json found trying again");
            // No valid JSON object found
            match = Regex.Match(s+"}", @"\{.*\}", RegexOptions.Singleline);
            if (match.Success)
            {
                string json = match.Value;
                return json;
            }
            else
            {
                match = Regex.Match(s + "\"}", @"\{.*\}", RegexOptions.Singleline);
                if (match.Success)
                {
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
    }

    public void contentLinker()
    {
        getContent();
    }

    async void getContent()
    {
        await Task.Delay(1000);
        string s = StreamResponse.finalString;
        Debug.Log(s);
        try
        {
            current = JsonUtility.FromJson<story>(s);

        }
        catch (Exception)
        {

            s = SanitizeJSONString(s);
            current = JsonUtility.FromJson<story>(s);
        }
        if (current != null)
        {


            try
            {
                if (!String.IsNullOrEmpty(current.paragraph))
                {
                    paragraph.GetComponent<ContentSizeFitter>().enabled = true;
                    buttons.alpha = 1;
                    buttons.interactable = true;
                    madeChoice = false;
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
                    await Task.Delay(10);
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
    public string summary;
    public string paragraph ;
    public string A ;
    public string B ;
    public string C ;
}