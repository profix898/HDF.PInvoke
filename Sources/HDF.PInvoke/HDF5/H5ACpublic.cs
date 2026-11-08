/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Runtime.InteropServices;

using hbool_t = System.Byte;
using size_t = System.IntPtr;

namespace HDF.PInvoke
{
    /// <summary>
    /// Defines the native HDF5 metadata-cache configuration contract.
    /// </summary>
    public unsafe sealed class H5AC
    {
        /// <summary>
        /// Initializes HDF5 before this API is first used.
        /// </summary>
        static H5AC()
        {
            H5.open();
        }

        /// <summary>
        /// The version value required by the native metadata-cache configuration structure.
        /// </summary>
        public const int CURR_CACHE_CONFIG_VERSION = 1;

        /// <summary>
        /// The maximum trace filename length in bytes, excluding the terminating NUL.
        /// </summary>
        public const int MAX_TRACE_FILE_NAME_LEN = 1024;

        /// <summary>
        /// Specifies how parallel ranks distribute metadata-cache writes.
        /// </summary>
        public enum metadata_write_strategy_t : int
        {
            /// <summary>
            /// Only rank zero writes dirty metadata to the file.
            /// </summary>
            PROCESS_0_ONLY = 0,

            /// <summary>
            /// Dirty metadata writes are distributed among participating ranks.
            /// </summary>
            DISTRIBUTED = 1
        }

        /// <summary>
        /// Cache configuration struct used by H5F.[get,set]_mdc_config()
        /// </summary>
        public struct cache_config_t
        {
            /* general configuration fields: */
            /// <summary>
            /// The native configuration structure version; use CURR_CACHE_CONFIG_VERSION.
            /// </summary>
            public int version;

            /// <summary>
            /// Whether to enable cache resize reporting; zero disables it and one enables it.
            /// </summary>
            public hbool_t rpt_fcn_enabled;

            /// <summary>
            /// Whether applying the configuration opens the specified cache trace file.
            /// </summary>
            public hbool_t open_trace_file;

            /// <summary>
            /// Whether applying the configuration closes the current cache trace file.
            /// </summary>
            public hbool_t close_trace_file;

            /// <summary>
            /// The NUL-terminated native byte buffer containing the cache trace filename.
            /// </summary>
            public fixed byte trace_file_name[MAX_TRACE_FILE_NAME_LEN + 1];

            /// <summary>
            /// Whether the cache may evict entries; zero disables eviction and one enables it.
            /// </summary>
            public hbool_t evictions_enabled;

            /// <summary>
            /// Whether initial_size replaces the cache size when this configuration is applied.
            /// </summary>
            public hbool_t set_initial_size;

            /// <summary>
            /// The requested initial cache size in bytes.
            /// </summary>
            public size_t initial_size;

            /// <summary>
            /// The minimum fraction of the cache that should contain clean entries.
            /// </summary>
            public double min_clean_fraction;

            /// <summary>
            /// The maximum allowed cache size in bytes.
            /// </summary>
            public size_t max_size;

            /// <summary>
            /// The minimum allowed cache size in bytes.
            /// </summary>
            public size_t min_size;

            /// <summary>
            /// The number of cache accesses in an automatic-resize evaluation epoch, represented as native C long.
            /// </summary>
            public CLong epoch_length;

            /* size increase control fields: */
            /// <summary>
            /// The policy for increasing the cache size.
            /// </summary>
            public H5C.cache_incr_mode incr_mode;

            /// <summary>
            /// The hit-rate threshold below which the cache may grow.
            /// </summary>
            public double lower_hr_threshold;

            /// <summary>
            /// The multiplicative factor used when growing the cache.
            /// </summary>
            public double increment;

            /// <summary>
            /// Whether max_increment limits each cache-size increase.
            /// </summary>
            public hbool_t apply_max_increment;

            /// <summary>
            /// The maximum permitted size increase in bytes per resize operation.
            /// </summary>
            public size_t max_increment;

            /// <summary>
            /// The policy for increasing cache size in response to a large entry.
            /// </summary>
            public H5C.cache_flash_incr_mode flash_incr_mode;

            /// <summary>
            /// The multiplier used to calculate a flash cache-size increase.
            /// </summary>
            public double flash_multiple;

            /// <summary>
            /// The fraction of the current cache size above which an entry may trigger flash growth.
            /// </summary>
            public double flash_threshold;

