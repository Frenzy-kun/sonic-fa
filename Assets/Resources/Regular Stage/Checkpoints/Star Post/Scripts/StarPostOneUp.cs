using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPostOneUp : TriggerContactGimmick
{
    public GameObject mCollectParticles;
    public GameObject mIcon;
    public StarPostController mParentController;
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        GetComponent<Collider2D>().enabled = false;
        GMRegularStageScoreManager.Instance().IncrementLifeCount(1);
        mIcon.SetActive(false);
        mCollectParticles.SetActive(true);

        mParentController.ItemCollected();
        StartCoroutine(StartSequence(player));

    }

    private IEnumerator StartSequence (Player player)
    {
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);

    }
}
