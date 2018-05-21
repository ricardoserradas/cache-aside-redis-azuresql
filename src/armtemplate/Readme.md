# ARM Template for the Cache-aside sample

This template provisions the following workloads:

* SQL Server
* SQL Database
* Redis Cache

To use it, fill in the [_parameters.json_](parameters.json) file with all the information you want to provide or just rename it so it will be ignored. Next, I'd recommend changing at least the following parameters on [_template.json_](template.json);

* **Redis_cacheaside_name**: It will be the name of your Redis Cache instance.
* **servers_cacheaside_name**: It will be the name of your Azure SQL Server.

Once it's done, use the following command (PowerShell sample):

`> .\deploy.ps1 -subscriptionId "<your subscription ID>" -resourceGroupName "<The name for the Resource Group>" -resourceGroupLocation "eastus" -deploymentName "<a deployment name>"`