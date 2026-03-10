# DAPR Demo Sample Repository

This repository contains several demo applications in .NET 10 to show the benefits and the power of the Distributed Application Runtime (DAPR). To be able to recreate these demo's yourself you can find some instructions below. 

## DAPR Commands for start-up

```csharp
dapr 
dapr init
dapr --version

// App launch
dapr run --components-path "C:\Development\dapr-multicloud-demo\components" dotnet run
```

## DAPR Multicloud Samples

DAPR has great benefits for multicloud purposes, to show this there is a DaprMulticloudDemo application in this repository. This application can:

- Get secrets from a local secret store and Azure Key Vault without changing application code.
- Write and retrieve state from a local state store or Azure Table Storage without changing application code.

### Setup

Build the application locally using `dotnet build`.

```
dapr run --components-path "C:\Development\dapr-multicloud-demo\components" dotnet run
```

### Endpoints

- https://localhost:7299/swagger/index.html
- http://localhost:5299/swagger/index.html

## DAPR State Store Demo

```
dapr run --app-id rockstars --dapr-http-port 3500

Invoke-RestMethod -Method Post -ContentType 'application/json' -Body '[{ "key": "name", "value": "David de Hoop"}]' -Uri 'http://localhost:3500/v1.0/state/statestore'
Invoke-RestMethod -Method Get -Uri 'http://localhost:3500/v1.0/state/statestore/name'
Invoke-RestMethod -Uri 'e'

docker exec -it dapr_redis redis-cli
keys * hgetall "rockstars||name"
```

## DAPR on Azure Container Apps

To be able to deploy an application with DAPR on Container Apps you'll need the following resources in your Resource Group to be present:

- Container Apps Managed Environment
- Log Analytics Workspace

### Deploy DAPR Components

While developing locally you tell the DAPR runtime where you're components are located. Because DAPR is integrated within Container Apps you do not have to package the DAPR components with your application. You can treat them as part of your infrastructure. DAPR components are like Environment variables and therefore set on the _Container Apps Managed Environment_ rather than on the Container App itself. You can do this using ARM or BICEP templates but of course also through the following CLI command.

```powershell
az containerapp env dapr-component set `
    --name live-ManagedEnvironment `
    --resource-group azconf-2022-live `
    --dapr-component-name azurestatestore `
    --yaml components/containerapps/azurestatestore.yaml
```

### Preparing your image

You can use an image from your own ACR (Azure Container Registry) or any public registry for that matter. I have an ACR setup. To push the latest version of my demo image there I need to tag it locally, then push it to ACR.

```powershell
az acr login --name daprcontainerappdemo
```

But first we need to login to the ACR so we are authorized to push the image.

```powershell
docker tag daprcontainerappdemo daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v6
docker push daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v6
```

### Create the Container App

The Container APP itself can be created from ARM/BICEP but also from the CLI. You provide the image and very important, you tell it to enable DAPR so a sidecar container will automaticcaly be spun up for you. Also keep in mind that `--dapr-port` and `--target-port` are the same and equivalent to the exposed port in your dockerfile or Docker Compose for your application as this will be the port where your application will be listening on and where DAPR will send data to.

NOTE: This command is idempotent so in the case your Container App already exists it will just update it.

```powershell
az containerapp create `
  --name daprdemoapp `
  --resource-group azconf-2022 `
  --environment dapr-demo-managedEnvironment `
  --image daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v5 `
  --target-port 3000 `
  --ingress external `
  --min-replicas 1 `
  --max-replicas 3 `
  --enable-dapr `
  --dapr-app-port 3000 `
  --dapr-app-id daprdemoapp `
  --registry-server daprcontainerappdemo.azurecr.io `
  --registry-username daprcontainerappdemo `
  --registry-password NuUX3mdGLLLIx0n7ZN+3pouWvZipenY6
```

### Azure CLI Commands

### Tag Image with version

```
docker tag daprcontainerappdemo daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v5
```

### Push image to ACR

```
docker push daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v5
```

### Create / Update Azure Container App

```powershell
az containerapp create 
--name daprdemoapp 
--resource-group azconf-2022 
--environment dapr-demo-managedEnvironment 
--image daprcontainerappdemo.azurecr.io/daprcontainerappdemo:v5 
--target-port 3000 
--ingress external 
--min-replicas 1 
--max-replicas 3 
--enable-dapr 
--dapr-app-port 3000 
--dapr-app-id daprdemoapp 
--registry-server daprcontainerappdemo.azurecr.io 
--registry-username daprcontainerappdemo 
--registry-password NuUX3mdGLLLIx0n7ZN+3pouWvZipenY6
```

## Create SPN for Key Vault authorization for DAPR

For DAPR to communicate with Azure it uses an application registration with a service principal, that you need to create below.

```powershell
az ad app create --display-name "dapr-demoapplication" --available-to-other-tenants false --oauth2-allow-implicit-flow false
az ad app credential reset --id "d4e2ab14-5aaa-4112-b3cd-17012d1c5d5f" --years 2 --password $(openssl rand -base64 30)

az ad sp create --id "d4e2ab14-5aaa-4112-b3cd-17012d1c5d5f"
```