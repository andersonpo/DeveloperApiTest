rmdir /s /q ".\TestResults\"
rmdir /s /q ".\coveragereport\"

dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test --nologo -v m --collect:"XPlat Code Coverage" /p:CoverletOutputFormat=cobertura /p:ExcludeByFile="../**/Program.cs"
reportgenerator -reports:".\TestResults\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
explorer ".\coveragereport\index.html"

D:\Projetos\DeveloperApiTest\DeveloperApiTest\Program.cs