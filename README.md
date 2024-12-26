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
-개발 환경 : Windows 11, Unity 6000.0.24f1, Android SDK <br>
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
### Firebase를 이용한 구글, 메일 회원가입 및 로그인 기능

### 클라우드 저장 기능

### AES복호화를 이용한 데이터 복호화

### Google API를 이용한 광고 시스템

### 풀링 오브젝트 시스템

### 포션 제작 & 판매 시스템
