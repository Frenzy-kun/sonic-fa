using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public Player mPlayer;
    public float mMaxOffset = 30f;
    float mOffset = 0f;
    public float mSpeed = 30f;
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mPlayer.currentPlayerDirection < 0)
        {
            if (mOffset > -mMaxOffset)
            {
                mOffset -= Time.deltaTime * mSpeed;
                if (mOffset < -mMaxOffset)
                    mOffset = -mMaxOffset;
            }
        }
        else if (mPlayer.currentPlayerDirection > 0)
        {
            if (mOffset < mMaxOffset)
            {
                mOffset += Time.deltaTime * mSpeed;
                if (mOffset > mMaxOffset)
                    mOffset = mMaxOffset;
            }
        }

        transform.position =  mPlayer.transform.position + Vector3.right * mOffset;
    }
}
