using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRing : TriggerContactGimmick
{
    public List<GameObject> mStars;
    public Vector3 maxSizes;
    public float mRotationSpeed = 1f;

    public Collider2D mCollider;

    public GameObject mParticles;

    float mCurrentSize;
    float mCurrentAngle = 0f;
    float mDivisor;

    Vector3 mCalculatedPosition;
    Vector3 mCalculatedAngle;
    Vector3 mScale = Vector3.zero;

    bool disable = false;

   
    // Start is called before the first frame update
    void OnEnable()
    {
        this.mDivisor = (2f * Mathf.PI) / (float)this.mStars.Count;
        mCurrentSize = 0f;
        mCurrentAngle = 0f;
        mCalculatedAngle = Vector3.zero;
        mScale = Vector3.zero;
        transform.localScale = mScale;
        disable = false;
        mCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAngle();
        CalculateSize();
        CalculateStarPosition();
        CalculateRotation();
        CalculateScale();
    }

    private void CalculateAngle()
    {
        mCurrentAngle += mRotationSpeed * Time.deltaTime;

        if (mCurrentAngle > 2f * Mathf.PI)
        {
            mCurrentAngle -= 2f * Mathf.PI;
        }
    }

    private void CalculateSize()
    {
        if (!disable)
        {
            if (mCurrentSize < 1f)
            {
                mCurrentSize += Time.deltaTime;
            }
            else
            {
                mCurrentSize = 1f;
            }
        }
        else
        {
            if (mCurrentSize > 0f)
            {
                mCurrentSize -= Time.deltaTime;
            }
            else
            {
                mCurrentSize = 0f;
                gameObject.SetActive(false);
            }
        }
        
    }

    private void CalculateStarPosition()
    {
        for (int i = 0; i < mStars.Count; i++)
        {
            mCalculatedPosition.x = Mathf.Cos(mCurrentAngle + (float)i * mDivisor) * mCurrentSize * maxSizes.x;
            mCalculatedPosition.y = Mathf.Cos(mCurrentAngle + (float)i * mDivisor) * mCurrentSize * maxSizes.y;
            mCalculatedPosition.z = Mathf.Sin(mCurrentAngle + (float)i * mDivisor) * mCurrentSize * maxSizes.z;

            mStars[i].transform.localPosition = mCalculatedPosition;
        }
    }

    private void CalculateRotation()
    {
        /*mCalculatedAngle.z = Mathf.Cos(mCurrentAngle) * 15f;

        transform.rotation = Quaternion.Euler(mCalculatedAngle);*/
    }

    private void CalculateScale()
    {
        if (mScale.x < 1f)
        {
            mScale.x += Time.deltaTime;
            mScale.y += Time.deltaTime;
            mScale.z += Time.deltaTime;
        }
        else
        {
            mScale = Vector3.one;
        }

        transform.localScale = mScale;
    }


    internal void StartDisableSequence()
    {
        mCollider.enabled = false;
        disable = true;
    }


    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.InPole;

        return triggerAction;
    }


    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.Waiting);

        mParticles.gameObject.SetActive(true);
        GMRegularStageScoreManager.Instance().SetRingCount(GMRegularStageScoreManager.Instance().GetRingCount() - 50);
        StartCoroutine(WaitforAnimation(player));
    }

    private IEnumerator WaitforAnimation(Player player)
    {
        
        yield return new WaitForSeconds(1.75f);
        GMRegularStageScoreManager.Instance().IncrementLifeCount(1);
        yield return new WaitForSeconds(1f);
        StartDisableSequence();
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.InPole);

    }
}
