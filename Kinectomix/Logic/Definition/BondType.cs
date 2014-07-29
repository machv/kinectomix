namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Defines arity of the bond.
    /// </summary>
    /// <remarks>
    /// According to the http://en.wikipedia.org/wiki/Sextuple_bond sextuple arity is maximal possible bond arity of one atom.
    /// </remarks>
    public enum BondType
    {
        /// <summary>
        /// No bond.
        /// </summary>
        None = 0,
        /// <summary>
        /// Single bond.
        /// </summary>
        Single = 1,
        /// <summary>
        /// Double bond.
        /// </summary>
        Double = 2,
        /// <summary>
        /// Triple bond.
        /// </summary>
        Triple = 3,
        /// <summary>
        /// Quadruple bond.
        /// </summary>
        Quadruple = 4,
        /// <summary>
        /// Quintuple bond.
        /// </summary>
        Quintuple = 5,
        /// <summary>
        /// Sextuple bond.
        /// </summary>
        Sextuple = 6
    }
}
