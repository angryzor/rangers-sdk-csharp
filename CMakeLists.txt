cmake_minimum_required(VERSION 3.28)
project(rangers-sdk-csharp VERSION 0.0.1
                 DESCRIPTION "C# bindings for rangers-sdk"
                 LANGUAGES CSharp CXX)

if(DEFINED ENV{SONIC_FRONTIERS_SDK})
    message("SONIC_FRONTIERS_SDK environment variable found. Using this version instead of the local submodule.")
    cmake_path(SET RANGERS_SDK_PATH NORMALIZE $ENV{SONIC_FRONTIERS_SDK})
else()
    set(RANGERS_SDK_PATH rangers-sdk)
endif()

set(CPPSHARP_LOCATION CACHE STRING "CppSharp location (bin output dir)")
cmake_path(SET CPPSHARP_LOCATION_NORMALIZED NORMALIZE ${CPPSHARP_LOCATION})

set(CPPSHARP_REFERENCE_ASSEMBLIES CppSharp.AST.dll CppSharp.dll CppSharp.Generator.dll CppSharp.Parser.CSharp.dll CppSharp.Parser.dll)
list(TRANSFORM CPPSHARP_REFERENCE_ASSEMBLIES PREPEND "${CPPSHARP_LOCATION_NORMALIZED}/")
set(CPPSHARP_REFERENCE_ASSEMBLIES_RUNTIME CppSharp.Runtime.dll)
list(TRANSFORM CPPSHARP_REFERENCE_ASSEMBLIES_RUNTIME PREPEND "${CPPSHARP_LOCATION_NORMALIZED}/")

add_subdirectory(${RANGERS_SDK_PATH} rangers-sdk EXCLUDE_FROM_ALL)

add_subdirectory(bindings-generator)
add_subdirectory(rangers-sdk-csharp)
add_subdirectory(rangers-sdk-native)