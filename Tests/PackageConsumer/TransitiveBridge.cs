using System;
using System.Runtime.InteropServices;
using HDF.PInvoke;

/// <summary>
/// Exercises C integer types from a .NET Standard 2.0 library so modern consumers validate type forwarding.
/// </summary>
public static class TransitiveBridge
{
    /// <summary>
    /// Reads metadata-cache and object-info structures containing <see cref="CLong"/> and <see cref="CULong"/> fields.
    /// </summary>
    public static ValueTuple<CLong, CULong> ReadCIntegers(long file)
    {
        var config = new H5AC.cache_config_t(H5AC.CURR_CACHE_CONFIG_VERSION);

        if (H5F.get_mdc_config(file, ref config) < 0)
        {
            throw new InvalidOperationException("Transitive H5F.get_mdc_config failed.");
        }

        var info = default(H5O.info_t);

        if (H5O.get_info(file, ref info) < 0)
        {
            throw new InvalidOperationException("Transitive H5O.get_info failed.");
        }

        return new ValueTuple<CLong, CULong>(config.epoch_length, info.fileno);
    }
}
