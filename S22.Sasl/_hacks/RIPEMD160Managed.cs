// MIT license.
//Copyright(c) 2000 - 2021 The Legion of the Bouncy Castle Inc. (https://www.bouncycastle.org)

//Permission is hereby granted, free of charge,
//to any person obtaining a copy of this software
//and associated documentation files (the "Software"),
//to deal in the Software without restriction,
//including without limitation the rights to use, copy,
//modify, merge, publish, distribute, sublicense,
//and/or sell copies of the Software, and to permit
//persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice
//shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
//THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace S22.Sasl
{


    internal class RIPEMD160Managed
        : System.Security.Cryptography.HashAlgorithm
    {

        protected RipeMD160Digest m_digest;



        public RIPEMD160Managed()
        {
            HashSizeValue = 160;
            this.m_digest = new RipeMD160Digest();
        }


        public override void Initialize()
        {
            this.m_digest = null;
            this.m_digest = new RipeMD160Digest();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            this.m_digest.BlockUpdate(array, ibStart, cbSize);
        }


        protected override byte[] HashFinal()
        {
            byte[] outArray = new byte[20];
            this.m_digest.DoFinal(outArray, 0);
            return outArray;
        }



        new public static RIPEMD160Managed Create()
        {
            return new RIPEMD160Managed();
        }

        new public static RIPEMD160Managed Create(string hashName)
        {
            return new RIPEMD160Managed();
        }
    }
}
