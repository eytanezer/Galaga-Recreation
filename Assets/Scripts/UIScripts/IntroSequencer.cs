using System.Collections;
using System.Collections.Generic;
using Scripts.Management;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequencer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Image displayImage;
    [Tooltip("How many game frames to wait before showing the next picture")]
    [SerializeField] private int framesPerPicture;

    [SerializeField] private float secondsPerImage;
    [SerializeField] private bool autoStart;
    [SerializeField] private float imageScale;

    [Header("The Pictures")]
    // Drag your pictures here in the Inspector
    [SerializeField] private List<Sprite> introFrames;
    
    
    private void Start()
    {
        if (displayImage != null)
        {
            displayImage.rectTransform.localPosition = Vector3.zero;
            displayImage.rectTransform.localScale = new Vector3(imageScale, imageScale, 1f);
        }
        
        if (autoStart)
        {
            StartCoroutine(PlaySequence());
        }
    }

    public IEnumerator PlaySequence()
    {
        // loop through every picture in the list
        var wait = new WaitForSeconds(secondsPerImage);

        foreach (Sprite frame in introFrames)
        {
            displayImage.sprite = frame;
            
            // wait for secondsPerImage
            yield return wait;
        }
        
        // yield return new WaitForSeconds(1f);

        // 3. When done, tell the Manager to switch scenes
        Debug.Log("Intro Finished. Switching to Menu.");
        if (GameStateManager.Instance is not null)
        {
            GameStateManager.Instance.GoToMenu();
            // StartCoroutine(GameStateManager.Instance.GoToMenu());
            gameObject.SetActive(false);
        }
    }
    
}
