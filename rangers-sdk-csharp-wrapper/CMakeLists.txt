add_custom_target(rangers-sdk-csharp-wrapper
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/ijwhost.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/CppSharp.CppParser.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND ${CMAKE_COMMAND} -E "copy" "${CPPSHARP_LOCATION_NORMALIZED}/Std-symbols.dll" "$<TARGET_FILE_DIR:bindings-generator>"
    COMMAND $<TARGET_FILE:bindings-generator> "." $<TARGET_PROPERTY:rangers-sdk,INCLUDE_DIRECTORIES> ${CMAKE_CURRENT_SOURCE_DIR}/entry.h $<TARGET_LINKER_FILE:rangers-sdk>
    BYPRODUCTS RangersSDK.cs
)
