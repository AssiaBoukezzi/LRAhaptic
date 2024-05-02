// HEADER

// INCLUDES

/// <summary>
/// Namespace for Needle Simulator
/// </summary>
namespace Utils.Animation
{

    /// <summary>
    /// Hand type model enum
    /// </summary>
    public enum HandType : int
    {
        MALE = 0,
        FEMALE = 1,
        ABSTRACT = 2,
        ROBOT = 3
    }

    /// <summary>
    /// Skin color for the hand enum
    /// </summary>
    public enum SkinColor : int
    {
        LIGHT = 0,
        MEDIUM = 1,
        DARK = 2
    }

    /// <summary>
    /// User's dominant hand
    /// </summary>
    public enum DominantHand : int
    {
        LEFT = 0,
        RIGHT = 1
    }

    /// <summary>
    /// User hand specifications
    /// </summary>
    public class HandSpecification
    {
        /// <summary>
        /// Model type
        /// </summary>
        public HandType HandType;

        /// <summary>
        /// Skin color
        /// </summary>
        public SkinColor SkinColor;

        /// <summary>
        /// Dominant Hand
        /// </summary>
        public DominantHand DominantHand;
    }
}