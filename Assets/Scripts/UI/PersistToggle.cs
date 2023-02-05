using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistToggle : MonoBehaviour
{
    public string key;
    public UnityEngine.UI.Toggle toggle;

    private void Start()
    {
        toggle.isOn = PlayerPrefs.GetInt(key, toggle.isOn ? 1 : 0) == 1;
        toggle.onValueChanged.AddListener(UpdatePrefs);
    }

    private void UpdatePrefs(bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
