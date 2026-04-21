using DefaultNamespace;
using ManagmentScripts.SoundScripts;
using UnityEngine;

/// <summary>
/// Poolable AudioSource component
/// </summary>
public class AudioSourcePoolable : MonoBehaviour, IPoolable
{
    private AudioSource audioSource;
    public AudioSource Source => audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }
    
    public void Reset()
    {
        audioSource.Stop();
        
        audioSource.pitch = 1f;
        audioSource.volume = 1f;
        audioSource.loop = false;
        
        // gameObject.SetActive(false);
        transform.SetParent(AudioPool.Instance.transform);
        transform.localPosition = Vector3.zero;
    }
}
