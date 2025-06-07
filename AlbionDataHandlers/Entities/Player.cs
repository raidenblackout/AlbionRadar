using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for [DisallowNull]

namespace AlbionDataHandlers.Entities
{
    /// <summary>  
    /// Represents the main player entity. Inherits interpolation logic  
    /// from InterpolatableEntity and implements equality checks.  
    /// </summary>  
    public class Player : InterpolatableEntity, IEqualityComparer<Player>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>  
        /// Carries position data from the network parser to the GameStateManager.  
        /// Not used for rendering. Use CurrentLerpedX for rendering.  
        /// </summary>  
        public float PositionX { get; set; }

        /// <summary>  
        /// Carries position data from the network parser to the GameStateManager.  
        /// Not used for rendering. Use CurrentLerpedY for rendering.  
        /// </summary>  
        public float PositionY { get; set; }

        #region IEqualityComparer<Player> Implementation  

        /// <summary>  
        /// Compares two Player objects for equality based on their IDs.  
        /// </summary>  
        public bool Equals(Player? x, Player? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.Id == y.Id;
        }

        /// <summary>  
        /// Generates a hash code for the Player based on its ID.  
        /// </summary>  
        public int GetHashCode(Player obj)
        {
            return obj.Id;
        }

        #endregion
    }
}