            /* size decrease control fields: */
            /// <summary>
            /// The policy for decreasing the cache size.
            /// </summary>
            public H5C.cache_decr_mode decr_mode;

            /// <summary>
            /// The hit-rate threshold above which the cache may shrink.
            /// </summary>
            public double upper_hr_threshold;

            /// <summary>
            /// The multiplicative factor used when shrinking the cache.
            /// </summary>
            public double decrement;

            /// <summary>
            /// Whether max_decrement limits each cache-size decrease.
            /// </summary>
            public hbool_t apply_max_decrement;

            /// <summary>
            /// The maximum permitted size decrease in bytes per resize operation.
            /// </summary>
            public size_t max_decrement;

            /// <summary>
            /// The number of resize epochs an entry may remain unused before age-based eviction.
            /// </summary>
            public int epochs_before_eviction;

            /// <summary>
            /// Whether age-based shrinking retains the empty_reserve fraction of free cache space.
            /// </summary>
            public hbool_t apply_empty_reserve;

            /// <summary>
            /// The fraction of cache space retained as an empty reserve during age-based shrinking.
            /// </summary>
            public double empty_reserve;

            /* parallel configuration fields: */
            /// <summary>
            /// The number of dirty metadata bytes that triggers synchronization in parallel builds.
            /// </summary>
            public size_t dirty_bytes_threshold;

            /// <summary>
            /// The native metadata_write_strategy_t value selecting the parallel metadata-write policy.
            /// </summary>
            public int metadata_write_strategy;

            /// <summary>
            /// Initializes a metadata-cache configuration with the specified version and zero-valued fields; this is not an operational default configuration.
            /// </summary>
            /// <param name="cache_config_version">The native configuration version, normally CURR_CACHE_CONFIG_VERSION.</param>
            public cache_config_t(int cache_config_version)
            {
                version = cache_config_version;

                rpt_fcn_enabled = 0;

                open_trace_file = 0;
                close_trace_file = 0;

                evictions_enabled = 0;

                set_initial_size = 0;
                initial_size = IntPtr.Zero;

                min_clean_fraction = 0.0;

                max_size = IntPtr.Zero;
                min_size = IntPtr.Zero;

                epoch_length = new CLong(0);

                incr_mode = H5C.cache_incr_mode.OFF;

                lower_hr_threshold = 0.0;

                increment = 0.0;

                apply_max_increment = 0;
                max_increment = IntPtr.Zero;

                flash_incr_mode = H5C.cache_flash_incr_mode.OFF;
                flash_multiple = 0.0;
                flash_threshold = 0.0;

                decr_mode = H5C.cache_decr_mode.OFF;

                upper_hr_threshold = 0.0;

                decrement = 0.0;

                apply_max_decrement = 0;
                max_decrement = IntPtr.Zero;

                epochs_before_eviction = 0;

                apply_empty_reserve = 0;
                empty_reserve = 0.0;

                dirty_bytes_threshold = IntPtr.Zero;
                metadata_write_strategy = 0;
            }
        }

        /// <summary>
        /// The structure version required for metadata-cache image configuration.
        /// </summary>
        public const int CURR_CACHE_IMAGE_CONFIG_VERSION = 1;

        /// <summary>
        /// Disables age-based removal of prefetched entries from successive cache images.
        /// </summary>
        public const int CACHE_IMAGE__ENTRY_AGEOUT__NONE = -1;

        /// <summary>
        /// The maximum permitted cache-image entry age-out value.
        /// </summary>
        public const int CACHE_IMAGE__ENTRY_AGEOUT__MAX = 100;

        /// <summary>
        /// Cache image configuration struct used by
        /// H5F.[get,set]_mdc_image_config()
        /// </summary>
        public struct cache_image_config_t
        {
            /// <summary>
            /// The native cache-image configuration structure version.
            /// </summary>
            public int version;

            /// <summary>
            /// Whether closing the file generates a metadata-cache image; zero disables it and one enables it.
            /// </summary>
            public hbool_t generate_image;

            /// <summary>
            /// Requests that resize configuration and state be saved; the native library currently ignores this field.
            /// </summary>
            public hbool_t save_resize_status;

            /// <summary>
            /// The maximum number of successive cache images that may retain an unused prefetched entry.
            /// </summary>
            public int entry_ageout;
        }
    }
}
