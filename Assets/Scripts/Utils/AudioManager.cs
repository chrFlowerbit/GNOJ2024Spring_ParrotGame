using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager instance;

    public static AudioManager Instance { 
        get {
            return instance;
        }
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as AudioManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

}
