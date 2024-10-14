add_library(rangers-sdk-csharp SHARED)
add_dependencies(rangers-sdk-csharp rangers-sdk)
message(STATUS "Using CppSharp location ${CPPSHARP_LOCATION_NORMALIZED}")
add_custom_command(
    OUTPUT RangersSDK.cs RangersSDK-symbols.cpp rangers-sdk.def
    COMMAND ${CMAKE_COMMAND} -E "copy_directory" "${CPPSHARP_LOCATION_NORMALIZED}/lib" "$<TARGET_FILE_DIR:bindings-generator>/lib"
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/ijwhost.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/CppSharp.CppParser.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/Std-symbols.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND $<TARGET_FILE:bindings-generator> "." $<TARGET_PROPERTY:rangers-sdk,INCLUDE_DIRECTORIES> ${CMAKE_CURRENT_SOURCE_DIR}/entry.h $<TARGET_LINKER_FILE:rangers-sdk>
    COMMAND ${CMAKE_COMMAND} "-DTHUNKS_FILE=$<TARGET_PROPERTY:rangers-sdk,SOURCE_DIR>/src/thunks.asm" "-DOUTPUT_FILE=rangers-sdk.def" -P "${CMAKE_CURRENT_SOURCE_DIR}/build-def.cmake"
    DEPENDS rangers-sdk bindings-generator
)
target_sources(rangers-sdk-csharp
    PRIVATE
        "RangersSDK.cs"
        "Extensions/GameObject.cs"
        "Extensions/ResourceManager.cs"
        "Interop/InteropIsomorphism.cs"
        "Interop/Memory.cs"
        "Interop/RefTypeReplacement.cs"
        "Replacements/Containers/Array.cs"
        "Replacements/Containers/Constants.cs"
        "Replacements/Containers/Hashmap.cs"
        "Replacements/Containers/InplaceMoveArray.cs"
        "Replacements/Containers/LinkList.cs"
        "Replacements/Containers/MoveArray.cs"
        "Replacements/Containers/MoveArray32.cs"
        "Replacements/Containers/PointerMap.cs"
        "Replacements/Containers/RflArray.cs"
        "Replacements/Containers/StringMap.cs"
        "Replacements/Color.cs"
        "Replacements/InplaceBitArray.cs"
        "Replacements/Pair.cs"
        "Replacements/String.cs"
        "Replacements/VariableString.cs"
        "Reflection/RflClasses.cs"
        "Reflection/GameObjects.cs"
        "Handle.cs"
        "Delegate.cs"
        "Shims.cs"
)
target_compile_options(rangers-sdk-csharp PRIVATE /unsafe)
set_target_properties(rangers-sdk-csharp
    PROPERTIES
        DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.2"
        VS_DOTNET_REFERENCES "System;System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;${CPPSHARP_REFERENCE_ASSEMBLIES_RUNTIME}"
        VS_GLOBAL_LangVersion "11.0"
        VS_GLOBAL_NoWarn "0108"
)
# set_target_properties(rangers-sdk-csharp
#     PROPERTIES
#         DOTNET_SDK "Microsoft.NET.Sdk"
#         DOTNET_TARGET_FRAMEWORK "net8.0"
#         VS_DOTNET_REFERENCES "${CPPSHARP_REFERENCE_ASSEMBLIES_RUNTIME}"
#         VS_GLOBAL_PlatformTarget ${CMAKE_GENERATOR_PLATFORM}
#         VS_GLOBAL_NoWarn "0108"
#         VS_GLOBAL_RunAnalyzersDuringLiveAnalysis "False"
#         VS_GLOBAL_RunAnalyzersDuringBuild "False"
#         VS_GLOBAL_EnableNETAnalyzers "False"
#         VS_GLOBAL_AnalysisLevel "none"
# )
