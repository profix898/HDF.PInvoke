/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace HDF.PInvoke
{
    /// <summary>
    /// Defines policies for automatically resizing the HDF5 metadata cache.
    /// </summary>
    public unsafe sealed class H5C
    {
        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5C()
        {
            H5.open();
        }

        /// <summary>
        /// Specifies the automatic metadata-cache growth policy.
        /// </summary>
        public enum cache_incr_mode
        {
            /// <summary>
            /// Disables automatic cache growth.
            /// </summary>
            OFF,

            /// <summary>
            /// Grows the cache when its hit rate falls below the configured threshold.
            /// </summary>
            THRESHOLD
        };

        /// <summary>
        /// Specifies the metadata-cache growth policy for unusually large entries.
        /// </summary>
        public enum cache_flash_incr_mode
        {
            /// <summary>
            /// Disables flash cache growth.
            /// </summary>
            OFF,

            /// <summary>
            /// Adds cache space when an entry exceeds the flash-growth threshold.
            /// </summary>
            ADD_SPACE
        };

        /// <summary>
        /// Specifies the automatic metadata-cache shrinking policy.
        /// </summary>
        public enum cache_decr_mode
        {
            /// <summary>
            /// Disables automatic cache shrinking.
            /// </summary>
            OFF,

            /// <summary>
            /// Shrinks the cache when its hit rate exceeds the configured threshold.
            /// </summary>
            THRESHOLD,

            /// <summary>
            /// Shrinks the cache by aging out entries that have not been accessed recently.
            /// </summary>
            AGE_OUT,

            /// <summary>
            /// Combines age-based shrinking with a hit-rate threshold.
            /// </summary>
            AGE_OUT_WITH_THRESHOLD
        };
    }
}
