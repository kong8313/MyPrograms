DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
    SELECT 'Setup.TestCertificateName', 'TestCertificateName', 'Setup', 'Test certificate name. Make sense if ''CertificateType'' parameter is Test', 2, 0, 'localhost'
    UNION ALL
    SELECT 'Setup.CertificatePath', 'CertificatePath', 'Setup', 'Path to a certificate file. Make sense if ''CertificateType'' parameter is Real', 2, 0, ''
    UNION ALL
    SELECT 'Setup.EncryptedCertificatePassword', 'EncryptedCertificatePassword', 'Setup', 'Encrypted password of a real certificate. Make sense if ''CertificateType'' parameter is Real', 2, 0, ''
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL

  ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.CertificateName'
  ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.RealCertificateThumbprint'
END


GO
PRINT N'Update complete.';


GO
