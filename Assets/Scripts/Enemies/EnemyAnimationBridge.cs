using UnityEngine;

namespace Fireball.Enemies
{
    /// <summary>
    /// Put this script on the same GameObject that has the Animator.
    /// It redirects Animation Events to the AI script on the parent.
    /// </summary>
    public class EnemyAnimationBridge : MonoBehaviour
    {
        private RivalMelee meleeAI;
        private RivalRanged rangedAI;

        private void Awake()
        {
            // Look for AI scripts on this object or any parent
            meleeAI = GetComponentInParent<RivalMelee>();
            rangedAI = GetComponentInParent<RivalRanged>();
        }

        // REDIRECT MELEE EVENT
        public void PerformDamage()
        {
            if (meleeAI != null) meleeAI.PerformDamage();
            else if (rangedAI != null) Debug.LogWarning("PerformDamage called on a Ranged enemy. Check your animation events!");
        }

        // REDIRECT RANGED EVENT
        public void PerformShoot()
        {
            if (rangedAI != null) rangedAI.PerformShoot();
            else if (meleeAI != null) Debug.LogWarning("PerformShoot called on a Melee enemy. Check your animation events!");
        }
    }
}
