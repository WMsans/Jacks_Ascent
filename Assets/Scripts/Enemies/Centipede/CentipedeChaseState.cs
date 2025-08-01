using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CentipedeChaseState : BaseState
{
    [SerializeField] private BaseState randomMoveState;
    public Transform player;
    public float forwardOffset = 5f;
    public float downOffset = 5f;
    public float teleportInterval = 2f;

    public override void OnEnterState()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        var endPos = player.position + Camera.main.transform.forward * forwardOffset + player.up * downOffset;
        SoundManager.Instance.FlyStart();
        StartCoroutine(FacePlayerWhileMoving(teleportInterval));
        Owner.transform.DOMove(endPos, teleportInterval).SetEase(Ease.OutQuad).OnComplete(() =>

        {
            Owner.ChangeState(randomMoveState);
        });
    }
    private IEnumerator FacePlayerWhileMoving(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector3 dir = player.position - transform.position;
            Owner.transform.rotation = Quaternion.LookRotation(dir);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
    
