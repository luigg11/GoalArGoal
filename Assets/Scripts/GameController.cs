﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// GameController hold some important references and functionality, used
/// by other parts of the game..
/// Score goal(play sound, update score board) functionality
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController instance;

    // references
    public GameObject playerBluePrefab;

    public GameObject playerRedPrefab;

    public GameObject arGame;

    public GameObject menuCameraGO;

    public GameObject playerCameraPc;

    public GameObject playerCameraAndroid;

    public GameObject ballReference;

    public Text textScoreBlue;

    public Text textScoreRed;

    // inspector config
    public int goalsTarget = 5;

    // other fields
    private NetworkView myNetworkView;

    private AudioSource myAudioSource;

    private int blueScore = 0;

    private int redScore = 0;

    private bool isOver = false;

    private string gameIsOverMessage = "";

    /// <summary>
    /// The player camera property.
    /// This is a normal camera in the pc version.
    /// This is an AR camera in the android version, positioned based on the smarthphone movement around the target image.
    /// </summary>
    public GameObject PlayerCamera
    {
        get
        {
#if UNITY_ANDROID
            return playerCameraAndroid;
#else
            return playerCameraPc;
#endif
        }
    }

    void Awake()
    {
        instance = this;
        myNetworkView = GetComponent<NetworkView>();
        myAudioSource = GetComponent<AudioSource>();
    }

    public void OnGUI()
    {
        if (isOver)
        {
            GUILayout.Label(gameIsOverMessage);
            // shows the button only for the server
            if (Network.isServer && GUILayout.Button("Restart"))
            {
                Reset();
            }
        }
    }

    /// <summary>
    /// Reset the game,play again.
    /// </summary>
    public void Reset()
    {
        blueScore = redScore = 0;
        GetComponent<NetworkView>().RPC("UpdateScoreBoard", RPCMode.All, blueScore, redScore);
        ballReference.transform.localPosition = new Vector3(0.0f, 8.3f, -60.6f);
        GetComponent<NetworkView>().RPC("SetIsOver", RPCMode.All, false);
    }

    /// <summary>
    /// Score blue goal, called by the goal trigger.
    /// </summary>
    public void ScoreBlueGoal()
    {
        if (!isOver)
        {
            blueScore++;
            myNetworkView.RPC("PlayScoreSound", RPCMode.All);
            if (blueScore == goalsTarget)
            {
                myNetworkView.RPC("SetIsOver", RPCMode.All, true);
                myNetworkView.RPC("SetGameIsOverMessage", RPCMode.All, "Player " + (blueScore == goalsTarget ? "Blue" : "Red") + " IS THE WINNER !!!");
            }
            myNetworkView.RPC("UpdateScoreBoard", RPCMode.All, blueScore, redScore);
        }
    }

    /// <summary>
    /// Score blue goal, called by the goal trigger.
    /// </summary>
    public void ScoreRedGoal()
    {
        if (!isOver)
        {
            redScore++;
            myNetworkView.RPC("PlayScoreSound", RPCMode.All);
            if (redScore == goalsTarget)
            {
                myNetworkView.RPC("SetIsOver", RPCMode.All, true);
                myNetworkView.RPC("SetGameIsOverMessage", RPCMode.All, "Player " + (blueScore == goalsTarget ? "Blue" : "Red") + " IS THE WINNER !!!");
            }
            myNetworkView.RPC("UpdateScoreBoard", RPCMode.All, blueScore, redScore);
        }
    }

    [RPC]
    private void UpdateScoreBoard(int bScore, int rScore)
    {
        textScoreBlue.text = bScore + "";
        textScoreRed.text = rScore + "";
    }

    [RPC]
    private void SetIsOver(bool value)
    {
        isOver = value;
    }

    [RPC]
    private void SetGameIsOverMessage(string message)
    {
        gameIsOverMessage = message;
    }

    [RPC]
    private void PlayScoreSound()
    {
        myAudioSource.time = 0.0f;
        myAudioSource.Play();
    }

}
