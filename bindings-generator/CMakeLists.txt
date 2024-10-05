add_executable(bindings-generator)
target_sources(bindings-generator
    PRIVATE
        "Program.cs"
        "TypeMaps/Base.cs"
        "TypeMaps/Math.cs"
        "TypeMaps/Color.cs"
        "TypeMaps/Containers.cs"
        "TypeMaps/Pointers.cs"
        "Passes/CheckBitsetsPass.cs"
        "Passes/ReadonlyContainerFieldsPass.cs"
)
set_target_properties(bindings-generator
    PROPERTIES
        DOTNET_SDK "Microsoft.NET.Sdk"
        DOTNET_TARGET_FRAMEWORK "net8.0"
        VS_DOTNET_REFERENCES "${CPPSHARP_REFERENCE_ASSEMBLIES}"
        # VS_PACKAGE_REFERENCES "CppSharp_1.1.5.3168"
        VS_GLOBAL_PlatformTarget ${CMAKE_GENERATOR_PLATFORM}
)
