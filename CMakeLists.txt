cmake_minimum_required(VERSION 3.28)
project(rangers-sdk-csharp VERSION 0.0.1
                 DESCRIPTION "C# bindings for rangers-sdk"
                 LANGUAGES CSharp)

if(DEFINED ENV{SONIC_FRONTIERS_SDK})
    message("SONIC_FRONTIERS_SDK environment variable found. Using this version instead of the local submodule.")
    cmake_path(SET RANGERS_SDK_PATH NORMALIZE $ENV{SONIC_FRONTIERS_SDK})
else()
    set(RANGERS_SDK_PATH rangers-sdk)
endif()

add_subdirectory(${RANGERS_SDK_PATH} rangers-sdk EXCLUDE_FROM_ALL)

add_subdirectory(bindings-generator)
add_subdirectory(rangers-sdk-csharp-wrapper)
add_subdirectory(rangers-sdk-csharp)
