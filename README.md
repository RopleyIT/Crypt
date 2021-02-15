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
