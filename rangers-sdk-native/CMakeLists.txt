add_library(rangers-sdk-native SHARED)
add_dependencies(rangers-sdk-native rangers-sdk-csharp)

target_compile_features(rangers-sdk-native PRIVATE cxx_std_20)
target_compile_options(rangers-sdk-native PRIVATE /permissive-)
target_compile_definitions(rangers-sdk-native
    PRIVATE
        RANGERS_SDK_CSHARP_NO_SHIMS=1
)
target_link_libraries(rangers-sdk-native PUBLIC rangers-sdk)
set_target_properties(rangers-sdk-native PROPERTIES
    OUTPUT_NAME "rangers-sdk"
    WINDOWS_EXPORT_ALL_SYMBOLS ON
)
target_sources(rangers-sdk-native
    PRIVATE
        "$<TARGET_PROPERTY:rangers-sdk-csharp,BINARY_DIR>/RangersSDK-symbols.cpp"
        "$<TARGET_PROPERTY:rangers-sdk-csharp,BINARY_DIR>/rangers-sdk.def"
        DllMain.cpp
)
