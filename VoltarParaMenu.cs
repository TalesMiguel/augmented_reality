using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR; 
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class VoltarParaMenu : MonoBehaviour
{
    private float cooldown = 0f;

    void Update()
    {
        if (cooldown > 0) cooldown -= Time.deltaTime;

        // ---------------------------------------------------------
        // 1. CHECAGEM PARA O COMPUTADOR
        // ---------------------------------------------------------
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            Debug.Log("Voltar para Menu acionado pelo TECLADO (M)");
            CarregarMenu();
            return;
        }

        // ---------------------------------------------------------
        // 2. CHECAGEM PARA O QUEST 2
        // ---------------------------------------------------------
        if (cooldown <= 0)
        {
            if (CheckButton(InputDeviceCharacteristics.Left) || 
                CheckButton(InputDeviceCharacteristics.Right))
            {
                Debug.Log("Voltar para Menu acionado pelo CONTROLE VR");
                CarregarMenu();
            }
        }
    }

    bool CheckButton(InputDeviceCharacteristics side)
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        
        InputDevices.GetDevicesWithCharacteristics(side | InputDeviceCharacteristics.Controller, devices);

        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryPressed) && primaryPressed)
                return true;

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out bool menuPressed) && menuPressed)
                return true;
        }
        return false;
    }

    void CarregarMenu()
    {
        cooldown = 2.0f; 
        SceneManager.LoadScene("MenuPrincipal");
    }
}
