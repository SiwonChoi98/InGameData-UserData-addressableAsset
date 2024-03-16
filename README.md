# 2024년 새 프로젝트

개발인원 : 2명 (기획1, 개발1)

게임장르 : 핵 앤 슬래시

<개발방법>

#### [유저 데이터 저장 방법] 

PlayFab 서버 사용 - 10만 유저 정보까지 무료 제공

저장)

UserData -> Json -> PlayFab Save

불러오기)

PlayFab UserId -> Json -> UserData

#### [인게임 데이터 불러오는 방법] 

Excel -> Csv -> Encrypt -> Local -> InGame Data

방법) 

1. Excel 시트를 Csv (,) 형태로 저장한 이후 Resources/Spec 에 저장
2. CustomEditor -> 암호화 버튼 클릭
3. 게임 시작

인 게임 데이터 까지의 흐름)

1. 게임 실행 시 암호화 된 csv 파일 -> 복호화 진행 -> 복호화된 csv 파일 파싱 진행 -> 각 파일 이름에 맞는 데이터로 삽입 

#### [업데이트 방법 (에셋 사용 방법)]

게임에서 필요한 에셋은 addressable 이용 (일부 제외) 

addressable 에셋은 aws s3 서버에서 받아온다.

방법)

새등록)

1. 사용 에셋은 addressable 등록
2. 등록된 에셋은 New Build 로 파일 저장
3. ServerData에 있는 저장된 파일을 aws s3에 저장

변경)

1. 변경할 에셋을 수정한 이후
2. Update Build를 통해서 파일 수정
3. 수정 파일만 Aws s3에 재 등록
