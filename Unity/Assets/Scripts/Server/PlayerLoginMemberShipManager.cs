using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerLoginMemberShipManager : MonoBehaviour
{
   
    public string LoginServerUrl = "https://localhost:3001";
    public string MemeberShipServerUrl = "https://localhost:3002";


    [Header("Login Fields")]
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private Button loginButton;


    [Header("Membership Fields")]
    [SerializeField] private TMP_InputField memberEmail;
    [SerializeField] private TMP_InputField memberPassword;
    [SerializeField] private TMP_InputField memberUsername;
    [SerializeField] private Button memberButton;

    private string currentAccessToken;
    public Looby lobbyUI;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        memberButton.onClick.AddListener(OnMembershipButtonClicked); 
        lobbyUI = GetComponent<Looby>();
    }


    public void OnLoginButtonClicked()
    {
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            Debug.LogError("로그인: 이메일과 비밀번호를 모두 입력해야 합니다.");
            return;
        }
        StartCoroutine(SendLoginRequest(loginEmail.text, loginPassword.text));
    }

    private IEnumerator SendLoginRequest(string userEmail, string userPassword)
    {
        loginButton.interactable = false;

        AuthRequest requestData = new AuthRequest
        {
            useremail = userEmail,
            userpassword = userPassword
        };

        string jsonRequestBody = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{LoginServerUrl}/user", ""))
        {
     
            www.certificateHandler = new CustomCertificateHandler();

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            loginButton.interactable = true;

            if (www.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(www.downloadHandler.text);

                if (response.success)
                {
                    //currentAccessToken = response.accessToken;
                    Debug.Log($"로그인 성공! 이름: {response.user.playerName}, 돈: {response.user.currentMoney}");
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    Debug.LogError($"로그인 실패: {response.message}");
                }
            }
            else
            {
                Debug.LogError($"로그인 서버 요청 실패: {www.error} (Code: {www.responseCode})");
            }
        }
    }


    public void OnMembershipButtonClicked()
    {
        if (string.IsNullOrEmpty(memberEmail.text) || string.IsNullOrEmpty(memberPassword.text) || string.IsNullOrEmpty(memberUsername.text))
        {
            Debug.LogError("회원가입: 모든 필드를 채워야 합니다.");
            return;
        }
        StartCoroutine(SendMembershipRequest(memberEmail.text, memberPassword.text, memberUsername.text));
    }

    private IEnumerator SendMembershipRequest(string userEmail, string userPassword, string userName)
    {
        memberButton.interactable = false;

        MembershipRequest requestData = new MembershipRequest
        {
            useremail = userEmail,
            userpassword = userPassword,
            username = userName
        };

        string jsonRequestBody = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{MemeberShipServerUrl}/membership", ""))
        {
       
            www.certificateHandler = new CustomCertificateHandler();

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            memberButton.interactable = true;

            if (www.result == UnityWebRequest.Result.Success)
            {
                MembershipResponse response = JsonConvert.DeserializeObject<MembershipResponse>(www.downloadHandler.text);

                if (response.success)
                {
                    Debug.Log($"회원가입 성공! 사용자 ID: {response.userId}. 이제 로그인할 수 있습니다.");
                    //lobbyUI.isLoginPanel = true;
                }
                else
                {
                    Debug.LogError($"회원가입 실패: {response.message}");
                }
            }
            else
            {
                Debug.LogError($"회원가입 서버 요청 실패: {www.error} (Code: {www.responseCode})");
            }
        }
    }
}