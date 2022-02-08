using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseDBManager : MonoBehaviour
{
    DatabaseReference reference;
    Firebase.FirebaseApp app;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //FirebaseDatabase.DefaultInstance.GetReference("gamestate").ValueChanged += HandleUpdateGameState;
    }

/**
    public void HandleUpdateGameState(object sender, ValueChangedEventArgs args)
    {
        DataSnapshot snapshot = args.Snapshot;
        Debug.Log("Firebase value: " + snapshot.Value);
        GameState myGameState;
        myGameState = JsonUtility.FromJson<GameState>(snapshot.GetRawJsonValue());
        Debug.Log("myObject.food: " + myGameState.food);
    }
*/
    public void UpdateGameState(GameState gameState)
    {
        FirebaseDatabase.DefaultInstance.GetReference("gamestate").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = JsonUtility.ToJson(gameState);
                reference.Child("gamestate").SetRawJsonValueAsync(json);
            }
        });
    }

    public GameState GetGameState()
    {
        GameState myGameState = null;
        FirebaseDatabase.DefaultInstance.GetReference("gamestate").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                GameState myGameState;
                myGameState = JsonUtility.FromJson<GameState>(snapshot.GetRawJsonValue());
            }
        });
        return myGameState;
    }

    public bool FirebaseCheck()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                return false;
            }
        });
        return false;
    }
}
