using System.Collections;
using System.Threading.Tasks;
using ElleRealTimeStd.Shared.Test.Interfaces.Service;
using Grpc.Core;
using MagicOnion.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public static Login Instance { get { return _instance; } }
    private static Login _instance;
    private bool IsLogged = false;

    private string usernameString = string.Empty;
    private string passwordString = string.Empty;

    private Rect windowRect = new Rect(0, 0, Screen.width, Screen.height);

    private static Channel channel = null;

    private ILoginService client = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        GUI.Window(0, windowRect, WindowFunction, "Login");
    }

    void WindowFunction(int windowID)
    {
        //User input log-in
        usernameString =
            GUI.TextField(new Rect(Screen.width / 3, 2 * Screen.height / 5, Screen.width / 3, Screen.height / 10),
                usernameString, 10);

        passwordString =
            GUI.PasswordField(new Rect(Screen.width / 3, 2 * Screen.height / 3, Screen.width / 3, Screen.height / 10),
                passwordString, "*"[0], 10);

        if (GUI.Button(new Rect(Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Login"))
        {
            if (!IsLogged)
            {
                NotLoggedClient c = new NotLoggedClient(usernameString, passwordString);
                c.Connect();
            }
        }

        GUI.Label(new Rect(Screen.width/3, 35*Screen.height/100, Screen.width/5, Screen.height/8), "Username");
        GUI.Label(new Rect(Screen.width/3, 62*Screen.height/100, Screen.width/5, Screen.height/8), "Password");
    }

    public static void HandleAfterLogin( bool isLogged, int accountId )
    {
        if (isLogged)
        {
            Debug.Log($"Welcome, {accountId}");
            //Change scene with accountId
            Client.GlobalVariables.CurrentAccountID = accountId;
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Wrong username or password.");
        }
    }
}
