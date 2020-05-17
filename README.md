# AzureStorageClient

1. Client to Azure Blob Storage
2. Client to Azure Table Storage

---
## Prerequests

- .Net Core v 2.0 or above

---
## How to use

#### 1. Blob Storage Client

	
   - register *AzureBlobClient* in IoC: `services.AddAzureBlobClient(Configuration);`
   - add to `appsettings.json` section: 
		```
		"AzureBlobClientSettings": {
			"ConnectionString": "",
			"ContainerName": ""
		},
		```
   - reference *IAzureBlobClient*
   - inject through constructor *IAzureTableClient*

#### 2. Table Storage Client

   - register *AzureTableClient* in IoC: `services.AddAzureTableClient(Configuration);`
   - add to `appsettings.json` section: 
		```
		"AzureTableClientSettings": {
			"ConnectionString": ""
		}
		```
   - inject through constructor *IAzureTableClient*


---
## Integration tests
- (optional) launch *Microsoft Azure Storage Emulator*
- Configure *appsettings.Development.json*
- Lists integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp2.2 --list-tests`
- Run integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp2.2 --logger trx --results-directory ./IntegrationTests/Results/`