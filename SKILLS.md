# ABS/EBS ECU 테스트 프로그램 - C# WinForm 프로젝트

## 프로젝트 개요
- **목적**: KI1A-ABST VB 프로젝트를 기반으로 C# WinForm ABS/EBS ECU 테스트 프로그램 개발
- **참고 자료**:
  - 코드 기준: `E:\범한\bhm-LDWS\KI1A-ABST` (VB6 프로젝트)
  - 프로세스 참고: `자일대우 통신명령 정리_180222.xlsx` - EBS 탭
  - 밸브 파라미터: VB 코드 기준 (자일대우 문서와 상이)

## 기술 스택
- **언어**: C# (.NET Framework 4.8)
- **UI**: WinForms
- **CAN 통신**: VRT_Device.dll (P/Invoke)
- **프로젝트 위치**: `E:\범한\ABS_Tester`

---

## 수동 테스트 순서 (UI 기준)

| 순서 | 항목 | 동작 |
|------|------|------|
| **1** | 연결 | USB 연결 → ECU 연결 |
| **2** | ECU 정보 | 읽기 버튼 |
| **3** | DTC | 읽기 → 삭제 |
| **4** | 전압 | 읽기 버튼 |
| **5** | WSS | 읽기 버튼 |
| **6** | 램프 테스트 | RED → AMBER → VDC → VDC FULLY → HILLHOLDER → 정지 |
| **7** | 밸브 테스트 | FL/FR/RL/RR 증압/감압 → 정지 |
| **8** | Air Gap | 초기화(Front/Rear) → 읽기 |
| **9** | LWS/FBM | LWS → Switch → FBM 시작 → Signal/Force 읽기 → FBM 정지 |

---

## 진행 상황

### Phase 1: 프로젝트 구조 설계 및 기본 셋업
- [x] C# WinForm 프로젝트 생성
- [x] 프로젝트 폴더 구조 설계
- [x] VRT DLL 연동 방식 결정 (P/Invoke)

### Phase 2: CAN 통신 모듈 개발
- [x] VRT DLL Wrapper 클래스 작성 (`Communication/VrtDevice.cs`)
- [x] CAN 메시지 송수신 기능
- [x] 종단저항 (Terminal Resistor/AT) 설정 - VB 코드 `chkAT_MT` 참조
- [ ] ISO-TP (멀티프레임) 처리 - 기본 구현 완료, 추가 테스트 필요

### Phase 3: ECU 통신 프로토콜 구현
- [x] UDS 서비스 구현 (`Protocol/UdsService.cs`)
  - [x] 0x10 - Diagnostic Session Control
  - [x] 0x27 - Security Access (Seed/Key 알고리즘 포함)
  - [x] 0x22 - Read Data By Identifier
  - [x] 0x19 - Read DTC
  - [x] 0x14 - Clear DTC
  - [x] 0x2F - Input Output Control (밸브 제어)
  - [x] 0x31 - Routine Control

### Phase 4: 검사 기능 구현 (`Protocol/EBS5Protocol.cs`)
- [x] ECU 연결 및 Security Access
- [x] ECU 정보 읽기 (ID, 버전, 시리얼 등)
- [x] DTC 읽기/삭제
- [x] 전압 측정
- [x] WSS (휠 속도 센서) 읽기
- [x] Air Gap 초기화/읽기
- [x] 램프 작동 테스트 (EBS RED, AMBER, VDC, VDC FULLY, HILLHOLDER)
- [x] 밸브 테스트 (증압/감압) - VB 코드 기준 파라미터 적용
- [x] SAS 설정
- [x] Steering Angle 읽기
- [x] LWS VALUE 읽기 (브레이크 라이닝 잔량)
- [x] Switch 상태 읽기
- [x] FBM 백업 시작/정지
- [x] FBM Signal 읽기
- [x] FBM 브레이크력 읽기

### Phase 5: UI 개발
- [x] 메인 폼 설계 (`Forms/MainForm.cs`)
- [x] Designer.cs 표준 형식으로 재작성 (Visual Studio Designer 호환)
- [x] 검사 항목별 화면 구성 (순서 번호 표시)
- [x] 로그 표시 기능
- [x] 검사 결과 표시
- [x] 로그 파일 저장 기능 (`Utils/Logger.cs`)
- [x] 자동 테스트 사이클 버튼

### Phase 6: 테스트 및 마무리
- [ ] 실제 ECU 연동 테스트
- [ ] 버그 수정 및 최적화
- [ ] 문서화

---

## 로그 파일 구조

### 경로
```
Logs\
  └── 2026\
      └── 02\
          └── 26\
              ├── ABS_Test_143052.log
              ├── ABS_Test_150230.log
              └── ABS_Test_161445.log
```

### 동작
- **ECU 연결 시** → 로그 파일 자동 생성
- **ECU 연결 해제 시** → 로그 파일 자동 저장
- 수동/자동 테스트 모두 동일 파일에 기록

---

## 핵심 참고 코드 (VB → C# 변환 완료)

### CAN ID 설정
```
ECU_TxID = 0x18DA0BF1
ECU_RxID = 0x18DAF10B
```

### Security Access (Level 3)
- 위치: `Protocol/EBS5Protocol.cs:84` - `PerformSecurityAccess()`
- Seed 요청: `02 27 03`
- Key 전송: `06 27 04 [Key1] [Key2] [Key3] [Key4]`
- Seed/Key 알고리즘: `Protocol/SeedKeyAlgorithm.cs`

