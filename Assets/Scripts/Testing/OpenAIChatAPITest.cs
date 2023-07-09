using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using OpenAI;

public class OpenAIChatAPITest : MonoBehaviour
{
    private string url = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "your_openai_api_key_here";

    void Start()
    {
        TextAsset textAsset;
        textAsset = Resources.Load<TextAsset>("APIKEY");
        //Model = "gpt-3.5-turbo",
        apiKey = textAsset.text;
        StartCoroutine(PostRequest());
    }

    private IEnumerator PostRequest()
    {
        var body = new
        {
            model = StreamResponse.GPTModel.GPT35,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = "What is the weather like in Boston?"
                }
            },
            functions = new[]
            {
                new
                {
                    name = "get_current_weather",
                    description = "Get the current weather in a given location",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            location = new
                            {
                                type = "string",
                                description = "The city and state, e.g. San Francisco, CA"
                            },
                            unit = new
                            {
                                type = "string",
                                enum_value = new string[] { "celsius", "fahrenheit" }
                            }
                        },
                        required = new string[] { "location" }
                    }
                }
            }
        };

        var jsonBody = JsonUtility.ToJson(body);
        jsonBody = @"{
        ""model"": ""gpt-3.5-turbo"",
        ""messages"": [
            {
                ""role"": ""user"",
                ""content"": ""What is the weather like in Boston?""
            },
            {
                ""role"": ""assistant"",
                ""content"": null,
                ""function_call"": {
                    ""name"": ""get_current_weather"",
                    ""arguments"": ""{ \""location\"": \""Boston, MA\"" }""
                }
            }, 
{
                ""role"": ""function"",
                ""name"": ""get_current_weather"",
                ""content"": ""{\""temperature\"": \""22\"", \""unit\"": \""celsius\"", \""description\"": \""Sunny\"" }""
}
        ],
        ""functions"": [
            {
                ""name"": ""get_current_weather"",
                ""description"": ""Get the current weather in a given location"",
                ""parameters"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""location"": {
                            ""type"": ""string"",
                            ""description"": ""The city and state, e.g. San Francisco, CA""
                        },
                        ""unit"": {
                            ""type"": ""string"",
                            ""enum"": [""celsius"", ""fahrenheit""]
                        }
                    },
                    ""required"": [""location""]
                }
            }
        ]
    }";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log($"{request.error}: {request.downloadHandler.text}");
        }
        else
        {
            Debug.Log($"Received: {request.downloadHandler.text}");
            // Here you can handle the response JSON string
        }
    }
}
