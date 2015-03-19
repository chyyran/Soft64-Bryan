echo "Setting up Binary directory"
xcopy %1\Resources\BinaryFiles\* %1\Binary /c /f /y /r /s /k /i
echo ".... DONE!"
