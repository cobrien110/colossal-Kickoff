using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.UI;
using TMPro;

public class SteamScript : MonoBehaviour
{
    public int matchesPlayed = 0;
    public GameObject textObject = null;

    string fileName = "matchesPlayed";

    CallResult<RemoteStorageFileWriteAsyncComplete_t> m_FileWriteResult;
    CallResult<RemoteStorageFileReadAsyncComplete_t> m_FileReadResult;
    //SteamAPICall_t hFileRead;

    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log("Persona Name: " + name);

            m_FileWriteResult = CallResult<RemoteStorageFileWriteAsyncComplete_t>.Create(OnWriteToFile);
            m_FileReadResult = CallResult<RemoteStorageFileReadAsyncComplete_t>.Create(OnReadFromFile);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WriteToFile()
    {
        byte[] bytes = BitConverter.GetBytes(matchesPlayed);

        SteamAPICall_t hFileWrite = SteamRemoteStorage.FileWriteAsync(fileName, bytes, (uint) bytes.Length);
        m_FileWriteResult.Set(hFileWrite);
    }

    void OnWriteToFile(RemoteStorageFileWriteAsyncComplete_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to write file to Steam Cloud: " + pCallback.m_eResult);
        }
        else
        {
            Debug.Log("File Write Result: " + m_FileWriteResult);
        }
    }

    public void ReadFromFile()
    {
        byte[] readBytes = new byte[SteamRemoteStorage.GetFileSize(fileName)];
        Debug.Log("File Read Bytes: " + SteamRemoteStorage.FileRead(fileName, readBytes, SteamRemoteStorage.GetFileSize(fileName)));
        int readMatches = BitConverter.ToInt32(readBytes, 0);
        Debug.Log("Read Matches Played: " + readMatches);
        textObject.GetComponent<TMP_Text>().text = readMatches.ToString();

        //SteamAPICall_t hFileRead = SteamRemoteStorage.FileReadAsync(fileName, (uint)SteamRemoteStorage.GetFileSize(fileName), (uint)SteamRemoteStorage.GetFileTimestamp(fileName));
        //m_FileReadResult.Set(hFileRead);
    }

    #region Async Method (not used)

    public void OnReadFromFile(RemoteStorageFileReadAsyncComplete_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to read file to Steam Cloud: " + pCallback.m_eResult);
        }
        else
        {
            Debug.Log("File Read Result: " + m_FileReadResult);
            byte[] bytes = new byte[pCallback.m_cubRead];
            bool success = SteamRemoteStorage.FileReadAsyncComplete(pCallback.m_hFileReadAsync, bytes, pCallback.m_cubRead);

            if (!success)
            {
                Debug.LogError("FileReadAsyncComplete failed.");
                return;
            }

            int matchesPlayed = BitConverter.ToInt32(bytes, 0);
            Debug.Log("Read Matches Played: " + matchesPlayed);
            textObject.GetComponent<TMP_Text>().text = matchesPlayed.ToString();
        }
    }


    #endregion
}
