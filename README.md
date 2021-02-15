# Crypt
## Overview
Crypt is a program that performs in-place AES256 encryption and decryption of individual files. 
If you have a file that you wish to protect, you can use this program to securely encrypt the file.
Attempting to decrypt with the wrong secret will flag an error indicating that you used the
wrong secret password to decrypt the file.
## Usage
Crypt is a command line tool, suitable for use in scripts. The command format is as follows:

`crypt -e encryptPassword filePath` to encrypt the file.

`crypt -d encryptPassword filePath` to decrypt the file.

The application uses an embedded secure hash to verify that the decryption used the correct
secret. This prevents overwriting the encrypted file with gibberish on use of an incorrect
secret.

### Licensing
This product is published under the [standard MIT License](https://opensource.org/licenses/MIT). The specific wording for this license is as follows:

Copyright 2021 [Ropley Information Technology Ltd.](http://www.ropley.com)
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
