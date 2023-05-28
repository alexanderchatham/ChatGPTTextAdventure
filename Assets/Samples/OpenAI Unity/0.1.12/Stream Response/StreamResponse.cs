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
        private OpenAIApi openai;
        private CancellationTokenSource token = new CancellationTokenSource();
        public string PlayerName;
        private void Start()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("APIKEY");
            var _apiKey = textAsset.text;
            openai = new OpenAIApi(_apiKey);
            handler.choiceFormatString = handler.choiceFormatString.Replace("Alex", PlayerName);
            SendMessage(initialMessage+" The characters name is "+PlayerName);
        }
        public string initialMessage = "You are now a text adventure game generator. Generate a paragraph for a new text adventure game along with 3 choices. Use this JSON format for your response. { \"character\": \"character name\", \"paragraph\":\"current part of the story goes here\",     \"A\": \"New Adventure\",     \"B\": \"New Adventure\",     \"C\": \"New Adventure\",   } ";


        public void SendMessage(string s)
        {

            var message = new List<ChatMessage>
            {
                new ChatMessage()
                {
                    Role = "user",
                    Content = s
                }
            };

            openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo",
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
