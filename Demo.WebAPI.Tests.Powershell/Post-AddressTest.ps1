# Arrange
$baseUri = "http://localhost:5000/api/address"
$address = @{
    line1="Another Line 1";
    line2="More Line 2";
    city="Some City";
    state="ST";
    zip="12345"; 
}
$addressJson = $address | ConvertTo-Json

# Act
$result = Invoke-RestMethod -Method Post -Uri $baseUri -Body $addressJson -ContentType 'application/json'

# Assert
if($result.id -gt 0){Write-Output "Success!"}
else {Write-Error "`nFailure!"}