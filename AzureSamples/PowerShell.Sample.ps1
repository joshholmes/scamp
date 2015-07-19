
function Run
{
    $BaseGroupName = "Testing.2015.CS150A"
    $GroupCount = 5

    # Clean up previous resource groups
    Get-AzureResourceGroup | ?{ $_.ResourceGroupName -like "Testing.*" } | Remove-AzureResourceGroup -Verbose -Force
    
    #foreach ($i in 1..$GroupCount)
    #{
    #    [PSCustomObject] @{
    #        MicrosoftId = "emaino@gmail.com";
    #        FirstName = "Eric.XXXXXXXXXX";
    #        LastName = "Maino.XXXXXXXXXXXXXXXXXXXXXXXX";
    #    } | New-SchoolResourceGroup -NamePrefix $BaseGroupName
    #}
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

    $groupKey = [System.IO.Path]::GetFileNameWithoutExtension([System.IO.Path]::GetRandomFileName())
    
    #Create the group
    $groupName = "$($NamePrefix).$($LastName).$($FirstName).$($groupKey)" -replace "[^a-z0-9.-]", ""
    
    "Creating Resource Group $($groupName)" | Write-Host
    New-AzureResourceGroup -Name $groupName  -Location $location -Force | Out-Null
    
    #Add user to the group
    $roles | %{ New-AzureRoleAssignment -Mail $userEmail -ResourceGroupName $groupName -RoleDefinitionName $_  | Out-Null }
}

Switch-AzureMode -Name AzureResourceManager
Run