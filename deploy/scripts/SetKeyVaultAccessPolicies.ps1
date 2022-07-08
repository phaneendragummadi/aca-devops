Install-Module -Name Az -Scope AllUsers
Login-AzAccount

# Key vault name
$VaultName = 'dev-we-lab-app-vault'
# ObjectId's  array
$ObjIDs = @("helena.coppieters@codit.eu")
# keys access policies array
#$KeysAP = @("all","get,list,delete","encrypt,decrypt")
# secrets access policies array
$SecretsAP = @("get,list","set,get,list,delete","backup,restore")
Set-AzContext -SubscriptionId "533f6b10-3a70-4c5d-8962-8b51e839a13c"
Set-AzKeyVaultAccessPolicy -VaultName 'dev-we-lab-app-vault' -UserPrincipalName 'Helena.Coppieters@codit.eu' -PermissionsToSecrets set,delete,get -PassThru
Write-Host "*** Granting access policies: "
For ($i = 0; $i -lt $ObjIDs.Length; $i++) {
    Write-Host "*** Create access policy"
    Set-AzKeyVaultAccessPolicy -VaultName $VaultName  -ObjectId $ObjIDs[$i] -PermissionsToSecrets ($SecretsAP[$i]).Split(',')
}

