add_library(rangers-sdk-csharp SHARED)
add_dependencies(rangers-sdk-csharp rangers-sdk-csharp-wrapper)
target_sources(rangers-sdk-csharp
    PRIVATE
        $<PATH:APPEND,$<TARGET_PROPERTY:rangers-sdk-csharp-wrapper,BINARY_DIR>,RangersSDK.cs>
        "Containers.cs"
        "Handle.cs"
        "Delegate.cs"
        "Shims.cs"
        "Interop.cs"
)
target_compile_options(rangers-sdk-csharp PRIVATE /unsafe)
set_target_properties(rangers-sdk-csharp
    PROPERTIES
        DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.2"
        VS_DOTNET_REFERENCES "System;System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;${CPPSHARP_REFERENCE_ASSEMBLIES_RUNTIME}"
        VS_GLOBAL_LangVersion "11.0"
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
