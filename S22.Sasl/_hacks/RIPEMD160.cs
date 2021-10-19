// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// <OWNER>Microsoft</OWNER>
// 

//
// RIPEMD160.cs
//


namespace S22.Sasl
{


    // https://raw.githubusercontent.com/microsoft/referencesource/master/mscorlib/system/security/cryptography/ripemd160.cs
    [System.Runtime.InteropServices.ComVisible(true)]
    internal abstract class RIPEMD160 
        : System.Security.Cryptography.HashAlgorithm
    {
        //
        // public constructors
        //

        protected RIPEMD160()
        {
            HashSizeValue = 160;
        }

        //
        // public methods
        //

        new static public RIPEMD160 Create()
        {
            return Create("System.Security.Cryptography.RIPEMD160");
        }

        new static public RIPEMD160 Create(string hashName)
        {
            return (RIPEMD160)System.Security.Cryptography.CryptoConfig.CreateFromName(hashName);
        }
    }
}
