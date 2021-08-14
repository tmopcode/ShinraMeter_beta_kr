@echo off
rem msbuild Tera.sln /t:rebuild /p:Configuration=Debug /p:Platform="Any CPU" /fl /flp:logfile=ShinraMeter.log;verbosity=diagnostic
msbuild Tera.sln /p:Configuration=Debug /p:Platform="Any CPU" /fl /flp:logfile=ShinraMeter.log
set output=.\ShinraMeterV
set source=.
set variant=Debug
rmdir /Q /S "%output%"
md "%output%
md "%output%\resources"
md "%output%\resources\config"
md "%output%\lib"

xcopy "%source%\DamageMeter.UI\bin\%variant%\net471" "%output%\" /E
xcopy "%source%\lib" "%output%\lib\" /E
copy "%source%\ReadmeUser.txt" "%output%\readme.txt"
copy "%source%\add_firewall_exception.bat" "%output%\add_firewall_exception.bat"
xcopy "%source%\resources" "%output%\resources\" /E /EXCLUDE:.\exclude.txt
copy "%source%\.git\modules\resources\data\refs\heads\master" "%output%\resources\head"
del "%output%\*.xml"
del "%output%\error.log"
del "%output%\*.vshost*"
del "%output%\resources\data\hotdot\glyph*.tsv"
del "%output%\resources\data\hotdot\abnormal.tsv"
"%source%\Publisher\bin\%variant%\net471\Publisher.exe"
