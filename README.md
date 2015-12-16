# MultiScrollNES
MultiScrollNES, an example multi-directional scroll engine for the Nintendo
Entertainment System (NES). This project is released under the MIT license.
Documentation for the various engine routines can be found in the 'reference'
folder.

# Eightbit.exe
An example map editor that uses meta-tiles (2x2 NES PPU tiles) and meta-cells
(8x8 meta-tiles) to drastically reduce the storage space required by a given
map. Written in C#/XNA. This project is released under the MIT license.

# If6502
A macro interpretter that allows the use of comparison operators, bracket
scoping, and while loops in 6502 asm files:
    .if a >= #240
    {
        ldx delta_hi
        .if x == #$ff
        {
            `add 240
        }
        .else
        {
            `sub 240
        }
    }

Assemble using Ophis: http://michaelcmartin.github.io/Ophis/



Requires a NES emulator with Mapper 28 support:
* Nintendulator v0.975 Beta
* http://www.qmtpro.com/~nes/nintendulator/nintendulator_bin_unicode.zip

