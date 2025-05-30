using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI outputText;

    private string token;

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login(usernameInput.text, passwordInput.text));
    }

    IEnumerator Login(string username, string password)
    {
        string jsonData = JsonUtility.ToJson(new UserCredentials(username, password));
        var request = new UnityWebRequest("http://127.0.0.1:8001/token", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
            token = response.access_token;
            outputText.text = "Login successful!";
            StartCoroutine(GetProfile());
        }
        else
        {
            outputText.text = "Login failed: " + request.error;
        }
    }

    IEnumerator GetProfile()
    {
        var request = UnityWebRequest.Get("http://127.0.0.1:8001/me");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            outputText.text = "User info: " + request.downloadHandler.text;
        }
        else
        {
            outputText.text = "Failed to get profile: " + request.error;
        }
    }

    [System.Serializable]
    public class UserCredentials
    {
        public string username;
        public string password;

        public UserCredentials(string u, string p)
        {
            username = u;
            password = p;
        }
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string access_token;
        public string token_type;
    }
}