using UnityEngine;

/// <summary>
/// A generic Singleton class for MonoBehaviours.
/// Example usage: public class GameManager : MonoSingleton<GameManager>
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool _isQuitting = false;
    private static T _instance;


    public static T Instance
    {
        get
        {
            if (_isQuitting) return null;
            
            
            if (_instance is not null)
                return _instance;

            _instance = FindFirstObjectByType<T>();
            if (_instance is null)
            {
                var singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                DontDestroyOnLoad(singletonObject); // Don't destroy the object when loading a new scene
            }

            return _instance;
        }
    }

    // Ensure no other instances can be created by having the constructor as protected
    protected MonoSingleton() { }
    
    protected virtual void Awake()
    {
        if (_instance == null) _instance = this as T;
    }
    
    // IMPORTANT: Reset the flag when a new game starts
    protected virtual void OnEnable() 
    {
        _isQuitting = false;
    }

    // IMPORTANT: Set the flag when the object is destroyed
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _isQuitting = true;
        }
    }
    
    // Double safety
    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}