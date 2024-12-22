## sur5val_Client

#### Server 
- [서버 깃허브](https://github.com/rettytnova/sur5val_Server)
  
#### 프로젝트 간략 소개
- 게임명 : SUR5VAL
- 장르 : 서바이벌 / RPG
- 특징 : 비대칭 pvp 서바이벌과 RPG요소 ( 역할부여, 성장 )를 섞은 게임

---

## 구현 기능 
1. 역할 분배
    - 게임 준비 단계에서 유저들에게 무작위 캐릭터가 서버로부터 주어집니다 (보스 1명 무조건 포함)
2. 라운드 시스템
    - 한 매치를 여러번의 라운드로 구성하였으며 지정한 시간마다 새로운 라운드를 시작합니다.    
3. 스킬, 장비, 아이템
    - 카드 사용 시 카드 유형에 따라(스킬, 장비, 아이템) 효과를 사용합니다.    
4. 상점 구매/판매
    - 매 라운드마다 서버에서 설정한 무작위 8장의 카드(장비, 아이템)가 상점에 있습니다.
    - 카드마다 등급을 주어 라운드가 진행될 수록 높은 등급의 카드가 상점에 등장하게 됩니다.
    - 누군가 카드를 구매하면 해당 카드는 품목에서 사라집니다.
    - 판매 기능을 통해 인벤토리에 있는 카드를 판매할 수 있습니다. 이 때, 판매한 카드가 상점에 다시 등록되지는 않습니다.
5. 몬스터
    - 몬스터는 라운드마다 강해집니다.
    - 생성된 몬스터는 계속해서 무작위 방향으로 이동하며 유저들을 공격합니다.
    - 몬스터를 쓰러뜨릴 시 쓰러뜨린 유저에게 경험치와 골드가 주어집니다.
6. 게임 결과
    - 마지막 라운드에 게임 결과 조건에 해당하는 이벤트 발생 시 알맞는 결과를 출력합니다.
7. 채팅
    - 엔터키를 입력해 채팅을 방에 참여 중인 유저들에게 채팅을 전송할 수 있습니다.

---

#### Code Convention
https://www.notion.so/teamsparta/Code-Convention-1342dc3ef51481b595a5d346dda6fbb1

#### Github Rules
https://www.notion.so/teamsparta/Github-Rules-1342dc3ef51481a9b95fef85d98a80e

---

### 플레이 방법

![image](https://github.com/user-attachments/assets/7abdc5ca-6691-478e-84b9-ab7aaa53e9d3)
![image](https://github.com/user-attachments/assets/79e08f48-b139-4f80-8c51-c6d27653efc1)
![image](https://github.com/user-attachments/assets/4428130b-bc6d-461a-94b2-ef4328ce1d66)
![image](https://github.com/user-attachments/assets/d9daab9a-2406-4e77-bf19-5368f3b28265)
![image](https://github.com/user-attachments/assets/d9a3ccea-2daa-4ffc-8ede-90a8a92d6d79)
![image](https://github.com/user-attachments/assets/87fc4609-b13d-4242-aef6-ccf4b77f5634)
![image](https://github.com/user-attachments/assets/8cbf6669-0120-4ad8-9543-88047e23c5cc)
![image](https://github.com/user-attachments/assets/6789be8c-d3b6-4c33-9d30-f95363215e85)
![image](https://github.com/user-attachments/assets/669dc050-ee43-4467-b412-2f00ef45f034)
![image](https://github.com/user-attachments/assets/6cbdf6fb-cb69-4f11-adb0-7dd3c74f3187)
![image](https://github.com/user-attachments/assets/d05062c3-b785-445f-9ca4-be1634b0e00d)
![image](https://github.com/user-attachments/assets/51e829c2-1941-4fb3-880a-2b95bf87652e)

---