### 종단저항 (Terminal Resistor / AT)
- VB 코드: `mod_VRTs.bas` - `mdi_Main.chkAT_MT`
- CAN 모드 설정 시 `Data(N04) = &H1` (On) 또는 `&H0` (Off)
- CAN 버스 양 끝에 120Ω 저항 필요 (신호 반사 방지)

### 밸브 제어 파라미터 (VB 코드 기준)
| 휠 | 증압 명령 | 감압 명령 |
|----|----------|----------|
| FL | `18 2F FD F5 03 04 00 ...` | `18 2F FD F5 03 00 0A ...` |
| FR | `18 2F FD F5 03 00 00 04 00 ...` | `18 2F FD F5 03 00 00 00 0A ...` |
| RL | `18 2F FD F5 03 00 00 00 00 04 00 ...` | `18 2F FD F5 03 00 00 00 00 00 0A ...` |
| RR | `18 2F FD F5 03 00 00 00 00 00 00 04 ...` | `18 2F FD F5 03 00 00 00 00 00 00 00 0A ...` |
| 정지 | `04 2F FD F5 00` | - |

### 램프 제어 (VB 코드 기준)
| 램프 | 명령 |
|------|------|
| EBS RED | `07 2F FD F7 03 01 00 00` |
| EBS AMBER | `07 2F FD F7 03 10 00 00` |
| VDC | `07 2F FD F7 03 40 00 00` |
| VDC FULLY | `07 2F FD F7 03 00 01 00` |
| HILLHOLDER | `07 2F FD F7 03 00 00 01` |
| 정지 | `04 2F FD F7 00` |

### Air Gap (VB 코드 기준)
- 위치: `Protocol/EBS5Protocol.cs` - `InitializeAirGap()`, `ReadAirGap()`
- DID: `0xFDF3`
- 읽기: `03 22 FD F3`
- 파싱: 1 bit = 1/512 * 3.6 km/h

### LWS / FBM (VB 코드 기준)
| 기능 | 명령 | 비고 |
|------|------|------|
| LWS VALUE | `03 22 FD 01` | 브레이크 라이닝 잔량 (1bit=0.4%), 마모센서 전압 (1bit=0.0196V) |
| Switch | `03 22 FD 07` | HILLHOLDER, BLENDING 상태 |
| FBM Signal | `03 22 FD 03` | FBM 신호 전압 (1bit=1/1024V) |
| FBM Force | `03 22 FD F5` | 브레이크력 (1bit=1/1024 bar) |
| FBM 시작 | `04 31 01 FE 55` | Routine Control |
| FBM 정지 | `04 31 02 FE 55` | Routine Control |

---

## 주요 VB 파일 → C# 클래스 매핑

| VB 파일 | 용도 | C# 클래스 |
|---------|------|-----------|
| mod_VRTs.bas | VRT DLL 통신, USB 연결 | VrtDevice.cs |
| mod_EBS5.bas | KNORR EBS5x 프로토콜 | EBS5Protocol.cs |
| mod_ABS8.bas | KNORR ABS8 프로토콜 | ABS8Protocol.cs (TODO) |
| modWABCO.bas | WABCO ABS 프로토콜 | WabcoProtocol.cs (TODO) |
| mod_ABS6.bas | KNORR ABS6 프로토콜 | ABS6Protocol.cs (TODO) |

---

## 자일대우 문서 vs 현재 구현 비교

### 문서 EBS 탭 순서 (참고용)
1. START COMM. & SECURITY ACCESS
2. READ ECU ID
3. READ DTC
4. CLEAR DTC
5. ECU CONFIG.
6. VOLTAGE MEASURE
7. SAS SETTING
8-9. WSS (FL, FR, RL, RR)
10-13. AIR GAP INITIAL/READ
14-19. ACTUATOR (EBS RED, AMBER, VDC, HILLHOLDER, STOP)
20-22. LWS VALUE, S/W, FBM SIGNAL
23-33. VALVE TEST (FL/FR/RL/RR 증압/감압)
34-38. BACK UP, STEERING ANGLE, STOP COMM

### 현재 미구현 항목
- 특별히 없음 (주요 기능 모두 구현 완료)

---

## 변경 이력
| 날짜 | 내용 |
|------|------|
| 2025-02-25 | 프로젝트 계획 수립, SKILLS.md 생성 |
| 2025-02-25 | C# 프로젝트 생성 및 기본 기능 구현 완료 |
| 2025-02-25 | 로그 파일 기능 및 자동 테스트 사이클 추가 |
| 2025-02-26 | Designer.cs 표준 형식으로 재작성 (Visual Studio Designer 호환) |
| 2025-02-26 | 로그 경로 구조 변경: `Logs\yyyy\MM\dd\ABS_Test_HHmmss.log` |
| 2025-02-26 | ECU 연결 시 자동 로그 시작/종료 기능 추가 |
| 2025-02-26 | UI에 수동 테스트 순서 번호 추가 (1-7) |
| 2025-02-26 | 종단저항(AT) 설정 VB 코드 참조 확인 |
| 2025-02-26 | VDC FULLY, HILLHOLDER 램프 테스트 추가 |
| 2025-02-26 | Air Gap 초기화/읽기 기능 추가 |
| 2025-02-26 | LWS VALUE, Switch 상태 읽기 기능 추가 |
| 2025-02-26 | FBM (Foundation Brake Module) 기능 추가 |
| 2025-02-26 | UI에 Air Gap, LWS/FBM 그룹 추가 (테스트 순서 8-9) |
