using UnityEngine;
using System;
public class Cheats : MonoBehaviour
{
    public static event Action OnResetGame;
    public static event Action OnResetPlayerPosition;
    public static event Action OnResetEnemiesSpawning;
    public static event Action OnResetPlayerHealth;
    public static event Action OnStartDance;
    public static event Action OnQuit;

    void Update()
    {
        bool isModifierHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Alpha1)) // reset game
        {
            OnResetGame?.Invoke();
            print("Game Reset");
        }
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Alpha2)) // reset player position
        {
            OnResetPlayerPosition?.Invoke();
            print("Player Position Reset");
        }
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Alpha3)) // reset enemy positions
        {
            OnResetEnemiesSpawning?.Invoke();
            print("Enemy Spawning Reset");
        }
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Alpha4)) // reset score
        {
            OnResetPlayerHealth?.Invoke();
            print("Player Health Reset");
        }
        
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Alpha5)) // reset health
        {
            OnStartDance?.Invoke();
            print("Start Dance");
        }
        
        if (isModifierHeld && Input.GetKeyDown(KeyCode.Q)) // reset health
        {
            OnQuit?.Invoke();
            print("quit");
        }
        
    }
}
