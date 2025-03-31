namespace ChessMonsterTactics
{
    internal static class HashUtility
    {
        /// <summary>
        /// Get 16 bit hash from a string
        /// Note: The hash generated will different for each application domain.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ushort Hash16(string s)
        {
            return Hash16(s.GetHashCode());
        }

        /// <summary>
        /// Get 16 bit hash from a string
        /// Note: The hash generated will different for each application domain.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ushort Hash16(float f)
        {
            return Hash16(f.GetHashCode());
        }

        /// <summary>
        /// Get 16 bit hash from a string
        /// Note: The hash generated will different for each application domain.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ushort Hash16(int i)
        {
            return (ushort)(i.GetHashCode() & 0xffff);
        }
    }
}
