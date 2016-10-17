@echo off

for /F "tokens=3" %%* in ('route print ^| findstr "\<0.0.0.0\>"') do set "gw=%%*"

IF %gw%==%%* (
  echo Error, connot find gateway
  pause
  exit
)

ipconfig /flushdns

@echo on

{{IPs}}route add {{ip}} mask {{mask}} %gw% metric 5

