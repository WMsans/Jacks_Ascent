using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD;
using UnityEngine;

public class RandomMoveState : BaseState
{
    public Transform player;
    public float flickerOffset = 1.5f;
    public float flickerDelay = 1f;
    public Quaternion endDir;
    public float centipedehealth = 2f;

    public override void OnEnterState()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 finalPos = transform.position;
        Vector3[] offsets = new Vector3[4]
        {
            Vector3.up * flickerOffset,
            Vector3.down * flickerOffset,
            Vector3.left * flickerOffset,
            Vector3.right * flickerOffset
        };
        int[] copyInt = new int[4];

        for (int i = 0; i < offsets.Length; i++)
            offsets[i] = player.TransformDirection(offsets[i]);

        for (int z = 0; z < 4; z++)
        {
            int randIndex = Random.Range(0, offsets.Length);
            if (randIndex == copyInt[0] || randIndex == copyInt[1] || randIndex == copyInt[2])
            {
                z--;
                continue;
            }
            copyInt[z] = randIndex;
        }

        StartCoroutine(FlickerSequence(finalPos, offsets, copyInt));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            centipedehealth--;
            SoundManager.Instance.PlayCentipedeDeathSFX();
            if (centipedehealth <= 0)
            {
                Destroy(gameObject);
                SoundManager.Instance.PlayCentipedeDeathSFX();
                SoundManager.Instance.FlyStop();
            }
        }
    }

    private IEnumerator FlickerSequence(Vector3 finalPos, Vector3[] offsets, int[] copyInt)
    {
        for (int z = 0; z < 4; z++)
        {
            Vector3 flickerPos = finalPos + offsets[copyInt[z]];
            endDir = Quaternion.LookRotation(player.position - flickerPos);
            Owner.transform.DORotateQuaternion(endDir, flickerDelay).SetEase(Ease.InQuad);
            yield return Owner.transform.DOMove(flickerPos, flickerDelay).SetEase(Ease.InQuad).WaitForCompletion();
        }
        for (int z = 0; z < 4; z++)
        {
            Vector3 flickerPos = finalPos + offsets[copyInt[z]];
            endDir = Quaternion.LookRotation(player.position - flickerPos);
            Owner.transform.DORotateQuaternion(endDir, flickerDelay).SetEase(Ease.InQuad);
            yield return Owner.transform.DOMove(flickerPos, flickerDelay).SetEase(Ease.InQuad).WaitForCompletion();
        }
        endDir = Quaternion.LookRotation(player.position - finalPos);
        Owner.transform.DORotateQuaternion(endDir, flickerDelay).SetEase(Ease.InQuad);
        yield return Owner.transform.DOMove(player.position, flickerDelay).SetEase(Ease.InQuad).WaitForCompletion();
        SoundManager.Instance.PlayCentipedeBiteSFX();
        SoundManager.Instance.FlyStop();
        Destroy (Owner.gameObject);
    }
}

