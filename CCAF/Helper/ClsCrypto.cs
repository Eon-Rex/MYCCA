﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CCAF.Helper
{
    //*
    //Auther : Abhishek Dheeman
    //Date :  09-Jun-2022
    //Ref : Web Application Security Assessment [V001] - Sensitive Data Exposure
    //
    //
    //*//

    public class ClsCrypto
    {
        private RijndaelManaged myRijndael = new RijndaelManaged();
        private int iterations;
        private byte[] salt;

        public ClsCrypto(string strPassword)
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = HexStringToByteArray("e84ad660c4721ae0e84ad660c4721ae0");

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            iterations = 1000;
            salt = System.Text.Encoding.UTF8.GetBytes("insight123resultxyz");
            myRijndael.Key = GenerateKey(strPassword);
        }

        public string Encrypt(string strPlainText)
        {
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
            byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(originalBytes);
        }

        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        private byte[] GenerateKey(string strPassword)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

            return rfc2898.GetBytes(128 / 8);
        }
    }
}