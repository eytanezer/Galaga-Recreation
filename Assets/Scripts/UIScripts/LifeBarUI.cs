using System.Collections.Generic;
using Scripts.PlayerScriptes;
using UnityEngine;

namespace Scripts.UIScripts
{
    public class LifeBarUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> lifeIcons;
        
        private void OnEnable()
        {
            PlayerHealth.UpdatePlayerHealth += UpdateLifeBar;
        }

        private void OnDisable()
        {
            PlayerHealth.UpdatePlayerHealth -= UpdateLifeBar;
        }
        
        private void UpdateLifeBar(int currentHealth)
        {
            for (int i = 0; i < lifeIcons.Count; i++)
            {
                // If the icon index is less than current health, keep it active
                lifeIcons[i].SetActive(i < currentHealth-1);
            }
        }
    }
}