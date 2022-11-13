dotnet build -c Release 
dotnet pack .\ServiceProviderEndpoint\ -c Release -o ..\_publish
dotnet pack .\ServiceProviderEndpoint.Client\ -c Release -o ..\_publish
