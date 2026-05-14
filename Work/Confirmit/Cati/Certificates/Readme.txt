Root certificate was created using following command line.
No password (press the "None" button)

..\_3rdpart\Microsoft\makecert -a SHA256 -len 4096 -cy authority -n "CN=Confirmit CATI Root Test Certificate Sha256" -r -pe -sv "Confirmit CATI Root Test Certificate Sha256.pvk" "Confirmit CATI Root Test Certificate Sha256.cer"

To create a SSL certificate following command line have to be used.

..\_3rdpart\Microsoft\makecert -a SHA256 -len 4096 -sk ConfirmitCATISSLTestCertificateSha256KeyName -eku 1.3.6.1.5.5.7.3.1 -iv "Confirmit CATI Root Test Certificate Sha256.pvk" -n "CN=localhost" -ic "Confirmit CATI Root Test Certificate Sha256.cer" -sr localmachine -ss my -sky exchange -pe localhost.cer

Click Once certificate created using following command line - see !CatiClickOnce.cmd