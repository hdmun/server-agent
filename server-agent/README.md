# server-agent

서버 모니터링용 agent 툴입니다.

윈도우 서비스 프로그램 기반으로 개발되었습니다.

프로그램 기능은 다음과 같습니다.

- 설정된 서버 프로세스를 실행
- 종료시 재실행
- 프로세스 스레드 프리징 감지
- 프로세스 데드락 감지

## 서비스 설치 제거

https://docs.microsoft.com/ko-kr/dotnet/framework/windows-services/how-to-install-and-uninstall-services

### 서비스 설치

```powershell
powershell New-Service -Name "server-agent" -BinaryPathName "BinaryPath"
```

### 서비스 제거

```powershell
powershell Remove-Service -Name "server-agent"
```

### 서비스 프로그램 디버깅

https://docs.microsoft.com/ko-kr/dotnet/framework/windows-services/how-to-debug-windows-service-applications

