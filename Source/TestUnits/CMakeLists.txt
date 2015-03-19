cmake_minimum_required(VERSION 2.8)

if(NOT EXISTS ${MIPSBIN})
	message(FATAL_ERROR "Provide the bin path to your MIPS VR4300 GNU toolchain using -DMIPSBIN= option")
endif()

project(N64TestUnits C)

# Define target names
set (TARGET_ELF_EMPTY "Test_Empty.elf")
set (TARGET_ELF_SIMPLE_ADD "Test_SimpleAdd.elf")

# Workaround to suppress -rdynamic flag
set (CMAKE_SHARED_LIBRARY_LINK_C_FLAGS "")
set (CMAKE_SHARED_LIBRARY_LINK_CXX_FLAGS "")

# Setup compiler stuff
set (CMAKE_INSTALL_PREFIX ${CMAKE_CURRENT_SOURCE_DIR}/../../Binary/)
set (CMAKE_C_FLAGS "-Wl,-emain -nostdlib -v")
set (CMAKE_C_COMPILER ${MIPSBIN}/mips64-gcc)
set (CMAKE_LINKER ${MIPSBIN}/mips64-ld)
set (CMAKE_ASSEMBLER ${MIPSBIN}/mips64-as)

# Compile the executables
add_executable (${TARGET_ELF_EMPTY} empty.c)
add_executable (${TARGET_ELF_SIMPLE_ADD} simple_add.c)

# Setup the install rule
INSTALL(TARGETS ${TARGET_ELF_EMPTY} RUNTIME DESTINATION elfs)
INSTALL(TARGETS ${TARGET_ELF_SIMPLE_ADD} RUNTIME DESTINATION elfs)