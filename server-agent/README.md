# server-agent

���� ����͸��� agent ���Դϴ�.

������ ���� ���α׷� ������� ���ߵǾ����ϴ�.

���α׷� ����� ������ �����ϴ�.

- ������ ���� ���μ����� ����
- ����� �����
- ���μ��� ������ ����¡ ����
- ���μ��� ����� ����
- ���μ��� ���� ó��

## ���� ��ġ ����

https://docs.microsoft.com/ko-kr/dotnet/framework/windows-services/how-to-install-and-uninstall-services

### ���� ��ġ

```powershell
powershell New-Service -Name "server-agent" -BinaryPathName "BinaryPath"
```

### ���� ����

```powershell
powershell Stop-Service -Name "server-agent"

powershell Remove-Service -Name "server-agent"
```

### ���� ���α׷� �����

https://docs.microsoft.com/ko-kr/dotnet/framework/windows-services/how-to-debug-windows-service-applications


### WebServer

```
netsh http add urlacl url="http://+:80/" user=DOMAIN\user
```
