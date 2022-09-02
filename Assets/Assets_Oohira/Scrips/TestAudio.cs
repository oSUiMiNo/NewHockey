using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    private AudioSource audioSource = null;
    [SerializeField] private AudioClip audioClip = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            audioSource.PlayOneShot(audioClip);
        }

    }
}
