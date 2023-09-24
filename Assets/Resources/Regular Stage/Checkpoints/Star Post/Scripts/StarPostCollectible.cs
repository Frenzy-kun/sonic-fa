using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPostCollectible : MonoBehaviour
{
    public GameObject mCollectibleVisuals;
    public GameObject mBubble;
    public GameObject mParticles;
    [Tooltip ("Object to collect. Requires collider")]
    public GameObject mCollectibleIcon;
    public GameObject mCollectibleIconFX;


    private void Start()
    {
        Setup();
    }

    internal void Open()
    {
        GMRegularStageScoreManager.Instance().SetRingCount(GMRegularStageScoreManager.Instance().GetRingCount() - 50);
        mBubble.SetActive(false);
        mParticles.SetActive(true);

        mCollectibleVisuals.GetComponent<Collider2D>().enabled = true;

        mCollectibleIcon.GetComponent<Collider2D>().enabled = true;
        mCollectibleIconFX.SetActive(true);
    }

    internal void Setup()
    {
        mCollectibleIcon.GetComponent<Collider2D>().enabled = false;
        mCollectibleVisuals.GetComponent<Collider2D>().enabled = false;
        mBubble.SetActive(true);
        mParticles.SetActive(false);
        mCollectibleIconFX.SetActive(false);
    }
}
