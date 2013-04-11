@echo Off
set config=%1
if "%config%" == "" (
   set config=debug
)
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild %~dp0\Build\UserStories.proj /p:Configuration="%config%" /v:M /fl /flp:LogFile=userStories.log;Verbosity=Normal /nr:false