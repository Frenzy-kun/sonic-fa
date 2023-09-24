using System.Collections;
using UnityEngine;
/// <summary>
/// The starpost checkpoint type
/// </summary>
public class StarPostController : CheckpointController
{
    [SerializeField]
    private StarPostHeadController starPostHeadController;
    [Tooltip("The audio played when the spring is touched")]
    public AudioClip starPostTouchedSound;

    [Tooltip("Stars to activate On Touch")]
    public StarRing mStars;
    [Tooltip("Collectible to show")]
    public GameObject mCollectible;

    [Tooltip("Star Post Collectible object")]
    public StarPostCollectible mSPCollectible;

    
    public GameObject mRingDiscount;

    [Header("Identifier for savegames. https://guidgenerator.com/online-guid-generator.aspx")]
    public string id;

    public override void Reset() => this.starPostHeadController = this.GetComponentInChildren<StarPostHeadController>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        mRingDiscount.SetActive(false);

        if (this.starPostHeadController == null)
        {
            this.Reset();
        }
        if (this.CheckPointIsActive())
        {
            this.starPostHeadController.AlreadyActive();

            mStars.gameObject.SetActive(false);

            mSPCollectible.Setup();
        }

        Debug.Assert(id != "", "Save Post doesn't have a correct ID");

        if (GMHistoryManager.instance.GetGeneralData(id) > -1)
        {
            mSPCollectible.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activate the checkpoint on contact with the solid box
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = true; //this.starPostHeadController.GetActivated() == false;

        return triggerAction;
    }

    /// <summary>
    /// Set the activated flag to begin rotation of the starpost head
    /// <param name="player">The player object  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.starPostHeadController.SetActivated(true);
        GMAudioManager.Instance().PlayOneShot(this.starPostTouchedSound);
        this.RegistorCheckPointPosition();

        if (GMRegularStageScoreManager.Instance().GetRingCount() >= 50 && GMHistoryManager.instance.GetGeneralData(id) == -1) //check for rings
        {
            mSPCollectible.Open();
            StartCoroutine(DiscountRings(player.transform.position.y));
            //mStars.gameObject.SetActive(true);
        }

    }

    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        mStars.StartDisableSequence();
    }

    private IEnumerator DiscountRings(float vertical)
    {
        Vector3 position = Vector3.zero;
        position.x = transform.position.x;
        position.y = vertical;

        float t = 0f;
        mRingDiscount.SetActive(true);
        while (t < 2f)
        {
            position.y += Time.deltaTime * 8f;
            t += Time.deltaTime;
            mRingDiscount.transform.position = position;

            yield return null;
        }
        mRingDiscount.SetActive(false);

    }

    internal void ItemCollected()
    {
        GMHistoryManager.instance.SetGeneralData(id, 1);
    }
}
