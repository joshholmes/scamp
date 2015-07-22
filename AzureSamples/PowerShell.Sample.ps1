
function Run
{
    $BaseGroupName = "Testing.2015.CS150A"
    $GroupCount = 1

    # Clean up previous resource groups
    Get-AzureResourceGroup | ?{ $_.ResourceGroupName -like "Testing.*" } | Remove-AzureResourceGroup -Verbose -Force
    
    $results = foreach ($i in 2..$GroupCount)
    {
        [PSCustomObject] @{
            MicrosoftId = "emaino@gmail.com";
            FirstName = "Eric";
            LastName = "Maino";
        } | New-SchoolResourceGroup -NamePrefix $BaseGroupName
    } 

    $json = $results | ConvertTo-Json -Depth 10
    $json | Out-File -FilePath  C:\src\SCAMP\results.json
}

function New-SchoolResourceGroup
{
    param(
        [Parameter(mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $NamePrefix,
        [Parameter(valueFromPipelineByPropertyName=$true,mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $FirstName,
        [Parameter(valueFromPipelineByPropertyName=$true,mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $LastName,
        [Parameter(valueFromPipelineByPropertyName=$true,mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $MicrosoftId,

        [String[]]
        $Roles = @(
                    "WebSite Contributor",
                    "Storage Account Contributor",
                    "Sql Server Contributor"
                ),

        [String]
        $Location = "West Us"
    )

    $servicePlan = "Student-Web-Site"
    $groupKey = ([System.IO.Path]::GetRandomFileName() -replace "[^a-z0-9]", "")
    
    #Create the group
    $groupName = "$($NamePrefix).$($LastName).$($FirstName).$($groupKey)" -replace "[^a-z0-9.-]", ""
    
    "Creating Resource Group $($groupName)" | Write-Host
    New-AzureResourceGroup -Name $groupName  -Location $location -Force | Out-Null
    
    #Add user to the group
    $roles | %{ New-AzureRoleAssignment -Mail $MicrosoftId -ResourceGroupName $groupName -RoleDefinitionName $_  | Out-Null }


    New-AzureAppServicePlan -ResourceGroupName $groupName -Name $servicePlan -Location $Location -Sku Shared | Out-Null
    New-AzureWebApp -Location $Location -ResourceGroupName $groupName -Name "$($groupKey)" -AppServicePlan $servicePlan | Out-Null

    ((Get-AzureWebAppPublishingProfile -ResourceGroupName $groupName -Name $groupKey) | ?{ $_.PublishMethod -like "FTP" } | Select -First 1) | %{
        [PSCustomObject] @{
            WebSite = $_.DestinationAppUri;
            FtpAddress = $_.PublishUrl;
            FtpUserName = $_.UserName;
            FtpPassword = $_.UserPassword;
        }
    }
}

Switch-AzureMode -Name AzureResourceManager
Run