# AzureStorageClient

Client to Azure Blob Storage

---
## Import

- register *StorageClient* in IoC: `services.AddStorageClient(Configuration);`

---
## Integration tests
- (optional) launch *Microsoft Azure Storage Emulator*
- Configure *appsettings.Development.json*
- Lists integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp2.2 --list-tests`
- Run integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp2.2 --logger trx --results-directory ./IntegrationTests/Results/`