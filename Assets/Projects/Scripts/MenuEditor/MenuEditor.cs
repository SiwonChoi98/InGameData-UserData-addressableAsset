using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PlayFab.ProfilesModels;
using Unity.VisualScripting;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

public class MenuEditor
{
    /// <summary>
    /// 파일경로는 빌드 후 기기에서 다시 체크
    /// </summary>
    private static string csvPath = ".csv";
    private static string csvFilePath = "Assets/Resources/Spec/";
    private static string resourcesLoadAllPath = "Spec/";
    private static string encryptedcsvFilePath = "Assets/Resources/Spec/";

    private static string encryptionKey = "sadmjdnqwj#^";

#if UNITY_EDITOR
    [MenuItem("Custom/CSV 파일 암호화/적용하기")]
    public static void EncryptCSV()
    {
        TextAsset[] textAsset = Resources.LoadAll<TextAsset>(resourcesLoadAllPath);
        
        //암호화된 파일 생성
        for (int i = 0; i < textAsset.Length; i++)
        {
            csvFilePath = "Assets/Resources/Spec/";
            encryptedcsvFilePath = "Assets/Resources/Spec/";
            
            string fileName = textAsset[i].name;
            
            csvFilePath += fileName + csvPath;
            encryptedcsvFilePath += fileName + "Encrypt" + csvPath;

            byte[] key = SpecDataManager._key;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (FileStream fsOutput = new FileStream(encryptedcsvFilePath, FileMode.Create))
                {
                    fsOutput.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                    {
                        using (FileStream fsInput = new FileStream(csvFilePath, FileMode.Open))
                        {
                            fsInput.CopyTo(csEncrypt);
                        }
                    }
                }
            }
            
            Debug.Log(fileName + "암호화 성공");
        }

        //기존 파일 삭제
        for (int i = 0; i < textAsset.Length; i++)
        {
            string fileName = textAsset[i].name;
            
            csvFilePath = "Assets/Resources/Spec/";
            csvFilePath += fileName + csvPath;
            
            File.Delete(csvFilePath);
            
            Debug.Log(fileName + " 기존 파일 삭제");
        }
        
    }
#endif
}
