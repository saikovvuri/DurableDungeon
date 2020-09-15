$fileContents = @"
env = {
    'apiHost': '$env:APIHOST'
}
"@

Write-Output $fileContents