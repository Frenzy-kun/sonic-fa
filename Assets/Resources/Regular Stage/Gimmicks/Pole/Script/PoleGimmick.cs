using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleGimmick : TriggerContactGimmick
{
    private Player activePlayer;
    Vector3 mPolePosition;
    bool mHPressed = true;
    bool mVPressed = true;

    public float velocity;

    public bool allowHMovement;
    public bool allowVMovement = true;
    public bool leaveOnHPress = true;
    public bool leaveOnVPress;
    public bool leaveOnJump;
    public Collider2D lockMoveOnbounds;

    public float mSpeed = 10f;

    Vector2 borderX;
    Vector2 borderY;

    Vector2 mSavedVelocity;


    private void Start()
    {
        if (lockMoveOnbounds)
        {
            Bounds b = lockMoveOnbounds.bounds;

            borderX.x = b.min.x;
            borderX.y = b.max.x;

            borderY.x = b.min.y;
            borderY.y = b.max.y;

        }
    }

    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InPole;

        return triggerAction;
    }

    /// <summary>
    /// Set the activated flag to begin rotation of the starpost head
    /// <param name="player">The player object  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.InPole);
        mPolePosition = player.transform.position;
        mPolePosition.x = transform.position.x;
        player.transform.position = mPolePosition;
        player.SetGrounded(false);
        player.GetActionManager().EndCurrentAction();
        this.mSavedVelocity.x = Mathf.Max(Mathf.Abs(player.velocity.x), 5f);
        this.mSavedVelocity.y = Mathf.Max(Mathf.Abs(player.velocity.y), 5f);

        player.velocity = Vector2.zero;

        //player.GetInputManager().SetInputRestriction(InputRestriction.All);//Stop recieving player input
        player.SetMovementRestriction(MovementRestriction.Both);//Restrict both movement

        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Pole);
        this.activePlayer = player;

        this.mHPressed = true;
        this.mVPressed = true;

    }

    private void OnDrawGizmos()
    {
        /*if (lockMoveOnbounds)
        {
            Bounds b = lockMoveOnbounds.bounds;

            borderX.x = b.min.x;
            borderX.y = b.max.x;

            borderY.x = b.min.y;
            borderY.y = b.max.y;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(borderX.x, borderY.x, 0f), new Vector3(borderX.y, borderY.x, 0f));

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new Vector3(borderX.x, borderY.y, 0f), new Vector3(borderX.y, borderY.y, 0f));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(borderX.x, borderY.x, 0f), new Vector3(borderX.x, borderY.y, 0f));

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(new Vector3(borderX.y, borderY.x, 0f), new Vector3(borderX.y, borderY.y, 0f));

        }*/
        
    }

    public override void HedgeOnCollisionStay(Player player)
    {

        


        base.HedgeOnCollisionStay(player);
        if (player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InPole)
        {
            return;
        }

        Vector2 input = player.GetInputManager().GetCurrentInput();

        if (allowHMovement && input.x != 0f)
        {
            mPolePosition.x += mSpeed * (input.x > 0 ? 1f : -1f) * Time.fixedDeltaTime;
        }

        if (allowVMovement && input.y != 0f)
        {
            if (input.y > 0f)
            {
                RaycastHit2D ceilingHit = player.GetSensors().ceilingCollisionInfo.GetCurrentCollisionInfo().GetHit();

                if (ceilingHit && ceilingHit.collider)
                {
                    
                }
                else
                {
                    if (CanGoUp())
                    {
                        mPolePosition.y += mSpeed * Time.fixedDeltaTime;
                    }
                }
            }
            else
            {
                RaycastHit2D groundHit = player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit();
                if (groundHit && groundHit.collider)
                {
                    
                }
                else
                {
                    if (CanGoDown())
                    {
                        mPolePosition.y -= mSpeed * Time.fixedDeltaTime;
                    }
                }
            }
            
        }


        mPolePosition.z = -10f;

        player.transform.position = mPolePosition;
        player.SetGrounded(false);


        if (input.x == 0)
        {
            mHPressed = false;
        }

        if (input.y == 0)
        {
            mVPressed = false;
        }


        
        if (leaveOnHPress && !mHPressed)
        {
            if (input.x != 0)
            {
                
                player.velocity.x = this.mSavedVelocity.x * 1.2f * (input.x > 0 ? 1f : -1f);
                player.velocity.y = 0f;
                LeavePole(player);
            }
        }

        if (leaveOnVPress && !mVPressed)
        {
            if (input.y != 0)
            {

                player.velocity.y = this.mSavedVelocity.y * 1.2f * (input.y > 0 ? 1f : -1f);
                player.velocity.x = 0f;
                LeavePole(player);
            }
        }

        player.GetAnimatorManager().GetAnimator().SetFloat("AnimationMultiplier", Mathf.Abs(this.mSavedVelocity.x * 0.2f));

    }

    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        if (player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InPole)
        {
            return;
        }

        LeavePole(player);
    }


    private void LeavePole(Player player)
    {
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        player.GetInputManager().SetInputRestriction(InputRestriction.None);//Stop recieving player input
        player.SetMovementRestriction(MovementRestriction.None);//Restrict both movement
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.None);

    }

    private bool CanGoUp()
    {
        return (!lockMoveOnbounds || this.activePlayer.transform.position.y < borderY.y);
    }

    private bool CanGoDown()
    {
        return (!lockMoveOnbounds || this.activePlayer.transform.position.y > borderY.x);
    }
}
