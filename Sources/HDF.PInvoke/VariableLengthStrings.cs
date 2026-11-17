using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF.PInvoke
{
    public partial class H5D
    {
        /// <summary>Reads an entire variable-length UTF-8 dataset in row-major order. Returns a negative status on native failure.</summary>
        /// <param name="dataset">Open dataset identifier.</param>
        /// <param name="values">Decoded strings on success, or null on native failure.</param>
        /// <exception cref="DecoderFallbackException">A stored string contains invalid UTF-8.</exception>
        /// <returns>A nonnegative status on success or a negative native error code.</returns>
        public static int read_utf8_strings(long dataset, out string[] values) =>
            VariableLengthStrings.Transfer(dataset, false, null, out values);

        /// <summary>Writes one UTF-8 string per dataset element. Null entries represent native null pointers.</summary>
        /// <param name="dataset">Open dataset identifier.</param>
        /// <param name="values">Strings in row-major order, one per extent element.</param>
        /// <exception cref="ArgumentException">The array is null, has the wrong length, or contains an embedded NUL or invalid UTF-16.</exception>
        /// <returns>A nonnegative status on success or a negative native error code.</returns>
        public static int write_utf8_strings(long dataset, string[] values) =>
            VariableLengthStrings.Transfer(dataset, false, values ?? throw new ArgumentNullException(nameof(values)), out _);
    }

    public partial class H5A
    {
        /// <summary>Reads an entire variable-length UTF-8 attribute in row-major order. Returns a negative status on native failure.</summary>
        /// <param name="attribute">Open attribute identifier.</param>
        /// <param name="values">Decoded strings on success, or null on native failure.</param>
        /// <exception cref="DecoderFallbackException">A stored string contains invalid UTF-8.</exception>
        /// <returns>A nonnegative status on success or a negative native error code.</returns>
        public static int read_utf8_strings(long attribute, out string[] values) =>
            VariableLengthStrings.Transfer(attribute, true, null, out values);

        /// <summary>Writes one UTF-8 string per attribute element. Null entries represent native null pointers.</summary>
        /// <param name="attribute">Open attribute identifier.</param>
        /// <param name="values">Strings in row-major order, one per extent element.</param>
        /// <exception cref="ArgumentException">The array is null, has the wrong length, or contains an embedded NUL or invalid UTF-16.</exception>
        /// <returns>A nonnegative status on success or a negative native error code.</returns>
        public static int write_utf8_strings(long attribute, string[] values) =>
            VariableLengthStrings.Transfer(attribute, true, values ?? throw new ArgumentNullException(nameof(values)), out _);
    }

    /// <summary>Owns temporary pointer arrays for synchronous whole-object string transfers.</summary>
    internal static unsafe class VariableLengthStrings
    {
        /// <summary>Rejects malformed input during encoding and decoding.</summary>
        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false, true);

        /// <summary>Owns the temporary dataspace, memory datatype and pinned string slots for one call.</summary>
        internal static int Transfer(long id, bool attribute, string[] input, out string[] values)
        {
            values = null;
            var space = attribute ? H5A.get_space(id) : H5D.get_space(id);
            if (space < 0)
            {
                return -1;
            }

            long type = -1;
            try
            {
                var count = H5S.get_simple_extent_npoints(space);
                if (count < 0)
                {
                    return -1;
                }
                if (input != null && input.LongLength != count)
                    throw new ArgumentException("Supply one string per dataspace element.", nameof(input));

                type = H5T.copy(H5T.C_S1);
                if (type < 0 || H5T.set_size(type, H5T.VARIABLE) < 0 || H5T.set_cset(type, H5T.cset_t.UTF8) < 0)
                {
                    return -1;
                }

                // Even an empty transfer receives a valid pointer to a zeroed slot.
                var pointers = new IntPtr[Math.Max(1, checked((int)count))];
                fixed (IntPtr* slots = pointers)
                {
                    var buffer = (IntPtr)slots;
                    if (input != null)
                    {
                        try
                        {
                            for (var i = 0; i < input.Length; i++)
                            {
                                var value = input[i];
                                if (value == null)
                                {
                                    continue;
                                }
                                if (value.IndexOf('\0') >= 0)
                                {
                                    throw new ArgumentException("HDF5 strings cannot contain NUL.", nameof(input));
                                }
                                var bytes = Encoding.GetBytes(value);
                                pointers[i] = Marshal.AllocHGlobal(checked(bytes.Length + 1));
                                Marshal.Copy(bytes, 0, pointers[i], bytes.Length);
                                Marshal.WriteByte(pointers[i], bytes.Length, 0);
                            }
                            return attribute ? H5A.write(id, type, buffer) : H5D.write(id, type, space, H5S.ALL, H5P.DEFAULT, buffer);
                        }
                        finally
                        {
                            foreach (var pointer in pointers) Marshal.FreeHGlobal(pointer);
                        }
                    }

                    if (!attribute)
                    {
                        return ReadDataset(id, type, space, buffer, pointers, (int)count, out values);
                    }

                    var status = H5A.read(id, type, buffer);
                    if (status < 0)
                    {
                        return status;
                    }

                    string[] decoded;
                    try
                    {
                        decoded = Decode(pointers, (int)count);
                    }
                    finally
                    {
                        status = H5T.reclaim(type, space, H5P.DEFAULT, buffer);
                    }

                    if (status >= 0)
                    {
                        values = decoded;
                    }

                    return status;
                }
            }
            finally
            {
                if (type >= 0)
                {
                    H5T.close(type);
                }
                H5S.close(space);
            }
        }

        /// <summary>Tracks allocations independently of the result slots, which can be incomplete after a failed read.</summary>
        private static int ReadDataset(long dataset, long type, long space, IntPtr buffer, IntPtr[] pointers, int count, out string[] values)
        {
            values = null;
            var transfer = H5P.create(H5P.DATASET_XFER);
            if (transfer < 0)
            {
                return -1;
            }

            IntPtr head = IntPtr.Zero;
            try
            {
                if (H5P.set_vlen_mem_manager(transfer, Allocate, (IntPtr)(&head), DeferFree, IntPtr.Zero) < 0)
                {
                    return -1;
                }

                var status = H5D.read(dataset, type, space, H5S.ALL, transfer, buffer);
                if (status >= 0)
                {
                    values = Decode(pointers, count);
                }

                return status;
            }
            finally
            {
                H5P.close(transfer);
                while (head != IntPtr.Zero)
                {
                    var block = head;
                    head = Marshal.ReadIntPtr(block);
                    Marshal.FreeHGlobal(block);
                }
            }
        }

        /// <summary>Root the native allocator and release callback for the lifetime of the binding.</summary>
        private static readonly H5MM.allocate_t Allocate = AllocateBlock;
        private static readonly H5MM.free_t DeferFree = DeferRelease;

        /// <summary>Prepends a pointer-sized ownership record to each native string allocation.</summary>
        private static IntPtr AllocateBlock(IntPtr size, IntPtr context)
        {
            var length = size.ToInt64();
            if (length < 0 || length > long.MaxValue - IntPtr.Size)
            {
                return IntPtr.Zero;
            }

            try
            {
                var block = Marshal.AllocHGlobal(new IntPtr(checked(length + IntPtr.Size)));
                Marshal.WriteIntPtr(block, Marshal.ReadIntPtr(context));
                Marshal.WriteIntPtr(context, block);
                return IntPtr.Add(block, IntPtr.Size);
            }
            catch (OutOfMemoryException)
            {
                return IntPtr.Zero;
            }
            catch (OverflowException)
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>The synchronous read releases every block together after decoding or native failure.</summary>
        private static void DeferRelease(IntPtr memory, IntPtr context)
        {
        }

        /// <summary>Copies native UTF-8 strings into managed storage while preserving null slots.</summary>
        private static string[] Decode(IntPtr[] pointers, int count)
        {
            var decoded = new string[count];
            for (var i = 0; i < count; i++)
            {
                if (pointers[i] == IntPtr.Zero)
                {
                    continue;
                }

                var bytes = (byte*)pointers[i];
                var length = 0;
                while (bytes[length] != 0)
                {
                    length = checked(length + 1);
                }

                decoded[i] = Encoding.GetString(bytes, length);
            }

            return decoded;
        }
    }
}
