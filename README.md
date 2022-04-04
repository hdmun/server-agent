# server-agent

서버 프로세스 모니터링용 프로젝트입니다.

## Dependency

- .Net Framework 4.8
- NetMQ
- log4net
- Newtonsoft.Json


## 프로젝트 구조

프로젝트 구조에 대해 설명합니다.


### server-agent

메인 프로젝트입니다. 자세한 설명은 `server-agent` 폴더 내 `README.md`를 참고해주세요.


### sql

데이터베이스 쿼리 폴더입니다.

호스트, 서버 프로세스 스키마 및 테스트 데이터 쿼리가 있습니다.


### Tests

`MSTest` 를 사용하는 테스트 프로젝트입니다.

테스트 코드는 이 프로젝트에서 작성합니다.


### TestServer

테스트용 서버 프로젝트입니다.
