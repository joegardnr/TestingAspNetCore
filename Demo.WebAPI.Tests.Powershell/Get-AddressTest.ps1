# Arrange
$baseUri = "http://localhost:5000/api/address"
$id = 1

# Act
$result = Invoke-RestMethod -Method Get -Uri "$baseUri/$id"

# Assert
if($result.id -eq 1) {Write-Output "Success!"}
else {Write-Error "`nFailure!"}
