using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = default;
    [SerializeField] private GameObject healthBarParent = default;
    [SerializeField] private Image healthBarImg = default;

    private void Awake()
    {
        health.ClientOnHealthUpdate += HandleHealthBar;
    }
    
    private void OnDestroy()
    {
        health.ClientOnHealthUpdate -= HandleHealthBar;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }
    
    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    private void HandleHealthBar(int currentHealth, int maxHealth)
    {
        healthBarImg.fillAmount = (float)currentHealth / maxHealth;
    }
}
