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

    public static ContentHandler instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(instance);
        }
        
    }

    private void Start()
    {
        StreamResponse = GetComponent<OpenAI.StreamResponse>();
        buttons.alpha = 0f;
        buttons.interactable = false;
    }


    private bool madeChoice = false;
    private string choiceFormatString = 
        " is the option the player chose in the following paragraph, use this and create a new paragraph and more choices for the player." +
        " If using quotes in the story use ' instead of \"." +
        " Do not include any explanations, only provide a RFC8259 compliant JSON response following this format without deviation. Only send one JSON object for the next part of the story \n";


    public ScrollRect scrollRect;

    public void ScrollToTop()
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    private string choice;
    public void selectOption(string option)
    {
        if (EconomyCode.instance.getTokens() <= 0)
        {
            EconomyCode.instance.showTokenOptions();
            return;
        }
        ScrollToTop();
        FindObjectOfType<LerpTextMeshPro>().Reset();
        Debug.Log(option);
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
        finished = false;
        StreamResponse.SendMessage(choice);
    }

    public string choice_prompt_format(string option)
    {
        choice = StreamResponse.finalString + option + choiceFormatString;
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
        print("no json found trying again");
        // No valid JSON object found
        match = Regex.Match(s+"}", @"\{.*\}", RegexOptions.Singleline);
        if (match.Success)
        {
            string json = match.Value;
            return json;
        }
        match = Regex.Match(s + "\"}", @"\{.*\}", RegexOptions.Singleline);
        if (match.Success)
        {
            string json = match.Value;
            return json;
        }
        print("no json found");
        // No valid JSON object found
        return null;
                
        
    }
    

    public bool finished = false;
    public void contentLinker()
    {
        finished = true;
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
            try
            {
                current = JsonUtility.FromJson<story>(s);

            }
            catch (Exception e)
            {
                buttons.alpha = 1;
                buttons.interactable = true;
                a.transform.parent.gameObject.SetActive(false);
                b.transform.parent.gameObject.SetActive(false);
                c.transform.parent.gameObject.SetActive(false);
                restartButton.SetActive(true);
                restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "ChatGPT Error";
                Console.WriteLine(e);
                throw;
                
            }
            
        }
        
        var vert = paragraph.transform.parent.GetComponent<VerticalLayoutGroup>();
        vert.enabled = false;
        await Task.Delay(100);
        vert.enabled = true;
        
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
                        restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "End";
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

                    


                }
            }
            catch (Exception e)
            {
                a.transform.parent.gameObject.SetActive(false);
                b.transform.parent.gameObject.SetActive(false);
                c.transform.parent.gameObject.SetActive(false);
                restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
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