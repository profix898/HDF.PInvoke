cmake_minimum_required(VERSION 3.26)
project(HDF5InteropProbes LANGUAGES C)
enable_testing()
find_package(HDF5 2.2.0 EXACT CONFIG REQUIRED COMPONENTS C HL shared)
# Header-derived layout contract, checked by the managed smoke test on each RID.
add_executable(hdf5_abi abi.c)
target_link_libraries(hdf5_abi PRIVATE hdf5-shared)
# Windows needs the installed core DLL beside the ABI probe.
if(WIN32)
  add_custom_command(TARGET hdf5_abi POST_BUILD
    COMMAND "${CMAKE_COMMAND}" -E copy_if_different "$<TARGET_FILE:hdf5-shared>" "$<TARGET_FILE_DIR:hdf5_abi>" VERBATIM)
endif()
add_test(NAME native_abi_layout COMMAND hdf5_abi "${CMAKE_BINARY_DIR}/abi-layout.txt")

# A deliberately incompatible library verifies rejection before binding globals.
add_library(incompatible_hdf5 SHARED incompatible.c)
set_target_properties(incompatible_hdf5 PROPERTIES OUTPUT_NAME hdf5
  RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/incompatible/$<CONFIG>"
  LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/incompatible/$<CONFIG>")
