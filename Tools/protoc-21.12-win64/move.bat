@echo off

for %%i in (out/csharp/*.cs) do ( 
    move out\csharp\%%i ..\..\Common\Proto\
    echo %%i Done
)