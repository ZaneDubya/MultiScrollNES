# MultiScrollNES
MultiScrollNES, an example multi-directional scroll engine for the Nintendo
Entertainment System (NES). This project is released under the MIT license.
References for the various engine routines can be found in the 'documentation'
folder.

## Eightbit.exe
An example map and CHRRAM editor that uses meta-tiles (2x2 NES PPU tiles) and
meta-cells (8x8 meta-tiles) to drastically reduce the storage space required by
a given map. Written in C#/XNA. This project is released under the MIT license.

## If6502.exe
A macro interpretter that allows limited use of comparison operators, bracket
scoping, and while loops in 6502 asm files. This project is released under the
MIT license. An example of usage follows:
````
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
````

Assemble using Ophis: http://michaelcmartin.github.io/Ophis/

Runs on iNES Mapper 1 (MMC1). Also includes code which would allow compilation
for MMC3 and Action 53 (the latter  requires a NES emulator with Mapper 28
support, such as [Nintendulator v0.975 Beta](http://www.qmtpro.com/~nes/nintendulator)).
However, these two have not been updated in some time, and would require updating
their PRGROM bank references to match the latest changes to the MMC1 PRGROMs.