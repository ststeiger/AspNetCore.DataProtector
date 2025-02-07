﻿/* ===============================================
* Function description: AspNetCore.DataProtection.Base64Encoders
* Creator: WeiGe
* Creation date: 9/12/2018 11:49:08 PM
* ===============================================*/

using System;
using System.Diagnostics;
using System.Globalization;


namespace AspNetCore.DataProtector
{


    /// <summary>
    /// 
    /// </summary>
    internal class Base64Encoders
    {
        private static readonly byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Decodes a base64url-encoded string.
        /// </summary>
        /// <param name="input">The base64url-encoded input to decode.</param>
        /// <returns>The base64url-decoded form of the input.</returns>
        /// <remarks>
        /// The input must not contain any whitespace or padding characters.
        /// Throws <see cref="FormatException"/> if the input is malformed.
        /// </remarks>
        public static byte[] Base64Decode(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Base64Decode(input, offset: 0, count: input.Length);
        }

        /// <summary>
        /// Decodes a base64url-encoded substring of a given string.
        /// </summary>
        /// <param name="input">A string containing the base64url-encoded input to decode.</param>
        /// <param name="offset">The position in <paramref name="input"/> at which decoding should begin.</param>
        /// <param name="count">The number of characters in <paramref name="input"/> to decode.</param>
        /// <returns>The base64url-decoded form of the input.</returns>
        /// <remarks>
        /// The input must not contain any whitespace or padding characters.
        /// Throws <see cref="FormatException"/> if the input is malformed.
        /// </remarks>
        public static byte[] Base64Decode(string input, int offset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ValidateParameters(input.Length, nameof(input), offset, count);

            // Special-case empty input
            if (count == 0)
            {
                return EmptyBytes;
            }

            // Create array large enough for the Base64 characters, not just shorter Base64-URL-encoded form.
            char[] buffer = new char[GetArraySizeRequiredToDecode(count)];

            return Base64Decode(input, offset, buffer, bufferOffset: 0, count: count);
        }


        /// <summary>
        /// Decodes a base64url-encoded <paramref name="input"/> into a <c>byte[]</c>.
        /// </summary>
        /// <param name="input">A string containing the base64url-encoded input to decode.</param>
        /// <param name="offset">The position in <paramref name="input"/> at which decoding should begin.</param>
        /// <param name="buffer">
        /// Scratch buffer to hold the <see cref="char"/>s to decode. Array must be large enough to hold
        /// <paramref name="bufferOffset"/> and <paramref name="count"/> characters as well as Base64 padding
        /// characters. Content is not preserved.
        /// </param>
        /// <param name="bufferOffset">
        /// The offset into <paramref name="buffer"/> at which to begin writing the <see cref="char"/>s to decode.
        /// </param>
        /// <param name="count">The number of characters in <paramref name="input"/> to decode.</param>
        /// <returns>The base64url-decoded form of the <paramref name="input"/>.</returns>
        /// <remarks>
        /// The input must not contain any whitespace or padding characters.
        /// Throws <see cref="FormatException"/> if the input is malformed.
        /// </remarks>
        public static byte[] Base64Decode(string input, int offset, char[] buffer, int bufferOffset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            ValidateParameters(input.Length, nameof(input), offset, count);
            if (bufferOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferOffset));
            }

            if (count == 0)
            {
                return EmptyBytes;
            }

            // Assumption: input is base64url encoded without padding and contains no whitespace.

