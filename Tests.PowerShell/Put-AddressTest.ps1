# Arrange
$baseUri = "http://localhost:5000/api/address"
$id = 1
$address = @{
    id=$id;
    line1="Another Line 1";
    line2="More Line 2";
    line3="And Line 3";
    city="Some City";
    state="ST";
    zip="12345"; 
}
$addressJson = ConvertTo-Json $address

# Act
$result = Invoke-RestMethod -Method Put -Uri "$baseUri/$id" -Body $addressJson -ContentType 'application/json' 

# Assert
if($address.line3 -eq $result.line3){Write-Output "Success!"}
else {Write-Error "`nFailure!`n Expected: $($address.line3)`n Actual: $($result.line3)`n"}
