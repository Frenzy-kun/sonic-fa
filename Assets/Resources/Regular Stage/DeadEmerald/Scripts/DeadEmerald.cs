using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEmerald : TriggerContactGimmick
{
    [Serializable]
    public class WaveMovement
    {
        public Vector2 angularSpeed;
        public Vector2 radius;

        internal Vector2 currentAngle;

        public WaveMovement ()
        {
            radius = Vector2.zero;
            radius.x = UnityEngine.Random.Range(5f, 20f);
            radius.y = UnityEngine.Random.Range(5f, radius.x + 1f);

            angularSpeed.x = UnityEngine.Random.Range(2f, 20f);
            angularSpeed.y = UnityEngine.Random.Range(2f, 20f);
            Reset();
        }

        public void Reset()
        {
            currentAngle.x = UnityEngine.Random.Range(0f, 360f);
            currentAngle.y = UnityEngine.Random.Range(0f, 360f);
        }

        public Vector3 Update(float deltaTime, float xmult, float ymult)
        {
            Vector3 offset = Vector2.zero;

            currentAngle.x += deltaTime * angularSpeed.x * xmult;
            currentAngle.y += deltaTime * angularSpeed.y * ymult;

            offset.x = radius.x * Mathf.Cos(currentAngle.x);
            offset.y = radius.y * Mathf.Sin(currentAngle.y);

            return offset;
        }

    }

    public Collider2D mCollider;
    public GameObject mParticles;
    public GameObject mIcon;

    internal List<WaveMovement> mMovements;

    internal Vector3 mTargetPosition;

    internal bool mInitialized = false;

    //movementOptions
    public float maxDistance = 10f;
    float distance = 0f;

    float xangleSpeedMultiplier = 1f;
    float yangleSpeedMultiplier = 1f;

    float rerandomize = 1f;

    internal string savedID = "";
    bool collected = false;

    public void Setup (Vector2 targetPosition, string ID)
    {
        mTargetPosition = targetPosition;
        mCollider.enabled = false;
        mInitialized = false;
        savedID = ID;

        collected = false;
        ResetObject();
    }

    private void ResetObject()
    {
        distance = 0f;
        rerandomize = 1f;

        Rerrandomize();

        float d = 0;
        WaveMovement wm;
        mMovements = new List<WaveMovement>();
        while (d < maxDistance)
        {
            wm = new WaveMovement();
            d += wm.radius.x;
            mMovements.Add(wm);

        }

        /*for (int i = 0; i < mMovements.Count; i++)
        {
            mMovements[i].Reset();
        }*/
    }

    private void Rerrandomize ()
    {
        xangleSpeedMultiplier = UnityEngine.Random.Range(1f, 4f);
        yangleSpeedMultiplier = UnityEngine.Random.Range(1f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 calculatedOffset = Vector3.zero;

        if (distance < 1f)
        {
            distance += Time.unscaledDeltaTime * 0.5f;
        }
        else
        {
            distance = 1f;
            mCollider.enabled = true;
        }

        /*rerandomize -= Time.deltaTime;
        if (rerandomize <= 0f)
        {
            rerandomize = UnityEngine.Random.Range(2f, 10f);
            Rerrandomize();

        }

        xangleSpeed += Time.deltaTime * xangleSpeedMultiplier;
        yangleSpeed += Time.deltaTime * yangleSpeedMultiplier;

        xangle += Time.deltaTime * speed * (Mathf.Cos(xangleSpeed) + 1f);
        yangle += Time.deltaTime * speed * (Mathf.Cos(yangleSpeed) + 1f);



        calculatedOffset.x = distance * Mathf.Cos(xangle);
        calculatedOffset.y = distance * Mathf.Sin(yangle);*/

        rerandomize -= Time.unscaledDeltaTime;
        if (rerandomize <= 0f)
        {
            rerandomize = UnityEngine.Random.Range(2f, 10f);
            Rerrandomize();

        }

        for (int i = 0; i < mMovements.Count; i++)
        {
            calculatedOffset += mMovements[i].Update(Time.unscaledDeltaTime * 0.03f , xangleSpeedMultiplier, yangleSpeedMultiplier);
        }

        transform.position = mTargetPosition + calculatedOffset * distance;
    }

    private void Start()
    {
        mTargetPosition = transform.position;
        ResetObject();
    }

    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = player.GetHealthManager().GetHealthStatus() != HealthStatus.Death && distance >= 1f && savedID != "";

        return triggerAction;
    }

    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        mCollider.enabled = false;
        if (!collected)
        {
            GMRegularStageScoreManager.Instance().IncrementLifeCount(1);
            GMHistoryManager.instance.RemoveRegisteredDeath(savedID);
            mIcon.SetActive(false);
            mParticles.SetActive(true);
            StartCoroutine(CollectSequence(player));
        }
        collected = true;

        
    }

    private IEnumerator CollectSequence(Player player)
    {
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);

    }
}
