using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class InitManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject WaitMessege;
    public GameObject DownMessege;

    public Slider DownSlider;
    public Text SizeInfoTxt;
    public Text DownValTxt;

    [Header("Label")] 
    public AssetLabelReference DefaultLabel;
    private long _patchSize;
    //패치의 크기를 저장하고 관리하는 용도
    private Dictionary<string, long> _patchMap = new Dictionary<string, long>();
    private void Start()
    {
        WaitMessege.SetActive(true);
        DownMessege.SetActive(false);
        
        StartCoroutine(InitAddressable());
        StartCoroutine(CheckUpdateFiles());
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

        //업데이트 항목이 있는지 체크
        if (_patchSize > decimal.Zero)
        {
            WaitMessege.SetActive(false);
            DownMessege.SetActive(true);

            SizeInfoTxt.text = "" + GetFileSize(_patchSize);
        }
        else
        {
            DownValTxt.text = " 100 % ";
            DownSlider.value = 1f;
            yield return new WaitForSeconds(2f);
            LoadingManager.LoadScene("InGame");
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

    public void Button_DownLoad()
    {
        StartCoroutine(PatchFiles());
    }

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

    private IEnumerator CheckDownLoad()
    {
        var total = 0f;
        DownValTxt.text = " 0 % ";
        
        while (true)
        {
            total += _patchMap.Sum(tmp => tmp.Value);

            DownSlider.value = total / _patchSize;
            DownValTxt.text = (int)(DownSlider.value * 100) + " % ";

            if (total  == _patchSize)
            {
                LoadingManager.LoadScene("InGame");
                break;
            }

            total = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
   
}
