add_custom_target(rangers-sdk-csharp-wrapper
    COMMAND $<TARGET_FILE:bindings-generator> "." $<TARGET_PROPERTY:rangers-sdk,INCLUDE_DIRECTORIES> ${CMAKE_CURRENT_SOURCE_DIR}/entry.h $<TARGET_LINKER_FILE:rangers-sdk>
    BYPRODUCTS RangersSDK.cs
)
