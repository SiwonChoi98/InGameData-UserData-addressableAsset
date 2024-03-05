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

public class MenuEditor
{
    public static string csvPath = ".csv";
    public static string csvFilePath = "Assets/Resources/Spec/monster" + csvPath;
    public static string encryptedcsvFilePath = "Assets/Resources/Spec/monster" + "Encrypt" + csvPath;

    private static string encryptionKey = "aksn&%#!wjsaghws#$$%%@lwjka";
    
    
    [MenuItem("Custom/CSV 파일 암호화/적용하기")]
    public static void EncryptCSV()
    {
        byte[] keyBytes = new byte[32];
        RandomNumberGenerator.Create().GetBytes(keyBytes);
        string encryptionKey = Convert.ToBase64String(keyBytes);
        
        //기존 암호화 안된 파일 삭제 후 암호화 적용
        //File.Delete(csvFilePath);
        
        byte[] key = Convert.FromBase64String(encryptionKey);
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
        
        
        Debug.Log("암호화 success");
    }

    [MenuItem("Custom/CSV 파일 복호화/적용하기")]
    public static void DecryptCSV()
    {
        byte[] keyBytes = new byte[32];
        RandomNumberGenerator.Create().GetBytes(keyBytes);
        encryptionKey = Convert.ToBase64String(keyBytes);
        
        byte[] key = Convert.FromBase64String(encryptionKey);
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            using (FileStream fsInput = new FileStream(encryptedcsvFilePath, FileMode.Open))
            {
                fsInput.Read(iv, 0, iv.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (CryptoStream csDecrypt = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                {
                    using (FileStream fsOutput = new FileStream(csvFilePath, FileMode.Create))
                    {
                        csDecrypt.CopyTo(fsOutput);
                    }
                }
            }
        }
        Debug.Log("복호화 success");
    }
    
    // JSON 파일 저장 경로
    public static string jsonFilePath = "Assets/Resources/Spec/monster.csv";
    
    [MenuItem("Custom/CSV To Json/적용하기")]
    public static void ConvertCSVToJson()
    {
        // CSV 파일의 모든 행을 저장할 리스트 생성
        List<Dictionary<string, object>> csvData = new List<Dictionary<string, object>>();

        // CSV 파일 읽기
        string[] csvLines = File.ReadAllLines(csvFilePath);

        // CSV 파일의 첫 번째 행을 헤더로 사용
        string[] headers = csvLines[0].Split(',');

        // CSV 파일의 나머지 행을 읽어들임
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] values = csvLines[i].Split(',');
            Dictionary<string, object> row = new Dictionary<string, object>();

            // 각 행의 값을 헤더에 맞게 딕셔너리에 추가
            for (int j = 0; j < headers.Length; j++)
            {
                // 필요에 따라 값 형식을 적절히 변환하여 저장할 수 있음
                row.Add(headers[j], values[j]);
            }

            // 딕셔너리를 리스트에 추가
            csvData.Add(row);
        }

        // JSON 파일로 변환하여 저장
        string jsonData = JsonConvert.SerializeObject(csvData, Formatting.Indented);
        File.WriteAllText(jsonFilePath, jsonData);

        Debug.Log("CSV 파일이 JSON으로 성공적으로 변환되었습니다.");
    }
    
    
}
