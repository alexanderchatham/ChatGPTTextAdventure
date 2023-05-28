using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChatAPI : MonoBehaviour
{
    private Uri baseURL = new Uri("https://api.openai.com/v1/chat/completions");
    private string apiKey;

    public OpenAIChatAPI(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public IEnumerator SendChatCompletionRequest(Model model, List<Message> messages, Action<ChatCompletionResponse> callback, bool stream = false)
    {
        var request = new ChatCompletionRequest()
        {
            model = model,
            messages = messages,
            stream = stream
        };

        var json = JsonConvert.SerializeObject(request);

        var webRequest = new UnityWebRequest(baseURL.ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Request failed: {webRequest.error}");
        }
        else
        {
            var result = webRequest.downloadHandler.text;
            callback(JsonConvert.DeserializeObject<ChatCompletionResponse>(result));
        }
    }
}

public enum Model
{
    gpt35Turbo,
    gpt35Turbo0301,
    gpt4
}

public enum Role
{
    system,
    user,
    assistant
}

public class Message
{
    public Role role { get; set; }
    public string content { get; set; }
}

public class Stop
{
    public string _string { get; set; }
    public List<string> array { get; set; }
}

public class ChatCompletionRequest
{
    public Model model { get; set; }
    public List<Message> messages { get; set; }
    public double? temperature { get; set; }
    public double? top_p { get; set; }
    public int? n { get; set; }
    public bool? stream { get; set; }
    public Stop stop { get; set; }
    public int? max_tokens { get; set; }
    public double? presence_penalty { get; set; }
    public double? frequency_penalty { get; set; }
    public Dictionary<string, double>? logit_bias { get; set; }
    public string user { get; set; }
}

public class Delta
{
    public Role? role { get; set; }
    public string content { get; set; }
}

public class Choice
{
    public int index { get; set; }
    public Message message { get; set; }
    public string finish_reason { get; set; }
    public Delta delta { get; set; }
}

public class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class ChatCompletionResponse
{
    public string id { get; set; }
    public string responseObject { get; set; }  // Rename "object" to "responseObject" or any other name that isn't a reserved keyword
    public int created { get; set; }
    public List<Choice> choices { get; set; }
    public Usage usage { get; set; }
    public string model { get; set; }
}

