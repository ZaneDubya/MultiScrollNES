REM Make.bat
REM Created for MultiScrollNES by Zane Wagner. (c) 2013.
REM Expects two inputs: 1 = codebase directory, 2 = mapper directory within codebase.

:start
@echo off
REM Make sure Ophis is installed.
ophis >nul 2>nul
if %ERRORLEVEL% neq 9009 goto ophis_exists
echo Ophis is not installed or is not in path.
echo Please download Ophis from http://michaelcmartin.github.io/Ophis/
echo And add the Ophis install directory to the PATH system variable.
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
set mapperdir=%srcdir%\%2
echo %mapperdir%


REM Delete existing work directory and create a new work directory
if not exist %workdir% goto make_work_dir
echo Cleaning OBJ folder...
call utilities/clrdir %workdir%
REM wait while the directory is deleted...
:make_work_dir
if exist %workdir% goto made_work_dir
mkdir %workdir%
:made_work_dir

REM Copy prg/code/data from the source directory to the work directory.
REM PRG Files.
echo|set /p=Copying %mapperdir% PRGROM files...
xcopy %mapperdir% %workdir% /S /q
REM Data files
echo|set /p=Copying Data files... 
mkdir %workdir%\data
xcopy %srcdir%\data %workdir%\data /S /q
REM Code files. We process them with if6502.
echo|set /p=Processing and Copying Code files... 
echo.
utilities\if6502 %srcdir%/code %workdir%/code -all

REM Create make.asm file to combine all assembled PRGs into the final ROM.
if not exist %workdir%\make.asm goto make_make_asm
del %workdir%\make.asm
:make_make_asm
copy /y NUL %workdir%\make.asm >NUL
echo .outfile "bin/%2.nes" >> %workdir%\make.asm
echo .include "header.asm" >> %workdir%\make.asm

REM compile each of the banks in prg.txt, and add each to the make.asm file.
for /f "tokens=1-2 delims=," %%G in (%workdir%/prg.txt) do (
	echo|set /p=%%G: 
	ophis -o "%workdir%/%%H.bin" "%workdir%\%%H.asm"
	echo .incbin "%%H.bin" >> %workdir%\make.asm
)

if not exist bin goto move
call utilities/clrdir bin
echo Cleaning BIN folder...
timeout /t 1 /nobreak >NUL
:move
if exist bin goto made_bin_dir
mkdir bin
:made_bin_dir

REM Combine the banks
echo Combining banks into ROM:
ophis obj/make.asm

choice /c:yn /m "Make again?"
if errorlevel 2 goto :end
if errorlevel 1 goto :start

:end
exit /b