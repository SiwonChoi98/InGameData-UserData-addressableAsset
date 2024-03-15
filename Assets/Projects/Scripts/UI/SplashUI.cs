using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;



public class SplashUI : MonoBehaviour
{
    //priave------------------------------------------
    [Header("UI")]
    [SerializeField] private GameObject _DownMessege;
    [SerializeField] private GameObject _guestLoginObj;
        
    [SerializeField] private Slider _downSlider;
    [SerializeField] private Text _waitMessegeTxt;
    [SerializeField] private Text _sizeInfoTxt;
    [SerializeField] private Text _downValTxt;
    
    private long _patchSize;
    
    //패치의 크기를 저장하고 관리하는 용도
    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();
    
    //씬 이동 상태
    private bool _isSuccess = false;
    private bool _isUserData = false;
    
    //public------------------------------------------
    [Header("Label")] 
    public AssetLabelReference DefaultLabel;
    
    private void Awake()
    {
        //어드레서블 초기화
        StartCoroutine(InitAddressable());
    }

    private IEnumerator Start()
    {
        _isSuccess = false;
        _isUserData = false;
        
        _waitMessegeTxt.text = "Update Check...";
        
        _waitMessegeTxt.gameObject.SetActive(true);
        _guestLoginObj.SetActive(false);
        _DownMessege.SetActive(false);
        
        //네트워크가 제대로 작동하고 있는지 체크 (루프)
        
        //업데이트 파일 있는지 체크 (AWS 에서 받음)
        yield return CheckUpdateFiles();
        _waitMessegeTxt.text = "업데이트 체크 완료";
        
        //스펙 csv 파일 -> 인 스펙 데이터 저장
        yield return DecryptAndParseCSV(); 
        _waitMessegeTxt.text = "스펙데이터 다운로드 완료";
        
        //로그인 창 띄움 //게스트 로그인 (플레이 팹에 계정생성)
        //로그인 시 정보 있는지 없는지 체크
        
        //로그인 데이터 -> 인게임 데이터 저장
        
        //씬 이동 가능 상태 버튼/텍스트 활성화
        _guestLoginObj.SetActive(true);
    }

    //씬 이동
    public void OnClick_MoveToMain()
    {
        //if(Userdata.IsClass == false)
        PlayfabManager.Instance.OnClickGuestLogin();
        //계정 체크 후 이동
        if(_isSuccess && _isUserData) LoadingManager.LoadScene("Make");
        else if(_isSuccess) LoadingManager.LoadScene("InGame");
    }
    
    //게스트 로그인 키 삭제
    public void OnClick_DeletePlayerPrefKey()
    {
        //플레이팹 사이트에서 아이디 삭제 후 이 동작까지 해줘야 함
        if (PlayerPrefs.HasKey(PlayfabManager.Instance.PlayerPrefKey))
        {
            Debug.Log("플레이어 아이디 삭제 되었습니다.");
            PlayerPrefs.DeleteKey(PlayfabManager.Instance.PlayerPrefKey);
        }
    }
    
    //어드레서블 초기화 (혹시나 안될 수 있어서 막기위해 사용)
    private IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        Debug.Log("어드레서블 초기화 완료");
        yield return init;
    }

    #region 패치 파일 체크 
    
    private IEnumerator CheckUpdateFiles()
    {
        var labels = new List<string> { DefaultLabel.labelString };

        _patchSize = default;

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            yield return handle;

            _patchSize += handle.Result;
        }

        //업데이트 항목이 있는지 체크 없으면 바로 인게임
        if (_patchSize > decimal.Zero)
        {
            _waitMessegeTxt.gameObject.SetActive(false);
            _DownMessege.SetActive(true);

            _sizeInfoTxt.text = "" + GetFileSize(_patchSize);
        }
        else
        {
            _downValTxt.text = " 100 % ";
            _downSlider.value = 1f;
            yield return new WaitForSeconds(1f);
            
            //모든 준비가 완료 됐으면
            
            _isSuccess = true;
            _isUserData = true;
            //LoadingManager.LoadScene("InGame");
        }
    }

    private string GetFileSize(long byteCnt)
    {
        string size = "0 Bytes";

        if (byteCnt >= 1073741824.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 073741824.0) + "GB";
        }
        else if (byteCnt >= 1048576.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1048576.0) + "MB";
        }
        else if (byteCnt >= 1024.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1024.0) + "KB";
        }
        else if (byteCnt > 0 && byteCnt < 1024.0)
        {
            size = byteCnt.ToString() + "Bytes";
        }
        
        return size;
    }
    
    #endregion

    #region 패치 파일 다운로드

    //다운로드 버튼
    public void Button_DownLoad()
    {
        StartCoroutine(PatchFiles());
    }

    //패치 파일 저장
    private IEnumerator PatchFiles()
    {
        var labels = new List<string> { DefaultLabel.labelString };

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            yield return handle;

            if (handle.Result != decimal.Zero)
            {
                StartCoroutine(DownLoadLabel(label));
            }
        }

        yield return CheckDownLoad();
    }

    //패치 파일 다운로드
    private IEnumerator DownLoadLabel(string label)
    {
        _patchMap.Add(label, 0);

        var handle = Addressables.DownloadDependenciesAsync(label, false);

        while (!handle.IsDone)
        {
            _patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
            yield return new WaitForEndOfFrame();
        }

        _patchMap[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);
    }
    
    //패치 상태  
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CheckDownLoad()
    {
        var total = 0f;
        _downValTxt.text = " 0 % ";
        
        while (true)
        {
            total += _patchMap.Sum(tmp => tmp.Value);

            _downSlider.value = total / _patchSize;
            _downValTxt.text = (int)(_downSlider.value * 100) + " % ";

            if (total  == _patchSize)
            {
                //패치 이후 다시 체크
                StartCoroutine(Start());
                //LoadingManager.LoadScene("InGame");
                break;
            }

            total = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
   
    #region 복호화, 파싱, 데이터 저장
    public IEnumerator DecryptAndParseCSV()
    {
        TextAsset[] encryptedTextAssets = Resources.LoadAll<TextAsset>("Spec");
        
        foreach (TextAsset encryptedTextAsset in encryptedTextAssets)
        {
            // 암호화된 파일을 복호화하여 메모리에 로드
            byte[] encryptedData = encryptedTextAsset.bytes;
            byte[] key = SpecDataManager._key;
            byte[] iv = new byte[16]; // IV는 암호화 파일의 첫 16바이트에 저장되어 있음
            Array.Copy(encryptedData, iv, iv.Length);
            byte[] encryptedBytes = new byte[encryptedData.Length - iv.Length];
            Array.Copy(encryptedData, iv.Length, encryptedBytes, 0, encryptedBytes.Length);
            
            string decryptedData = DecryptStringFromBytes(encryptedBytes, key, iv);
            
            // 복호화된 데이터를 파싱하여 처리
            ParseCSV(encryptedTextAsset.name, decryptedData);
        }

        return null;
    }
    private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        // 암호화된 데이터를 복호화
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    
    private void ParseCSV(string specTextAssetName,string csvData)
    {
        // CSV 데이터 파싱하여 처리하는 로직을 여기에 추가
        string[] lines = csvData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        SpecDataManager.Instance.SpecToInnerDatas(specTextAssetName, lines);
    }
    
    #endregion
}
