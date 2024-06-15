@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\json
@SET CODE_FOLDER=.\cs
@SET EXE=..\..\..\Tools\excel2json\excel2json.exe

@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for /f "delims=" %%i in ('dir /b /a-d /s %EXCEL_FOLDER%\*.xlsx') do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel %EXCEL_FOLDER%\%%~nxi --json %JSON_FOLDER%\%%~ni.json --csharp %CODE_FOLDER%\%%~ni.cs --header 3
)


@SET DEST_FOLDER_1=..\..\GameServer\bin\Debug\net6.0\Data\Json
@SET DEST_FOLDER_2=..\..\..\MMORPG\Assets\Resources\Json

md %DEST_FOLDER_1%
md %DEST_FOLDER_2%

@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for /r %JSON_FOLDER% %%i in (*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
)

pause