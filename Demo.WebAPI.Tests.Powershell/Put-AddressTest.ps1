# Arrange
$baseUri = "http://localhost:5000/api/address"
$id = 1
$address = @{
    id=$id;
    line1="Another Line 1";
    line2="More Line 2";
    city="Some City";
    state="ST";
    zip="12345"; 
}
$addressJson = ConvertTo-Json $address

# Act
$result = Invoke-RestMethod -Method Put -Uri "$baseUri/$id" -Body $addressJson -ContentType 'application/json' 

# Assert
if($address.line2 -eq $result.line2){Write-Output "Success!"}
else {Write-Error "`nFailure!`n Expected: $($address.line2)`n Actual: $($result.line2)`n"}
