# AzureStorageClient

1. Client to Azure Blob Storage
2. Client to Azure Table Storage

---
## Prerequests

One of the below:
- .Net Core v 2.0 or higher
- .Net 5 or higher
- .Net Framework 4.6.1 (4.7.2 suggested) or higher

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
   - (optionally) use method `applicationBuilder.InitializeAzureBlobClient();` to initialize blob container

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
- Lists integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp3.1 --list-tests`
- Run integration tests: `dotnet test .\Tests\AzureStorageClient.IntegrationTests\ --configuration {Debug|Release} --framework netcoreapp3.1 --logger trx --results-directory ./IntegrationTests/Results/`


## Roadmap
- lock blob while upserting content
- use AsyncStream to stream content of large blob folder
- add permission functionality around blobs (separate package)
- create hangfire/background scheduler with TableStorage