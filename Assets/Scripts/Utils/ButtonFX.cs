using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    [SerializeField] private AudioClip highlightFX;
    [SerializeField] private AudioClip clickFX;

    private AudioSource audioSource;


    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayHighlightFX() 
    {
        audioSource.PlayOneShot(highlightFX);
    }

    public void PlayClickFX() 
    {
        audioSource.PlayOneShot(clickFX);
    }
}
