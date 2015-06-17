
Website: http://bryanperris.github.io/Soft64-Bryan/

# Notice: #
This is not a fully funtional emulator, its work in progress, and only it runs a basic interpereter.  That means no RCP emulation yet until the CPU is functional and fully tested.

# What Works
* WPF hexadecimal editor works very nicely!
* Unified memory system seems to work nicely too (its thread safe too!)
* Bootloader based on PJ64 and Mupen64P both work correctly
* JSON based configuration is easy to use
* Emulator thread management is simple and OOP, works good, and supports local debugger control
* MIPS interpreter works and executes some implemented common opcodes
* ELF will load and run, but its run environment is very loose, no SysV, etc
* Cartridge rom loader is complete pretty much with some nice methods to detect the rom format
* TLB cache looks good, but I doubt it will be used much
* Logging is awsome with Nlog, all great info printed in WPF richtextbox :)
* We got a tracelog to dump to a file to see how the MIPS executes

# Project Goals for revision 1 #
* Implement high level graphics system
* Implement high level audio system
* Basic controller support
* Full blown WPF based debugger system
* Implement HLE features into the RCP core
* Implement SRAM, and flash memory
* Fully working "Pure" interpreter

# Disclaimer #
You agree to the Soft64 terms when you run Soft64.  By agreeing to these terms, you are fully responsible for the risks with Soft64.  Soft64 does not support any use of unauthorized copyrighted material of any kind.  We will disreguard questions and issues related to software we do not own, we only support the emulator itself.
