using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Database;




public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Custom variables from me
    [Header("Custom Vars")]
    public GameObject registerPrompt;
    public GameObject forgotPasswordPrompt;



    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });


    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;

        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {

        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }


    //Forgot Password Method

    public void ForgotPassword()
    {


     StartCoroutine(ForgotPasswordCatch(emailLoginField.text));


    }




    private IEnumerator ForgotPasswordCatch(string _email)
    {
        //Call the Firebase auth forgot password
      
        //Don't take this one line of code for granted. It took 4 hours to find this. Google has terrible documentation.
      
        var FPTask =  auth.SendPasswordResetEmailAsync(_email);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => FPTask.IsCompleted);


        //In all honesty, I have no clue if this function can error out, but I'll take precautions.

        if (FPTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {FPTask.Exception}");

            string message = "Reset failed!";
    
            warningLoginText.text = message;
        }
        else
        {
            
            //User is now logged in
            //Now get the result
            //User = FPTask.Result;
            //Debug.LogFormat("Reset password email sent successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Email sent!";

            //Remove forgot password button
            forgotPasswordPrompt.SetActive(false);


        }
    }








    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Invalid username.";
                    forgotPasswordPrompt.SetActive(false);
                    registerPrompt.SetActive(true);
                    break;
                case AuthError.MissingPassword:
                    message = "Invalid password.";
                    registerPrompt.SetActive(false);
                    forgotPasswordPrompt.SetActive(true);
                    break;
                case AuthError.WrongPassword:
                    message = "Invalid password.";
                    registerPrompt.SetActive(false);
                    forgotPasswordPrompt.SetActive(true);
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid username.";
                    forgotPasswordPrompt.SetActive(false);
                    registerPrompt.SetActive(true);
                    break;
                case AuthError.UserNotFound:
                    message = "Invalid username.";
                    forgotPasswordPrompt.SetActive(false);
                    registerPrompt.SetActive(true);
                    break;
            }
            confirmLoginText.text = "";
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);

            //Get rid of register/login buttons
            forgotPasswordPrompt.SetActive(false);
            registerPrompt.SetActive(false);

            //Probably put some LoadScene, or loading thing right here.

            //Go to game!!!




            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing username";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password does not match!";
        }
        else 
        {

            //First, make sure that username aint taken.

           //Must be logged in to access the database?
           // I need a workaround.


             var DBTask2 = DBreference.Child("username").Child(_username).GetValueAsync();


            yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);

            if (DBTask2.Result.Value != null)
             {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
                    warningRegisterText.text = "Username already taken!";
                }
             else
             {

    

            //Database username is now updated
        
            
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }
                //Handle adding _username at end when account is created. No reason to do it immediately.
                //var DBClearTask1 = DBreference.Child("username").Child(_username).SetValueAsync(null);
                confirmLoginText.text = "";
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {


                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _username};




                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username set failed!";
                    }
                    else
                    {

                        //Login right here..

                        Login(_email,_password);


                        
            var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

            //If task fails, clear everything, including any uuid, username, password, etc. created
            var DBClearTask1 = DBreference.Child("username").Child(_username).SetValueAsync(null);
            var DBClearTask = DBreference.Child("users").Child(User.UserId).SetValueAsync(null);

        }
        else
        {


            //So, its going to set a temporary null pointer. Once the user is logged in, itll change it to the newly generated UUID.
           //need to set it so if any further method errors out, itll null out and clear this data.
            var DBTask1 = DBreference.Child("username").Child(_username).SetValueAsync("");


            yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask1.Exception != null)
             {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask1.Exception}");

                    

                    //If task fails, clear it in case anything got written.
                    var DBClearTask1 = DBreference.Child("username").Child(_username).SetValueAsync(null);
        
                }
             else
             {


                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }
}
}
}
}