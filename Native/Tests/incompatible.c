/* Loader regression fixture, never packaged with HDF5. */
#if defined(_WIN32)
__declspec(dllexport)
#else
__attribute__((visibility("default")))
#endif
int H5get_libversion(unsigned *major, unsigned *minor, unsigned *release)
{
    *major = 1;
    *minor = 10;
    *release = 0;
    return 0;
}
