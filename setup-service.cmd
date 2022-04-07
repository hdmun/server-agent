
SET SERVER_AGENT_PATH=%1

sc stop ServerAgent
sc delete ServerAgent
sc create ServerAgent binPath="E:\__hdmun.github.com\server-agent\server-agent\bin\Release\server-agent.exe" start=delayed-auto DisplayName="ServerAgent"
rem IF %errorlevel% EQU 1073 GOTO 
sc start ServerAgent
