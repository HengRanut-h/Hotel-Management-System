param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

dotnet ef migrations add $Name `
    --output-dir src/Persistence/Migrations
