![Typing SVG](https://readme-typing-svg.demolab.com?font=Fira+Code&size=30&pause=1000&width=435&lines=Potion+Seller)
---
# Description
- **프로젝트 소개** <br>
  구글 플레이 출시를 목적으로 제작한 3D 롤플레잉 게임입니다. <br>
  Google Admob API를 이용하여 게임 내에 보상형 광고를 추가하였으며 <br>
  Firebase 서버를 이용하여 구글, 메일 로그인 기능을 구현했습니다. <br>
  또한 게임 내 데이터를 Firebase의 데이터베이스에 저장하도록 하여 사용자의 기기가 변경되는 상황에도 대처할 수 있도록 하였습니다.


- **개발 기간** : 2024.11.13 - 2024.12.23
- **사용 기술** <br>
-언어 : C#<br>
-엔진 : Unity Engine <br>
-SDK : Firebase Auth, Firbase Firestore, Google Sign in, Google Admob <br>
-개발 환경 : Windows 11, Unity 6000.0.24f1, Android <br>
<br>

---
## 목차
- 기획 의도
- 발생 및 해결한 주요 문제점
- 핵심 로직
<br>

---
## 기획 의도
- 안드로이드 환경에서 안정적으로 구동되는 모바일 게임 개발
- 광고, 회원가입, 로그인, 데이터베이스 저장 시스템 등 실제 서비스 환경 구축
- 모바일 환경에서도 최적화된 성능 제공
<br>

---
## 발생 및 해결한 주요 문제점
### 다양한 SDK 추가로 인한 Plugin 충돌 문제<br>
![image](https://github.com/user-attachments/assets/d207a58c-b250-41fd-b99d-7b917ce3d3a0)

- 구글링을 통해 나오는 모든 방법을 동원해 Gradle 파일과 Android Manifest 파일을 수정 후 빌드 했지만 계속 실패하는 문제가 있었습니다.
- 플러그인을 하나씩 삭제하고 추가하는 것을 반복하며 원인이 되는 plugin이 무엇인지 찾으려했고,<br>
 Google Admob의 문제라는 것을 확인했습니다.
- 정확한 원인을 찾으니 서칭 또한 더욱 정확해지게 되어 결국에는 해결법을 찾을 수 있었습니다.<br>
   Android Manifest에서 AdMob SDK와 Android AdServices를 연결하는 코드를 추가하여 문제를 해결했습니다. 

<br><br>
### Google Sign in 빌드 오류 
- Google Sign in 플러그인 추가 이후 빌드를 하면 **Unable to load DLL 'native-googlesignin'** 메시지가 뜨는 문제가 있었습니다.
- 저와 같은 문제를 겪은 사람이 있는지 찾아보았고<br> Google Signin support 파일을 로드할 수 없어서 뜨는 문제라는 것을 확인했습니다.
  
  ![image](https://github.com/user-attachments/assets/223f2b38-661c-4ad9-9e87-a88f6ca12727)
  
- srcaar 파일의 확장자명을 aar로 바꿔주어 유니티에서 읽어드릴 수 있게 하여 문제를 해결했습니다.
  <br><br>
---

## 핵심 로직
### [![Link](https://img.shields.io/badge/Link-%23181717.svg?&style=for-the-badge&logo=github&logoColor=white)](https://github.com/znlsnel/PotionSeller/blob/main/PROJECT_2024_11_12/Assets/Scripts/Managers/LoginManager.cs) - Firebase를 이용한 구글, 메일 회원가입 및 로그인 기능
- 구글 아이디를 이용하여 로그인 하는 기능과 이메일 형식의 계정으로 회원가입, 로그인 하는 기능을 만들었습니다.
<div style="display: flex; justify-content: space-around;">
  <img src="https://github.com/user-attachments/assets/2f8cd3fe-c7fd-48c3-a578-7aa0a121d190" alt="KakaoTalk_20241230_212921650_01" width="200">
  <img src="https://github.com/user-attachments/assets/8d4efbb1-bf8b-4f25-b370-9b259966d6c6" alt="KakaoTalk_20241230_212921650_02" width="200">
  <img src="https://github.com/user-attachments/assets/2e60af82-07e6-454b-b73f-f031d1f6166e" alt="KakaoTalk_20241230_212921650" width="200">
</div>

<br><br>



### [![Link](https://img.shields.io/badge/Link-%23181717.svg?&style=for-the-badge&logo=github&logoColor=white)](https://github.com/znlsnel/PotionSeller/blob/main/PROJECT_2024_11_12/Assets/Scripts/Managers/DataBase.cs) - 클라우드 저장 기능
- Firebase firestore를 이용하여 데이터를 저장하는 기능을 추가했습니다.
<div style="display: flex; justify-content: space-around;">
  <img src="https://github.com/user-attachments/assets/50ccef01-0ee5-46be-b4ce-97b8f61564d4" alt="Image" width="600">
</div>

<br><br>

### [![Link](https://img.shields.io/badge/Link-%23181717.svg?&style=for-the-badge&logo=github&logoColor=white)](https://github.com/znlsnel/PotionSeller/blob/main/PROJECT_2024_11_12/Assets/Scripts/Managers/Utils.cs) - AES 알고리즘을 이용한 데이터 보호
<div style="display: flex; justify-content: space-around;">
  <img src="https://github.com/user-attachments/assets/128f2e65-d1d2-4c92-baa7-ff4eae578ddc" alt="Image" width="700">
</div>

<br><br>

### [![Link](https://img.shields.io/badge/Link-%23181717.svg?&style=for-the-badge&logo=github&logoColor=white)](https://github.com/znlsnel/PotionSeller/blob/main/PROJECT_2024_11_12/Assets/Scripts/Managers/AdmobManager.cs) - Google API를 이용한 광고 시스템
- Google Admob api를 이용하여 광고 기능을 추가했습니다. <br>
  광고가 중복으로 호출되는 문제가 발생하여 이를 해결하기 위해 lock을 걸어두는 방법을 사용했습니다.
<div style="display: flex; justify-content: space-around;">
  <img src="https://github.com/user-attachments/assets/e9e21993-1aa2-4b71-881e-5a648a4a84e8" alt="Image" width="600">
</div>


