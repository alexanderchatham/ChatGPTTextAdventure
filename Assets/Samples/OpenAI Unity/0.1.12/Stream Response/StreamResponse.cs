using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenAI
{
    public class StreamResponse : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        public ContentHandler handler;
        public UIManager uiManager; // Add this line
        private OpenAIApi openai;
        private CancellationTokenSource token = new CancellationTokenSource();
        private ChatMessage systemMessage;
        private void Start()
        {
            systemMessage = new ChatMessage()
            {
                Role = "user",
                Content = "You are an Text adventure game generator."
            };
            TextAsset textAsset;
            if (model == GPTModel.GPT4)
                textAsset = Resources.Load<TextAsset>("APIKEYGPT4");
            else
                textAsset = Resources.Load<TextAsset>("APIKEY");
            //Model = "gpt-3.5-turbo",
            var _apiKey = textAsset.text;
            openai = new OpenAIApi(_apiKey);
        }
        public void StartStory()
        {
            FindObjectOfType<MenuManager>().ShowGamePanel();
            text.text = "";
            handler.buttons.alpha = 0;
            handler.buttons.interactable = false;
            SendMessage(BuildInitialMessage()); // Replace the old SendMessage call

        }
        private string BuildInitialMessage()
        {
            //the story length is {uiManager.GetStoryLength()},
            // Use the settings from the UIManager to build the initial message
            string message = $"You are now a text adventure game generator. Generate a paragraph for a new text adventure game along with 3 choices. The game theme is {uiManager.GetTheme()}, the character's name is {uiManager.GetCharacterName()}, the character's gender is {uiManager.GetCharacterGender()},  the genre is {uiManager.GetGenre()}. " + initialMessage;
            return message;
        }

        private string initialMessage = "Do not include any explanations, only provide a RFC8259 compliant JSON response following this format without deviation.. { \"character\": \"character name\", \"summary\": \"summarize the entire story so far here\", \"paragraph\":\"current part of the story goes here\",     \"A\": \"New Adventure\",     \"B\": \"New Adventure\",     \"C\": \"New Adventure\"  } ";
        public enum GPTModel
        {
            GPT35,
            GPT4

        }
        public GPTModel model;
        public void SendMessage(string s)
        {
            Debug.Log(s);
            List<ChatMessage> message = new List<ChatMessage>();
            message.Add(systemMessage);
            message.Add(
                    new ChatMessage()
                    {
                        Role = "user",
                        Content = s
                    }
            );
            var gptModel = "";
            if (model == GPTModel.GPT35)
                gptModel = "gpt-3.5-turbo";
            else if (model == GPTModel.GPT4)
                gptModel = "gpt-4";
                //Model = "gpt-3.5-turbo",
            openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
            {
                Model = gptModel,
                Messages = message,
                Stream = true
            }, HandleResponse
            ,  handler.contentLinker
            ,  token);


        }

        public string finalString;
        private void HandleResponse(List<CreateChatCompletionResponse> responses)
        {
            finalString = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));
            //Debug.Log(finalString);
            string pattern = "\"paragraph\"\\s*:\\s*\"(.*?)\"";
            Match m = Regex.Match(finalString, pattern);
            if (m.Success)
            {
                //Debug.Log("matched regex");
                text.text = m.Groups[1].Value;

            }
            else if (finalString.Contains("paragraph\": \""))
                text.text = finalString.Substring(finalString.IndexOf("paragraph\": \"")+13, finalString.Length - finalString.IndexOf("paragraph\": \"")-13);
            else if (finalString.Contains("paragraph\":\""))
                text.text = finalString.Substring(finalString.IndexOf("paragraph\":\"") + 12, finalString.Length - finalString.IndexOf("paragraph\":\"") - 12);
            else
                text.text = "";
        }
        private void OnDestroy()
        {
            token.Cancel();
        }
    }
}
