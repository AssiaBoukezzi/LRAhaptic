// HEADER

// INCLUDES
using UnityEngine;

/// <summary>
/// Namespace for classes with general functionalities
/// </summary>
namespace Utils.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public class RigidHandController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public Transform HandModels;

        /// <summary>
        /// 
        /// </summary>
        public Material[] MaleSkin;

        /// <summary>
        /// 
        /// </summary>
        public Material[] FemaleSkin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handParams"></param>
        public void showUserHand(HandSpecification handParams)
        {
            // render hand model
            string handModel = handParams.HandType + "_" + handParams.DominantHand;
            Transform hand = HandModels.Find(handModel);
            hand.gameObject.SetActive(true);

            // specify skin material
            switch (handParams.HandType)
            {
                case HandType.FEMALE:
                    hand.GetComponentInChildren<Renderer>().material = FemaleSkin[(int)handParams.SkinColor];
                    break;
                case HandType.MALE:
                    hand.GetComponentInChildren<Renderer>().material = MaleSkin[(int)handParams.SkinColor];
                    break;
                default:
                    hand.GetComponentInChildren<Renderer>().material = FemaleSkin[(int)handParams.SkinColor];
                    break;
            }

            // set chosen posture
            hand.GetComponent<HandLerp>().Play();
        }
    }
}