            int paddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);
            int arraySizeRequired = checked(count + paddingCharsToAdd);
            Debug.Assert(arraySizeRequired % 4 == 0, "Invariant: Array length must be a multiple of 4.");

            if (buffer.Length - bufferOffset < arraySizeRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Invalid {0}, {1} or {2} length.",
                        nameof(count),
                        nameof(bufferOffset),
                        nameof(input)),
                    nameof(count));
            }

            // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
            int i = bufferOffset;
            for (int j = offset; i - bufferOffset < count; i++, j++)
            {
                char ch = input[j];
                if (ch == '-')
                {
                    buffer[i] = '+';
                }
                else if (ch == '_')
                {
                    buffer[i] = '/';
                }
                else
                {
                    buffer[i] = ch;
                }
            }

            // Add the padding characters back.
            for (; paddingCharsToAdd > 0; i++, paddingCharsToAdd--)
            {
                buffer[i] = '=';
            }

            // Decode.
            // If the caller provided invalid base64 chars, they'll be caught here.
            return Convert.FromBase64CharArray(buffer, bufferOffset, arraySizeRequired);
        }

        /// <summary>
        /// Gets the minimum <c>char[]</c> size required for decoding of <paramref name="count"/> characters	 
        /// </summary>
        /// <param name="count">The number of characters to decode.</param>
        /// <returns>
        /// The minimum <c>char[]</c> size required for decoding  of <paramref name="count"/> characters.
        /// </returns>
        public static int GetArraySizeRequiredToDecode(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count == 0)
            {
                return 0;
            }

            int numPaddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);

            return checked(count + numPaddingCharsToAdd);
        }

        /// <summary>
        /// Encodes <paramref name="input"/> using base64url encoding.
        /// </summary>
        /// <param name="input">The binary input to encode.</param>
        /// <returns>The base64url-encoded form of <paramref name="input"/>.</returns>
        public static string Base64Encode(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Base64Encode(input, offset: 0, count: input.Length);
        }

        /// <summary>
        /// Encodes <paramref name="input"/> using base64url encoding.
        /// </summary>
        /// <param name="input">The binary input to encode.</param>
        /// <param name="offset">The offset into <paramref name="input"/> at which to begin encoding.</param>
        /// <param name="count">The number of bytes from <paramref name="input"/> to encode.</param>
        /// <returns>The base64url-encoded form of <paramref name="input"/>.</returns>
        public static string Base64Encode(byte[] input, int offset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ValidateParameters(input.Length, nameof(input), offset, count);

            // Special-case empty input
            if (count == 0)
            {
                return string.Empty;
            }

            char[] buffer = new char[GetArraySizeRequiredToEncode(count)];
            int numBase64Chars = Base64Encode(input, offset, buffer, outputOffset: 0, count: count);

            return new String(buffer, startIndex: 0, length: numBase64Chars);
        }

        /// <summary>
        /// Encodes <paramref name="input"/> using base64url encoding.
        /// </summary>
        /// <param name="input">The binary input to encode.</param>
        /// <param name="offset">The offset into <paramref name="input"/> at which to begin encoding.</param>
        /// <param name="output">
        /// Buffer to receive the base64url-encoded form of <paramref name="input"/>. Array must be large enough to
        /// hold <paramref name="outputOffset"/> characters and the full base64-encoded form of
        /// <paramref name="input"/>, including padding characters.
        /// </param>
        /// <param name="outputOffset">
        /// The offset into <paramref name="output"/> at which to begin writing the base64url-encoded form of
        /// <paramref name="input"/>.
        /// </param>
        /// <param name="count">The number of <c>byte</c>s from <paramref name="input"/> to encode.</param>
        /// <returns>
        /// The number of characters written to <paramref name="output"/>, less any padding characters.
        /// </returns>
        public static int Base64Encode(byte[] input, int offset, char[] output, int outputOffset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            ValidateParameters(input.Length, nameof(input), offset, count);
            if (outputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputOffset));
            }

            int arraySizeRequired = GetArraySizeRequiredToEncode(count);
            if (output.Length - outputOffset < arraySizeRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Invalid {0}, {1} or {2} length.",
                        nameof(count),
                        nameof(outputOffset),
                        nameof(output)),
                    nameof(count));
            }

            // Special-case empty input.
            if (count == 0)
            {
                return 0;
            }

            // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

            // Start with default Base64 encoding.
            int numBase64Chars = Convert.ToBase64CharArray(input, offset, count, output, outputOffset);

            // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
            for (int i = outputOffset; i - outputOffset < numBase64Chars; i++)
            {
                char ch = output[i];
                if (ch == '+')
                {
                    output[i] = '-';
                }
                else if (ch == '/')
                {
                    output[i] = '_';
                }
                else if (ch == '=')
                {
                    // We've reached a padding character; truncate the remainder.
                    return i - outputOffset;
                }
            }

            return numBase64Chars;
        }

        /// <summary>
        /// Get the minimum output <c>char[]</c> size required for encoding <paramref name="count"/>
        /// <see cref="byte"/>s with the <see cref="Base64Encode(byte[], int, char[], int, int)"/> method.
        /// </summary>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>
        /// The minimum output <c>char[]</c> size required for encoding <paramref name="count"/> <see cref="byte"/>s.
        /// </returns>
        public static int GetArraySizeRequiredToEncode(int count)
        {
            int numWholeOrPartialInputBlocks = checked(count + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }

        private static int GetNumBase64PaddingCharsInString(string str)
        {
            // Assumption: input contains a well-formed base64 string with no whitespace.

            // base64 guaranteed have 0 - 2 padding characters.
            if (str[str.Length - 1] == '=')
            {
                if (str[str.Length - 2] == '=')
                {
                    return 2;
                }
                return 1;
            }
            return 0;
        }

        private static int GetNumBase64PaddingCharsToAddForDecode(int inputLength)
        {
            switch (inputLength % 4)
            {
                case 0:
                    return 0;
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    throw new FormatException("TODO: Malformed input.");
            }
        }

        private static void ValidateParameters(int bufferLength, string inputName, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (bufferLength - offset < count)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Invalid {0}, {1} or {2} length.",
                        nameof(count),
                        nameof(offset),
                        inputName),
                    nameof(count));
            }
        }

    }
}
