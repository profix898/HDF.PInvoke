using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HDF.PInvoke
{
    /// <summary>Provides native event-set lifecycle, completion and error inspection.</summary>
    /// <remarks>Event sets do not own application buffers. Keep all operation buffers and callbacks alive until completion; the packaged native connector executes synchronously.</remarks>
    public unsafe sealed partial class H5ES
    {
        /// <summary>Initializes native HDF5 before using event sets.</summary>
        static H5ES()
        {
            H5.open();
        }

        /// <summary>Requests synchronous execution without tracking an event.</summary>
        public const long NONE = 0;

        /// <summary>Waits until all queued operations complete.</summary>
        public const ulong WAIT_FOREVER = ulong.MaxValue;

        /// <summary>Polls completion without waiting.</summary>
        public const ulong WAIT_NONE = 0;

        /// <summary>Reports the completion state of an operation.</summary>
        public enum status_t
        {
            /// <summary>The operation has not completed.</summary>
            IN_PROGRESS = 0,

            /// <summary>The operation completed successfully.</summary>
            SUCCEED = 1,

            /// <summary>The operation was canceled.</summary>
            CANCELED = 2,

            /// <summary>The operation failed.</summary>
            FAIL = 3
        }

        /// <summary>Contains borrowed operation metadata valid during an event callback.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct op_info_t
        {
            /// <summary>Borrowed UTF-8 API name.</summary>
            public IntPtr api_name;

            /// <summary>Borrowed UTF-8 argument description.</summary>
            public IntPtr api_args;

            /// <summary>Borrowed UTF-8 source filename.</summary>
            public IntPtr app_file_name;

            /// <summary>Borrowed UTF-8 source function name.</summary>
            public IntPtr app_func_name;

            /// <summary>Source line number.</summary>
            public uint app_line_num;

            /// <summary>Operation insertion counter.</summary>
            public ulong op_ins_count;

            /// <summary>Insertion timestamp.</summary>
            public ulong op_ins_ts;

            /// <summary>Execution start timestamp.</summary>
            public ulong op_exec_ts;

            /// <summary>Execution duration in nanoseconds.</summary>
            public ulong op_exec_time;
        }

        /// <summary>Owns error information returned by get_err_info; release its strings and error stack through free_err_info.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct err_info_t
        {
            /// <summary>Borrowed UTF-8 API name.</summary>
            public IntPtr api_name;

            /// <summary>Borrowed UTF-8 argument description.</summary>
            public IntPtr api_args;

            /// <summary>Borrowed UTF-8 source filename.</summary>
            public IntPtr app_file_name;

            /// <summary>Borrowed UTF-8 source function name.</summary>
            public IntPtr app_func_name;

            /// <summary>Source line number.</summary>
            public uint app_line_num;

            /// <summary>Operation insertion counter.</summary>
            public ulong op_ins_count;

            /// <summary>Insertion timestamp.</summary>
            public ulong op_ins_ts;

            /// <summary>Execution start timestamp.</summary>
            public ulong op_exec_ts;

            /// <summary>Execution duration in nanoseconds.</summary>
            public ulong op_exec_time;

            /// <summary>Identifies the error stack owned by this error record.</summary>
            public long err_stack_id;
        }

        /// <summary>Receives a borrowed <see cref="op_info_t"/> pointer for a newly inserted operation; do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int event_insert_t(IntPtr information, IntPtr state);

        /// <summary>Receives a borrowed <see cref="op_info_t"/> pointer, completion status and error stack; do not throw through native code.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int event_complete_t(IntPtr information, status_t status, long errorStack, IntPtr state);

        /// <summary>Creates an event set; wait for operations and then release it with close.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5EScreate"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial long create();

        /// <summary>Waits up to timeout nanoseconds and reports remaining operations and error status.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESwait"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int wait(long events, ulong timeout, out UIntPtr inProgress, out byte errorOccurred);

        /// <summary>Requests cancellation; operations reported as not canceled still require completion before buffers are released.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5EScancel"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int cancel(long events, out UIntPtr notCanceled, out byte errorOccurred);

        /// <summary>Gets the number of active events.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESget_count"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_count(long events, out UIntPtr count);

        /// <summary>Gets the operation insertion counter.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESget_op_counter"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_op_counter(long events, out ulong counter);

        /// <summary>Gets whether an event failed.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESget_err_status"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_err_status(long events, out byte errorOccurred);

        /// <summary>Gets the number of stored errors.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESget_err_count"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_err_count(long events, out UIntPtr count);

        /// <summary>Transfers error records into caller-owned storage sized for capacity err_info_t elements.</summary>
        /// <remarks>Release the cleared records with free_err_info, then free the storage itself. The returned strings and stack identifiers belong to the records.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESget_err_info"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int get_err_info(long events, UIntPtr capacity, IntPtr information, out UIntPtr cleared);

        /// <summary>Releases strings and stack identifiers in count error records, leaving the array storage caller-owned.</summary>
        /// <remarks>Call only when get_err_info returned a positive record count; HDF5 rejects zero.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESfree_err_info"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int free_err_info(UIntPtr count, IntPtr information);

        /// <summary>Registers an insertion callback.</summary>
        /// <remarks>Keep callback and state rooted until replaced or until all operations complete and the event set closes.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESregister_insert_func"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int register_insert_func(long events, event_insert_t callback, IntPtr state);

        /// <summary>Registers a completion callback.</summary>
        /// <remarks>Keep callback and state rooted until replaced or until all operations complete and the event set closes.</remarks>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESregister_complete_func"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int register_complete_func(long events, event_complete_t callback, IntPtr state);

        /// <summary>Closes an event set after pending operations have completed.</summary>
        [LibraryImport(Constants.DLLFileName, EntryPoint = "H5ESclose"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial int close(long events);

    }
}

