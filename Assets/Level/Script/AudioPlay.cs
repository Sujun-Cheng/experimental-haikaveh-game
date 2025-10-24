using UnityEngine;

public class AudioPlay : MonoBehaviour

{
    AudioSource aud;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void play_sound()

        { aud.Play(); }

}
