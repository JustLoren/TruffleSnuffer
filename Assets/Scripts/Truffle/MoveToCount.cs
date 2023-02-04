using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCount : MonoBehaviour
{
    public float moveDelay = .5f;
    public float moveTime = .5f;    
    private void Start()
    {
        StartCoroutine(DoMovement());
    }
    private IEnumerator DoMovement()
    {
        yield return new WaitForSeconds(moveDelay);
        var initialPosition = this.transform.position;
        var moveProgress = 0f;
        while (moveProgress < moveTime)
        {
            moveProgress = Mathf.Clamp(moveProgress + Time.deltaTime, 0, moveTime);
            var movePct = moveProgress / moveTime;

            this.transform.position = Vector3.Lerp(initialPosition, InGameUI.Instance.truffleCount.transform.position, movePct);

            if (movePct == 1f)            
                Destroy(this.gameObject);

            yield return null;
        }
    }
}
