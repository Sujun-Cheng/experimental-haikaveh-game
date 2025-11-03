using UnityEngine;

public class GuitarMusic : MonoBehaviour
{
    private AudioSource guitarAudio;
    private AudioEventManager audioEventManager;
    public Transform player;

    public float maxHearingDistance = 5f;
    public float minBackgroundVolume = 0.2f; // Background music won't go below this

    private float originalBackgroundVolume;

    void Start()
    {
        guitarAudio = GetComponent<AudioSource>();

        // Find the AudioEventManager in the scene (contains background music)
        audioEventManager = FindFirstObjectByType<AudioEventManager>();

 
    }

    void Update()
    {
        if (audioEventManager == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Calculate how close the player is (0 = far away, 1 = very close)
        float proximity = 1f - Mathf.Clamp01(distance / maxHearingDistance);

        // Get reference to the background music AudioSource
        AudioSource bgmAudioSource = audioEventManager.GetComponent<AudioSource>();

        // Reduce background music volume based on proximity
        bgmAudioSource.volume = Mathf.Lerp(originalBackgroundVolume, minBackgroundVolume, proximity);
    }
}