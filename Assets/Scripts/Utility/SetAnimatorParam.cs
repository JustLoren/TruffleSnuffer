using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorParam : MonoBehaviour
{
    public Animator animator;
    public string paramName;
    public float paramValue;

    private void Awake()
    {
        animator.SetFloat(paramName, paramValue);
    }
}
