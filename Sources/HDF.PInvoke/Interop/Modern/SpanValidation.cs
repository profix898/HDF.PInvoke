using System;

namespace HDF.PInvoke
{
    /// <summary>Checks managed buffer boundaries before passing span addresses to length-unaware native APIs.</summary>
    internal static unsafe class SpanValidation
    {
        /// <summary>Requires enough elements for an explicit native count, allowing null optional outputs.</summary>
        internal static void Capacity(int length, ulong required, string parameter, bool optional = false)
        {
            if (!(optional && length == 0) && (ulong)length < required)
            {
                throw new ArgumentException("The span is smaller than the native operation requires.", parameter);
            }
        }

        /// <summary>Checks that a scalar managed value covers its native datatype representation.</summary>
        internal static bool Scalar<T>(long type, string parameter) where T : unmanaged
        {
            var size = H5T.get_size(type).ToInt64();
            if (size <= 0)
            {
                return false;
            }
            Capacity(sizeof(T), (ulong)size, parameter);
            return true;
        }

        /// <summary>Checks a packed buffer in bytes rather than assuming T matches the native datatype size.</summary>
        internal static bool Packed<T>(long type, ulong elements, int length, string parameter) where T : unmanaged
        {
            var size = H5T.get_size(type).ToInt64();
            if (size <= 0)
            {
                return false;
            }
            var available = checked((ulong)length * (ulong)sizeof(T));
            if (elements > available / (ulong)size)
            {
                throw new ArgumentException("The span does not cover the native datatype and element count.", parameter);
            }
            return true;
        }

        /// <summary>Checks storage through the selection bounding endpoint, including sparse selections and offsets.</summary>
        internal static bool Selection<T>(long type, long space, int length, string parameter) where T : unmanaged
        {
            var count = H5S.get_select_npoints(space);
            if (count < 0)
            {
                return false;
            }
            if (count == 0)
            {
                return true;
            }
            var rank = H5S.get_simple_extent_ndims(space);
            if (rank < 0 || H5S.select_valid(space) <= 0)
            {
                return false;
            }
            if (rank == 0)
            {
                return Packed<T>(type, 1, length, parameter);
            }
            // HDF5 caps rank at 32; stack storage keeps the successful validation path allocation-free.
            if (rank > H5S.MAX_RANK)
            {
                return false;
            }
            ulong* dimensions = stackalloc ulong[rank];
            ulong* start = stackalloc ulong[rank];
            ulong* end = stackalloc ulong[rank];
            if (H5S.get_simple_extent_dims(space, dimensions, null) < 0 ||
                H5S.get_select_bounds_native(space, start, end) < 0)
            {
                return false;
            }
            ulong last = 0;
            for (var dimension = 0; dimension < rank; dimension++)
            {
                last = checked(last * dimensions[dimension] + end[dimension]);
            }
            return Packed<T>(type, checked(last + 1), length, parameter);
        }

        /// <summary>Checks an explicit memory selection, packed block or implicit file selection before dataset I/O.</summary>
        internal static bool Dataset<T>(long dataset, long type, long memorySpace, long fileSpace, int length, string parameter) where T : unmanaged
        {
            if (memorySpace == H5S.PLIST || fileSpace == H5S.BLOCK)
            {
                return false;
            }

            if (memorySpace != H5S.ALL && memorySpace != H5S.BLOCK)
            {
                return Selection<T>(type, memorySpace, length, parameter);
            }

            if (fileSpace != H5S.ALL && fileSpace != H5S.PLIST)
            {
                if (memorySpace == H5S.ALL)
                {
                    return Selection<T>(type, fileSpace, length, parameter);
                }

                var selected = H5S.get_select_npoints(fileSpace);
                return selected >= 0 && Packed<T>(type, (ulong)selected, length, parameter);
            }

            // HDF5 does not expose the DXPL selection. The full extent bounds every valid hyperslab.
            var space = H5D.get_space(dataset);
            if (space < 0)
            {
                return false;
            }

            try
            {
                var elements = H5S.get_simple_extent_npoints(space);
                return elements >= 0 && Packed<T>(type, (ulong)elements, length, parameter);
            }
            finally
            {
                H5S.close(space);
            }
        }

        /// <summary>Checks the packed extent of an attribute and releases its temporary dataspace.</summary>
        internal static bool Attribute<T>(long attribute, long type, int length, string parameter) where T : unmanaged
        {
            var space = H5A.get_space(attribute);
            if (space < 0)
            {
                return false;
            }
            try
            {
                var count = H5S.get_simple_extent_npoints(space);
                return count >= 0 && Packed<T>(type, (ulong)count, length, parameter);
            }
            finally
            {
                H5S.close(space);
            }
        }

        /// <summary>Queries dataset rank without leaking the temporary dataspace identifier.</summary>
        internal static int DatasetRank(long dataset)
        {
            var space = H5D.get_space(dataset);
            if (space < 0)
            {
                return -1;
            }
            try
            {
                return H5S.get_simple_extent_ndims(space);
            }
            finally
            {
                H5S.close(space);
            }
        }
    }
}
