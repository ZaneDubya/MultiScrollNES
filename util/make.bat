REM Make.bat
REM Created for Galezie by Zane Wagner. (c) 2013.

:start
@echo off
REM Make sure Ophis is installed.
ophis >nul 2>nul
if %ERRORLEVEL% neq 9009 goto ophis_exists
echo Ophis is not installed or is not in path.
echo Please download Ophis from http://michaelcmartin.github.io/Ophis/
pause
exit /b
:ophis_exists

REM Fail if no source directory has been passed.
if "%1" neq "" (goto has_src_dir)
echo Cannot run this script from the command line.
exit /b
:has_src_dir

REM Set the work directory.
set workdir=obj
set srcdir=%1

REM Delete existing work directory and create a new work directory
if not exist %workdir% goto make_work_dir
rmdir /s /q %workdir%
:make_work_dir
mkdir %workdir%

REM Copy data/code from the source directory to the work directory.
echo|set /p=Copying make files...
copy %srcdir%\prg.txt %workdir%\prg.txt >NUL
echo  done.
echo|set /p=Copying Data files... 
mkdir %workdir%\data
xcopy %srcdir%\data %workdir%\data /S /q
echo|set /p=Copying PRG files... 
mkdir %workdir%\PRG
xcopy %srcdir%\PRG %workdir%\PRG /S /q
util\if6502 %srcdir%/code %workdir%/code -all

REM Create make.asm file to combine all assembled PRGs into the final ROM.
if not exist %workdir%\make.asm goto make_make_asm
del %workdir%\make.asm
:make_make_asm
copy /y NUL %workdir%\make.asm >NUL
echo .outfile "bin/%srcdir%.nes" >> %workdir%\make.asm
echo .include "code/header.asm" >> %workdir%\make.asm

REM compile each of the banks in prg.txt, and add each to the make.asm file.
for /f "tokens=1-2 delims=," %%G in (%workdir%/prg.txt) do (
	echo|set /p=%%G: 
	ophis -o "%workdir%/%%H.bin" "%workdir%\PRG\%%H.asm"
	echo .incbin "%%H.bin" >> %workdir%\make.asm
)

if not exist bin goto move
rmdir /s /q bin
:move
mkdir bin

REM Combine the banks
echo Combining banks into ROM:
ophis obj/make.asm

choice /c:yn /m "Make again?"
if errorlevel 2 goto :end
if errorlevel 1 goto :start

:end
exit /b