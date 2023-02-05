using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSelecter : MonoBehaviour
{
    private void OnEnable()
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        btn.Select();
    }
}
