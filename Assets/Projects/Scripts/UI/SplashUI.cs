using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private IEnumerator Start()
    {
        _isSuccess = false;
        _isUserData = false;
        _waitMessegeTxt.text = "Update Check...";
        
        _waitMessegeTxt.gameObject.SetActive(true);
        _guestLoginObj.SetActive(false);
        _DownMessege.SetActive(false);
        
        //네트워크가 제대로 작동하고 있는지 체크 (루프)
        
        //어드레서블 초기화
        yield return InitAddressable();
        
        //업데이트 파일 있는지 체크 (AWS 에서 받음)
        yield return CheckUpdateFiles();
        
        //로그인 창 띄움 //게스트 로그인
        
        //로그인 시 정보 있는지 없는지 체크
        
        //씬 이동 가능 상태 버튼/텍스트 활성화
        _guestLoginObj.SetActive(true);
    }

    public void Button_MoveToMain()
    {
        //계정 체크 후 이동
        if(_isSuccess && _isUserData) LoadingManager.LoadScene("Make");
        else if(_isSuccess) LoadingManager.LoadScene("InGame");
    }
        
    //어드레서블 초기화 (혹시나 안될 수 있어서 막기위해 사용)
    private IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }

    #region Check Down 
    
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
            yield return new WaitForSeconds(2f);
            
            //모든 준비가 완료 됐으면

            _waitMessegeTxt.text = "Success!!";
            
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

    #region DownLoad

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
                _isSuccess = true;
                //LoadingManager.LoadScene("InGame");
                break;
            }

            total = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
   
}
