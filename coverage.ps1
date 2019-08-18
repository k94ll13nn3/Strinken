dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet tool install -g coverlet.console
dotnet build
coverlet .\tests\Strinken.Public.Tests\bin\Debug\netcoreapp2.2\Strinken.Public.Tests.dll --target "dotnet" --targetargs "test .\tests\Strinken.Public.Tests\Strinken.Public.Tests.csproj --no-build" --format cobertura --output coverage.public.xml
coverlet .\tests\Strinken.Tests\bin\Debug\netcoreapp2.2\Strinken.Tests.dll --target "dotnet" --targetargs "test .\tests\Strinken.Tests\Strinken.Tests.csproj --no-build" --format cobertura --output coverage.xml
reportgenerator -reports:"coverage.xml;coverage.public.xml" -targetdir:coverage -reporttypes:"HtmlInline_AzurePipelines"
ii coverage\index.htm